using Common;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chapter6.AutomatedUnderwriting
{
    [Example(Message = "Show examples of compiling Underwriting Rules", Sequence = 2)]
    public class UnderwritingRuleCompiler
    {
        [Example(Message = "Show examples of compiling Underwriting Rules", Sequence = 1)]
        public static void CompileUnderwritingRules ()
        {
            var work = MSBuildWorkspace.Create();
            var solution = work.OpenSolutionAsync(@"..\..\..\RoslynPlayGround.sln").Result;
            var project = solution.Projects.FirstOrDefault(p => p.Name == "EmptyProject");
            if (project == null)
                throw new Exception("Could not find the empty project");
            var generator = new UnderwritingRuleGenerator();
            var tree = generator.GenerateRules();
            var doc = project.AddDocument("UnderwritingRules", tree.GetRoot().NormalizeWhitespace());
            if (!work.TryApplyChanges(doc.Project.Solution))
                Console.WriteLine("Failed to add the generated code to the project");

            var compiler = project.GetCompilationAsync().Result;
            var results = compiler.GetDiagnostics();
            foreach (var item in results)
            {
                if (item.Severity == DiagnosticSeverity.Error)
                Console.WriteLine(item.GetMessage() + item.Location);
            }
            compiler.Emit("UnderwritingRules.dll");
        }
    }
}
