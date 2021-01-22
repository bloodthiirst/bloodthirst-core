using Bloodthirst.Core.Clonable;
using Bloodthirst.System.CommandSystem;

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
