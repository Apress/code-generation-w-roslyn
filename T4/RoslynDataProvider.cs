using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;

namespace T4
{
    public class RoslynDataProvider
    {
        public MSBuildWorkspace Workspace { get; set; }

        private Compilation compilation { get; set; }

        public IEnumerable<ClassDeclarationSyntax> FindControllers(Project project)
        {
            compilation = project.GetCompilationAsync().Result;
            var targetType = compilation.GetTypeByMetadataName("System.Web.Http.ApiController");

            foreach (var document in project.Documents)
            {
                var tree = document.GetSyntaxTreeAsync().Result;
                var semanticModel = compilation.GetSemanticModel(tree);
                foreach (var type in tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>()
                    .Where(type => GetBaseClasses(semanticModel, type).Contains(targetType)))
                {
                    yield return type;
                }
            }
        }


        public static IEnumerable<INamedTypeSymbol> GetBaseClasses
            (SemanticModel model, BaseTypeDeclarationSyntax type)
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

        public static IEnumerable<Type> GetBaseTypes(Type type)
        {
            if (type.BaseType == null) return type.GetInterfaces();

            return Enumerable.Repeat(type.BaseType, 1)
                .Concat(type.GetInterfaces())
                .Concat(type.GetInterfaces().SelectMany(GetBaseTypes))
                .Concat(GetBaseTypes(type.BaseType));
        }

        public Project GetWebApiProject()
        {
            var solution = Workspace.CurrentSolution;
            var project = solution.Projects.FirstOrDefault(p => p.Name.ToUpper().EndsWith("WEBAPI"));
            if (project == null)
                throw new ApplicationException("WebApi project not found in solution ");
            return project;
        }
        protected IEnumerable<TypeInfo> FindAssociatedModel
            (SemanticModel semanticModel, TypeDeclarationSyntax controller)
        {
            var returnValue = new List<TypeInfo>();
            var attributes = controller.DescendantNodes().OfType<AttributeSyntax>()
                .Where(a => a.Name.ToString() == "ResponseType");
            var parameters = attributes.Select(a => a.ArgumentList.Arguments.FirstOrDefault());
            var types = parameters.Select(p => p.Expression).OfType<TypeOfExpressionSyntax>();
            foreach (var t in types)
            {
                var symbol = semanticModel.GetTypeInfo(t.Type);
                if (symbol.Type.SpecialType == SpecialType.System_Void) continue;
                returnValue.Add(symbol);
            }
            return returnValue.Distinct();
        }

        public IEnumerable<string> GetActions(ClassDeclarationSyntax controller)
        {
            var semanticModel = compilation.GetSemanticModel(controller.SyntaxTree);
            var actions = controller.Members.OfType<MethodDeclarationSyntax>();
            var returnValue = new List<string>();
            foreach (var action in actions.Where
                (a => a.Modifiers.Any(m => m.Kind() == SyntaxKind.PublicKeyword)))
            {
                var mapName = MapByMethodName(semanticModel, action);
                if (mapName != null)
                    returnValue.Add(mapName);
                else
                {
                    mapName = MapByAttribute(semanticModel, action);
                    if (mapName != null)
                        returnValue.Add(mapName);
                }
            }
            return returnValue.Distinct();
        }

        public IEnumerable<TypeInfo> GetModels(ClassDeclarationSyntax controller)
        {
            var semanticModel = compilation.GetSemanticModel(controller.SyntaxTree);
            return FindAssociatedModel(semanticModel, controller);
        }

        public IEnumerable<ISymbol> GetProperties(IEnumerable<TypeInfo> models)
        {
            return models.Select(typeInfo => typeInfo.Type.GetMembers()
                .Where(m => m.Kind == SymbolKind.Property))
                .SelectMany(properties => properties).Distinct();
        }


        public IEnumerable<ISymbol> GetProperties(TypeInfo model)
        {
            return model.Type.GetMembers().Where(m => m.Kind == SymbolKind.Property
                                                      && m.CanBeReferencedByName);
        }

        private static bool IdentifyIEnumerable(SemanticModel semanticModel,
            MethodDeclarationSyntax action)
        {
            var symbol = semanticModel.GetSymbolInfo(action.ReturnType);
            var typeSymbol = symbol.Symbol as ITypeSymbol;
            if (typeSymbol == null) return false;
            return typeSymbol.AllInterfaces.Any(i => i.Name == "IEnumerable");
        }

        private static string MapByAttribute(SemanticModel semanticModel, MethodDeclarationSyntax action)
        {
            var attributes = action.DescendantNodes().OfType<AttributeSyntax>().ToList();
            if (attributes.Any(a => a.Name.ToString() == "HttpGet"))
                return IdentifyIEnumerable(semanticModel, action) ? "query" : "get";
            var targetAttribute = attributes.FirstOrDefault(a => a.Name.ToString().StartsWith("Http"));
            return targetAttribute?.Name.ToString().Replace("Http", "").ToLower();
        }

        private static string MapByMethodName(SemanticModel semanticModel,
            MethodDeclarationSyntax action)
        {
            if (action.Identifier.Text.Contains("Get"))
                return IdentifyIEnumerable(semanticModel, action) ? "query" : "get";
            var regex = new Regex(@"\b(?'verb'post|put|delete)", RegexOptions.IgnoreCase);
            if (regex.IsMatch(action.Identifier.Text))
                return regex.Matches(action.Identifier.Text)[0]
                    .Groups["verb"].Value.ToLower();
            return null;
        }


    }
}
