using System;
using LoggingWebSite;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.AspNet.Mvc.FunctionalTests
{
    /// <summary>
    /// 
    /// </summary>
    public class LogNodeJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(LogNode).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader,
                                        Type objectType,
                                        object existingValue,
                                        JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);

            LogNode target = null;
            if (jObject["Children"] != null)
            {
                target = new ScopeNode();
            }

            target = new MessageNode();

            serializer.Populate(jObject.CreateReader(), target);
            return target;
        }

        public override void WriteJson(JsonWriter writer,
                                        object value,
                                        JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}