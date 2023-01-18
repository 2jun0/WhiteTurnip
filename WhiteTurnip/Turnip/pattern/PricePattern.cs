using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhiteTurnip.utils;

namespace WhiteTurnip.weekPrice.pattern
{
    interface PricePattern
    {
        public int[,] Prices();
    }
}
