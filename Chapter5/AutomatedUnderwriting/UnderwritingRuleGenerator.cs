using Common;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chapter5.AutomatedUnderwriting
{
    [Example(Message = "Show examples of generating the Underwriting Rules", Sequence = 3)]
    public class UnderwritingRuleGenerator
    {
        [Example(Message = "Show an example of generating the Underwriting Rules with Inverted Logic",
            Sequence = 1)]
        public static void GenerateRules()
        {
            var code = @"public class UnderwritingRules{}";
            var method = @"public bool Rule(ILoanCodes data)
                {
                    var target = new []{};
                }";
            var syntaxTree = CSharpSyntaxTree.ParseText(code).GetRoot();
            var classDeclaration = syntaxTree.DescendantNodes().OfType<ClassDeclarationSyntax>()
                .FirstOrDefault();
            var originalDeclaration = classDeclaration;
            foreach (var rule in GetUnderwritingRules())
            {
                var methodSyntaxTree = CSharpSyntaxTree.ParseText(method).GetRoot();
                var underwritingRule = methodSyntaxTree.DescendantNodes()
                    .OfType<MethodDeclarationSyntax>().FirstOrDefault();
                underwritingRule = RenameRule(rule.RuleName, underwritingRule);
                underwritingRule = underwritingRule
                    .WithLeadingTrivia(new SyntaxTrivia[] { SyntaxFactory.Comment("// " + rule.ShortDescription) });
                underwritingRule = underwritingRule.WithBody(ProcessLoanCodes(rule, underwritingRule.Body));
                var currentBlock = underwritingRule.Body;
                currentBlock = currentBlock.AddStatements(new StatementSyntax[] { ReturnTrue() });
                underwritingRule = underwritingRule.WithBody(currentBlock);
                classDeclaration = classDeclaration.AddMembers(underwritingRule);
            }
            Console.WriteLine(classDeclaration.NormalizeWhitespace());
        }

        private static MethodDeclarationSyntax RenameRule(string ruleName, MethodDeclarationSyntax underwritingRule)
        {
            var identifierToken = underwritingRule.DescendantTokens()
                .First(t => t.IsKind(SyntaxKind.IdentifierToken)
                    && t.Parent.Kind() == SyntaxKind.MethodDeclaration);
            var newIdentifier = SyntaxFactory.Identifier(ruleName.Replace(" ", ""));
            underwritingRule = underwritingRule.ReplaceToken(identifierToken, newIdentifier);
            return underwritingRule;
        }

        private static BlockSyntax ProcessLoanCodes(UnderwritingRule rule,
            BlockSyntax underwritingRule)
        {
            var loanCodeTypes = from d in rule.Details
                                group d by new { d.LoanCodeTypeId, d.IsRange } into loanCodes
                                select loanCodes;
            foreach (var loanCodeType in loanCodeTypes)
            {
                var loanCodeTypeId = loanCodeType.Key.LoanCodeTypeId;
                if (!loanCodeType.Key.IsRange)
                {
                    var initializationExpressions = new List<LiteralExpressionSyntax>();
                    foreach (var detail in loanCodeType)
                    {
                        initializationExpressions.Add(SyntaxFactory.LiteralExpression
                        (SyntaxKind.StringLiteralExpression,
                        SyntaxFactory.Literal(detail.LoanCodeId)));
                    }
                    underwritingRule = ProcessLoanCodeCondition(loanCodeTypeId, 
                        underwritingRule, initializationExpressions);
                }
                else
                {
                    foreach (var detail in loanCodeType)
                    {
                        if (detail.Max.HasValue)
                            underwritingRule = ProcessLoanCodeRangeCondition(loanCodeTypeId, underwritingRule,
                                SyntaxKind.GreaterThanExpression, detail.Max.Value);
                        if (detail.Min.HasValue)
                            underwritingRule = ProcessLoanCodeRangeCondition(loanCodeTypeId, underwritingRule,
                                SyntaxKind.LessThanExpression, detail.Min.Value);
                    }
                }
            }

            return underwritingRule;
        }

        private static BlockSyntax ProcessLoanCodeRangeCondition(int loanCodeTypeId, 
            BlockSyntax underwritingRule,SyntaxKind comparisonType, decimal codeValue)
        {
            var codeExpression = SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxFactory.IdentifierName("data"),
                    SyntaxFactory.IdentifierName("Code" + loanCodeTypeId));
            var condition = SyntaxFactory.BinaryExpression(comparisonType,
                codeExpression,SyntaxFactory.LiteralExpression(
                    SyntaxKind.NumericLiteralExpression,SyntaxFactory.Literal(codeValue)));

            var newConditional = SyntaxFactory.IfStatement(condition, ReturnFalse());
            return underwritingRule.AddStatements(new StatementSyntax[] { newConditional });

        }

        private static BlockSyntax ProcessLoanCodeCondition(int loanCode, BlockSyntax currentBlock,
            List<LiteralExpressionSyntax> initializationExpressions)
        {
            var assignmentStatement = ReinitializeTargetArrayy(currentBlock, initializationExpressions);

            currentBlock = currentBlock.AddStatements
                (new StatementSyntax[] { assignmentStatement });

            var codeExpression = SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxFactory.IdentifierName("data"),
                    SyntaxFactory.IdentifierName("Code" + loanCode));
            var target = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                SyntaxFactory.IdentifierName("target"), SyntaxFactory.IdentifierName("Contains"));

            var argument = SyntaxFactory.Argument(codeExpression);
            var argumentList = SyntaxFactory.SeparatedList(new[] { argument });

            var contains = SyntaxFactory.InvocationExpression(target, SyntaxFactory.ArgumentList(argumentList));
            var notContains = SyntaxFactory.BinaryExpression(SyntaxKind.NotEqualsExpression, contains,
                SyntaxFactory.LiteralExpression(SyntaxKind.TrueLiteralExpression));
            var newConditional = SyntaxFactory.IfStatement(notContains, ReturnFalse());
            return currentBlock.AddStatements(new StatementSyntax[] { newConditional });
        }

        private static ExpressionStatementSyntax ReinitializeTargetArrayy(BlockSyntax currentBlock, List<LiteralExpressionSyntax> initializationExpressions)
        {
            var declarator = currentBlock.DescendantNodes().OfType<VariableDeclaratorSyntax>()
                .FirstOrDefault();
            var init = declarator.Initializer;

            var initializationExpression = currentBlock.DescendantNodes()
                .OfType<ImplicitArrayCreationExpressionSyntax>().FirstOrDefault();
            initializationExpression =
                initializationExpression.AddInitializerExpressions(initializationExpressions.ToArray());
            var variableIdentifier = SyntaxFactory.IdentifierName("target");
            var assignment = SyntaxFactory.AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
                variableIdentifier, initializationExpression);
            var assignmentStatement = SyntaxFactory.ExpressionStatement(assignment);
            return assignmentStatement;
        }

        private static ReturnStatementSyntax ReturnTrue()
        {
            return SyntaxFactory.ReturnStatement(SyntaxFactory.LiteralExpression(
                    SyntaxKind.TrueLiteralExpression));
        }

        private static ReturnStatementSyntax ReturnFalse()
        {
            return SyntaxFactory.ReturnStatement(SyntaxFactory.LiteralExpression(
                    SyntaxKind.FalseLiteralExpression));
        }
        private static IEnumerable<UnderwritingRule> GetUnderwritingRules()
        {
            var server = new Server("localhost");
            var database = DBUtilities.GetAllDatabases(server)
                .FirstOrDefault(db => db.Name.ToUpper() == "PLAYGROUND");
            return DBUtilities.GettingUnderwritingRules(database);
        }

        private bool DirectLogic (ILoanCodes data)
        {
            if (data.Code4 == 1 || data.Code4 == 2 || data.Code4 == 3 || data.Code5 == 4)
                return true;
            return false;
        }

        private bool InvertedLogic (ILoanCodes data)
        {
            if (data.Code4 != 1 && data.Code4 != 2 && data.Code4 != 3 && data.Code5 != 4)
                return false;
            return true;
        }
        private bool ContainsLogic (ILoanCodes data)
        {
            var target = new[] { 1, 2, 3, 4 };
            if (!target.Contains(data.Code4))
                return false;
            return true;
        }
    }
    public class UnderwritingRules
    {
        // Full Doc requires 2 years Bank Statements for Purchase of Single Family Residence
        public bool FullDocRequirements(ILoanCodes data)
        {
            var target = new int [] { };
            target = new[] { 1 };
            if (target.Contains(data.Code1) != true)
                return false;
            target = new[] { 7 };
            if (target.Contains(data.Code3) != true)
                return false;
            target = new[] { 10, 11 };
            if (target.Contains(data.Code4) != true)
                return false;
            target = new[] { 4, 5, 6 };
            if (target.Contains(data.Code2) != true)
                return false;
            return true;
        }

        // Appraisal with 3 Comps required for Purchase
        public bool AppraisalRequirements(ILoanCodes data)
        {
            var target = new int [] { };
            target = new[] { 7 };
            if (target.Contains(data.Code3) != true)
                return false;
            target = new[] { 21 };
            if (target.Contains(data.Code6) != true)
                return false;
            target = new[] { 16, 17, 18 };
            if (target.Contains(data.Code5) != true)
                return false;
            return true;
        }

        // Maximum LTV is 80% if the Credit Score is below 650 on a Refinance unless the DTI is below 40%
        public bool LTVGuideline(ILoanCodes data)
        {
            var target = new int[] { };
            target = new[] { 8, 9 };
            if (target.Contains(data.Code3) != true)
                return false;
            if (data.Code10 > 650M)
                return false;
            if (data.Code8 > 40M)
                return false;
            if (data.Code9 > 80M)
                return false;
            return true;
        }
    }
}
