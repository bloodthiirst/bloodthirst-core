using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.BISDSystem.Base
{
    public interface IEntityState<T> : IEntityState where T : EntityData
    {
        T Data { get; set; }
    }

    public interface IEntityState
    {
        int Id { get; set; }
    }
}
