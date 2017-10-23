using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Common;

namespace Chapter4
{
    [Example (Message = "Show examples of simple Code Generation", Sequence = 4)]
    public static class CodeGenerationExample
    {
        [Example (Message = "Show an example using ParseText", Sequence = 1)]
        public static void ExampleOne()
        {
            var emptyClassTree = SimpleGenerator.CreateEmptyClass("GreetingBusinessRule");
            var emptyClass =
                emptyClassTree.GetRoot().DescendantNodes().
                     OfType<ClassDeclarationSyntax>().FirstOrDefault();
            if (emptyClass == null)
                return;
            Console.WriteLine(emptyClass.NormalizeWhitespace().ToString());

        }
        [Example (Message = "Show an example using SyntaxFactory to add nodes", Sequence =2 )]
        public static void ExampleTwo()
        {
            var reference = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            var compilation = CSharpCompilation.Create("internal")
                .WithReferences(reference);
            var intType = compilation.GetTypeByMetadataName("System.Int32");
            var stringType = compilation.GetTypeByMetadataName("System.String");
            var dateTimeType = compilation.GetTypeByMetadataName("System.DateTime");
            var emptyClassTree = SimpleGenerator.CreateEmptyClass("GreetingBusinessRule");
            var emptyClass =
                emptyClassTree.GetRoot().DescendantNodes().
                     OfType<ClassDeclarationSyntax>().FirstOrDefault();
            if (emptyClass == null)
                return;
            emptyClass = emptyClass.AddProperty("Age", intType)
                            .AddProperty("FirstName", stringType)
                            .AddProperty("LastName", stringType)
                            .AddProperty("DateOfBirth", dateTimeType)
                            .NormalizeWhitespace();
            Console.WriteLine(emptyClass.ToString());

        }
        [Example (Message = "Show an example Code Generator creting a Constructor", Sequence = 3)]
        public static void ExampleThree()
        {
            var reference = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            var compilation = CSharpCompilation.Create("internal")
                .WithReferences(reference);
            var intType = compilation.GetTypeByMetadataName("System.Int32");
            var stringType = compilation.GetTypeByMetadataName("System.String");
            var dateTimeType = compilation.GetTypeByMetadataName("System.DateTime");
            var emptyClassTree = SimpleGenerator.CreateEmptyClass("GreetingBusinessRule");
            var emptyClass =
                emptyClassTree.GetRoot().DescendantNodes().
                     OfType<ClassDeclarationSyntax>().FirstOrDefault();
            if (emptyClass == null)
                return;
            emptyClass = emptyClass.AddProperty("Age", intType)
                            .AddProperty("FirstName", stringType)
                            .AddProperty("LastName", stringType)
                            .AddProperty("DateOfBirth", dateTimeType)
                            .AddConstructor().NormalizeWhitespace();
            Console.WriteLine(emptyClass.ToString());
        }
    }
}
