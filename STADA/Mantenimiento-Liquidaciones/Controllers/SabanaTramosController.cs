using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Mantenimiento_Liquidaciones;

namespace Mantenimiento_Liquidaciones.Controllers
{
    public class SabanaTramosController : Controller
    {
        private CLUBS_AX_TESTEntities db = new CLUBS_AX_TESTEntities();

        // GET: SabanaTramos
        /*public ActionResult Index()
        {
            return View(db.SabanaTramos.ToList());
        }*/

        public ActionResult Index()
        {
            try
            {
                return View(db.SabanaTramos.ToList());
            }
            catch (Exception ex)
            {
                // ex.Message suele ser el mensaje genérico de EF; la causa real está en la excepción más interna
                Exception causa = ex;
                while (causa.InnerException != null) { causa = causa.InnerException; }
                System.Diagnostics.Trace.TraceError("SabanaTramos/Index: " + ex);
                ViewBag.Error = "No se pudo cargar la tabla Sábana Tramos. Detalle: " + causa.Message;
                return View(new List<SabanaTramo>());
            }
        }

        public ActionResult ReadComboTramos()
        {
            // Obtener los valores distintos de IdTramo
            var idTramos = db.SabanaTramos.Select(s => new { IdTramo = s.IdTramo }).Distinct().ToList(); // Proyectar a un objeto anónimo con la propiedad IdTramo

            // Retornar un JsonResult con la propiedad Data
            return Json(new { Data = idTramos }, JsonRequestBehavior.AllowGet);
        }

        // GET: SabanaTramos/Details/5
        public ActionResult Details(int? idTramo, double? fRangoMin)
        {
            if (idTramo == null || fRangoMin == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SabanaTramo sabanaTramo = db.SabanaTramos.Find(idTramo, fRangoMin);
            if (sabanaTramo == null)
            {
                return HttpNotFound();
            }
            return View(sabanaTramo);
        }

        // GET: SabanaTramos/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SabanaTramos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdTramo,fRangoMin,fRangoMax,fDescuento,dFechaActualizacion,sUsuarioActualizacion")] SabanaTramo sabanaTramo)
        {
            ModelState.Remove("dFechaActualizacion");
            ModelState.Remove("sUsuarioActualizacion");
            sabanaTramo.dFechaActualizacion = DateTime.Now;
            sabanaTramo.sUsuarioActualizacion = string.IsNullOrEmpty(User.Identity.Name) ? "SYSTEM" : User.Identity.Name;

            if (ModelState.IsValid)
            {
                db.SabanaTramos.Add(sabanaTramo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(sabanaTramo);
        }

        // GET: SabanaTramos/Edit/5
        public ActionResult Edit(int? idTramo, double? fRangoMin)
        {
            if (idTramo == null || fRangoMin == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SabanaTramo sabanaTramo = db.SabanaTramos.Find(idTramo, fRangoMin);
            if (sabanaTramo == null)
            {
                return HttpNotFound();
            }
            return View(sabanaTramo);
        }

        // POST: SabanaTramos/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        //  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdTramo,fRangoMin,fRangoMax,fDescuento,dFechaActualizacion,sUsuarioActualizacion")] SabanaTramo sabanaTramo)
        {
            ModelState.Remove("dFechaActualizacion");
            ModelState.Remove("sUsuarioActualizacion");
            sabanaTramo.dFechaActualizacion = DateTime.Now;
            sabanaTramo.sUsuarioActualizacion = string.IsNullOrEmpty(User.Identity.Name) ? "SYSTEM" : User.Identity.Name;

            if (ModelState.IsValid)
            {
                db.Entry(sabanaTramo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(sabanaTramo);
        }

        // GET: SabanaTramos/Delete/5
        public ActionResult Delete(int? idTramo, double? fRangoMin)
        {
            if (idTramo == null || fRangoMin == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SabanaTramo sabanaTramo = db.SabanaTramos.Find(idTramo, fRangoMin);
            if (sabanaTramo == null)
            {
                return HttpNotFound();
            }
            return View(sabanaTramo);
        }

        // POST: SabanaTramos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int idTramo, double fRangoMin)
        {
            SabanaTramo sabanaTramo = db.SabanaTramos.Find(idTramo, fRangoMin);
            db.SabanaTramos.Remove(sabanaTramo);
            db.SaveChanges();
            return RedirectToAction("Index");
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
