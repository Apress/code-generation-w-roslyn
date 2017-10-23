using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chapter5.GreetingRule
{
    public interface IGreetingProfile
    {
        int Hour { get; set; }
        int Gender { get; set; }
        int MaritalStatus { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
    }

}
