﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Packages.BloodthirstCore.Runtime.BISDSystem.Base
{
    public interface IInstanceRegister
    {
        void Register<T>(T instance);
    }
}