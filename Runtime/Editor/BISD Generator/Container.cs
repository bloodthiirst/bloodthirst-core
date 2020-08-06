using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Packages.com.bloodthirst.bloodthirst_core.Runtime.Editor.BISD_Generator
{
    public class Container<T>
    {
        public T Behaviour { get; set; }
        public T Instance { get; set; }
        public T State { get; set; }
        public T Data { get; set; }
    }
}
