using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Collections.Generic;

namespace T4
{
    internal class Program
    {
        private static void Main()
        {
            var work = MSBuildWorkspace.Create();
            var solution = work.OpenSolutionAsync(@"..\..\..\RoslynPlayGround.sln").Result;
            var metadata = new RoslynDataProvider() { Workspace = work };
            var template = new AngularResourceService
            {
                MetadataProvider = metadata,
                Url = @"http://localhost:53595/"
            };
            var results = template.TransformText();
            var project = metadata.GetWebApiProject();
            var folders = new List<string>() { "Scripts" };
            var document = project.AddDocument("factories.js", results, folders)
                .WithSourceCodeKind(SourceCodeKind.Script)
                ;
            if (!work.TryApplyChanges(document.Project.Solution))
                Console.WriteLine("Failed to add the generated code to the project");
            Console.WriteLine(results);

            Console.ReadLine();
        }
    }
}