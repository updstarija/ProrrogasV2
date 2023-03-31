using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Prorrogas.Models;

namespace Prorrogas.Controllers
{
    public class ReporteController : Controller
    {
        // GET: Reporte
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                ProrrogasEntities db = new ProrrogasEntities();
                var rol = db.Usuario.Single(x => x.usuario1 == User.Identity.Name).Rol.nombre;
                if (rol == "Administrador" || rol == "Encargado" || rol == "Auxiliar")
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

        public ActionResult BuscarMateria(string nombre, int idModulo)
        {
            SAADSTJEntities db = new SAADSTJEntities();
            List<object> lista = new List<object>();
            var grupoAperturado = db.GrupoAperturado.SqlQuery("select ga.* from GrupoAperturado ga, OfertaSistemaEstudio ose, Oferta o, MateriaContenido mc where ga.IdOferta = ose.IdOferta and ga.IdSistemaEstudio = ose.IdSistemaEstudio and o.Id = ose.IdOferta and mc.Id = ga.IdMateriaContenido and o.Id = " + idModulo + " and mc.Nombre like '%" + nombre + "%' order by ga.nombre").ToList();
            foreach (var item in grupoAperturado)
            {
                string materia = db.MateriaContenido.Single(x => x.Id == item.IdMateriaContenido).Nombre;
                string turno = db.Turno.Single(x => x.Id == item.IdTurno).Nombre;
                var docente = db.Persona.Single(x => x.Id == item.IdDocente);
                object o = new
                {
                    id = item.Id,
                    nombre = item.Nombre,
                    materia = materia,
                    turno = turno,
                    docente = docente.Nombre + " " + docente.ApellidoPaterno + " " + docente.ApellidoMaterno,
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
            var docente = db.Persona.Single(x => x.Id == ga.IdDocente);
            object o = new
            {
                id = ga.Id,
                nombre = ga.Nombre,
                materia = materia,
                turno = turno,
                docente = docente.Nombre + " " + docente.ApellidoPaterno + " " + docente.ApellidoMaterno,
            };
            return Json(o, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CantidadDeProrrogasPorTipo()
        {
            ProrrogasEntities db = new ProrrogasEntities();
            SAADSTJEntities dbS = new SAADSTJEntities();
            var tiposDeProrroga = db.TipoProrroga.Where(x => x.estado == true);
            var moduloActual = dbS.Oferta.OrderByDescending(x => x.Id).First();
            string modulo = moduloActual.Descripcion;
            string ntipo1 = "", ntipo2 = "", ntipo3 = "", ntipo4 = "", ntipo5 = "", ntipo6 = "";
            int ctipo1 = 0, ctipo2 = 0, ctipo3 = 0, ctipo4 = 0, ctipo5 = 0, ctipo6 = 0, contador = 1;
            foreach (var item in tiposDeProrroga)
            {
                int prorrogas = db.Prorroga.Count(x => x.idTipoProrroga == item.id && x.idModulo == moduloActual.Id && x.idEstado == 1);
                switch (contador)
                {
                    case 1:
                        ntipo1 = item.nombre;
                        ctipo1 = prorrogas;
                        break;
                    case 2:
                        ntipo2 = item.nombre;
                        ctipo2 = prorrogas;
                        break;
                    case 3:
                        ntipo3 = item.nombre;
                        ctipo3 = prorrogas;
                        break;
                    case 4:
                        ntipo4 = item.nombre;
                        ctipo4 = prorrogas;
                        break;
                    case 5:
                        ntipo5 = item.nombre;
                        ctipo5 = prorrogas;
                        break;
                }
                contador++;

            }
            object o = new
            {
                nTipo1 = ntipo1,
                cTipo1 = ctipo1,
                nTipo2 = ntipo2,
                cTipo2 = ctipo2,
                nTipo3 = ntipo3,
                cTipo3 = ctipo3,
                nTipo4 = ntipo4,
                cTipo4 = ctipo4,
                nTipo5 = ntipo5,
                cTipo5 = ctipo5,
                modulo = modulo

            };
            return Json(o, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Comprobante(int idProrroga)
        {
            ProrrogasEntities db = new ProrrogasEntities();
            SAADSTJEntities dbS = new SAADSTJEntities();
            string Tipo = "PDF";
            var reporte = "Comprobante Registro.rdlc";
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Reportes"), reporte);
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                ViewBag.dir = path;
                return View("Index");
            }
            List<Comprobante> lista = new List<Comprobante>();
            var pro = db.Prorroga.Single(x=> x.id == idProrroga);
            var p = dbS.Persona.Single(x => x.Id == pro.idEstudiante);
            string carrera = dbS.PlanEstudio.Single(x => x.Id == pro.idCarrera).Nombre;
            var grupo = dbS.GrupoAperturado.Single(x => x.Id == pro.idGrupo);
            string materia = dbS.MateriaContenido.Single(x => x.Id == grupo.IdMateriaContenido).Nombre;
            var horario = dbS.HorarioGrupo.First(x => x.IdGrupoAperturado == grupo.Id);
            string modulo = dbS.Oferta.Single(x => x.Id == pro.idModulo).Descripcion;
            string turno = dbS.Turno.Single(x => x.Id == grupo.IdTurno).Nombre;
            string aula = dbS.Aula.Single(x => x.Id == horario.IdAula).Nombre;
            string usuario = db.Usuario.Single(x => x.id == pro.idUsuario).usuario1;
            string fechaFinalizacion = pro.fechaFin.ToShortDateString();
            if (pro.fechaExtension != null)
            {
                fechaFinalizacion = Convert.ToDateTime(pro.fechaExtension).ToString("dd/MM/yyyy");
            }
            Comprobante c = new Comprobante();
            c.TipoProrroga = pro.TipoProrroga.nombre.ToUpper();
            c.Ci = p.DocumentoIdentidad;
            c.Estudiante = p.Nombre + " " + p.ApellidoPaterno + " " + p.ApellidoMaterno;
            c.Carrera = carrera;
            c.FechaRegistro = pro.fechaRegistro.ToShortDateString() + " " + pro.fechaRegistro.ToShortTimeString();
            c.Materia = materia;
            c.Modulo = modulo;
            c.Turno = turno;
            c.Aula = aula;
            c.FechaLimite = fechaFinalizacion;
            c.Usuario = usuario;
            lista.Add(c);
            ReportDataSource rd = new ReportDataSource("DataSet1", lista);
            lr.DataSources.Add(rd);
            string reportType = Tipo;
            string mimeType;
            string encoding;
            string fileNameExtension;

            string deviceInfo =

            "<DeviceInfo>" +
            "  <OutputFormat>" + Tipo + "</OutputFormat>" +
            "  <PageWidth>2.9in</PageWidth>" +
            "  <PageHeight>5.1in</PageHeight>" +
            "  <MarginTop>0.1in</MarginTop>" +
            "  <MarginLeft>0.1in</MarginLeft>" +
            "  <MarginRight>0.1in</MarginRight>" +
            "  <MarginBottom>0.1in</MarginBottom>" +
            "</DeviceInfo>";

            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;

            renderedBytes = lr.Render(
                reportType,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);
            return File(renderedBytes, mimeType);

        }
        
        public ActionResult EstudiantesConDobleProrroga(int idModulo, string tipo)
        {
            ProrrogasEntities db = new ProrrogasEntities();
            SAADSTJEntities dbS = new SAADSTJEntities();
            var reporte = "EstudiantesConDosProrrogas.rdlc";
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Reportes"), reporte);
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                ViewBag.dir = path;
                return View("Index");
            }

            var prorrogas = db.Prorroga.SqlQuery("select * from Prorroga pro where pro.idModulo = " + idModulo + " and pro.idEstado <= 2 and (select count(idEstudiante) from Prorroga p where p.idEstudiante = pro.idEstudiante and p.idModulo = " + idModulo + " and p.idEstado <= 2) > 1 order by pro.idEstudiante");
            List<ReporteProrroga> lista = new List<ReporteProrroga>();
            string modulo = dbS.Oferta.Single(x => x.Id == idModulo).Descripcion;
            foreach (var item in prorrogas)
            {
                string tipoProrroga = db.TipoProrroga.Single(x => x.id == item.idTipoProrroga).nombre;
                var estudiante = dbS.Persona.Single(x => x.Id == item.idEstudiante);
                string nombreEstudiante = estudiante.Nombre + " " + estudiante.ApellidoPaterno + " " + estudiante.ApellidoMaterno;
                string documentoIdentidad = estudiante.DocumentoIdentidad;
                string carrera = dbS.PlanEstudio.Single(x => x.Id == item.idCarrera).Nombre;
                string fechaRegistro = item.fechaRegistro.ToShortDateString();
                var grupo = dbS.GrupoAperturado.Single(x => x.Id == item.idGrupo);
                string materia = dbS.MateriaContenido.Single(x => x.Id == grupo.IdMateriaContenido).Nombre;
                var horario = dbS.HorarioGrupo.First(x => x.IdGrupoAperturado == grupo.Id);
                string turno = dbS.Turno.Single(x => x.Id == grupo.IdTurno).Nombre;
                string aula = dbS.Aula.Single(x => x.Id == horario.IdAula).Nombre;
                string fechaFinalizacion = item.fechaFin.ToShortDateString();
                if (item.fechaExtension != null)
                {
                    fechaFinalizacion = Convert.ToDateTime(item.fechaExtension).ToString("dd/MM/yyyy");
                }
                ReporteProrroga pro = new ReporteProrroga();
                pro.Id = item.id;
                pro.Estudiante = nombreEstudiante;
                pro.Ci = documentoIdentidad;
                pro.Carrera = carrera;
                pro.FechaRegistro = fechaRegistro;
                pro.Modulo = modulo;
                pro.Materia = materia;
                pro.Turno = turno;
                pro.Aula = aula;
                pro.TipoProrroga = tipoProrroga;
                pro.FechaLimite = fechaFinalizacion;
                lista.Add(pro);
            }
            ReportDataSource rd = new ReportDataSource("DataSet1", lista);
            lr.DataSources.Add(rd);
            string reportType = tipo;
            string mimeType;
            string encoding;
            string fileNameExtension;

            string deviceInfo =

            "<DeviceInfo>" +
            "  <OutputFormat>" + tipo + "</OutputFormat>" +
            "  <PageWidth>8.5in</PageWidth>" +
            "  <PageHeight>11in</PageHeight>" +
            "  <MarginTop>0.1in</MarginTop>" +
            "  <MarginLeft>0.1in</MarginLeft>" +
            "  <MarginRight>0.1in</MarginRight>" +
            "  <MarginBottom>0.1in</MarginBottom>" +
            "</DeviceInfo>";

            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;

            renderedBytes = lr.Render(
                reportType,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);
            return File(renderedBytes, mimeType);
        }

        public ActionResult ProrrogasPorModulo(int idModulo, string tipo)
        {
            ProrrogasEntities db = new ProrrogasEntities();
            SAADSTJEntities dbS = new SAADSTJEntities();
            var reporte = "ProrrogasPorModulo.rdlc";
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Reportes"), reporte);
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                ViewBag.dir = path;
                return View("Index");
            }

            var prorrogas = db.Prorroga.Where(x => x.idModulo == idModulo && x.idEstado <= 2);
            List<ReporteProrroga> lista = new List<ReporteProrroga>();
            string modulo = dbS.Oferta.Single(x => x.Id == idModulo).Descripcion;
            foreach (var item in prorrogas)
            {
                string tipoProrroga = db.TipoProrroga.Single(x => x.id == item.idTipoProrroga).nombre;
                var estudiante = dbS.Persona.Single(x => x.Id == item.idEstudiante);
                string nombreEstudiante = estudiante.Nombre + " " + estudiante.ApellidoPaterno + " " + estudiante.ApellidoMaterno;
                string documentoIdentidad = estudiante.DocumentoIdentidad;
                string carrera = dbS.PlanEstudio.Single(x => x.Id == item.idCarrera).Nombre;
                string fechaRegistro = item.fechaRegistro.ToShortDateString();
                var grupo = dbS.GrupoAperturado.Single(x => x.Id == item.idGrupo);
                string materia = dbS.MateriaContenido.Single(x => x.Id == grupo.IdMateriaContenido).Nombre;
                var horario = dbS.HorarioGrupo.First(x => x.IdGrupoAperturado == grupo.Id);
                string turno = dbS.Turno.Single(x => x.Id == grupo.IdTurno).Nombre;
                string aula = dbS.Aula.Single(x => x.Id == horario.IdAula).Nombre;
                string fechaFinalizacion = item.fechaFin.ToShortDateString();
                string estado = item.EstadoProrroga.nombre;
                if (item.fechaExtension != null)
                {
                    fechaFinalizacion = Convert.ToDateTime(item.fechaExtension).ToString("dd/MM/yyyy");
                }
                ReporteProrroga pro = new ReporteProrroga();
                pro.Id = item.id;
                pro.Estudiante = nombreEstudiante;
                pro.Ci = documentoIdentidad;
                pro.Carrera = carrera;
                pro.FechaRegistro = fechaRegistro;
                pro.Modulo = modulo;
                pro.Materia = materia;
                pro.Turno = turno;
                pro.Aula = aula;
                pro.TipoProrroga = tipoProrroga;
                pro.FechaLimite = fechaFinalizacion;
                pro.Estado = estado;
                lista.Add(pro);
            }
            ReportDataSource rd = new ReportDataSource("DataSet1", lista);
            lr.DataSources.Add(rd);
            string reportType = tipo;
            string mimeType;
            string encoding;
            string fileNameExtension;

            string deviceInfo =

            "<DeviceInfo>" +
            "  <OutputFormat>" + tipo + "</OutputFormat>" +
            "  <PageWidth>8.5in</PageWidth>" +
            "  <PageHeight>11in</PageHeight>" +
            "  <MarginTop>0.1in</MarginTop>" +
            "  <MarginLeft>0.1in</MarginLeft>" +
            "  <MarginRight>0.1in</MarginRight>" +
            "  <MarginBottom>0.1in</MarginBottom>" +
            "</DeviceInfo>";

            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;

            renderedBytes = lr.Render(
                reportType,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);
            return File(renderedBytes, mimeType);
        }

        public ActionResult RepProrrogasPorModuloPorGrupo(int idModulo, int idGrupo, string tipo)
        {
            ProrrogasEntities db = new ProrrogasEntities();
            SAADSTJEntities dbS = new SAADSTJEntities();
            var reporte = "ProrrogasPorGrupo.rdlc";
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Reportes"), reporte);
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                ViewBag.dir = path;
                return View("Index");
            }

            var prorrogas = db.Prorroga.Where(x => x.idModulo == idModulo && x.idEstado <= 2 && x.idGrupo == idGrupo);
            List<ReportePorGrupos> lista = new List<ReportePorGrupos>();
            string modulo = dbS.Oferta.Single(x => x.Id == idModulo).Descripcion;
            var grupo = dbS.GrupoAperturado.Single(x => x.Id == idGrupo);
            string materia = dbS.MateriaContenido.Single(x => x.Id == grupo.IdMateriaContenido).Nombre;
            var horario = dbS.HorarioGrupo.First(x => x.IdGrupoAperturado == grupo.Id);
            string turno = dbS.Turno.Single(x => x.Id == grupo.IdTurno).Nombre;
            string aula = dbS.Aula.Single(x => x.Id == horario.IdAula).Nombre;
            var docente = dbS.Persona.Single(x => x.Id == grupo.IdDocente);
            string nombreDoccente = docente.Nombre + " " + docente.ApellidoPaterno + " " + docente.ApellidoMaterno;
            foreach (var item in prorrogas)
            {
                string tipoProrroga = db.TipoProrroga.Single(x => x.id == item.idTipoProrroga).nombre;
                var estudiante = dbS.Persona.Single(x => x.Id == item.idEstudiante);
                string nombreEstudiante = estudiante.Nombre + " " + estudiante.ApellidoPaterno + " " + estudiante.ApellidoMaterno;
                string documentoIdentidad = estudiante.DocumentoIdentidad;
                string carrera = dbS.PlanEstudio.Single(x => x.Id == item.idCarrera).Nombre;
                string fechaRegistro = item.fechaRegistro.ToShortDateString();
                string fechaFinalizacion = item.fechaFin.ToShortDateString();
                string estado = item.EstadoProrroga.nombre;
                if (item.fechaExtension != null)
                {
                    fechaFinalizacion = Convert.ToDateTime(item.fechaExtension).ToString("dd/MM/yyyy");
                }
                ReportePorGrupos pro = new ReportePorGrupos();
                pro.Grupo = grupo.Nombre;
                pro.Estudiante = nombreEstudiante;
                pro.Ci = documentoIdentidad;
                pro.Carrera = carrera;
                pro.FechaRegistro = fechaRegistro;
                pro.Modulo = modulo;
                pro.Materia = materia;
                pro.Turno = turno;
                pro.Aula = aula;
                pro.TipoProrroga = tipoProrroga;
                pro.FechaLimite = fechaFinalizacion;
                pro.Docente = nombreDoccente;
                pro.Estado = estado;
                lista.Add(pro);
            }
            ReportDataSource rd = new ReportDataSource("DataSet1", lista);
            lr.DataSources.Add(rd);
            string reportType = tipo;
            string mimeType;
            string encoding;
            string fileNameExtension;

            string deviceInfo =

            "<DeviceInfo>" +
            "  <OutputFormat>" + tipo + "</OutputFormat>" +
            "  <PageWidth>8.5in</PageWidth>" +
            "  <PageHeight>11in</PageHeight>" +
            "  <MarginTop>0.1in</MarginTop>" +
            "  <MarginLeft>0.1in</MarginLeft>" +
            "  <MarginRight>0.1in</MarginRight>" +
            "  <MarginBottom>0.1in</MarginBottom>" +
            "</DeviceInfo>";

            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;

            renderedBytes = lr.Render(
                reportType,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);
            return File(renderedBytes, mimeType);
        }

        public ActionResult RepRegistrosPorModuloPorGrupo(int idModulo, int idGrupo, string tipo)
        {
            ProrrogasEntities db = new ProrrogasEntities();
            SAADSTJEntities dbS = new SAADSTJEntities();
            var reporte = "RegistrosPorGrupo.rdlc";
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Reportes"), reporte);
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                ViewBag.dir = path;
                return View("Index");
            }

            var registros = dbS.RegistroDeMateria.OrderBy(x=> x.ComprobanteRegistro.FechaHora).Where(x => x.IdGrupoAperturado == idGrupo && x.IdEstadoMateriaRegistrada != 6);
            List<ReportePorGrupos> lista = new List<ReportePorGrupos>();
            string modulo = dbS.Oferta.Single(x => x.Id == idModulo).Descripcion;
            var grupo = dbS.GrupoAperturado.Single(x => x.Id == idGrupo);
            string materia = dbS.MateriaContenido.Single(x => x.Id == grupo.IdMateriaContenido).Nombre;
            var horario = dbS.HorarioGrupo.First(x => x.IdGrupoAperturado == grupo.Id);
            string turno = dbS.Turno.Single(x => x.Id == grupo.IdTurno).Nombre;
            string aula = dbS.Aula.Single(x => x.Id == horario.IdAula).Nombre;
            var docente = dbS.Persona.Single(x => x.Id == grupo.IdDocente);
            string nombreDoccente = docente.Nombre + " " + docente.ApellidoPaterno + " " + docente.ApellidoMaterno;
            foreach (var item in registros)
            {
                var estudiante = dbS.Persona.Single(x => x.Id == item.ComprobanteRegistro.InscripcionCarrera.IdEstudiante);
                string nombreEstudiante = estudiante.Nombre + " " + estudiante.ApellidoPaterno + " " + estudiante.ApellidoMaterno;
                string documentoIdentidad = estudiante.DocumentoIdentidad;
                string celular = estudiante.Celular;
                string carrera = dbS.PlanEstudio.Single(x => x.Id == item.ComprobanteRegistro.InscripcionCarrera.IdPlanEstudio).Nombre;
                string fechaRegistro = Convert.ToDateTime(item.ComprobanteRegistro.FechaHora).ToShortDateString();
                ReportePorGrupos pro = new ReportePorGrupos();
                pro.Grupo = grupo.Nombre;
                pro.Estudiante = nombreEstudiante;
                pro.Ci = documentoIdentidad;
                pro.Carrera = carrera;
                pro.FechaRegistro = fechaRegistro;
                pro.Modulo = modulo;
                pro.Materia = materia;
                pro.Turno = turno;
                pro.Aula = aula;
                pro.Docente = nombreDoccente;
                pro.TipoProrroga = celular;
                lista.Add(pro);
            }
            ReportDataSource rd = new ReportDataSource("DataSet1", lista);
            lr.DataSources.Add(rd);
            string reportType = tipo;
            string mimeType;
            string encoding;
            string fileNameExtension;

            string deviceInfo =

            "<DeviceInfo>" +
            "  <OutputFormat>" + tipo + "</OutputFormat>" +
            "  <PageWidth>8.5in</PageWidth>" +
            "  <PageHeight>11in</PageHeight>" +
            "  <MarginTop>0.1in</MarginTop>" +
            "  <MarginLeft>0.1in</MarginLeft>" +
            "  <MarginRight>0.1in</MarginRight>" +
            "  <MarginBottom>0.1in</MarginBottom>" +
            "</DeviceInfo>";

            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;

            renderedBytes = lr.Render(
                reportType,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);
            return File(renderedBytes, mimeType);
        }
    }
}