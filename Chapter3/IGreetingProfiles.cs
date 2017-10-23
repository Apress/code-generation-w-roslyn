using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chapter3
{
    public interface IGreetingProfile
    {
        int Hour { get; set; }
        int Gender { get; set; }
        int MaritalStatus { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
    }

    public class GreetingProfile : IGreetingProfile
    {
        public string FirstName { get; set; }

        public int Gender { get; set; }

        public int Hour { get; set; }

        public string LastName { get; set; }

        public int MaritalStatus { get; set; }
    }
}
