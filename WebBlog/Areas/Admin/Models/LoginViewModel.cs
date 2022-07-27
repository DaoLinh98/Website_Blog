using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebBlog.Areas.Admin.Models
{
    public class LoginViewModel
    {
        [Key]
        [MaxLength(50)]
        [Required(ErrorMessage ="Vui lòng nhập email")]
        [Display(Name = "Dia chi email")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage ="Vui long nhap email")]
        public string Email { get; set; }

        [Display(Name ="Mat Khau")]
        [Required(ErrorMessage ="Vui long nhap mat khau")]
        [MaxLength(30,ErrorMessage ="ky tu qua dai")]
        public string Password { get; set; }
    }
}
