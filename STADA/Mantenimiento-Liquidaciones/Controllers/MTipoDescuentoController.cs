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
    public class MTipoDescuentoController : Controller
    {
        private CLUBS_AX_TESTEntities db = new CLUBS_AX_TESTEntities();

        // GET: MTipoDescuento
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult ReadMTipoDescuento([DataSourceRequest] DataSourceRequest request, string sTipoDescuento)
        {
            IQueryable<MTipoDescuento> tipoDescuentos = db.MTipoDescuentoes;

            if (!string.IsNullOrEmpty(sTipoDescuento))
            {
                tipoDescuentos = tipoDescuentos.Where(td => td.sTipoDescuento.Contains(sTipoDescuento));
            }

            // Ordenar por dFechaActualizacion descendente (más reciente primero)
            tipoDescuentos = tipoDescuentos.OrderBy(td => td.dFechaActualizacion);

            DataSourceResult result = tipoDescuentos.ToDataSourceResult(request);

            return Json(result, JsonRequestBehavior.AllowGet);
        }



        // POST: MTipoDescuento/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create([Bind(Include = "IdTipoDescuento,sTipoDescuento,dFechaActualizacion,sUsuarioActualizacion")] MTipoDescuento mTipoDescuento)
        {
            if (ModelState.IsValid)
            {
                mTipoDescuento.sUsuarioActualizacion = "SYSTEM";
                mTipoDescuento.dFechaActualizacion = DateTime.Now;
                db.MTipoDescuentoes.Add(mTipoDescuento);
                db.SaveChanges();
                return JavaScript("window.location.reload();");
            }

            return View(mTipoDescuento);
        }

      
        // POST: MTipoDescuento/Edit/5
        [HttpPost]
        public ActionResult Edit([Bind(Include = "IdTipoDescuento,sTipoDescuento,dFechaActualizacion,sUsuarioActualizacion")] MTipoDescuento mTipoDescuento)
        {
            if (ModelState.IsValid)
            {
                mTipoDescuento.sUsuarioActualizacion = "SYSTEM";
                mTipoDescuento.dFechaActualizacion = DateTime.Now;
                db.Entry(mTipoDescuento).State = EntityState.Modified;
                db.SaveChanges();
                return JavaScript("window.location.reload();");
            }
            return View(mTipoDescuento);
        }


        // POST: MTipoDescuento/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                // Buscar el subclub por su clave primaria compuesta
                MTipoDescuento mTipoDescuento = db.MTipoDescuentoes.Find(id);

                if (mTipoDescuento == null)
                {
                    return Json(new { success = false, message = "No se encontró el Tipo Descuento." });
                }

                db.MTipoDescuentoes.Remove(mTipoDescuento);
                db.SaveChanges();

                return Json(new { success = true, message = "Subclub eliminado con éxito" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al eliminar el Tipo Decuento: " + ex.Message });
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
