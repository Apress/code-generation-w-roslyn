using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chapter5.AutomatedUnderwriting
{
    public class UnderwritingRuleDetail
    {
        public int UnderwritingRuleDetailId { get; set; }
        public int UnderwritingRuleId { get; set; }
        public int LoanCodeId { get; set; }
        public decimal? Min { get; set; }
        public decimal? Max { get; set; }
        public int Sequence { get; set; }
        public int LoanCodeTypeId { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public bool IsRange { get; set; }
    }
}
