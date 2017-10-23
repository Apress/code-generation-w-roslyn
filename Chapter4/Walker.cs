using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Chapter4
{
    public class Walker : CSharpSyntaxWalker
    {
        public Walker() : base(SyntaxWalkerDepth.StructuredTrivia)
    {
        }
        static int Tabs = 0;
        public override void Visit(SyntaxNode node)
        {
            Tabs++;
            var indents = new String(' ', Tabs * 3);
            Console.WriteLine(indents + node.Kind());
            base.Visit(node);
            Tabs--;
        }
        public override void VisitLetClause(LetClauseSyntax node)
        {
            Console.WriteLine("Found a let clause " + node.Identifier.Text);
            base.VisitLetClause(node);
        }
        public override void VisitToken(SyntaxToken token)
        {
            var indents = new String(' ', Tabs * 3);
            Console.WriteLine(string.Format("{0}{1}:\t{2}", indents , token.Kind() , token.Text));
            base.VisitToken(token);
        }
        public override void VisitTrivia(SyntaxTrivia trivia)
        {
            base.VisitTrivia(trivia);
        }
        
    }
}
