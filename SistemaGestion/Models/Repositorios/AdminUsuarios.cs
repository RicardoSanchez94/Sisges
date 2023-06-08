using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using SistemaGestion.Models;
using SistemaGestion.Models.DAO;
using SistemaGestion.Models.ViewModels;

namespace SistemaGestion.Models.Repositorios
{
    public class AdminUsuarios
    {
        private SisGesEntities3 db;
        //private interfacesIntellecEntities db;

        public AdminUsuarios()
        {
            db = new SisGesEntities3();
        }

        //valida si existe el usuario
        public bool ValidaUsuarioExista(string loginName)
        {
            bool usuario = false;
            var userData = db.Persona.Where(z => z.Run.ToString().Replace(".", "") == loginName).FirstOrDefault();
            if (userData != null)
            {
                usuario = db.Usuario.Where(x => x.Id == userData.Id).Any();
            }
            return usuario;
        }

        // OBTIENE EL ID GUID DEL USUARIO
        public Guid GetUserID(string loginName)
        {

            var dataUser = db.Persona.Where(x => x.Run.ToString().Replace(".", "") == loginName.Replace(".", "")).FirstOrDefault();
            return dataUser.Id;
        }
        // OBTIENE EL PASS DEL USUARIO 
        public bool GetPasswordUsuario(string loginName, string pass)
        {
            //var usuario = db.Usuarios.Where(o => o.LoginName.ToLower().Equals(loginName));
            var idUser = GetUserID(loginName);
            var usuario = db.Usuario.Where(x => x.Id == idUser).FirstOrDefault();

            if (usuario != null)
            {
                var matched = BCrypt.Net.BCrypt.Verify(pass, usuario.Password);
                return matched == true ? true : false;
            }
            else
                return false;
        }

        //OBTIENE EL ESTADO DEL USUARIO, SI ES 0 ES QUE NO HA CAMBIADO SU CONTRASEÑA
        public bool GetCambioPassword(string loginName)
        {
            var id = GetUserID(loginName);
            var cambio = db.Usuario.Where(z => z.Id == id && z.Activo == true).Any();

            //if (cambio.Any())
            //{
            //    //SE ACTUALIZA EL ULTIMO INGRESO AL SISTEMA
            //    var usuario = db.Usuario.Where(o => o.LoginName.ToLower().Equals(loginName));
            //    usuario.FirstOrDefault().fechaUltimoIngreso = DateTime.Now;
            //    db.SaveChanges();
            //    return true;
            //}
            return cambio;
        }
        // valida que la contraseña por defecto al primer ingreso sea la correcta
        public bool ValidaPassInicial(string loginName, string password)
        {
            var id = GetUserID(loginName);
            var pass = (from p1 in db.Usuario where p1.Id == id && p1.Password.Replace(".", "") == password select p1).Any();
            return pass == true ? true : false;
        }

        //OBTIENE EL FORMULARIO PARA CAMBIO DE PASSWORD
        public ChangePassView GetFormChangePass(string loginName)
        {
            var changepass = new ChangePassView();

            changepass.loginName = loginName;

            return changepass;
        }
        //RESETEA LA CONTRASEÑA Y CAMBIA ESTADO DE CAMBIO DE CONTRASEÑA REALIZADO
        public ResponseModel SetPassUser(ChangePassView cambio)
        {
            var response = new ResponseModel();
            try
            {
                var idUser = GetUserID(cambio.loginName);
                var userCambio = db.Usuario.Where(x => x.Id == idUser).FirstOrDefault();

                var salt = BCrypt.Net.BCrypt.HashPassword(cambio.newPass, BCrypt.Net.BCrypt.GenerateSalt(12));


                userCambio.Password = salt;
                userCambio.Activo = true;

                db.SaveChanges();

                response.error = false;
                response.respuesta = "CONTRASEÑA CAMBIADA CORRECTAMENTE, REDIRECCIONADO...";
                return response;
            }
            catch (Exception)
            {

                response.error = false;
                response.respuesta = "LA CONTRASEÑA NO A SIDO CAMBIADA CORRECTAMENTE, INTENTELO NUEVAMENTE";
                return response;
            }
        }

