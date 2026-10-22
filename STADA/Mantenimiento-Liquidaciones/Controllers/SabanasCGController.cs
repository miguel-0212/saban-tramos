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
    public class SabanasCGController : Controller
    {
        private CLUBS_AX_TESTEntities db = new CLUBS_AX_TESTEntities();

        // GET: SabanasCG
        public ActionResult Index()
        {
            return View(db.sabanaCGs.ToList());
        }

        // GET: SabanasCG/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            sabanaCG sabanaCG = db.sabanaCGs.Find(id);
            if (sabanaCG == null)
            {
                return HttpNotFound();
            }
            return View(sabanaCG);
        }

        // GET: SabanasCG/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SabanasCG/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "sIdMayoristaAgrupador,sMayoristaAgrupador,sRed,IdPeriodicidad,IdTipoVenta,idClub,idSubClub,IdTipoCosteGestion,IdTipoVariableCG,IdSabanaCGMat,IdTramo,ImpCosteGestion,PTCosteGestion,dFechaIni,dFechaFin,dFechaActualizacion,sUsuarioActualizacion")] sabanaCG sabanaCG)
        {
            if (ModelState.IsValid)
            {
                db.sabanaCGs.Add(sabanaCG);
                db.SaveChanges();
                return JavaScript("window.location.reload();");
            }

            return View(sabanaCG);
        }

        // GET: SabanasCG/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            sabanaCG sabanaCG = db.sabanaCGs.Find(id);
            if (sabanaCG == null)
            {
                return HttpNotFound();
            }
            return View(sabanaCG);
        }

        // POST: SabanasCG/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "sIdMayoristaAgrupador,sMayoristaAgrupador,sRed,IdPeriodicidad,IdTipoVenta,idClub,idSubClub,IdTipoCosteGestion,IdTipoVariableCG,IdSabanaCGMat,IdTramo,ImpCosteGestion,PTCosteGestion,dFechaIni,dFechaFin,dFechaActualizacion,sUsuarioActualizacion")] sabanaCG sabanaCG)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sabanaCG).State = EntityState.Modified;
                db.SaveChanges();
                return JavaScript("window.location.reload();");
            }
            return View(sabanaCG);
        }

        // GET: SabanasCG/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            sabanaCG sabanaCG = db.sabanaCGs.Find(id);
            if (sabanaCG == null)
            {
                return HttpNotFound();
            }
            return View(sabanaCG);
        }

        // POST: SabanasCG/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            sabanaCG sabanaCG = db.sabanaCGs.Find(id);
            db.sabanaCGs.Remove(sabanaCG);
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
