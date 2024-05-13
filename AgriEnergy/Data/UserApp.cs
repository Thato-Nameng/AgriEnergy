using Microsoft.AspNetCore.Identity;

namespace AgriEnergy.Data
{
    public class UserApp : IdentityUser
    {
        public string? FName { get; set; }
        public string? LName { get; set; }
        public string? Picture { get; set; }


    }
}
