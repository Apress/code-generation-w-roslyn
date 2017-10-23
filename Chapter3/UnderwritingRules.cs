using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chapter3
{
    public class UnderwritingRules
    {
        public bool Rule1(ILoanCodes data)
        {
            if (data.Code1 != 1) return false;
            if (data.Code3 != 7) return false;
            if (data.Code4 != 10 && data.Code4 != 11) return false;
            if (data.Code2 == 4 || data.Code4 == 5 || data.Code4 == 6)
                return true;
            return false;
        }

    }
}
