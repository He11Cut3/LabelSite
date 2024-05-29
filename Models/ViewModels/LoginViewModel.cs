using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace LabelSite.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required, MinLength(4, ErrorMessage = "Минимальная длина - 4 символа")]
        [Display(Name = "Username")]
        public string Email { get; set; }

        [DataType(DataType.Password), Required, MinLength(4, ErrorMessage = "Минимальная длина - 4 символа")]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }
    }
}
