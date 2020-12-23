using Bloodthirst.Core.Clonable;
using Bloodthirst.System.CommandSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Packages.com.bloodthirst.bloodthirst_core.Runtime.CommandSystem.Types.ParallelQueue
{
    public class ParallelLayerCloneable : ParallelLayer, ICloneable<ParallelLayer>
    {
        public ParallelLayer Clone()
        {
            return new ParallelLayer();
        }
    }
}
