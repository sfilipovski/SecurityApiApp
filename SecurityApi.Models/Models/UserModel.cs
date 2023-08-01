using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SecurityApi.Model.Models
{
    public class UserModel
    {
        [Required]
        [MinLength(2)]
        [MaxLength(50)]
        public string FirstName { get; set; }
        [Required]
        [MinLength(2)]
        [MaxLength(50)]
        public string LastName { get; set; }
        [Required]
        [MinLength(2)]
        [MaxLength(50)]
        public string Username { get; set; }
        [Required]
        public string Role { get; set; }
        [Required]
        public string PasswordHash { get; set; }
    }
}
