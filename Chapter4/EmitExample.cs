using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.MSBuild;
using Common;
namespace Chapter4
{
    [Example(Message = "Show examples for emitting the Assembly", Sequence = 3)]
    public static class EmitExample
    {
        [Example (Message =  "Show example of emitting an Assembly and looping thorugh the associated diagnostic messages", Sequence = 1)]
        public static void ExampleOne()
        {
            var work = MSBuildWorkspace.Create();

            var solution = work.OpenSolutionAsync(@"..\..\..\RoslynPlayGround.sln").Result;
            var project = solution.Projects.FirstOrDefault(p => p.Name == "Chapter3");
            if (project == null)
                throw new Exception("Could not find the Chapter 3 project");
            var compilation = project.GetCompilationAsync().Result;
            var results = compilation.Emit("Chapter3.dll", "Chapter3.pdb");
            if (!results.Success)
            {
                foreach (var item in results.Diagnostics)
                {
                    if (item.Severity == DiagnosticSeverity.Error)
                    {
                        Console.WriteLine(item.GetMessage());
                    }
                }
            }
        }

        [Example (Message =  "Show an example of emitting an Assembly to memory and then loading this assembly", Sequence = 2)]
        public static void ExampleTwo()
        {
            var work = MSBuildWorkspace.Create();

            var solution = work.OpenSolutionAsync(@"..\..\..\RoslynPlayGround.sln").Result;
            var project = solution.Projects.FirstOrDefault(p => p.Name == "Chapter3");
            if (project == null)
                throw new Exception("Could not find the Chapter 3 project");
            var compilation = project.GetCompilationAsync().Result;
            var memory = new MemoryStream();
            var results = compilation.Emit(memory);
            if (!results.Success)
            {
                foreach (var item in results.Diagnostics)
                {
                    if (item.Severity == DiagnosticSeverity.Error)
                    {
                        Console.WriteLine(item.GetMessage());
                    }
                }
                return;
            }

            var assembly = Assembly.Load(memory.ToArray());
            var types = assembly.GetTypes();
            var greetingRules = types.FirstOrDefault(t => t.Name == "GreetingRules");
            if (greetingRules == null)
                throw new Exception("Could not find the GreetingRules");
            foreach (var rule in greetingRules.GetMethods(BindingFlags.Instance
                             | BindingFlags.DeclaredOnly
                             | BindingFlags.Public))
            {
                Console.WriteLine(rule.Name);
            }

        }

        [Example (Message =  "Show example of emitting an assembly and displaying the Diagnostic messages",Sequence = 3)]
        public static void ExampleThree()
        {
            var work = MSBuildWorkspace.Create();

            var solution = work.OpenSolutionAsync(@"..\..\..\RoslynPlayGround.sln").Result;
            var project = solution.Projects.FirstOrDefault(p => p.Name == "Chapter3");
            if (project == null)
                throw new Exception("Could not find the Chapter 3 project");
            var options = new CSharpCompilationOptions(OutputKind.ConsoleApplication,
                optimizationLevel: OptimizationLevel.Release, platform: Platform.X64);
            project = project.WithCompilationOptions(options);

            var compilation = project.GetCompilationAsync().Result;
            var results = compilation.Emit("Chapter3.dll", "Chapter3.pdb");
            if (!results.Success)
            {
                foreach (var item in results.Diagnostics)
                {
                    if (item.Severity == DiagnosticSeverity.Error)
                    {
                        Console.WriteLine(item.GetMessage());
                    }
                }
            }

        }
    }
}
