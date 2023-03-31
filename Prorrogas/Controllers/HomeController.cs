using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Prorrogas.Models;

namespace Prorrogas.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        public ActionResult CargarDatos()
        {
            ProrrogasEntities db = new ProrrogasEntities();
            SAADSTJEntities dbS = new SAADSTJEntities();
            int idModuloActual = dbS.Oferta.OrderByDescending(x => x.Id).First().Id;
            int prrogasActivas = db.Prorroga.Count(x=> x.idModulo == idModuloActual && x.idEstado == 1);
            int prrogasExtendidas = db.Prorroga.Count(x => x.idModulo == idModuloActual && x.idEstado == 1 && x.fechaExtension != null); ;
            int prrogasInactivas = db.Prorroga.Count(x => x.idModulo == idModuloActual && x.idEstado == 2);

            string card1 = "<div class='col-sm-12'>" +
                                "<div class='card text-dark mb-3' style='width:100%; background-color: #A3E4D7;'>" +
                                    "<div class='card-header'><h5 class='card-title'>Prórroga activas</h5></div>" +
                                        "<div class='card-body text-center'>" +
                                            "<p class='card-text fs-3'>" + prrogasActivas + " <i class='fas fa-thumbs-up'></i></p>" +
                                        "</div>" +
                                "</div>" +
                           "</div>";

            string card2 = "<div class='col-sm-12'>" +
                                "<div class='card text-dark mb-3' style='width:100%; background-color: #AED6F1;'>" +
                                    "<div class='card-header'><h5 class='card-title'>Prórroga extendidas</h5></div>" +
                                        "<div class='card-body text-center'>" +
                                            "<p class='card-text fs-3'>" + prrogasExtendidas + " <i class='fas fa-calendar-plus'></i></p>" +
                                        "</div>" +
                                "</div>" +
                           "</div>";

            string card3 = "<div class='col-sm-12'>" +
                                "<div class='card text-dark mb-3' style='width:100%; background-color: #F5B7B1;'>" +
                                    "<div class='card-header'><h5 class='card-title'>Prórrogas inactivas</h5></div>" +
                                        "<div class='card-body text-center'>" +
                                            "<p class='card-text fs-3'>" + prrogasInactivas + " <i class='fas fa-thumbs-down'></i></p>" +
                                        "</div>" +
                                "</div>" +
                           "</div>";

            string principal = card1 + card2 + card3;
            return Json(principal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Estadisticas()
        {
            ProrrogasEntities db = new ProrrogasEntities();
            SAADSTJEntities dbS = new SAADSTJEntities();
            var modulos = dbS.Oferta.SqlQuery("select top 6 * from Oferta order by Id desc");
            int c = 1;
            string modulo1 = "", modulo2 = "", modulo3 = "", modulo4 = "", modulo5 = "", modulo6 = "";
            int cantidadModulo1 = 0, cantidadModulo2 = 0, cantidadModulo3 = 0, cantidadModulo4 = 0, cantidadModulo5 = 0, cantidadModulo6 = 0;
            foreach (var item in modulos)
            {
                switch (c)
                {
                    case 1:
                        modulo1 = item.Descripcion;
                        cantidadModulo1 = db.Prorroga.Count(x => x.idModulo == item.Id && x.idEstado <= 2);
                        break;
                    case 2:
                        modulo2 = item.Descripcion;
                        cantidadModulo2 = db.Prorroga.Count(x => x.idModulo == item.Id && x.idEstado <= 2);
                        break;
                    case 3:
                        modulo3 = item.Descripcion;
                        cantidadModulo3 = db.Prorroga.Count(x => x.idModulo == item.Id && x.idEstado <= 2);
                        break;
                    case 4:
                        modulo4 = item.Descripcion;
                        cantidadModulo4 = db.Prorroga.Count(x => x.idModulo == item.Id && x.idEstado <= 2);
                        break;
                    case 5:
                        modulo5 = item.Descripcion;
                        cantidadModulo5 = db.Prorroga.Count(x => x.idModulo == item.Id && x.idEstado <= 2);
                        break;
                    case 6:
                        modulo6 = item.Descripcion;
                        cantidadModulo6 = db.Prorroga.Count(x => x.idModulo == item.Id && x.idEstado <= 2);
                        break;
                }
                c++;
            }
            object o = new 
            { 
                modulo1 = modulo1, 
                cantidadModulo1 = cantidadModulo1,
                modulo2 = modulo2,
                cantidadModulo2 = cantidadModulo2,
                modulo3 = modulo3,
                cantidadModulo3 = cantidadModulo3,
                modulo4 = modulo4,
                cantidadModulo4 = cantidadModulo4,
                modulo5 = modulo5,
                cantidadModulo5 = cantidadModulo5,
                modulo6 = modulo6,
                cantidadModulo6 = cantidadModulo6,

            };
            return Json(o, JsonRequestBehavior.AllowGet);
        }
    }
}