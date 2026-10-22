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
    public class MTipoVentaController : Controller
    {
        private CLUBS_AX_TESTEntities db = new CLUBS_AX_TESTEntities();

        // GET: MTipoVenta
        public ActionResult Index()
        {
            return View(db.MTipoVentas.ToList());
        }
        public ActionResult ReadMTipoVenta([DataSourceRequest] DataSourceRequest request, string sTipoSubVenta, string sTipoVenta)
        {
            IQueryable<MTipoVenta> tipoVentas = db.MTipoVentas;

            if (!string.IsNullOrEmpty(sTipoSubVenta))
            {
                tipoVentas = tipoVentas.Where(tv => tv.STipoSubVenta.Contains(sTipoSubVenta));
            }

            if (!string.IsNullOrEmpty(sTipoVenta))
            {
                tipoVentas = tipoVentas.Where(tv => tv.sTipoVenta.Contains(sTipoVenta));
            }

            DataSourceResult result = tipoVentas.ToDataSourceResult(request);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    

        // POST: MTipoVenta/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create([Bind(Include = "IdTipoSubVenta,STipoSubVenta,IdTipoVenta,sTipoVenta,dFechaActualizacion,sUsuarioActualizacion")] MTipoVenta mTipoVenta)
        {
            if (ModelState.IsValid)
            {
                mTipoVenta.sUsuarioActualizacion = "SYSTEM";
                mTipoVenta.dFechaActualizacion = DateTime.Now;
                db.MTipoVentas.Add(mTipoVenta);
                db.SaveChanges();
                return JavaScript("window.location.reload();");
            }

            return View(mTipoVenta);
        }


        // POST: MTipoVenta/Edit/5
         [HttpPost]
        public ActionResult Edit([Bind(Include = "IdTipoSubVenta,STipoSubVenta,IdTipoVenta,sTipoVenta,dFechaActualizacion,sUsuarioActualizacion")] MTipoVenta mTipoVenta)
        {
            if (ModelState.IsValid)
            {
                mTipoVenta.sUsuarioActualizacion = "SYSTEM";
                mTipoVenta.dFechaActualizacion = DateTime.Now;
                db.Entry(mTipoVenta).State = EntityState.Modified;
                db.SaveChanges();
                return JavaScript("window.location.reload();");
            }
            return View(mTipoVenta);
        }

   
        // POST: MTipoVenta/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                MTipoVenta mTipoVenta = db.MTipoVentas.Find(id);
                if (mTipoVenta == null)
                {
                    return Json(new { success = false, message = "No se encontró el tipo de material." });
                }
                db.MTipoVentas.Remove(mTipoVenta);
                db.SaveChanges();
                return Json(new { success = true, message = "Tipo de Venta eliminado lógicamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al eliminar el tipo de venta: " + ex.Message });
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
