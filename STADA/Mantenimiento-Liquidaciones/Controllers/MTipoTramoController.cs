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
    public class MTipoTramoController : Controller
    {
        private CLUBS_AX_TESTEntities db = new CLUBS_AX_TESTEntities();

        // GET: MTipoTramo
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ReadMTipoTramo([DataSourceRequest] DataSourceRequest request, string sTipoTramo)
        {
            IQueryable<MTipoTramo> tipoTramos = db.MTipoTramoes; // Obtener todos los tipos de tramo inicialmente

            // Aplicar filtro si el parámetro no es nulo ni vacío
            if (!string.IsNullOrEmpty(sTipoTramo))
            {
                tipoTramos = tipoTramos.Where(tt => tt.sTipoTramo.Contains(sTipoTramo));
            }

            // Ordenar por dFechaActualizacion descendente (más reciente primero)
            tipoTramos = tipoTramos.OrderByDescending(tt => tt.dFechaActualizacion);

            // Aplicar paginación, ordenación y filtrado utilizando el DataSourceRequest
            DataSourceResult result = tipoTramos.ToDataSourceResult(request);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // POST: MTipoTramo/Create
        [HttpPost]
        public ActionResult Create([Bind(Include = "IdTipoTramo,sTipoTramo,dFechaActualizacion,sUsuarioActualizacion")] MTipoTramo mTipoTramo)
        {
            if (ModelState.IsValid)
            {
                mTipoTramo.sUsuarioActualizacion = "SYSTEM";
                mTipoTramo.dFechaActualizacion = DateTime.Now;
                db.MTipoTramoes.Add(mTipoTramo);
                db.SaveChanges();
                return JavaScript("window.location.reload();");
            }

            return View(mTipoTramo);
        }
        
        // POST: MTipoTramo/Edit/5
        [HttpPost]
        public ActionResult Edit([Bind(Include = "IdTipoTramo,sTipoTramo,dFechaActualizacion,sUsuarioActualizacion")] MTipoTramo mTipoTramo)
        {
            if (ModelState.IsValid)
            {
                mTipoTramo.sUsuarioActualizacion = "SYSTEM";
                mTipoTramo.dFechaActualizacion = DateTime.Now;
                db.Entry(mTipoTramo).State = EntityState.Modified;
                db.SaveChanges();
                return JavaScript("window.location.reload();");
            }
            return View(mTipoTramo);
        }
        // POST: MTipoTramo/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                // Buscar el tipo de tramo por su clave primaria 
                MTipoTramo mTipoTramo = db.MTipoTramoes.Find(id);

                if (mTipoTramo == null)
                {
                    return Json(new { success = false, message = "No se encontró el tipo de tramo." });
                }

                db.MTipoTramoes.Remove(mTipoTramo);
                db.SaveChanges();

                return Json(new { success = true, message = "Tipo de tramo eliminado con éxito" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al eliminar el tipo de tramo: " + ex.Message });
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
    }
}
