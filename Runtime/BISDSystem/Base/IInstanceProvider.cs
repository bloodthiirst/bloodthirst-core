﻿namespace Bloodthirst.Core.BISDSystem
{
    public interface IInstanceProvider
    {
        T Get<T>();
    }
}
