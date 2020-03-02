using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UserAPI.Dtos
{
    public class AppUserRegisterDto
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [StringLength(20, MinimumLength =8, ErrorMessage ="Password shoud be at least 8 characters long")]
        public string Password { get; set; }
    }
}
