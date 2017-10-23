using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.MSBuild;
using Common;
namespace Chapter4
{
    [Example(Message = "Show examples of Binding", Sequence = 4)]
    public class BindingExample
    {
        [Example(Message = "Show an Example using the Semantic Model to get all Classes Implemented a specified Interface", Sequence = 1)]
        public static void ExampleOne()
        {
            var work = MSBuildWorkspace.Create();

            var solution = work.OpenSolutionAsync(@"..\..\..\RoslynPlayGround.sln").Result;
            var project = solution.Projects.FirstOrDefault(p => p.Name == "Chapter3");
            if (project == null)
                throw new Exception("Could not find the Chapter 3 project");
            var compilation = project.GetCompilationAsync().Result;
            var targetType = compilation.GetTypeByMetadataName("Chapter3.IGreetingProfile");
            var type = Symbols.FindClassesDerivedOrImplementedByType(compilation, targetType);
            Console.WriteLine(type.First().Identifier.ToFullString());

        }
    }
}
