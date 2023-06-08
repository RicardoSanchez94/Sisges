using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SistemaGestion.Models.ViewModels
{
    public class ChangePassView
    {
        public string loginName { get; set; }
        [Display(Name = "INGRESE SU ACTUAL CONTRASEÑA")]
        public string oldPass { get; set; }
        [Display(Name = "INGRESE SU NUEVA CONTRASEÑA")]
        public string newPass { get; set; }
        [Display(Name = "REPITA SU NUEVA CONTRASEÑA")]
        public string repeatNewPass { get; set; }
    }
}