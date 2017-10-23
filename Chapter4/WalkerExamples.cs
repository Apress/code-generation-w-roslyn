using System;
using System.IO;
using Microsoft.CodeAnalysis.CSharp;
using Common;
namespace Chapter4
{
    [Example(Message = "Show examples of using SyntaxWalker to walk through a Syntax Tree", Sequence =  1)]
    public static class WalkerExamples 
    {
        [Example (Message = "Show walking through the Syntax Tree for a Simple Parsed Statement", Sequence = 1)]
        public static void ExampleOne()
        {
            var tree = CSharpSyntaxTree.ParseText("a=b+c;");
            var walker = new Walker();
            walker.Visit(tree.GetRoot());
            Console.WriteLine("We can get back to the original code by call ToFullString ");
            Console.WriteLine(tree.GetRoot().ToFullString());
        }
        [Example (Message = "Show walking through the Syntax Tree for a complete class loaded from a file", Sequence = 2)]
        public static void ExampleTwo()
        {
            string code;
            using (var sr = new StreamReader("../../Code3_2.cs"))
            {
                code = sr.ReadToEnd();
            }
            var tree = CSharpSyntaxTree.ParseText(code);
            var walker = new Walker();
            walker.Visit(tree.GetRoot());
        }
    }
}
