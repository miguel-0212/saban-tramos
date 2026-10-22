using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Mantenimiento_Liquidaciones;

namespace Mantenimiento_Liquidaciones.Controllers
{
    public class MPeriodicidadController : Controller
    {
        private CLUBS_AX_TESTEntities db = new CLUBS_AX_TESTEntities();

        // GET: MPeriodicidad
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ReadMPeriodicidad([DataSourceRequest] DataSourceRequest request, string sPeriodicidad)
        {
            // Inicializa la consulta base
            var query = db.MPeriodicidads.AsQueryable();

            // Aplicar filtros si se proporcionó un valor para sPeriodicidad
            if (!string.IsNullOrEmpty(sPeriodicidad))
            {
                query = query.Where(mp => mp.sPeriodicidad.Contains(sPeriodicidad)); // Filtrado por periodicidad
            }

            // Llamar al stored procedure y mapear los resultados a una lista de MPeriodicidad
            var lstMPeriodicidad = query.OrderByDescending(m => m.dFechaActualizacion).ToList(); // Obtiene los resultados filtrados

            // Aplicar paginación, ordenación y filtrado utilizando el DataSourceRequest
            DataSourceResult result = lstMPeriodicidad.ToDataSourceResult(request);

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        // POST: MPeriodicidad/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create([Bind(Include = "IdPeriodicidad,sPeriodicidad,dFechaActualizacion,sUsuarioActualizacion")] MPeriodicidad mPeriodicidad)
        {
            if (ModelState.IsValid)
            {
                mPeriodicidad.dFechaActualizacion = DateTime.Now;
                mPeriodicidad.sUsuarioActualizacion = "SYSTEM";
                db.MPeriodicidads.Add(mPeriodicidad);
                db.SaveChanges();
                return JavaScript("window.location.reload();");
            }

            return View(mPeriodicidad);
        }

     

        // POST: MPeriodicidad/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit([Bind(Include = "IdPeriodicidad,sPeriodicidad,dFechaActualizacion,sUsuarioActualizacion")] MPeriodicidad mPeriodicidad)
        {
            if (ModelState.IsValid)
            {
                mPeriodicidad.dFechaActualizacion = DateTime.Now;
                mPeriodicidad.sUsuarioActualizacion = "SYSTEM";
                db.Entry(mPeriodicidad).State = EntityState.Modified;
                db.SaveChanges();
                return JavaScript("window.location.reload();");
            }
            return View(mPeriodicidad);
        }


        // POST: MPeriodicidad/Delete/5
        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(int IdPeriodicidad)
        {
            var response = new { success = false, message = "Error al eliminar el registro" }; // Respuesta inicial

            // Buscar la periodicidad
            MPeriodicidad mPeriodicidad = db.MPeriodicidads.Find(IdPeriodicidad);

            if (mPeriodicidad != null)
            {
                db.MPeriodicidads.Remove(mPeriodicidad);
                db.SaveChanges();

                // Actualizar la respuesta en caso de éxito
                response = new { success = true, message = "Registro eliminado correctamente" };
            }
            else
            {
                response = new { success = false, message = "Registro no encontrado" };
            }

            return Json(response);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
