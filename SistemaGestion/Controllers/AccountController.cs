using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using SistemaGestion.Models.DAO;
using SistemaGestion.Models.Repositorios;
using SistemaGestion.Models.Security;
using SistemaGestion.Models.ViewModels;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace SistemaGestion.Controllers
{
    public class AccountController : Controller
    {
        #region Objects


        private RolesUsuario RU = new RolesUsuario();
        private AdminUsuarios ADU = new AdminUsuarios();
        private RolesDisponibles RD = new RolesDisponibles();
        private UserProfileView UPV = new UserProfileView();
        private Negocio NG = new Negocio();

        #endregion
        //GET: Account
        public ActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public ActionResult LogIn(UserLoginView ULV, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (ADU.ValidaUsuarioExista(ULV.LoginName) == false)
                {
                    ModelState.AddModelError("", "EL USUARIO INGRESADO NO ES VALIDO");
                }
                else
                {
                    ULV.idUsuario = ADU.GetUserID(ULV.LoginName);
                    var Json = JsonConvert.SerializeObject(ULV);
                    //FormsAuthentication.SetAuthCookie(ULV.LoginName, false);
                    FormsAuthentication.SetAuthCookie(Json, false);
                    var roles = ADU.GetRolesUser(ULV.LoginName);
                    //var identity = new ClaimsIdentity(DefaultAuthenticationTypes.ApplicationCookie);
                    //identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, ULV.idUsuario.ToString()));
                    //identity.AddClaim(new Claim(ClaimTypes.Name, ULV.LoginName));
                    //foreach (var role in roles.rolesUsuario)
                    //{
                    //    identity.AddClaim(new Claim(ClaimTypes.Role, role.RolNombre));
                    //}
                    //var principal = new ClaimsPrincipal(identity);
                    //var serializer = new JavaScriptSerializer();
                    //var userData = serializer.Serialize(ULV);
                    //var ticket = new FormsAuthenticationTicket(1, ULV.LoginName, DateTime.Now, DateTime.Now.AddMinutes(30), false, userData);
                    //var encryptedTicket = FormsAuthentication.Encrypt(ticket);
                    //var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                    //cookie.HttpOnly = true;
                    //Response.Cookies.Add(cookie);
                    //HttpContext.User = principal;



                    // SE VERIFICA SI CAMBIO PASSWORD
                    if (ADU.GetCambioPassword(ULV.LoginName))
                    {
                        if (ADU.GetPasswordUsuario(ULV.LoginName, ULV.Password))
                        {
                            //return RedirectToAction("Dash", "Admin");
                            //foreach (var item in roles.rolesUsuario.OrderBy(x => x.idRol))
                            //{

                            //    //                                Administrador
                            //    //Cuadratura
                            //    //Contraloria
                            //    //Zonal
                            //    //Tienda
                            //    //Tesoreria
                            //    //RRHH
                            //    switch (item.RolNombre)
                            //    {
                            //        case "Administrador":
                            //            return RedirectToAction("Dash", "Admin");
                            //            break;
                            //        case "Cuadratura":
                            //            return RedirectToAction("Dash", "Admin");
                            //            break;
                            //        case "Contraloria":
                            //            return RedirectToAction("nc", "MediosPago");
                            //            break;
                            //        case "Zonal":
                            //            return RedirectToAction("nc", "MediosPago");
                            //            break;                                    
                            //        case "Tesoreria":
                            //            return RedirectToAction("Index", "Tesoreria");
                            //            break;
                            //        case "RRHH":
                            //            return RedirectToAction("Dash", "Admin");
                            //            break;
                            //        case "Tienda":
                            //            return RedirectToAction("Sencillo", "Tienda");
                            //            break;
                            //        default:                                        
                            //            break;
                            //    }
                            //}

                            if (roles.rolesUsuario.Where(x => x.RolNombre.Equals("Administrador") || x.RolNombre.Equals("Cuadratura")).Any())
                            {
                                return RedirectToAction("Dash", "Admin");
                            }
                            //if (roles.rolesUsuario.Where(x => x.RolNombre.Equals("Cuadratura")).Any())
                            //{
                            //    return RedirectToAction("Dash", "Admin");
                            //}
                            if (roles.rolesUsuario.Where(x => x.RolNombre.Equals("Contraloria") || x.RolNombre.Equals("Zonal")).Any())
                            {
                                return RedirectToAction("nc", "MediosPago");
                            }
                            //if (roles.rolesUsuario.Where(x => x.RolNombre.Equals("Zonal")).Any())
                            //{
                            //    return RedirectToAction("nc", "MediosPago");
                            //}
                            if (roles.rolesUsuario.Where(x => x.RolNombre.Equals("Tesoreria")).Any())
                            {
                                return RedirectToAction("Index", "Tesoreria");
                            }
                            if (roles.rolesUsuario.Where(x => x.RolNombre.Equals("RRHH")).Any())
                            {
                                return RedirectToAction("SobranteFaltante", "Tienda");
                            }
                            if (roles.rolesUsuario.Where(x => x.RolNombre.Equals("Tienda")).Any())
                            {
                                return RedirectToAction("Sencillo", "Tienda");
                            }
                            if (roles.rolesUsuario.Where(x=>x.RolNombre.Equals("Existencia")).Any())
                            {
                                return RedirectToAction("NotaCredito", "Informes");
                            }
                            else
                            {
                                return RedirectToAction("Dash", "Admin");
                            }




                        }
                        else
                        {
                            ModelState.AddModelError("", "EL PASSWORD INGRESADO ES INCORRECTO");
                        }
                    }
                    else
                    {
                        if (ADU.ValidaPassInicial(ULV.LoginName, ULV.Password))
                        {
                            //var newLogiName = ULV.LoginName.Replace("-","");
                            var varPasoLoginame = Convert.ToBase64String(Encoding.UTF8.GetBytes(ULV.LoginName));
                            return RedirectToAction("changePass", "Account", new { obj = varPasoLoginame });
                        }
                        else
                        {
                            ModelState.AddModelError("", "EL PASSWORD INGRESADO NO ES VALIDO");
                        }
                    }
                }
            }
            // si ocurre algo mas, remostramos el formulario 
            ModelState.AddModelError("", "INGRESE USUARIO Y CONTRASEÑA VALIDOS");
            return View(ULV);
        }

        #region CAMBIO DE CONTRASEÑAS AL PRIMER INGRESO

        //OBTIENE EL FORMULARIO DEL PRIMER CAMBIO DE CONTRASEÑA AL PRIMER INGRESO 
        [HttpGet]
        public ActionResult changePass(string obj)
        {
            var loginname = Encoding.UTF8.GetString(Convert.FromBase64String(obj));
            return View(ADU.GetFormChangePass(loginname));
        }

        //SE INGRESA EL CAMBIO DE CONTRASEÑA
        [HttpPost]
        public ActionResult changePass(ChangePassView cambio)
        {
            var response = new ResponseModel();
            if (cambio.newPass == cambio.repeatNewPass)
            {
                response = ADU.SetPassUser(cambio);
            }
            else
            {
                response.error = true;
                response.respuesta = "LAS CONTRASEÑAS INGRESADAS NO COINCIDEN, VERIFIQUE.";
            }

            return Json(response);
        }

        #endregion

        #region CAMBIO DE CONTRASEÑA DE USUARIOS DESDE ADMINISTRADOR
        //OBTIENE EL FORMULARIO PARA EL CAMBIO DE CONTRASEÑA DESDE EL ADMINISTRADOR 
        [HttpGet]
        [AuthorizeRoles("Administrador")]
        public JsonResult changePassAdmin(string obj)
        {
            var loginname = Encoding.UTF8.GetString(Convert.FromBase64String(obj));

            var res = ADU.ResetPassWordAdmin(loginname);
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        //SE INGRESA EL CAMBIO DE CONTRASEÑA DE USUARIOS DESDE EL ADMINISTRADOR
        [HttpPost]
        public ActionResult changePassAdmin(ChangePassView cambio)
        {
            var response = new ResponseModel();
            if (cambio.newPass == cambio.repeatNewPass)
            {
                response = ADU.SetPassUser(cambio);
                response.redirect = "ManageUserPartial";
            }
            else
            {
                response.error = false;
                response.respuesta = "LAS CONTRASEÑAS INGRESADAS NO COINCIDEN, VERIFIQUE.";
            }

            return Json(response);
        }
        #endregion


        //OBTIENE LA LISTA DE ROLES POR USUARIO 
        public ActionResult ListaRolesUsuarioModal(string LogName)
        {
            return PartialView(ADU.GetRolesByLogiName(LogName));
        }

        public ActionResult homeRedirect()
        {
            string users = User.Identity.Name;
            var Deserialize = JsonConvert.DeserializeObject<UserLoginView>(users);
            var usuariorol = NG.GetUsuario(Deserialize.idUsuario);
            //return RedirectToAction("Dash", "Admin");
            if (usuariorol.Rol.Where(x => x.Nombre.Equals("Administrador") || x.Nombre.Equals("Cuadratura")).Any())
            {
                return RedirectToAction("Dash", "Admin");
            }
           
            if (usuariorol.Rol.Where(x => x.Nombre.Equals("Contraloria") || x.Nombre.Equals("Zonal")).Any())
            {
                return RedirectToAction("nc", "MediosPago");
            }
           
            if (usuariorol.Rol.Where(x => x.Nombre.Equals("Tesoreria")).Any())
            {
                return RedirectToAction("Index", "Tesoreria");
            }
            if (usuariorol.Rol.Where(x => x.Nombre.Equals("RRHH")).Any())
            {
                return RedirectToAction("SobranteFaltante", "Tienda");
            }
            if (usuariorol.Rol.Where(x => x.Nombre.Equals("Tienda")).Any())
            {
                return RedirectToAction("Sencillo", "Tienda");
            }
            if (usuariorol.Rol.Where(x => x.Nombre.Equals("Existencia")).Any())
            {
                return RedirectToAction("NotaCredito", "Informes");
            }
            else
            {
                return RedirectToAction("Dash", "Admin");
            }
        }

        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("LogIn", "Account");
        }

        public ActionResult CreateUsuario(Guid? idUsuario)
        {
            ViewBag.Title = "NUEVO USUARIO";
            UPV = ADU.GetForm((Guid?)idUsuario);
            if (UPV.idUsuario != null)
            {
                ViewBag.Title = "ACTUALIZANDO USUARIO";
            }
            return PartialView("_FormUsuario", UPV);
        }

        [HttpPost]
        public ActionResult SaveUser(UserProfileView user, int[] roles_seleccionados = null)
        {
            var res = new ResponseModel();
            try
            {

                if (ADU.validarRut(user.rut))
                {
                    if (roles_seleccionados != null)
                    {
                        res = ADU.SaveUser(user, roles_seleccionados);
                        //return Json(res);
                    }
                    else
                    {
                        res.error = false;
                        res.respuesta = "DEBE SELECCIONAR A LO MENOS 1 ROL";
                        //return Json(res);
                    }
                }
                else
                {
                    res.error = false;
                    res.respuesta = "RUT INVALIDO, VERIFIQUE";
                    //return Json(res);
                }
            }
            catch (Exception EX)
            {
                res.error = false;
                res.respuesta = EX.ToString();
                //return Json(res);
            }

            return Json(res);
        }

        public ActionResult UnAuthorized()
        {
            return View();
        }

    }
}