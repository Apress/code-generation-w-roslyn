using Common;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chapter6
{
    class Program
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
