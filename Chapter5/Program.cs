using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using Microsoft.SqlServer.Management.Smo;
using System.Data;
using Common;

namespace Chapter5
{
    public class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                var selection = ReflectionHelper.PromptForTopLevelSelection();
                selection = ReflectionHelper.PromptForMethodLevelSelection(selection);
                selection.AssociatedMethod.Invoke(null, null);
            }
        }

 

    }
}
