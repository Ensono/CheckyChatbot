using System;
using Newtonsoft.Json;

namespace Healthbot {
    public class TimeSpanConverter : JsonConverter {
        public override bool CanConvert(Type objectType) {
            return (objectType == typeof(TimeSpan) || objectType == typeof(TimeSpan?));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
                                        JsonSerializer serializer) {
            string rawTimeSpan = (string) reader.Value;

            if (rawTimeSpan.EndsWith("ms")) {
                rawTimeSpan = rawTimeSpan.Split(' ')[0];
                return TimeSpan.FromSeconds(Int32.Parse(rawTimeSpan));
            } else {
                return TimeSpan.Parse(rawTimeSpan);
            }
        }

        public override bool CanWrite {
            get { return false; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            throw new NotImplementedException();
        }
    }
}