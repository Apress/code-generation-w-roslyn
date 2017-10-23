using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chapter3
{
    public class GreetingRules 
    {
        public string Rule1(IGreetingProfile data)
        {
            if (data.Hour > 11) return null;
            if (data.Gender != 1) return null;
            return "Good Morning Mr. " + data.LastName;
        }

        public string Rule2(IGreetingProfile data)
        {
            if (data.Hour > 11) return null;
            if (data.Gender != 2) return null;
            if (data.MaritalStatus != 1) return null;
            return "Good Morning Mrs. " + data.LastName;
        }

        public string Rule3(IGreetingProfile data)
        {
            if (data.Hour > 11) return null;
            if (data.Gender != 2) return null;
            if (data.MaritalStatus != 2) return null;
            return "Good Morning Ms. " + data.LastName;
        }

        public string Rule4(IGreetingProfile data)
        {
            if (data.Hour < 12) return null;
            if (data.Hour < 17) return null;
            if (data.Gender != 1) return null;
            return "Good Afternoon Mr. " + data.LastName;
        }
    }
}
