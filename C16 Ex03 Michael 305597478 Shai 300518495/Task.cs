﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace C16_Ex03_Michael_305597478_Shai_300518495
{
    public abstract class Task
    {
        public bool Serve()
        { 
            bool IsSatisfied = IsConditionSatisfied();
            if (IsSatisfied)
            {
                InvokeAction();
            }

            return IsSatisfied;
        }

        protected abstract bool IsConditionSatisfied();

        protected abstract void InvokeAction();
    }
}
