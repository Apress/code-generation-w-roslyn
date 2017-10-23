using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chapter6.AutomatedUnderwriting
{
    public class UnderwritingRule
    {
        public string RuleName { get; set; }
        public string ShortDescription { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public List<UnderwritingRuleDetail> Details { get; set; }
    }
}