        //SE RESETA LA CONTRASEÑA A LA POR DEFECTO DESDE EL ADMINISTRADOR
        public ResponseModel ResetPassWordAdmin(string loginName)
        {
            var respuesta = new ResponseModel();
            try
            {
                var usua = db.Persona.Where(x => x.Run.Trim() == loginName.Trim()).FirstOrDefault();

                usua.Usuario.Password = usua.Run;
                usua.Usuario.Activo = false;

                db.SaveChanges();

                respuesta.respuesta = "CONTRASEÑA RESETEADA CORRECTAMENTE.";
                respuesta.error = true;
                return respuesta;
            }
            catch (Exception)
            {
                respuesta.respuesta = "LA CONTRASEÑA NO PUDO SER RESETEADA, CONTACTE AL ADMINISTRADOR DE SISTEMA.";
                respuesta.error = false;
                return respuesta;
            }
        }

        public Persona GetPersona (string loginname)
        {
            var idUsu = GetUserID(loginname);
            var persona = db.Persona.Where(x => x.Id == idUsu).SingleOrDefault();
            return persona;
        }

        public RolesUsuario GetRolesUser(string loginName)
        {
            var roles = new RolesUsuario();
            var rolesLista = new List<RolesDisponibles>();
            var idUsu = GetUserID(loginName);
            var fn = db.Usuario.Where(x => x.Id == idUsu).FirstOrDefault();
            fn.Rol.ToList()
                .ForEach(item => rolesLista.Add(new RolesDisponibles()
                {
                    idRol = (int)item.Codigo,
                    RolNombre = item.Nombre
                }));
            roles.rolesUsuario = rolesLista;
            roles.idUsuario = GetUserID(loginName);
            return roles;
        }
        //OBTIENE O VALIDA QUE EL USUARIO ES ADMINISTRADOR DE SISTEMA
        public bool isAdmin(string loginName)
        {
            // obtengo el id de usuario
            var idUsu = GetUserID(loginName);
            var posee = db.fnUsuarioRol().Where(z => z.IdUsuario == idUsu && z.CodigoRol == 1).Any();
            return posee;
        }

        //OBTIENE O VALIDA QUE EL USUARIO APARTE DE SER ADMINISTRADOR SEA :EVALUADOR, EVALUADO Y/O REVISOR 
        public bool isAdminOrMore(string loginName)
        {
            // obtengo el id de usuario
            var idUsu = GetUserID(loginName);
            var posee = db.fnUsuarioRol().Where(z => z.IdUsuario == idUsu && z.CodigoRol != 1).Any();
            return posee;
        }


        public List<RolesDisponibles> GetAllRoles()
        {

            var roles = db.Rol.Select(o => new RolesDisponibles
            {
                idRol = o.Codigo,
                RolNombre = o.Nombre,
                RolDescription = o.Descripcion
            }).ToList();

            return roles;

        }

        public bool IsUserInRole(string loginName, string roleName)
        {

            var idUser = GetUserID(loginName);
            var SU = db.Usuario.Where(o => o.Id.Equals(idUser)).FirstOrDefault();
            if (SU != null)
            {
                //var roles = from q in db.fnUsuarioRol().ToList()
                //            join r in db.Rol on q.CodigoRol equals r.Codigo
                //            where r.Nombre == roleName && q.IdUsuario == SU.Id
                //            select r.Nombre;
                var roles = SU.Rol.ToList();

                if (roles != null && roles.Where(x=>x.Nombre.Equals(roleName)).Any())
                {
                    return roles.Any();
                }
            }
            return false;

        }


        //public void prueba(string login)
        //{
        //    var id = GetUserID(login);
        //    var data = db.Usuarios.Where(x => x.Id == id).FirstOrDefault().Rols.ToList();

        //    //var rol = data.Rols.ToList();


        //    var jajajaj = "";

        //    var dda = db.Rols.Where(x => x.Codigo == 1).FirstOrDefault();

        //    var user = dda.Usuarios.ToList();



        //    var jajajajajajajaj = "";
        //}

