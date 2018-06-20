using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace Shopping.Models
{
    
    public class UserPassModel
    {
        [Required]
        [MinLength(5, ErrorMessage = "Password must be at least 5 characters long")]
        [MaxLength(15, ErrorMessage = "Password cannot be longer than 15 characters")]
        [RegularExpression("^(?=.{5,15}$)(?![_.])(?!.*[_.]{2})[a-zA-Z0-9._]+(?<![_.])$", ErrorMessage = "Cannot contain _ or . ar begining or end and can only contain leters, numbers, ., and _")]
        public string username { get; set; }

        [Required]
        [MinLength(5, ErrorMessage = "Password must be at least 5 characters long")]
        [MaxLength(15, ErrorMessage = "Password cannot be longer than 15 characters")]
        [RegularExpression("^(?=.{5,15}$)(?![_.])(?!.*[_.]{2})[a-zA-Z0-9._]+(?<![_.])$", ErrorMessage = "Cannot contain _ or . ar begining or end and can only contain leters, numbers, ., and _")]
        public string password { get; set; }
    }
}
