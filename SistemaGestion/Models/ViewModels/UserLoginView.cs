using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SistemaGestion.Models.ViewModels
{
    public class UserLoginView
    {
        [Key]
        public Guid idUsuario { get; set; }

        [Required(ErrorMessage = " ")]
        [Display(Name = "Nombre Usuario")]
        public string LoginName { get; set; }

        [Required(ErrorMessage = " ")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }
}