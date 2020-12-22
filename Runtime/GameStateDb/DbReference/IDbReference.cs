using Newtonsoft.Json;
using Sirenix.Serialization;
using System;

namespace JsonDB
{
    public interface IDbReference
    {
        Type EntityType { get; }
        [OdinSerialize]
        int? ReferenceId { get; set; }
        [JsonIgnore]
        IDbEntity Entity { get; set; }
    }
}