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
    public class MAgrupadorMayoristaClubsController : Controller
    {
        private CLUBS_AX_TESTEntities db = new CLUBS_AX_TESTEntities();

        // GET: MAgrupadorMayoristaClubs
        public ActionResult Index()
        {
            return View(db.MAgrupadorMayoristaClubs.ToList());
        }

        // GET: MAgrupadorMayoristaClubs/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MAgrupadorMayoristaClub mAgrupadorMayoristaClub = db.MAgrupadorMayoristaClubs.Find(id);
            if (mAgrupadorMayoristaClub == null)
            {
                return HttpNotFound();
            }
            return View(mAgrupadorMayoristaClub);
        }

        // GET: MAgrupadorMayoristaClubs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MAgrupadorMayoristaClubs/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "sIdMayoristaAgrupador,sIdMayorista,sIdClub,sIdRed,dFechaIni,dFechaFin,dFechaActualizacion,sUsuarioActualizacion")] MAgrupadorMayoristaClub mAgrupadorMayoristaClub)
        {
            if (ModelState.IsValid)
            {
                db.MAgrupadorMayoristaClubs.Add(mAgrupadorMayoristaClub);
                db.SaveChanges();
                return JavaScript("window.location.reload();");
            }

            return View(mAgrupadorMayoristaClub);
        }

        // GET: MAgrupadorMayoristaClubs/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MAgrupadorMayoristaClub mAgrupadorMayoristaClub = db.MAgrupadorMayoristaClubs.Find(id);
            if (mAgrupadorMayoristaClub == null)
            {
                return HttpNotFound();
            }
            return View(mAgrupadorMayoristaClub);
        }

        // POST: MAgrupadorMayoristaClubs/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "sIdMayoristaAgrupador,sIdMayorista,sIdClub,sIdRed,dFechaIni,dFechaFin,dFechaActualizacion,sUsuarioActualizacion")] MAgrupadorMayoristaClub mAgrupadorMayoristaClub)
        {
            if (ModelState.IsValid)
            {
                db.Entry(mAgrupadorMayoristaClub).State = EntityState.Modified;
                db.SaveChanges();
                return JavaScript("window.location.reload();");
            }
            return View(mAgrupadorMayoristaClub);
        }

        // GET: MAgrupadorMayoristaClubs/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MAgrupadorMayoristaClub mAgrupadorMayoristaClub = db.MAgrupadorMayoristaClubs.Find(id);
            if (mAgrupadorMayoristaClub == null)
            {
                return HttpNotFound();
            }
            return View(mAgrupadorMayoristaClub);
        }

        // POST: MAgrupadorMayoristaClubs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            MAgrupadorMayoristaClub mAgrupadorMayoristaClub = db.MAgrupadorMayoristaClubs.Find(id);
            db.MAgrupadorMayoristaClubs.Remove(mAgrupadorMayoristaClub);
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
