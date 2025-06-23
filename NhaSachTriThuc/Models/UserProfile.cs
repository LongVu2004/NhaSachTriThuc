using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NhaSachTriThuc.Models
{
    public class UserProfile
    {
        [Key]
        public string UserId { get; set; } // khóa chính, đồng thời là khóa ngoại tới AspNetUsers

        [Required]
        public string FullName { get; set; }

        public string? Avatar { get; set; }

        [ForeignKey("UserId")]
        public IdentityUser User { get; set; }
    }

}
