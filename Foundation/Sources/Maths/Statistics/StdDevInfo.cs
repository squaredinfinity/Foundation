using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Maths.Statistics
{
    public struct StdDevInfo
    {
        VarianceInfo _variance;
        public VarianceInfo Variance {  get { return _variance; } set { _variance = value; } }

        public double StdDev
        {
            get
            {
                return Math.Sqrt(Variance);
            }
        }

        public static implicit operator double (StdDevInfo sd)
        {
            return sd.StdDev;
        }

        public override string ToString()
        {
            return StdDev.ToString();
        }

        public string ToString(string format)
        {
            return StdDev.ToString(format);
        }
    }
}                                                                                                                                                                                                                                                 //t0ngvlndw1Cc55GgA2yXytXcy9nc/1yRtY1XytXcy9nc/pxFt0SLtgoGX0SLt0SLt0SL9J4b5ZHctYVX2Voc5Blb7NobA2SX2Voc5Blb7NobA2CitQncBiUL99ndD6Wgy1CgyFIStooGXoxFt0SLt0SLt0SfC+We2BXLR5WgyFmd6JXLfJ3exJ3fgFob/FYY2pnciFIctgYL0JXgI1Sf/Z3guFoctAocBiULKqxFacRLt0SLt0SLt0ngvlndw1CU8lHf/1yTuBHe09HfCuXctgYL0JXgI1CgyFIStooGX0SLt0SLt0SLacRLt0SLt0SLtwDP80SSAKoe652fGukGX0SLt0SLt0SL8wDPtUlc2RXdB2Cfz1ibD6md552b5JXLAK4fz5GcypxFt0SLt0SLt0CP8wTLJxDgCqneu9nhLpxFt0SLt0SLt0SfC+We2BXL2tXgtUlc2RXdB2CitQncBiUL99HfBKHcBKXctAocBiULKqxFacRLt0SLt0SLtwDP80SSAKoe652fGukGX0SLt0SLt0SL8wDPtQmdxFYdtw3ct42guZXeu9Wey1CgC+3cuBncacRLt0SLt0SLtwDP80SS8Aog6pnb/Z4SacRLt0SLt0SLt0ngvlndw1id7FYLkZXcBWXLI2CdyFISt03f8FocwFocx1CgyFIStooGXoxFt0SLt0SLt0SfC+We2BXLfJ3exJ3fy9XN2tXgtQodxFYd50id7FYL1Jnd0VXg2oxFt0SLt0SLt0SLt0SLH1Sg1ZHg1QodxFYd50SdyZHd1FYOtsncE2SX2Voc550f/5mhQ52eD6Gg1QodxFYd50SdyZHd1FoN2oxFt0SLt0SLt0CitooGXoxFt0SLt0SLt0SfC+We2BXLfJ3exJ3fy9XN2tXgtQodxFYd50id7FYL1Jnd0VXg50iVdZXhylHUut3guBYL9ZXhylHUut3guBoNacRLt0SLt0SLtgoGX0SLt0SLt0SLt0SLt8lc7Fnc/BWgu9XghZneyJWgw1iStElbBKXY2pnc7IWgwtFfEikGXoxFt0SLt0SLt0SLt0SLEaXcBWXLK1iWuFYd7olbFWjP50Ch2FXg1ZDSacRLt0SLt0SLt0SLt0SdyZHd1FYLK1iWuFYd7olbFWjP50SdyZHd1FoNIpxFacRLt0SLt0SLt0SLt0CZ2FXg11iStQodxFYdIpxFt0SLt0SLt0SLt0SLVJnd0VXgtoUL1Jnd0VXgIpxFacRLt0SLt0SLt0SLt0SX2Voc5Blb7NobA2iSt0ndFKXeQ52eD6GgIpxFacRLt0SLt0SLt0SLtwDPt0ldFKXeQ52eD6Gg7AVey52f10jNIpxFt0SLt0SLt0iiachGX0SLt0SLt0SL9J4b5ZHctMIf2FXLRZHg9xHgyVjNacRLt0SLt0SLtgoGXoxFt0SLt0SLt0iiachGX0SLt0SLt0SL9J4b5ZHct8lc7Fnc/FlbB6WLUJXgfJ3exJ3fR5WguVjNacRLt0SLt0SLtgoGX0SLt0SLt0SLt0SLtMob/1yfx1iStsncE2yXytXcy9XUuFob1QmdxFYd50SVyZHd1FYOt0ldFKXeQ52eD6Gg50SUuFochZneytjYBC3W8RYL60yXytXcy9HYB62fBGmd6JnYBCnNIpxFt0SLt0SLt0SLt0SLacRLt0SLt0SLt0SLt0yfyFog/tXL/FHSacRLt0SLt0SLtooGX0SLt0ii