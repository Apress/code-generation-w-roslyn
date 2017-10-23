using System;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace Common
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ExampleAttribute : Attribute
    {
        public string Message { get; set; }
        public int Sequence { get; set; }
        public Type AssociatedType { get; set; }
        public MethodInfo AssociatedMethod { get; set; }
    }
    
}