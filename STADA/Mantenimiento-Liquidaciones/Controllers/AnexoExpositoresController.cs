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
    public class AnexoExpositoresController : Controller
    {
        private CLUBS_AX_TESTEntities db = new CLUBS_AX_TESTEntities();

        // GET: AnexoExpositores
        public ActionResult Index()
        {
            return View();
        }




        public ActionResult ReadAnexoExpositores([DataSourceRequest] DataSourceRequest request, string sCodigoExpositor, string sCNOk, string IdMayorista)
        {
            IQueryable<AnexoExpositores> anexoExpositores = db.AnexoExpositores1; // Obtener todos los registros inicialmente

            // Aplicar filtros si los parámetros no son nulos ni vacíos
            if (!string.IsNullOrEmpty(sCodigoExpositor))
            {
                if (long.TryParse(sCodigoExpositor, out long codigoExpositor))
                {
                    anexoExpositores = anexoExpositores.Where(a => a.sCodigoExpositor == codigoExpositor);
                }
            }

            if (!string.IsNullOrEmpty(sCNOk))
            {
                if (int.TryParse(sCNOk, out int cnOk))
                {
                    anexoExpositores = anexoExpositores.Where(a => a.sCNOk == cnOk);
                }
            }

            if (!string.IsNullOrEmpty(IdMayorista))
            {
                anexoExpositores = anexoExpositores.Where(a => a.IdMayorista.Contains(IdMayorista));
            }

            // Ordenar por dFechaActualizacion descendente (más reciente primero)
            anexoExpositores = anexoExpositores.OrderByDescending(a => a.dFechaActualizacion);

            // Aplicar paginación, ordenación y filtrado utilizando el DataSourceRequest
            DataSourceResult result = anexoExpositores.ToDataSourceResult(request);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
       
        // POST: AnexoExpositores/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create([Bind(Include = "sCodigoExpositor,sCNOk,sDescripcion,IdMayorista,sMayorista,dFechaIni,dFechaFin,dFechaActualizacion,sUsuarioActualizacion")] AnexoExpositores anexoExpositores)
        {
            if (ModelState.IsValid)
            {
                anexoExpositores.dFechaActualizacion = DateTime.Now;
                anexoExpositores.sUsuarioActualizacion = "SYSTEM";
                db.AnexoExpositores1.Add(anexoExpositores);
                db.SaveChanges();
                return JavaScript("window.location.reload();");
            }

            return View(anexoExpositores);
        }


        // POST: AnexoExpositores/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit([Bind(Include = "sCodigoExpositor,sCNOk,sDescripcion,IdMayorista,sMayorista,dFechaIni,dFechaFin,dFechaActualizacion,sUsuarioActualizacion")] AnexoExpositores anexoExpositores)
        {
            if (ModelState.IsValid)
            {
                anexoExpositores.dFechaActualizacion = DateTime.Now;
                anexoExpositores.sUsuarioActualizacion = "SYSTEM";
                db.Entry(anexoExpositores).State = EntityState.Modified;
                db.SaveChanges();
                return JavaScript("window.location.reload();");
            }
            return View(anexoExpositores);
        }

      
        // POST: AnexoExpositores/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(long id)
        {
            AnexoExpositores anexoExpositores = db.AnexoExpositores1.Find(id);
            db.AnexoExpositores1.Remove(anexoExpositores);
            db.SaveChanges();
            return JavaScript("window.location.reload();");
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