        //OBTIENE TODOS LOS DATOS DE PERFIL DE USUARIOS
        public List<UserProfileView> GetAllUserProfiles()
        {
            List<UserProfileView> profiles = new List<UserProfileView>();

            UserProfileView UPV;
            var users = db.Usuario.ToList();

            foreach (Usuario u in users)
            {
                UPV = new UserProfileView();
                UPV.idUsuario = u.Id;
                UPV.LoginName = u.Persona.Run;
                UPV.Password = u.Password;

                var SUP = db.Persona.Where(x => x.Id == u.Id).FirstOrDefault();
                if (SUP != null)
                {
                    UPV.idUsuario = SUP.Id;
                    UPV.nombres = SUP.Nombres;
                    UPV.ApellidoPaterno = SUP.ApellidoPaterno;
                    UPV.ApellidoMaterno = SUP.ApellidoMaterno;

                }


                var SUR = db.Usuario.Where(o => o.Id == u.Id).FirstOrDefault().Rol.ToList();
                if (SUR.Any())
                {
                    var userRole = SUR.FirstOrDefault();
                    UPV.idRol = (int)userRole.Codigo;
                    UPV.RolNombre = userRole.Nombre;
                    UPV.IsRoleActive = userRole.Activo;
                }

                profiles.Add(UPV);
            }

            return profiles;
        }

        // RETORNA LISTA DE PERFILES DE USUARIOS 
        public List<UserProfileView> GetUsersDataView(string loginName)
        {
            var perfiles = (from a in db.Usuario
                            join b in db.Persona
                            on a.Id equals b.Id

                            select new UserProfileView
                            {
                                idUsuario = a.Id,
                                LoginName = b.Run,
                                Password = a.Password,
                                nombres = b.Nombres,
                                ApellidoPaterno = b.ApellidoPaterno,
                                ApellidoMaterno = b.ApellidoMaterno
                                //Correo = b.correo
                            }

                      ).ToList();
            return perfiles;
        }

        //RETORNA INFORMACION DE UN USUARIO ESPECIFICO
        public UserProfileView GetUserDataPerfil(string loginName)
        {
            var perfil = (from a in db.Usuario
                          join b in db.Persona
                          on a.Id equals b.Id
                          where b.Run.Trim() == loginName.Trim()

                          select new UserProfileView
                          {
                              LoginName = b.Run,
                              Password = a.Password,
                              nombres = b.Nombres,
                              ApellidoPaterno = b.ApellidoPaterno,
                              ApellidoMaterno = b.ApellidoMaterno,
                              //Correo = b.correo,
                              //nomCargo = b.cargo
                          }

                      ).FirstOrDefault();
            return perfil;
        }

        //FORMULARIO DE CREACION DE USUARIOS 
        public UserProfileView GetForm(Guid? idUsuario)
        {
            var user = new UserProfileView();
            if (idUsuario == null)
            {
                user.roles = GetAllRoles();
                return user;
            }
            else
            {
                var datos = db.Persona.Where(x => x.Id == idUsuario).FirstOrDefault();
                user.idUsuario = datos.Id;
                user.rut = datos.Run;
                user.nombres = datos.Nombres;
                user.ApellidoPaterno = datos.ApellidoPaterno;
                user.ApellidoMaterno = datos.ApellidoMaterno;
                user.roles = GetAllRoles();
                user.rolesSeleccionados = GetRolesByLogiName(datos.Run);
                return user;
            }
        }

        public RolesDisponibles GetFormRoles(int? idUsuario)
        {
            var roles = new RolesDisponibles();
            return roles;
        }

