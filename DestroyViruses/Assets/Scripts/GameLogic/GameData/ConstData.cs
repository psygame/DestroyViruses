﻿using System;
namespace DestroyViruses
{
    public class ConstTable
    {
        public static TableConst table
        {
            get
            {
                return TableConst.Get(1);
            }
        }
    }
}