using System;

namespace Bloodthirst.BDeepCopy
{
    public static class BConverterManger
    {
        internal static BJsonProvider Json { get; private set; } = new BJsonProvider();

        public static T FromJson<T>(string json)
        {
            Type t = typeof(T);
            IBJsonConverterInternal c = Json.GetOrCreate(t);

            return (T) c.ConvertFrom(json);
        }

        public static void PopulateFromJson<T>(T instance , string json) where T : class
        {
            Type t = typeof(T);
            IBJsonConverterInternal c = Json.GetOrCreate(t);
            c.Populate(instance, json);
        }


    }
}