        //  CREACIÓN DE USUARIOS Y ROLES
        public ResponseModel SaveUser(UserProfileView user, int[] roles_seleccionados)
        {
            var response = new ResponseModel();
            response.error = false;
            response.respuesta = "OCURRIO UN ERROR AL INTENTAR CREAR EL USUARIO";

            List<RolesDisponibles> rolesTomados = new List<RolesDisponibles>();

            try
            {
                foreach (var items in roles_seleccionados)
                {
                    rolesTomados.Add(new RolesDisponibles()
                    {
                        idRol = items
                    });
                }
                user.rolesSeleccionados = rolesTomados;
                if (user.idUsuario == Guid.Empty)
                {
                    if (ValidaUsuarioExista(user.rut) == false)
                    {
                        var NewGuid = Guid.NewGuid();
                        //primero se crea a la persona
                        Persona perUsu = (
                           new Persona
                           {
                               Id = NewGuid,
                               Run = user.rut,
                               Nombres = user.nombres,
                               ApellidoPaterno = user.ApellidoPaterno,
                               ApellidoMaterno = user.ApellidoMaterno,
                               RunCuerpo = Convert.ToInt32(user.rut.Split('-')[0]),
                               RunDigito = user.rut.Split('-')[1].ToString()
                           });
                        db.Persona.Add(perUsu);
                        db.SaveChanges();

                        Usuario usu = (new Usuario
                        {
                            Id = NewGuid,
                            Password = user.rut,
                            FechaCreacion = DateTime.Now.ToString("dd-MM-yyyy"),
                            Activo = false
                        });

                        foreach (int i in roles_seleccionados)
                        {
                            Rol r = db.Rol.Find(i);
                            usu.Rol.Add(r);

                        }
                        db.Usuario.Add(usu);
                        db.SaveChanges();

                        response.error = false;
                        response.respuesta = "USUARIO CREADO CORRECTAMENTE";
                        return response;
                    }
                    else
                    {
                        response.error = true;
                        response.respuesta = "EL USUARIO YA EXISTE, INTENTE CON OTRO";
                        return response;
                    }
                }
                else
                {
                    // ACTUALIZA DATOS DEL USUARIO
                    Persona pusuario = db.Persona.Where(v => v.Id == user.idUsuario).FirstOrDefault();
                    pusuario.Nombres = user.nombres;
                    pusuario.ApellidoPaterno = user.ApellidoPaterno;
                    pusuario.ApellidoMaterno = user.ApellidoMaterno;

                    var userRols = db.Usuario.Where(x => x.Id == user.idUsuario).FirstOrDefault();

                    foreach (var a in userRols.Rol.ToList())
                    {
                        userRols.Rol.Remove(a);
                    }

                    foreach (int i in roles_seleccionados)
                    {
                        Rol r = db.Rol.Find(i);
                        pusuario.Usuario.Rol.Add(r);

                    }
                    db.SaveChanges();
                    response.error = false;
                    response.respuesta = "SE ACTUALIZARON LOS DATOS DEL USUARIO";
                    return response;
                }
            }
            catch (Exception EX)
            {

                return response;
            }

        }

        public ResponseModel SaveRoles(RolesDisponibles rols)
        {
            var respon = new ResponseModel();

            try
            {
                Rol rolo = new Rol();
                rolo.Nombre = rols.RolNombre;
                rolo.Descripcion = rols.RolDescription;
                //rolo.fechaCreacion = DateTime.Now.Date;
                rolo.Activo = true;

                db.Rol.Add(rolo);

                db.SaveChanges();
                respon.error = true;
                respon.respuesta = "ROL CREADO CORRECTAMENTE";
                return respon;
            }
            catch (Exception)
            {
                respon.error = false;
                respon.respuesta = "OCURRIO UN ERROR AL INTENTAR CREAR EL ROL, ROL NO CREADO";
                return respon;
            }

        }

        //public bool ValidaExisteUsuario(string loginame)
        //{
        //    var res = db.Usuarios.Where(z => z.LoginName.Trim() == loginame.Trim()).Any();
        //    return res;
        //}




        //OBTIENE LA LISTA DE ROLES POR USUARIO MEDIANTE LOGINNAME
        public List<RolesDisponibles> GetRolesByLogiName(string logiName)
        {
            var idUsuario = GetUserID(logiName.Trim());

            var roles = new List<RolesDisponibles>();
            db.Usuario.Where(x => x.Id == idUsuario).FirstOrDefault()
              .Rol.ToList()
              .ForEach(item => roles.Add(new RolesDisponibles()
              {
                  idRol = item.Codigo,
                  RolDescription = item.Descripcion,
                  RolNombre = item.Nombre
              }));



            //var roles = (from a in db.Usuario.ToList()
            //             where
            //                 a.Id == idUsuario
            //             select new RolesDisponibles
            //             {
            //                 idRol = (int)a.Rol,
            //                 RolDescription = a.Rol.descRol,
            //                 RolNombre = a.Rol.nombreRol
            //             }).ToList();
            return roles;
        }


        public bool validarRut(string rut)
        {

            bool validacion = false;
            try
            {
                rut = rut.ToUpper();
                rut = rut.Replace(".", "");
                rut = rut.Replace("-", "");
                int rutAux = int.Parse(rut.Substring(0, rut.Length - 1));

                char dv = char.Parse(rut.Substring(rut.Length - 1, 1));

                int m = 0, s = 1;
                for (; rutAux != 0; rutAux /= 10)
                {
                    s = (s + rutAux % 10 * (9 - m++ % 6)) % 11;
                }
                if (dv == (char)(s != 0 ? s + 47 : 75))
                {
                    validacion = true;
                }
            }
            catch (Exception)
            {

            }
            return validacion;
        }

