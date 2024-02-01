using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace UserService.Data
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public byte[] Salt { get; set; }
        public string Token { get; set; }
        [JsonIgnore]
        public ICollection<Order> Order { get; set; }
    }
}
