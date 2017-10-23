using Common;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Chapter6.GreetingRule
{
    [Example(Message = "Show examples of generating the Greeting Rules", Sequence = 2)]
    public class GreetingRuleGenerator
    {
        [Example(Message = "Show an example of generating the Greeting Rules with Inverted Logic", 
            Sequence = 1)]
        public static void CreateGreetingRules()
        {
            var code = @"public class GreetingRules{}";
            var method = @"public string Rule(IGreetingProfile data){}";
            var syntaxTree = CSharpSyntaxTree.ParseText(code).GetRoot();
            var methodSyntaxTree = CSharpSyntaxTree.ParseText(method).GetRoot();
            var classDeclaration = syntaxTree.DescendantNodes().OfType<ClassDeclarationSyntax>()
                .FirstOrDefault();
            var originalDeclaration = classDeclaration;
            foreach (var rule in GetGreetingRuleDetails())
            {
                var greetingRule = methodSyntaxTree.DescendantNodes().OfType<MethodDeclarationSyntax>()
                    .FirstOrDefault();
                var identifierToken = greetingRule.DescendantTokens()
                    .First(t => t.IsKind(SyntaxKind.IdentifierToken)
                        && t.Parent.Kind() == SyntaxKind.MethodDeclaration);
                var newIdentifier = SyntaxFactory.Identifier
                    ("Rule" + rule.GreetingRuleId);
                greetingRule = greetingRule.ReplaceToken(identifierToken, newIdentifier);
                if (rule.HourMin.HasValue)
                {
                    var newBlock = ProcessHourCondition(rule.HourMin.Value, 
                        greetingRule.Body, SyntaxKind.LessThanExpression);
                    greetingRule = greetingRule.WithBody(newBlock);
                }
                if (rule.HourMax.HasValue)
                {
                    var newBlock = ProcessHourCondition(rule.HourMax.Value, 
                        greetingRule.Body, SyntaxKind.GreaterThanExpression);
                    greetingRule = greetingRule.WithBody(newBlock);
                }
                if (rule.Gender.HasValue)
                {
                    var newBlock = ProcessEqualityComparison("Gender", 
                        rule.Gender.Value, greetingRule.Body);
                    greetingRule = greetingRule.WithBody(newBlock); 
                }
                if (rule.MaritalStatus.HasValue)
                {
                    var newBlock = ProcessEqualityComparison("MaritalStatus", 
                        rule.MaritalStatus.Value, greetingRule.Body);
                    greetingRule = greetingRule.WithBody(newBlock);
                }
                var currentBlock = AddRuleReturnValue(greetingRule.Body,  rule);
                greetingRule = greetingRule.WithBody(currentBlock);
                classDeclaration = classDeclaration.AddMembers(greetingRule);
            }
            syntaxTree = syntaxTree.ReplaceNode(originalDeclaration, classDeclaration);
            Console.Write(classDeclaration.NormalizeWhitespace());
        }

        private static BlockSyntax ProcessEqualityComparison (string whichEquality, 
            int gender, BlockSyntax currentBlock)
        {
            var genderReference = SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxFactory.IdentifierName("data"),
                    SyntaxFactory.IdentifierName(whichEquality));
            var condition = SyntaxFactory.BinaryExpression(SyntaxKind.NotEqualsExpression, 
                genderReference, SyntaxFactory.LiteralExpression(
                            SyntaxKind.NumericLiteralExpression,
                            SyntaxFactory.Literal(gender)));
            var newConditional = SyntaxFactory.IfStatement(condition, ReturnNull());
            return currentBlock.AddStatements(new StatementSyntax[] { newConditional });

        }
        private static ReturnStatementSyntax ReturnNull()
        {
            return SyntaxFactory.ReturnStatement(SyntaxFactory.LiteralExpression(
                    SyntaxKind.NullLiteralExpression));
        }
        private static BlockSyntax AddRuleReturnValue(BlockSyntax currentBlock, GreetingRuleDetail rule)
        {
            var ruleGreeting = SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression,
                SyntaxFactory.Literal(rule.Greeting));
            var lastName = SyntaxFactory.MemberAccessExpression
                (SyntaxKind.SimpleMemberAccessExpression, SyntaxFactory.IdentifierName("data"),
                SyntaxFactory.IdentifierName("LastName"));
             var assignment = SyntaxFactory.BinaryExpression(SyntaxKind.AddExpression,
                ruleGreeting, lastName);
            var returnStatement = SyntaxFactory.ReturnStatement(assignment);
            return currentBlock.AddStatements(new StatementSyntax[] { returnStatement });

        }
        private static BlockSyntax ProcessHourCondition(int hourValue, BlockSyntax currentBlock, 
            SyntaxKind comparisonType)
        {
            var hourExpression = SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxFactory.IdentifierName("data"),
                    SyntaxFactory.IdentifierName("Hour"));
            var condition = SyntaxFactory.BinaryExpression(comparisonType,
                    hourExpression,
                    SyntaxFactory.LiteralExpression(
                            SyntaxKind.NumericLiteralExpression,
                            SyntaxFactory.Literal(hourValue)));
            var newConditional = SyntaxFactory.IfStatement(condition,ReturnNull());
            return currentBlock.AddStatements(new StatementSyntax[] { newConditional });
        }
        private static IEnumerable<GreetingRuleDetail> GetGreetingRuleDetails()
        {
            var server = new Server("localhost");
            var database = DBUtilities.GetAllDatabases(server)
                .FirstOrDefault(db => db.Name.ToUpper() == "PLAYGROUND");
            return DBUtilities.GetGreetingRules(database);
        }
    }
}
