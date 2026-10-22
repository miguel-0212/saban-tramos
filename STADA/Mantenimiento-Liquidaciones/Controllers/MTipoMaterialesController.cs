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
    public class MTipoMaterialesController : Controller
    {
        private CLUBS_AX_TESTEntities db = new CLUBS_AX_TESTEntities();

        // GET: MTipoMateriales
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ReadMTipoMaterial([DataSourceRequest] DataSourceRequest request, string IdTipo, string sTipo, string sIdRed)
        {
            IQueryable<MTipoMaterial> tipoMateriales = db.MTipoMaterials; // Obtener todos los tipos de material inicialmente

            // Aplicar filtros si los parámetros no son nulos ni vacíos
            if (!string.IsNullOrEmpty(IdTipo))
            {
                // Asumiendo que IdTipo es de tipo numérico en tu modelo
                if (int.TryParse(IdTipo, out int idTipo))
                {
                    tipoMateriales = tipoMateriales.Where(tm => tm.IdTipo == idTipo);
                }
            }

            if (!string.IsNullOrEmpty(sTipo))
            {
                tipoMateriales = tipoMateriales.Where(tm => tm.sTipo.Contains(sTipo));
            }

            if (!string.IsNullOrEmpty(sIdRed))
            {
                tipoMateriales = tipoMateriales.Where(tm => tm.sIdRed.Contains(sIdRed));
            }

            // Ordenar por dFechaActualizacion descendente (más reciente primero)
            tipoMateriales = tipoMateriales.OrderByDescending(tm => tm.dFechaActualizacion);

            // Aplicar paginación, ordenación y filtrado utilizando el DataSourceRequest
            DataSourceResult result = tipoMateriales.ToDataSourceResult(request);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        
        // POST: MTipoMateriales/Create
        [HttpPost]
        public ActionResult Create([Bind(Include = "IdTipo,sTipo,bCuentaBaseTramo,bCuentaBaseCalculo,dFechaIni,dFechaFin,sIdRed,dFechaActualizacion,sUsuarioActualizacion")] MTipoMaterial mTipoMaterial)
        {
            if (ModelState.IsValid)
            {
                mTipoMaterial.sUsuarioActualizacion = "SYSTEM";
                mTipoMaterial.dFechaActualizacion = DateTime.Now;
                db.MTipoMaterials.Add(mTipoMaterial);
                db.SaveChanges();
                return JavaScript("window.location.reload();");
            }

            return View(mTipoMaterial);
        }

        // POST: MTipoMateriales/Edit/5
        [HttpPost]
        public ActionResult Edit([Bind(Include = "IdTipo,sTipo,bCuentaBaseTramo,bCuentaBaseCalculo,dFechaIni,dFechaFin,sIdRed,dFechaActualizacion,sUsuarioActualizacion")] MTipoMaterial mTipoMaterial)
        {
            if (ModelState.IsValid)
            {
                mTipoMaterial.sUsuarioActualizacion = "SYSTEM";
                mTipoMaterial.dFechaActualizacion = DateTime.Now;
                db.Entry(mTipoMaterial).State = EntityState.Modified;
                db.SaveChanges();
                return JavaScript("window.location.reload();");
            }
            return View(mTipoMaterial);
        }

        // POST: MTipoMateriales/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id, string dFechaIni) // Recibir ambos valores de la clave primaria
        {
            try
            {
                DateTime? fechaIni = TryParseDate(dFechaIni);
                // Buscar el tipo de material por su clave primaria compuesta
                MTipoMaterial mTipoMaterial = db.MTipoMaterials.Find(id, fechaIni);

                if (mTipoMaterial == null)
                {
                    return Json(new { success = false, message = "No se encontró el tipo de material." });
                }

                // Actualizar la fecha de fin, usuario de actualización y fecha de actualización
                mTipoMaterial.dFechaFin = DateTime.Now;
                mTipoMaterial.sUsuarioActualizacion = "SYSTEM";
                mTipoMaterial.dFechaActualizacion = DateTime.Now;

                db.SaveChanges();

                return Json(new { success = true, message = "Tipo de material eliminado lógicamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al eliminar el tipo de material: " + ex.Message });
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
