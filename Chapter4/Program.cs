using System;
using System.Collections.Generic;
using Common;
using Microsoft.CodeAnalysis.MSBuild;
using System.Linq;

namespace Chapter4
{
    public class Program
    {
        static void Main()
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
