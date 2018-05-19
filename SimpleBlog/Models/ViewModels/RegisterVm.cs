using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SimpleBlog.Models.ViewModels
{
    public class RegisterVm
    {
        [Required(ErrorMessage = "Please enter Email")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter your password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please re-enter your password")]
        [DataType(DataType.Password)]
        [Compare("Password",ErrorMessage = "Password and Re-Password does not match")]
        public string RePassword { get; set; }

        [Required(ErrorMessage = "Please enter password")]
        public string FullName { get; set; }

        public string Gender { get; set; }

        public string ProfileImageLocation { get; set; }
        
        [DataType(DataType.Date)]
        public DateTime Dob { get; set; }

        public bool IsAgreeTermsAndCondition { get; set; }
    }
}