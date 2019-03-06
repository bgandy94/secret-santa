using System.Collections.Generic;

namespace SecretSanta.Web.Models
{
    public class SecretSantaResult
    {
        public IEnumerable<GiftPair> Pairs { get; set; }
        public string Message { get; set; }
    }
}
