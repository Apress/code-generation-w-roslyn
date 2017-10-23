using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chapter5.GreetingRule
{
    public class GreetingRuleDetail
    {
        public int GreetingRuleId { get; set; }
        public int? HourMin { get; set; }
        public int? HourMax { get; set; }
        public int? Gender { get; set; }
        public int? MaritalStatus { get; set; }
        public string Greeting { get; set; }
    }
}
