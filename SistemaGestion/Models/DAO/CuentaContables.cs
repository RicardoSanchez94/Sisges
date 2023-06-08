using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaGestion.Models.DAO
{
    public class CuentaContables
    {
        private SisGesEntities3 db = new SisGesEntities3();
        public List<Cuenta> ObtenerCuentasContables()
        {
            List<Cuenta> lst = new List<Cuenta>();
            lst = db.Cuenta.ToList();
            return lst;
        }

        public Cuenta ObtenerCuentasContablesID(int Codigo)
        {
            Cuenta cuenta = new Cuenta();
            cuenta = db.Cuenta.Where(x => x.Codigo == Codigo).SingleOrDefault();
            return cuenta;
        }

        public ResponseModel ModificarCuenta(Cuenta cuenta)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                using (var context = db.Database.BeginTransaction())
                {

                    Cuenta CuentaModify = db.Cuenta.Where(x => x.Codigo == cuenta.Codigo).SingleOrDefault();

                    CuentaModify.Estado = cuenta.Estado;
                    CuentaModify.Descripcion = cuenta.Descripcion;
                    CuentaModify.NumeroCuenta = cuenta.NumeroCuenta;

                    db.SaveChanges();
                    context.Commit();

                }
                response.respuesta = "Se Modifico Correctamente";
                response.error = false;
                return response;

            }
            catch (Exception ex)
            {
                response.error = true;
                response.respuesta = "Procedimiento de Modificacion No se genero " + ex.Message;
                return response;
            }
        }

        public ResponseModel InsertarCuentaContable (Cuenta cuenta)
        {
            ResponseModel response = new ResponseModel();
            Cuenta cuentainset = new Cuenta();
            try
            {
                using (var context = db.Database.BeginTransaction())
                {
                    
                        //cuentainset.Codigo = cuentainset.Codigo++;
                        cuentainset.Estado = true;
                        cuentainset.Descripcion = cuenta.Descripcion;
                        cuentainset.NumeroCuenta = cuenta.NumeroCuenta;

                        db.Cuenta.Add(cuentainset);
                        db.SaveChanges();
                        context.Commit();
                   
                   
                }
                response.respuesta = "Se Inserto Correctamente";
                response.error = false;
                return response;
            }
            catch (Exception ex)
            {

                response.error = true;
                response.respuesta = "Procedimiento de Inserccion No se genero " + ex.Message;
                return response;
            }
        }


    }
}