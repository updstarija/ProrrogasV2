using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Prorrogas.Models;

namespace Prorrogas.Controllers
{
    public class TipoProrrogaController : Controller
    {
        // GET: TipoProrroga
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                ProrrogasEntities db = new ProrrogasEntities();
                var rol = db.Usuario.Single(x => x.usuario1 == User.Identity.Name).Rol.nombre;
                if (rol == "Administrador" || rol == "Encargado")
                {
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        public ActionResult Actualizar()
        {
            ProrrogasEntities db = new ProrrogasEntities();
            var btn = "";
            var btnEstado = "";
            string estado = "";
            var lista = db.TipoProrroga.ToList();
            List<object[]> tabla = new List<object[]>();
            int i = 1;
            foreach (var item in lista)
            {
                if (item.estado == false)
                {
                    estado = "Deshabilitada";
                    btnEstado = "<button  type='button'class='btn btn-sm btn-link' onclick='Habilitar(" + item.id + ")'><i class='fas fa-check'></i></button></div>";
                }
                else
                {
                    estado = "Habilitada";
                    btnEstado = "<button  type='button'class='btn btn-sm btn-link' onclick='Deshabilitar(" + item.id + ")'><i class='fas fa-times'></i></button></div>";
                }
                btn = "<div style='float:left'><button  type='button'class='btn btn-sm btn-link' data-bs-toggle='modal' data-bs-target='#modalTipos' onclick='Editar(" + item.id + ")'><i class='fa fa-pen'></i></button>";
                btn += btnEstado;
                object[] obj = { i, item.nombre, item.descripcion, estado, btn };
                tabla.Add(obj);
                i++;
            }
            return Json(tabla, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarSelect()
        {
            ProrrogasEntities db = new ProrrogasEntities();
            List<object> lista = new List<object>();
            foreach (var item in db.TipoProrroga.OrderBy(x => x.nombre).Where(x => x.estado == true))
            {
                object o = new { id = item.id, nombre = item.nombre.ToUpper() };
                lista.Add(o);
            }
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Guardar(TipoProrroga c)
        {
            ProrrogasEntities db = new ProrrogasEntities();
            Status s = new Status();
            try
            {
                if (c.nombre == "" || c.nombre == null)
                {
                    s.Tipo = 2;
                    s.Msj = "Debe asignar un nombre";
                }
                if (s.Msj == null)
                {
                    if (c.id == 0)
                    {
                        var existeTipoProrroga = db.TipoProrroga.SingleOrDefault(x => x.nombre == c.nombre);
                        if (existeTipoProrroga == null)
                        {
                            c.estado = true;
                            db.TipoProrroga.Add(c);
                            db.SaveChanges();
                            s.Tipo = 1;
                            s.Msj = "Tipo de prórroga registrada";
                        }
                        else
                        {
                            s.Tipo = 2;
                            s.Msj = "El tipo de prórroga ya existe";
                        }
                    }
                    else
                    {
                        var existeTipoProrroga = db.TipoProrroga.SingleOrDefault(x => x.nombre == c.nombre && x.id != c.id);
                        if (existeTipoProrroga == null)
                        {
                            var cat = db.TipoProrroga.SingleOrDefault(x => x.id == c.id);
                            cat.nombre = c.nombre;
                            cat.descripcion = c.descripcion;
                            db.SaveChanges();
                            s.Tipo = 1;
                            s.Msj = "Tipo de prórroga modificada";
                        }
                        else
                        {
                            s.Tipo = 2;
                            s.Msj = "El tipo de prórroga ya existe";
                        }
                    }
                }

            }
            catch (Exception)
            {
                s.Tipo = 3;
                s.Msj = "Se produjo un error comuniquese con el administrador";
            }
            return Json(s, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Editar(int id)
        {
            ProrrogasEntities db = new ProrrogasEntities();
            var c = db.TipoProrroga.SingleOrDefault(x => x.id == id);
            object o = new { id = c.id, nombre = c.nombre, descripcion = c.descripcion };
            return Json(o, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Deshabilitar(int id)
        {
            ProrrogasEntities db = new ProrrogasEntities();
            Status s = new Status();
            try
            {
                var c = db.TipoProrroga.SingleOrDefault(x => x.id == id);
                c.estado = false;
                db.SaveChanges();
                s.Tipo = 1;
                s.Msj = "Tipo de prórroga deshabilitada";
            }
            catch
            {
                s.Tipo = 3;
                s.Msj = "Se produjo un error comuniquese con el administrador";
            }
            return Json(s, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Habilitar(int id)
        {
            ProrrogasEntities db = new ProrrogasEntities();
            Status s = new Status();
            try
            {
                var c = db.TipoProrroga.SingleOrDefault(x => x.id == id);
                c.estado = true;
                db.SaveChanges();
                s.Tipo = 1;
                s.Msj = "Tipo de prórroga habilitada";
            }
            catch
            {
                s.Tipo = 3;
                s.Msj = "Se produjo un error comuniquese con el administrador";
            }
            return Json(s, JsonRequestBehavior.AllowGet);
        }
    }
}