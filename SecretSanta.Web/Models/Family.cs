using System.Collections.Generic;

namespace SecretSanta.Web.Models
{
    public class Family
    {
        public string Name { get; set; }
        public List<FamilyMember> Members { get; set; }
    }
}
