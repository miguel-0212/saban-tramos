using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Mantenimiento_Liquidaciones;

namespace Mantenimiento_Liquidaciones.Controllers
{
    public class SabanasController : Controller
    {
        private CLUBS_AX_TESTEntities db = new CLUBS_AX_TESTEntities();

        // GET: sabanas
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ReadSabanas([DataSourceRequest] DataSourceRequest request, string sMayoristaAgrupador, string sRed, int? IdTipoVenta, string idClub, string idSubClub)
        {
            IQueryable<sabana> sabanas = db.sabanas; // Obtener todas las sabanas inicialmente


            if (!string.IsNullOrEmpty(sMayoristaAgrupador))
            {
                sabanas = sabanas.Where(s => s.sMayoristaAgrupador.Contains(sMayoristaAgrupador));
            }

            if (!string.IsNullOrEmpty(sRed))
            {
                sabanas = sabanas.Where(s => s.sRed.Contains(sRed));
            }

            if (IdTipoVenta.HasValue)
            {
                if (IdTipoVenta > 0)
                {
                    sabanas = sabanas.Where(s => s.IdTipoVenta == IdTipoVenta);
                }
            }


            if (!string.IsNullOrEmpty(idClub))
            {
                sabanas = sabanas.Where(s => s.idClub.Contains(idClub));
            }

            if (!string.IsNullOrEmpty(idSubClub))
            {
                sabanas = sabanas.Where(s => s.idSubClub.Contains(idSubClub));
            }

            // Ordenar por dFechaActualizacion descendente (más reciente primero)
            sabanas = sabanas.OrderByDescending(s => s.dFechaActualizacion);

            // Aplicar paginación, ordenación y filtrado utilizando el DataSourceRequest
            DataSourceResult result = sabanas.ToDataSourceResult(request);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // GET: sabanas/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            sabana sabana = db.sabanas.Find(id);
            if (sabana == null)
            {
                return HttpNotFound();
            }
            return View(sabana);
        }

        // GET: sabanas/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: sabanas/Create

        [HttpPost]
        public ActionResult Create([Bind(Include = "sIdMayoristaAgrupador,sMayoristaAgrupador,sRed,IdPeriodicidad,IdTipoVenta,idClub,idSubClub,IdTipoDescuento,IdTipoDescuento2,dFechaIni,dFechaFin,IdTramo,IdTipoTramo,dFechaActualizacion,sUsuarioActualizacion")] sabana sabana)
        {
            if (ModelState.IsValid)
            {
                db.sabanas.Add(sabana);
                db.SaveChanges();
                return JavaScript("window.location.reload();");
            }

            return View(sabana);
        }


        // POST: sabanas/Edit/5
        [HttpPost]
        public ActionResult Edit([Bind(Include = "sIdMayoristaAgrupador,sMayoristaAgrupador,sRed,IdPeriodicidad,IdTipoVenta,idClub,idSubClub,IdTipoDescuento,IdTipoDescuento2,dFechaIni,dFechaFin,IdTramo,IdTipoTramo,dFechaActualizacion,sUsuarioActualizacion")] sabana sabana)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sabana).State = EntityState.Modified;
                db.SaveChanges();
                return JavaScript("window.location.reload();");
            }
            return View(sabana);
        }

        // POST: sabanas/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed([Bind(Include = "sIdMayoristaAgrupador,sMayoristaAgrupador,sRed,IdPeriodicidad,IdTipoVenta,idClub,idSubClub,IdTipoDescuento,IdTipoDescuento2,dFechaIni,dFechaFin,IdTramo,IdTipoTramo,dFechaActualizacion,sUsuarioActualizacion")] sabana sabana)
        {
            try
            {

                if (sabana == null)
                {
                    return Json(new { success = false, message = "No se encontró el registro en la sabana." });
                }
                db.Entry(sabana).State = EntityState.Modified;
                // Actualizar la fecha de fin para la eliminación lógica
                sabana.dFechaFin = DateTime.Now;
                sabana.sUsuarioActualizacion = "SYSTEM";
                sabana.dFechaActualizacion = DateTime.Now;
                db.SaveChanges();

                return Json(new { success = true, message = "Registro en la sabana eliminado lógicamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al eliminar el registro en la sabana: " + ex.Message });
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        private DateTime? TryParseDate(string dateString)
        {
            // Array de formatos de fecha que deseas probar
            var formats = new[]
            {
                "MM/dd/yy H:mm:ss",
                "MM/dd/yyyy H:mm:ss",
                "dd/MM/yy H:mm:ss",
                "dd/MM/yyyy H:mm:ss",
                "MM/dd/yy HH:mm:ss",
                "MM/dd/yyyy HH:mm:ss",
                "dd/MM/yy HH:mm:ss",
                "dd/MM/yyyy HH:mm:ss",
                "MM/dd/yy H:mm",
                "MM/dd/yyyy H:mm",
                "dd/MM/yy H:mm",
                "dd/MM/yyyy H:mm",
                "MM/dd/yy HH:mm",
                "MM/dd/yyyy HH:mm",
                "dd/MM/yy HH:mm",
                "dd/MM/yyyy HH:mm",
                "MM/dd/yy",
                "MM/dd/yyyy",
                "dd/MM/yy",
                "dd/MM/yyyy",
                "yyyy-MM-dd HH:mm:ss",
                "yyyy-MM-dd H:mm:ss",
                "yyyy-MM-dd HH:mm",
                "yyyy-MM-dd H:mm",
                "yyyy-MM-dd",
                "yyyy-MM-dd HH:mm:ss.fff"
            };

            foreach (var format in formats)
            {
                if (DateTime.TryParseExact(dateString, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateValue))
                {
                    return dateValue; // Retorna la fecha si se convierte correctamente
                }
            }

            // Si ningún formato funcionó, retornar nulo
            return null;
        }
    }
}
