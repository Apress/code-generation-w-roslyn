using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chapter5
{
    public class EnumTypeItem
    {
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public bool IsRange { get; set; }
        public int Id { get; set; }

        public IList<EnumTypeDetailItem> Details { get; set; }

        public override string ToString()
        {
            return ShortDescription;
        }
    }
}
