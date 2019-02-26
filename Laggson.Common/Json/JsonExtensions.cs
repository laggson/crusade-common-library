using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Laggson.Common.Json
{
    public static class JsonParsing
    {
        public static T FromJson<T>(this string source)
        {
            var result = JsonConvert.DeserializeObject<T>(source);

            return result;
        }

        public static IEnumerable<T> FromJsonArray<T>(this string source)
        {
            var result = JsonConvert.DeserializeObject(source);

            if (result.GetType() != typeof(JArray))
                return null;

            return ((JArray)result).ToObject<IEnumerable<T>>();
        }

        public static string ToJson<T>(this T item)
        {
            return JsonConvert.SerializeObject(item);
        }
    }
}
