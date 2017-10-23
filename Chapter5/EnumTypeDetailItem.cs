using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chapter5
{
    public class EnumTypeDetailItem
    {
        public int LoanCodeId { get; set; }
        public int LoanCodeTypeId { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }

        public override string ToString()
        {
            return ShortDescription;
        }
    }
}
