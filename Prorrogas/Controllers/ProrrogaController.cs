using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Prorrogas.Models;

namespace Prorrogas.Controllers
{
    public class ProrrogaController : Controller
    {
        // GET: Prorroga
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                ProrrogasEntities db = new ProrrogasEntities();
                var rol = db.Usuario.Single(x => x.usuario1 == User.Identity.Name).Rol.nombre;
                if (rol == "Administrador" || rol == "Encargado" || rol == "Auxiliar" || rol == "Ventanillas")
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

        public ActionResult Actualizar(int idModulo)
        {
            ProrrogasEntities db = new ProrrogasEntities();
            SAADSTJEntities dbS = new SAADSTJEntities();
            string rol = db.Usuario.Single(x => x.usuario1 == User.Identity.Name).Rol.nombre;
            string btnImprimir = "";
            string btnExtender = "";
            string btnAnular = "";
            string btnEliminar = "";
            var lista = db.Prorroga.OrderByDescending(x=> x.fechaRegistro).Where(x=> x.idEstado <= 2 && x.idModulo == idModulo).ToList();
            List<object[]> tabla = new List<object[]>();
            int i = 1;
            foreach (var item in lista)
            {
                btnImprimir = "<div style='float:left'><button type='button'class='btn btn-opcion-imprimir' data-bs-toggle='tooltip' data-bs-placement='top' title='Imprimir comprobante' onclick='ImprimirComprobante(" + item.id + ")'><i class='fas fa-file-pdf'></i></button></div>";
                btnExtender = "<div style='float:left'><button type='button'class='btn btn-opcion-extender' data-bs-toggle='tooltip' data-bs-placement='top' title='Extender prórroga' onclick='Extender(" + item.id + ")'><i class='fa fa-calendar-plus'></i></button></div>";    
                btnAnular = "<div style='float:left'><button type='button'class='btn btn-opcion-inactivar' data-bs-toggle='tooltip' data-bs-placement='bottom' title='Inactivar prórroga' onclick='Inactivar(" + item.id + ")'><i class='fas fa-times'></i></button></div>";
                if (rol == "Administrador" || rol == "Encargado")
                {
                    btnEliminar = "<div style='float:left'><button type='button'class='btn btn-opcion-eliminar' data-bs-toggle='tooltip' data-bs-placement='bottom' title='Eliminar o anular prórroga' onclick='Anular(" + item.id + ")'><i class='fas fa-trash-alt'></i></button></div>";
                }
                switch (item.idEstado)
                {
                    case 1:
                        btnImprimir += btnExtender + btnAnular + btnEliminar;
                        break;
                    case 2:
                        btnImprimir += btnExtender + btnEliminar;
                        break;
                }
                string carrera = dbS.PlanEstudio.Single(x => x.Id == item.idCarrera).Nombre;
                var p = dbS.Persona.Single(x => x.Id == item.idEstudiante);
                var grupo = dbS.GrupoAperturado.Single(x => x.Id == item.idGrupo);
                string fechaFinalizacion = item.fechaFin.ToShortDateString();
                if (item.fechaExtension != null)
                {
                    fechaFinalizacion = Convert.ToDateTime(item.fechaExtension).ToString("dd/MM/yyyy") + " <span data-bs-toggle='tooltip' data-bs-placement='top' title='Prórroga extendida'><i class='fas fa-hourglass-start'></i></span>";
                }
                object[] obj = { i, item.fechaRegistro.ToShortDateString(), carrera, p.Nombre + " " + p.ApellidoPaterno + " " + p.ApellidoMaterno, item.TipoProrroga.nombre, grupo.MateriaContenido.Nombre, grupo.Nombre, fechaFinalizacion ,item.EstadoProrroga.nombre, btnImprimir };
                tabla.Add(obj);
                i++;
            }
            return Json(tabla, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BuscarEstudiante(string ci)
        {
            SAADSTJEntities db = new SAADSTJEntities();
            List<object> lista = new List<object>();
            var estudiantes = db.Persona.SqlQuery("select top 5 p.* from Persona p, Estudiante e where p.Id = e.Id and p.DocumentoIdentidad like '%" + ci + "%'").ToList();
            foreach (var item in estudiantes)
            {
                string nombre = item.Nombre + " " + item.ApellidoPaterno + " " + item.ApellidoMaterno;
                object o = new { id = item.Id, nombreCompleto = nombre };
                lista.Add(o);
            }
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SeleccionarEstudiante(int idPersona) 
        {
            SAADSTJEntities db = new SAADSTJEntities();
            var p = db.Persona.Single(x => x.Id == idPersona);
            object o = new
            {
                nombre = p.Nombre,
                aPaterno = p.ApellidoPaterno,
                aMaterno = p.ApellidoMaterno,
                ci = p.DocumentoIdentidad,
                celular = p.Celular,
                correo = p.Estudiante.EmailOffice365
            };
            return Json(o, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CarrerasEstudiante(int idPersona)
        {
            SAADSTJEntities db = new SAADSTJEntities();
            var p = db.InscripcionCarrera.Where(x => x.Estudiante.Id == idPersona);
            List<object> Carreras = new List<object>();
            foreach (var item in p)
            {
                object o = new
                {
                    id = item.PlanEstudio.Id,
                    nombre = item.PlanEstudio.Nombre
                };
                Carreras.Add(o);
            }
            return Json(Carreras, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BuscarMateria(string nombre, int idModulo)
        {
            SAADSTJEntities db = new SAADSTJEntities();
            List<object> lista = new List<object>();
            var grupoAperturado = db.GrupoAperturado.SqlQuery("select ga.* from GrupoAperturado ga, OfertaSistemaEstudio ose, Oferta o, MateriaContenido mc where ga.IdOferta = ose.IdOferta and ga.IdSistemaEstudio = ose.IdSistemaEstudio and o.Id = ose.IdOferta and mc.Id = ga.IdMateriaContenido and o.Id = " + idModulo + " and mc.Nombre like '%"+ nombre +"%' order by ga.nombre").ToList();
            foreach (var item in grupoAperturado)
            {
                string materia = db.MateriaContenido.Single(x=> x.Id == item.IdMateriaContenido).Nombre;
                string turno = db.Turno.Single(x => x.Id == item.IdTurno).Nombre;
                string aula = db.HorarioGrupo.First(x => x.IdGrupoAperturado == item.Id).Aula.Nombre;
                string cupo = (item.CupoOfertado - item.CupoDisponible) + "/" + item.CupoOfertado;
                object o = new 
                { 
                    id = item.Id, 
                    nombre = item.Nombre,
                    materia = materia, 
                    turno = turno,
                    aula = aula,
                    cupo = cupo
                };
                lista.Add(o);
            }
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SeleccionarMateria(int idGrupo)
        {
            SAADSTJEntities db = new SAADSTJEntities();
            var ga = db.GrupoAperturado.Single(x => x.Id == idGrupo);
            string materia = db.MateriaContenido.Single(x => x.Id == ga.IdMateriaContenido).Nombre;
            string turno = db.Turno.Single(x => x.Id == ga.IdTurno).Nombre;
            string aula = db.HorarioGrupo.First(x => x.IdGrupoAperturado == ga.Id).Aula.Nombre;
            string cupo = (ga.CupoOfertado - ga.CupoDisponible) + "/" + ga.CupoOfertado;
            object o = new
            {
                id = ga.Id,
                nombre = ga.Nombre,
                materia = materia,
                turno = turno,
                aula = aula,
                cupo = cupo
            };
            return Json(o, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Guardar(Prorroga c)
        {
            ProrrogasEntities db = new ProrrogasEntities();
            Status s = new Status();
            try
            {
                if (c.fechaFin < c.fechaInicio)
                {
                    s.Tipo = 2;
                    s.Msj = "La fecha final debe ser mayor a la fecha inicial";
                }
                if (c.fechaFin == null || c.fechaFin.ToString() == "01/01/0001 0:00:00")
                {
                    s.Tipo = 2;
                    s.Msj = "Debe determinar una fecha final";
                }
                if (c.fechaInicio == null || c.fechaInicio.ToString() == "01/01/0001 0:00:00")
                {
                    s.Tipo = 2;
                    s.Msj = "Debe determinar una fecha final";
                }
                if (c.idGrupo == 0)
                {
                    s.Tipo = 2;
                    s.Msj = "Debe seleccionar un grupo";
                }
                if (c.idTipoProrroga == 0)
                {
                    s.Tipo = 2;
                    s.Msj = "Debe seleccionar un tipo de prórroga";
                }
                if (c.idCarrera == 0)
                {
                    s.Tipo = 2;
                    s.Msj = "Debe seleccionar una carrera";
                }
                if (c.idEstudiante == 0)
                {
                    s.Tipo = 2;
                    s.Msj = "Debe seleccionar un estudiante";
                }
                if (s.Msj == null)
                {
                    if (c.id == 0)
                    {
                        var existeProrroga = db.Prorroga.SingleOrDefault(x => x.idEstudiante == c.idEstudiante && x.idGrupo == c.idGrupo && x.idCarrera == c.idCarrera);
                        if (existeProrroga == null)
                        {
                            c.fechaRegistro = DateTime.Now;
                            int idUsuario = db.Usuario.Single(x => x.usuario1 == User.Identity.Name).id;
                            c.idUsuario = idUsuario;
                            c.idEstado = 1;
                            db.Prorroga.Add(c);
                            db.SaveChanges();
                            s.Id = c.id;
                            s.Tipo = 1;
                            s.Msj = "Prórroga registrada";
                        }
                        else
                        {
                            s.Tipo = 2;
                            s.Msj = "La prórroga ya existe";
                        }
                    }
                    else
                    {
                        var existeProrroga = db.Prorroga.SingleOrDefault(x => x.idEstudiante == c.idEstudiante && x.idGrupo == c.idGrupo && x.idCarrera == c.idCarrera && x.id != c.id);
                        if (existeProrroga == null)
                        {
                            var cat = db.Prorroga.SingleOrDefault(x => x.id == c.id);
                            cat.fechaFin = c.fechaFin;
                            cat.idEstudiante = c.idEstudiante;
                            cat.idCarrera = c.idCarrera;
                            cat.idGrupo = c.idGrupo;
                            cat.idTipoProrroga = c.idTipoProrroga;
                            db.SaveChanges();
                            s.Tipo = 1;
                            s.Msj = "Prórroga modificada";
                        }
                        else
                        {
                            s.Tipo = 2;
                            s.Msj = "La prórroga ya existe";
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

        public ActionResult Anular(int id)
        {
            ProrrogasEntities db = new ProrrogasEntities();
            Status s = new Status();
            try
            {
                var c = db.Prorroga.Single(x => x.id == id);
                c.idEstado = 3;
                db.SaveChanges();
                s.Tipo = 1;
                s.Msj = "Prórroga anulada";
            }
            catch
            {
                s.Tipo = 3;
                s.Msj = "Se produjo un error comuniquese con el administrador";
            }
            return Json(s, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Inactivar(int id)
        {
            ProrrogasEntities db = new ProrrogasEntities();
            Status s = new Status();
            try
            {
                var c = db.Prorroga.Single(x => x.id == id);
                c.idEstado = 2;
                db.SaveChanges();
                s.Tipo = 1;
                s.Msj = "Prórroga dada de baja";
            }
            catch
            {
                s.Tipo = 3;
                s.Msj = "Se produjo un error comuniquese con el administrador";
            }
            return Json(s, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MostrarModulos()
        {
            SAADSTJEntities db = new SAADSTJEntities();
            var modulos = db.Oferta.SqlQuery("select top 6 * from Oferta order by Id desc");
            List<object> lista = new List<object>();
            foreach (var item in modulos)
            {
                object o = new { id = item.Id, nombre = item.Descripcion };
                lista.Add(o);
            }
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MostrarProrroga(int idProrroga)
        {
            ProrrogasEntities db = new ProrrogasEntities();
            SAADSTJEntities dbS = new SAADSTJEntities();
            var p = db.Prorroga.Single(x => x.id == idProrroga);
            var ca = dbS.PlanEstudio.Single(x => x.Id == p.idCarrera);
            var g = dbS.GrupoAperturado.Single(x => x.Id == p.idGrupo);
            var horario = dbS.HorarioGrupo.First(x => x.IdGrupoAperturado == g.Id);
            string fechaFinalizacion = p.fechaFin.ToShortDateString();
            if (p.fechaExtension != null)
            {
                fechaFinalizacion = Convert.ToDateTime(p.fechaExtension).ToString("dd/MM/yyyy") + " <span data-bs-toggle='tooltip' data-bs-placement='top' title='Prórroga extendida'><i class='fas fa-hourglass-start'></i></span>";
            }
            object o = new
            {
                id = idProrroga,
                fechaRegistro = p.fechaRegistro.ToShortDateString(),
                fechaFin = fechaFinalizacion,
                carrera = ca.Nombre,
                idCarrera = ca.Id,
                idEstudiante = p.idEstudiante,
                tipo = p.TipoProrroga.nombre,
                grupo = g.Nombre,
                materia = g.MateriaContenido.Nombre,
                turno = g.Turno.Nombre,
                aula = horario.Aula.Nombre
            };
            return Json(o, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MostrarModuloActual()
        {
            SAADSTJEntities db = new SAADSTJEntities();
            DateTime fechaActual = DateTime.Now;
            var modulo = db.Oferta.SqlQuery("select top 1 o.* from Oferta o, OfertaSistemaEstudio ose where o.Id = ose.IdOferta and '" + fechaActual + "' between ose.FechaInicio and ose.FechaFin order by o.Id").SingleOrDefault();
            if (modulo == null)
            {
                modulo = db.Oferta.SqlQuery("select top 1 * from Oferta order by Id desc").Single();
            }
            object o = new
            {
                id = modulo.Id
            };
            return Json(o, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExtenderProrroga(int idProrroga, DateTime fechaExtension)
        {
            ProrrogasEntities db = new ProrrogasEntities();
            Status s = new Status();
            var prorroga = db.Prorroga.Single(x => x.id == idProrroga);
            if (prorroga.fechaFin > fechaExtension)
            {
                s.Tipo = 2;
                s.Msj = "La fecha de extensión debe ser mayor a la fecha final";
            }
            else
            {
                int idUsuario = db.Usuario.Single(x => x.usuario1 == User.Identity.Name).id;
                prorroga.fechaExtension = fechaExtension;
                prorroga.idEstado = 1;
                prorroga.idUsuario = idUsuario;
                db.SaveChanges();
                s.Tipo = 1;
                s.Msj = "Se extendió la prórroga con éxito";
            }
            return Json(s, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RangoFechas(int idModulo)
        {
            SAADSTJEntities dbS = new SAADSTJEntities();
            var ofertaSistemaEstudio = dbS.OfertaSistemaEstudio.First(x => x.IdOferta == idModulo);
            DateTime fechaInicio = ofertaSistemaEstudio.FechaInicio;
            DateTime fechaFin = ofertaSistemaEstudio.FechaInicio;
            int diasRestados = 3;
            while (diasRestados > 0)
            {
                fechaInicio = fechaInicio.AddDays(-1);
                if (fechaInicio.DayOfWeek == DayOfWeek.Saturday || fechaInicio.DayOfWeek == DayOfWeek.Sunday)
                {
                    continue;
                }
                diasRestados--;
            }
            int diasAgregados = 5;
            while (diasAgregados > 0)
            {
                fechaFin = fechaFin.AddDays(1);
                if (fechaFin.DayOfWeek == DayOfWeek.Saturday || fechaFin.DayOfWeek == DayOfWeek.Sunday)
                {
                    continue;
                }
                diasAgregados--;
            }
            object fechas = new {
                inicio = fechaInicio.ToString("yyyy-MM-dd"),
                fin = fechaFin.ToString("yyyy-MM-dd")
            };
            return Json(fechas, JsonRequestBehavior.AllowGet);
        }
    }
}