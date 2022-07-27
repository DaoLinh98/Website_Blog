using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebBlog.Areas.Admin.Models
{
    public class ChangePasswordViewModel
    {
        [Key]
        public int AccountId { get; set; }
        [Display(Name ="mật khẩu hiện tại")]
        public string PasswordNow { get; set; }
        [Display(Name = "mật khẩu mới")]
        [Required(ErrorMessage ="Vui lòng nhập mật khẩu")]
        [MinLength(5,ErrorMessage ="mật khẩu tối thiểu 5 ký tự")]
        public string Password { get; set; }
        [Display(Name = "nhập lại mật khẩu ")]
        [MinLength(5, ErrorMessage = "mật khẩu tối thiểu 5 ký tự")]
        [Compare("Password",ErrorMessage ="mật khẩu không giống nhau")]
        public string ConfirmPassword { get; set; }


        


    }
}
