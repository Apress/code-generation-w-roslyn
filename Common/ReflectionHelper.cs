using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class ReflectionHelper
    {
        public IList<ExampleAttribute> GetTopLevelExampleMenu()
        {
            var returnValue = new List<ExampleAttribute>();
            var newAssembly = Assembly.GetEntryAssembly();
            var assembly = this.GetType().Assembly;
            assembly = newAssembly;
            var types = assembly.GetTypes();
            foreach (var type in types)
                //Select(type => ).Where(examples => examples.Any()))
            {
                var example = type.GetCustomAttributes<ExampleAttribute>().FirstOrDefault();
                if (example != null)
                {
                    example.AssociatedType = type;
                    returnValue.Add(example);
                }
            }
            return returnValue.OrderBy(p => p.Sequence).ToList();
        }

        public IList<ExampleAttribute> GetMethodLevelExampleMenu(Type type)
        {
            var returnValue = new List<ExampleAttribute>();
            var methods = type.GetMethods();
            foreach (var method in methods)
            {
                var example = method.GetCustomAttributes<ExampleAttribute>().FirstOrDefault();
                if (example != null)
                {
                    example.AssociatedMethod = method;
                    returnValue.Add(example);
                }
            }
            return returnValue.OrderBy(p => p.Sequence).ToList();
        }

        public static ExampleAttribute PromptForMethodLevelSelection(ExampleAttribute selection)
        {
            var helper = new ReflectionHelper();
            var list = helper.GetMethodLevelExampleMenu(selection.AssociatedType);
            var index = 1;
            foreach (var item in list)
            {
                Console.WriteLine(index++ + "\t" + item.Message);
            }
            var enteredText = "Not a number";
            int enteredValue;
            while (!Int32.TryParse(enteredText, out enteredValue)
                   || (enteredValue < 0 || enteredValue > list.Count))
            {
                Console.Write("\nSelect exmple to view: ");
                var option = Console.ReadKey();
                enteredText = option.KeyChar.ToString();
            }
            Console.WriteLine();
            Console.WriteLine("You selected " + list[enteredValue - 1].Message);
            return list[enteredValue - 1];

        }

        public static ExampleAttribute PromptForTopLevelSelection()
        {
            var helper = new ReflectionHelper();
            var list = helper.GetTopLevelExampleMenu();
            var index = 1;
            foreach (var item in list)
            {
                Console.WriteLine(index++ + "\t" + item.Message);
            }
            var enteredText = "Not a number";
            int enteredValue;
            while (!Int32.TryParse(enteredText, out enteredValue)
                   || (enteredValue < 0 || enteredValue > list.Count))
            {
                Console.Write("\nSelect exmple to view: ");
                var selection = Console.ReadKey();
                enteredText = selection.KeyChar.ToString();
            }
            Console.WriteLine();
            Console.WriteLine("You selected " + list[enteredValue - 1].Message);
            return list[enteredValue - 1];
        }
    }
}
