﻿using System;
using System.Collections.Generic;

namespace Assets.SocketLayer.Identifier
{
    public class GUIDComparer : IEqualityComparer<Guid>
    {
        public bool Equals(Guid x, Guid y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(Guid obj)
        {
            return obj.GetHashCode();
        }
    }
}
