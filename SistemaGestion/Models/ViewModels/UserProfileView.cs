using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SistemaGestion.Models.ViewModels
{
    public class UserProfileView
    {
        [Key]
        public Guid idUsuario { get; set; }
        public int idRol { get; set; }
    
        [Required(ErrorMessage = " ")]
        [Display(Name = "Rol")]
        public string RolNombre { get; set; }
        public bool? IsRoleActive { get; set; }
      
        [Display(Name = "Nombre Usuario")]
        public string LoginName { get; set; }
      
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Nombre")]
        public string nombres { get; set; }

        [Display(Name = "Apellido Materno")]
        public string ApellidoMaterno { get; set; }
     

        [Display(Name = "Apellido Paterno")]
        public string ApellidoPaterno { get; set; }
  
      
        [Display(Name = "Rut")]
        public string rut { get; set; }


        [Display(Name = "Correo")]
        public string Correo { get; set; }

        public List<RolesDisponibles> roles { get; set; }
        public List<RolesDisponibles> rolesSeleccionados { get; set; }
    }
    public class RolesDisponibles
    {
        [Key]
        public int idRol { get; set; }
        [Required(ErrorMessage = "*")]
        [Display(Name = "Nombre")]
        public string RolNombre { get; set; }
        [Required(ErrorMessage = "*")]
        [Display(Name = "Descripción")]
        public string RolDescription { get; set; }
    }
    public class UsuariosRoles
    {
        public int? SelecRolID { get; set; }
        public IEnumerable<RolesDisponibles> UserRoleList { get; set; }
    }

    public class UserDataView
    {
        public IEnumerable<UserProfileView> UserProfile { get; set; }
        public UsuariosRoles UserRoles { get; set; }
    }
    public class RolesUsuario
    {
        //public int idUsuario { get; set; }
        public Guid idUsuario { get; set; }
        public List<RolesDisponibles> rolesUsuario { get; set; }
    }
}