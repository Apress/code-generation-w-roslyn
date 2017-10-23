using System;
using System.Linq;
using Microsoft.CodeAnalysis.MSBuild;
using Common;
namespace Chapter4
{
    [Example(Message = "Show examples creating a Compilation in preparation to access the Symbol Table", Sequence = 2)]
    public class SymbolExample
    {
        [Example(Message = "Show example using the MSBuildWorkspace to create the context to get a Compilation",
            Sequence = 1)]
        public static void ExampleOne()
        {
            var work = MSBuildWorkspace.Create();
            var solution = work.OpenSolutionAsync(@"..\..\..\RoslynPlayGround.sln").Result;
            var project = solution.Projects.FirstOrDefault(p => p.Name == "Chapter3");
            if (project == null)
                throw new Exception("Could not find the Chapter 3 project");
            var compilation = project.GetCompilationAsync().Result;
            // Do something with the compilation
        }

        [Example(Message = "Show creating a Compilation with the Workspace and accessing the Symbol Table", Sequence = 2
            )]
        public static void ExampleTwo()
        {
            var work = MSBuildWorkspace.Create();

            var solution = work.OpenSolutionAsync(@"..\..\..\RoslynPlayGround.sln").Result;
            var project = solution.Projects.FirstOrDefault(p => p.Name == "Chapter3");
            if (project == null)
                throw new Exception("Could not find the Chapter 3 project");
            var compilation = project.GetCompilationAsync().Result;
            Symbols.ReviewSymbolTable(compilation);
        }
    }
}