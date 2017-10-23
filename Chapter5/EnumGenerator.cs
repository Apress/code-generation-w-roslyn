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

namespace Chapter5
{
    [Example(Message = "Show examples of generating the Enums", Sequence = 1)]
    public class EnumGenerator
    {
        [Example(Message = "Show an example of generating the Enum using ParseText", Sequence = 1)]
        public static void ExampleOne()
        {
            var code = @"
            [EnumDescription("" "")]
            public enum EnumName 
            {
               [MemberDescription ("" "")]
               Name = Value;
            }";
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var codeSyntax = new List<SyntaxNode>();
            foreach(var data in GetEnumsDetails())
            {
                var newSyntaxTree = syntaxTree.GetRoot();
                if (!string.IsNullOrEmpty(data.LongDescription))
                {
                    var literal = newSyntaxTree.DescendantNodes()
                        .OfType<LiteralExpressionSyntax>().FirstOrDefault();
                    var newLiteral = SyntaxFactory.LiteralExpression
                        (SyntaxKind.StringLiteralExpression,
                        SyntaxFactory.Literal(data.LongDescription));
                    newSyntaxTree = newSyntaxTree.ReplaceNode(literal, newLiteral);
                }
                else
                {
                    var attribute = newSyntaxTree.DescendantNodes()
                        .OfType<AttributeSyntax>().FirstOrDefault();
                    if (attribute != null)
                        newSyntaxTree = newSyntaxTree.RemoveNode
                            (attribute, SyntaxRemoveOptions.KeepNoTrivia);
                }
                var identifierToken = newSyntaxTree.DescendantTokens()
                    .First(t => t.IsKind(SyntaxKind.IdentifierToken)
                                && t.Parent.Kind() == SyntaxKind.EnumDeclaration);
                var newIdentifier = SyntaxFactory.Identifier
                    (data.ShortDescription.Replace(" ", ""));

                newSyntaxTree = SyntaxFactory.SyntaxTree
                    (newSyntaxTree.ReplaceToken(identifierToken, newIdentifier)).GetRoot();
                foreach (var item in data.Details)
                {
                    var memberDeclaration = newSyntaxTree.DescendantNodes()
                        .OfType<EnumMemberDeclarationSyntax>().FirstOrDefault();
                    memberDeclaration = memberDeclaration.WithIdentifier
                        (SyntaxFactory.Identifier(item.ShortDescription.Replace(" ", "")));
                    memberDeclaration = memberDeclaration.
                        WithEqualsValue(SyntaxFactory.EqualsValueClause(SyntaxFactory.LiteralExpression
                        (SyntaxKind.NumericLiteralExpression,
                        SyntaxFactory.Literal(item.LoanCodeId))));
                    var attributeDeclaration = memberDeclaration.DescendantNodes().OfType<AttributeListSyntax>()
                        .FirstOrDefault();
                    if (string.IsNullOrEmpty(item.LongDescription))
                    {
                        memberDeclaration = memberDeclaration.RemoveNode
                            (attributeDeclaration, SyntaxRemoveOptions.KeepNoTrivia);
                    }
                    else
                    {
                        var description = attributeDeclaration.DescendantNodes().OfType<LiteralExpressionSyntax>()
                            .FirstOrDefault();
                        var newDescription =  SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, 
                            SyntaxFactory.Literal(item.LongDescription));
                        var newAttribute = attributeDeclaration.ReplaceNode(description, newDescription);
                        memberDeclaration = memberDeclaration.ReplaceNode(attributeDeclaration, newAttribute);
                    }
                    
                    var oldDeclaration =
                        newSyntaxTree.DescendantNodes().OfType<EnumDeclarationSyntax>().FirstOrDefault();
                    newSyntaxTree = newSyntaxTree.ReplaceNode(oldDeclaration,
                        oldDeclaration.WithMembers(oldDeclaration.Members.Add(memberDeclaration)));

                }
                var firstMember = newSyntaxTree.DescendantNodes()
                        .OfType<EnumMemberDeclarationSyntax>().FirstOrDefault();
                newSyntaxTree = newSyntaxTree.RemoveNode(firstMember, SyntaxRemoveOptions.KeepNoTrivia);
                Console.WriteLine(newSyntaxTree.NormalizeWhitespace());
            }
        }
        [Example(Message = "Show an example of generating the Enum using SyntaxFactory", Sequence = 2)]
        public static void ExampleTwo()
        {
            var codeSyntax = new List<SyntaxNode>();
            foreach (var data in GetEnumsDetails())
            {
                var generatedEnum = SyntaxFactory.EnumDeclaration(data.ShortDescription.Replace(" ", ""))
                 .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
                 .WithAttributeLists(SyntaxFactory.SingletonList(SyntaxFactory.AttributeList(
                  SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Attribute(SyntaxFactory.IdentifierName("Description"))
                   .WithArgumentList(SyntaxFactory.AttributeArgumentList(SyntaxFactory.SingletonSeparatedList(
                    SyntaxFactory.AttributeArgument(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(data.LongDescription))))))))));
                foreach (var item in data.Details)
                {
                    var attributes = SyntaxFactory.SingletonList(
                     SyntaxFactory.AttributeList(SyntaxFactory
                      .SingletonSeparatedList(SyntaxFactory.Attribute(
                       SyntaxFactory.IdentifierName("Description")).WithArgumentList(
                       SyntaxFactory.AttributeArgumentList(
                        SyntaxFactory.SingletonSeparatedList(SyntaxFactory.AttributeArgument(
                         SyntaxFactory.LiteralExpression(
                          SyntaxKind.StringLiteralExpression,
                          SyntaxFactory.Literal(item.LongDescription)))))))));
                    var objectCreationExpression = SyntaxFactory.EqualsValueClause(
                     SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression,
                      SyntaxFactory.Literal(item.LoanCodeId)));
                    if (!string.IsNullOrEmpty(item.LongDescription))
                    {
                        var member = SyntaxFactory.EnumMemberDeclaration(attributes, SyntaxFactory.Identifier(item.ShortDescription), objectCreationExpression);
                        generatedEnum = generatedEnum.AddMembers(member);
                    }
                    else
                    {
                        generatedEnum = generatedEnum.AddMembers(SyntaxFactory.EnumMemberDeclaration(SyntaxFactory.Identifier(item.ShortDescription))
                            .WithEqualsValue(SyntaxFactory.EqualsValueClause(SyntaxFactory.LiteralExpression
                            (SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(item.LoanCodeId)))));
                    }
                }
                codeSyntax.Add(generatedEnum);


            }
            codeSyntax.ForEach(a => Console.WriteLine(a.NormalizeWhitespace().ToString()));

        }
        private static IEnumerable<EnumTypeItem> GetEnumsDetails()
        {
            var server = new Server("localhost");
            var database = DBUtilities.GetAllDatabases(server).FirstOrDefault(db => db.Name.ToUpper() == "PLAYGROUND");
            return DBUtilities.GetEnumData(database).Where(d => !d.IsRange);
        }
    }

}
