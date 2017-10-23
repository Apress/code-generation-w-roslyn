using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chapter5.GreetingRule
{
    public class SampleRules
    {
        public string Rule1(IGreetingProfile data)
        {
            if (data.Hour > 11) return null;
            if (data.Gender != 1) return null;
            return "Good Morning Mr. " + data.LastName;
        }
        public string Rule1Initial (IGreetingProfile data)
        {
            if ((data.Hour <= 11) && (data.Gender == 1))
                return "Good Morning Mr. " + data.LastName;
            else return null;
        }
    }
}
