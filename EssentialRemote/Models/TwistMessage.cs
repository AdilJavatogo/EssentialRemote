using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EssentialRemote.Models
{
    public class TwistMessage
    {
        [JsonPropertyName("linearX")]
        public float LinearX { get; set; }

        [JsonPropertyName("angularZ")]
        public float AngularZ { get; set; }

        // En lille hjælpemetode til at lave objektet om til JSON
        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
