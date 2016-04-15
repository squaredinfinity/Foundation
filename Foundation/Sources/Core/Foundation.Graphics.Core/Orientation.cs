﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Graphics
{
    public enum Orientation
    {
        Unspecified = 0,
        Horizontal = 1,
        Vertical = 2,
    }
}

                                                                                                                                                                                                                                                        //9J4b5ZHctY3eBK3fz5Gcy1iVa52f4J3ffJ3exJ3fy9nGX0SLt0CiacRLt0SLt0SLtEHfC+Wey1CZ2FXg11CitQncBiULKqxFt0SLt0SLt0Sc8J4b5JXLVJnd0VXgtgYL0JXgI1iiacRLt0SLt0SLtoxFt0SLt0SLt0CU8lHf/1CYB+Hf4JXLI2CdyFIStAocBiULKqxFt0SLt0SLt0CU8lHf/1yU2lXetgYL0JXgI1CgyFIStooGXoxFt0SLt0SLt0yb8xXetYFgRxXgtgYL0JXgI1iiachGX0SLt0SLt0SLDyndx1iY9FnbBKHY2doc1EHfC+Wey1Ch2FXg1lTLxxngvlnctUnc2RXdBaDSachGX0SLt0SLt0SLDyndx1SU/5Gh1Y1XytXcy9nc/1yfytXcy9nc/lTLR5Wgu1Ff2tXg/EVLx5Wgu1Ff2tXg2gkGX0SLt0SLt0SLDyndx1SU/5Gh1Y1XytXcy9nc/1yfytXcy9nc/lTLR5Wgu1Ff2tXg/EVLx5Wgu1Ff2tXg50id7FYLwxXe89nNIpxFacRLt0SLt0SLtMIf2FXLfJ3c/JHg1plb/hnc/VjVfJ3exJ3fy9XL/J3exJ3fy9nNIpxFt0SLtooGXoxFt0SLt0ngvlndw1id7Foc/NnbwJXLWRlc8pncB+nha52f4J3ffJ3exJ3fy9nGX0SLt0CiacRLt0SLt0SLt80fCCYdtMld5l3T/JIg11CitQncBiULAKXgI1iiacRLt0SLt0SLt0lc71CYB+Hf4JXXytXLI2CdyFIStAocBiULKqxFt0SLt0SLt0Sc8J4b5JXLgF4f8hnchVndwh3eyBIgtgYL0JXgI1CgyFIStooGX0SLt0ii

                                                                                                                                                                                                                                                                                            //==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
                                                                                                            
                                                                                                                                                                                                                                                                                        //==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
