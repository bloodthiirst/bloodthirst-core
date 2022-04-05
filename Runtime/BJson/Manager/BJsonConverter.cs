using System;

namespace Bloodthirst.BJson
{
    public static class BJsonConverter
    {
        internal static BJsonProvider Json { get; private set; } = new BJsonProvider();

        public static string ToJson<T>(T json, BJsonSettings settings)
        {
            Type t = typeof(T);
            IBJsonConverterInternal c = Json.GetConverter(t);
            settings.Provider = Json;
            return c.Serialize(json, new BJsonContext(), settings);
        }

        public static string ToJson(object instance, Type t, BJsonSettings settings)
        {
            IBJsonConverterInternal c = Json.GetConverter(t);
            settings.Provider = Json;
            return c.Serialize(instance, new BJsonContext(), settings);
        }


        public static string ToJson<T>(T instance)
        {
            Type t = typeof(T);
            IBJsonConverterInternal c = Json.GetConverter(t);

            return c.Serialize(instance , new BJsonContext() , new BJsonSettings() { Provider = Json });
        }

        public static string ToJson( object instance , Type t)
        {
            IBJsonConverterInternal c = Json.GetConverter(t);

            return c.Serialize(instance, new BJsonContext(), new BJsonSettings() { Provider = Json });
        }



        public static T FromJson<T>(string json)
        {
            Type t = typeof(T);
            IBJsonConverterInternal c = Json.GetConverter(t);


            return (T) c.Deserialize(json , new BJsonContext() , new BJsonSettings() { Provider = Json });
        }

        public static void PopulateFromJson<T>(T instance , string json) where T : class
        {
            Type t = typeof(T);
            IBJsonConverterInternal c = Json.GetConverter(t);
            c.Populate(instance, json , new BJsonContext() , new BJsonSettings() {  Provider = Json });
        }

        public static void PopulateFromJson(object instance, string json , BJsonSettings settings )
        {
            Type t = instance.GetType();
            IBJsonConverterInternal c = Json.GetConverter(t);
            
            settings.Provider = Json;

            c.Populate(instance, json, new BJsonContext(), settings);
        }


    }
}
