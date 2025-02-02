
using System.ComponentModel.DataAnnotations;

namespace Models.Entities
{
    public class Login
    {
        public string EmailOrAlias { get; set; }
        public string Password { get; set; }
    }
}
