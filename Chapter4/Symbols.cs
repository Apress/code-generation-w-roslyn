using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Chapter4
{
    public static class Symbols
    {
        public static void ReviewSymbolTable(Compilation compilation)
        {
            foreach (var member in compilation.Assembly.GlobalNamespace.GetMembers()
                .Where(member => member.CanBeReferencedByName))
            {
                Console.WriteLine(member.Name);
                foreach (var item in member.GetTypeMembers()
                    .Where(item => item.CanBeReferencedByName))
                {
                    Console.WriteLine("\t{0}:{1}", item.TypeKind, item.Name);
                    foreach (var innerItem in item.GetMembers()
                        .Where(innerItem => innerItem.CanBeReferencedByName))
                    {
                        Console.WriteLine("\t\t{0}:{1}", innerItem.Kind, innerItem.Name);
                    }
                }
            }
        }

        public static IEnumerable<INamedTypeSymbol> GetBaseClasses(SemanticModel model, BaseTypeDeclarationSyntax type)
        {
            var classSymbol = model.GetDeclaredSymbol(type);
            var returnValue = new List<INamedTypeSymbol>();
            while (classSymbol.BaseType != null)
            {
                returnValue.Add(classSymbol.BaseType);
                if (classSymbol.Interfaces != null)
                    returnValue.AddRange(classSymbol.Interfaces);
                classSymbol = classSymbol.BaseType;
            }
            return returnValue;
        }

        public static IEnumerable<BaseTypeDeclarationSyntax>
            FindClassesDerivedOrImplementedByType(Compilation compilation, INamedTypeSymbol target)
        {
            foreach (var tree in compilation.SyntaxTrees)
            {
                var semanticModel = compilation.GetSemanticModel(tree);

                foreach (var type in tree.GetRoot().DescendantNodes().OfType<TypeDeclarationSyntax>())
                {
                    var baseClasses = GetBaseClasses(semanticModel, type);
                    if (baseClasses != null)
                        if (baseClasses.Contains(target))
                            yield return type;
                }
            }
        }
    }
}