        public bool isValidGUID(String str)
        {
            // Regex to check valid
            // GUID (Globally Unique Identifier)
            String regex
                = "^[{]?[0-9a-fA-F]{8}"
                  + "-([0-9a-fA-F]{4}-)"
                  + "{3}[0-9a-fA-F]{12}[}]?$";
            var ok = Regex.IsMatch(str, regex);
            return ok;

        }


        //////////////////////////////////////////////////////////////////
        ///  ADMINISTRACION DE ZONAS ZONALES Y TIENDAS
        ///  

        #region ASIGNACION DE ZONAS A USUARIOS ZONALES

        public List<zonas> getZonas()
        {
            return (from p in db.ZONA.ToList()
                    where p.Estado == true
                    select new zonas()
                    {
                        idZona = p.IdZona,
                        desZona = p.nombreZona,

                    }).ToList();
        }
        public adminZonalView getZonasByUser(Guid idUser)
        {
            var model = new adminZonalView();

            model.listZonas = getZonas();


            db.Usuario.Where(x => x.Id == idUser).FirstOrDefault().ZONA
                .ToList()
                .ForEach(z => model.listZonasSeleccionadas.Add(new zonas()
                {
                    idZona = z.IdZona,
                    desZona = z.nombreZona
                }));

            return model;
        }

        public List<UserProfileView> GetUsersRolZona()
        {
            var perfiles = (from a in db.Usuario
                            join b in db.Persona
                            on a.Id equals b.Id
                            where a.Activo == true && a.Rol.Where(x => x.Nombre == "Zonal").Any()
                            select new UserProfileView
                            {
                                idUsuario = a.Id,
                                nombres = b.Nombres + " " + b.ApellidoPaterno + " " + b.ApellidoMaterno
                            }

                      ).ToList();
            return perfiles;
        }

        public adminZonalView GetZonasUSer()
        {
            var model = new adminZonalView();

            model.listUserRolZonal = GetUsersRolZona();
            model.listZonas = getZonas();
            //model.listZonasSeleccionadas = getZonasByUser();

            return model;


        }

        public ResponseModel saveAsigZona(adminZonalView user, int[] zonas_seleccionadas = null)
        {
            var response = new ResponseModel();
            try
            {
                var users = db.Usuario.Where(x => x.Id == user.idUsuario).FirstOrDefault();

                // SE VALIDA QUE OTRO USUARIO NO TENGA LA ZONA ASIGNADA
                bool asignada = false;

                foreach (int i in zonas_seleccionadas)
                {
                    var data = db.ZONA.Where(x => x.IdZona == i).FirstOrDefault().Usuario.ToList();
                    if(data.Where(x=> x.Id != user.idUsuario).Any())
                    {
                        response.error = true;
                        response.respuesta = "LA ZONA '"+ data.FirstOrDefault().ZONA.Where(m=> m.IdZona== i).FirstOrDefault().nombreZona + "' YA ESTA ASIGANDA AL USUARIO "
                                                        + data.FirstOrDefault().Persona.Nombre;
                        asignada = true;
                        break;
                    }
                }

                if (!asignada)
                {
                    // SE QUITAN LAS ZONAS DEL USUARIO
                    foreach (var a in users.ZONA.ToList())
                    {
                        users.ZONA.Remove(a);
                    }
                    // SE ASIGNAN LAS NUEVAS ZONAS
                    foreach (int i in zonas_seleccionadas)
                    {
                        ZONA z = db.ZONA.Find(i);
                        users.ZONA.Add(z);
                    }
                    db.SaveChanges();
                    response.error = false;
                    response.respuesta = "ZONA(S) ASIGNADA(S) CORRECTAMENTE";
                }



            }
            catch (Exception EX)
            {
                response.error = true;
                response.respuesta = "OCURRIO UN ERROR AL INTENTAR ASIGNAR LA(S) ZONA(S): " + EX.Message.ToString();
            }

            return response;
        }



        #endregion

    }
}