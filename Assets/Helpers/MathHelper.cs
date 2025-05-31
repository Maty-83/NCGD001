using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Helpers
{
    public class MathHelper
    {
        public static float Remap(float value, float fromMin, float fromMax, float toMin, float toMax)
        {
            return toMin + ( value - fromMin ) * ( toMax - toMin ) / ( fromMax - fromMin );
        }
    }
}
