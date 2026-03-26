using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace EssentialRemote.Models
{
    public class TwistMessage
    {
        public float LinearX { get; set; }
        public float AngularZ { get; set; }

        // En lille hjælpemetode til at lave objektet om til JSON
        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
