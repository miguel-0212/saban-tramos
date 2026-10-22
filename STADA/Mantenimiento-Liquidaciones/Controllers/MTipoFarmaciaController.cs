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
    public class MTipoFarmaciaController : Controller
    {
        private CLUBS_AX_TESTEntities db = new CLUBS_AX_TESTEntities();

        // GET: MTipoFarmacia
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ReadMTipoFarmacia([DataSourceRequest] DataSourceRequest request, string IdTipoFarmacia, string sTipoFarmacia)
        {
            IQueryable<MTipoFarmacia> tipoFarmacias = db.MTipoFarmacias; // Obtener todos los tipos de farmacia inicialmente

            // Aplicar filtros si los parámetros no son nulos ni vacíos
            if (!string.IsNullOrEmpty(IdTipoFarmacia))
            {
                // Asumiendo que IdTipoFarmacia es de tipo numérico en tu modelo
                if (int.TryParse(IdTipoFarmacia, out int idTipoFarmacia))
                {
                    tipoFarmacias = tipoFarmacias.Where(tf => tf.IdTipoFarmacia == idTipoFarmacia);
                }
            }

            if (!string.IsNullOrEmpty(sTipoFarmacia))
            {
                tipoFarmacias = tipoFarmacias.Where(tf => tf.sTipoFarmacia.Contains(sTipoFarmacia));
            }

            // Ordenar por dFechaActualizacion descendente (más reciente primero)
            tipoFarmacias = tipoFarmacias.OrderByDescending(tf => tf.dFechaActualizacion);

            // Aplicar paginación, ordenación y filtrado utilizando el DataSourceRequest
            DataSourceResult result = tipoFarmacias.ToDataSourceResult(request);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // POST: MTipoFarmacia/Create
        [HttpPost]
        public ActionResult Create([Bind(Include = "IdTipoFarmacia,sTipoFarmacia,dFechaActualizacion,sUsuarioActualizacion")] MTipoFarmacia mTipoFarmacia)
        {
            if (ModelState.IsValid)
            {
                mTipoFarmacia.sUsuarioActualizacion = "SYSTEM";
                mTipoFarmacia.dFechaActualizacion = DateTime.Now;
                db.MTipoFarmacias.Add(mTipoFarmacia);
                db.SaveChanges();
                return JavaScript("window.location.reload();");
            }

            return View(mTipoFarmacia);
        }

        // POST: MTipoFarmacia/Edit/5
        [HttpPost]
        public ActionResult Edit([Bind(Include = "IdTipoFarmacia,sTipoFarmacia,dFechaActualizacion,sUsuarioActualizacion")] MTipoFarmacia mTipoFarmacia)
        {
            if (ModelState.IsValid)
            {
                mTipoFarmacia.sUsuarioActualizacion = "SYSTEM";
                mTipoFarmacia.dFechaActualizacion = DateTime.Now;
                db.Entry(mTipoFarmacia).State = EntityState.Modified;
                db.SaveChanges();
                return JavaScript("window.location.reload();");
            }
            return View(mTipoFarmacia);
        }


        // POST: MTipoDescuento/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                // Buscar el subclub por su clave primaria compuesta
                MTipoFarmacia mTipoFarmacia = db.MTipoFarmacias.Find(id);

                if (mTipoFarmacia == null)
                {
                    return Json(new { success = false, message = "No se encontró el Tipo Descuento." });
                }

                db.MTipoFarmacias.Remove(mTipoFarmacia);
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
