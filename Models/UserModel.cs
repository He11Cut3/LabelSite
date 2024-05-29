using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace LabelSite.Models
{
    public class UserModel
    {
        public long Id { get; set; }

        [Required, MinLength(2, ErrorMessage = "Минимальное кол-во символов - 4")]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        public byte[] UserPhoto { get; set; }

        [DataType(DataType.Password), Required, MinLength(4, ErrorMessage = "Минимальное кол-во символов - 4")]


        public string Password { get; set; }
    }
}
