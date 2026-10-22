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
    public class MSubClubsController : Controller
    {
        private CLUBS_AX_TESTEntities db = new CLUBS_AX_TESTEntities();

        // GET: MSubClubs
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ReadMSubClub([DataSourceRequest] DataSourceRequest request, string sNombreSubClub)
        {
            IQueryable<MSubClub> subClubs = db.MSubClubs;

            // Aplicar filtro si el parámetro no es nulo ni vacío
            if (!string.IsNullOrEmpty(sNombreSubClub))
            {
                subClubs = subClubs.Where(s => s.sNombreSubClub.Contains(sNombreSubClub));
            }

            // Aplicar paginación, ordenación y filtrado utilizando el DataSourceRequest
            DataSourceResult result = subClubs.ToDataSourceResult(request);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReadMSubClubActivo([DataSourceRequest] DataSourceRequest request)
        {
            IQueryable<MSubClub> subClubs = db.MSubClubs;

            subClubs = subClubs.Where(s => s.iEstado.Equals(0));

            // Aplicar paginación, ordenación y filtrado utilizando el DataSourceRequest
            DataSourceResult result = subClubs.ToDataSourceResult(request);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        // GET: MSubClubs/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MSubClub mSubClub = db.MSubClubs.Find(id);
            if (mSubClub == null)
            {
                return HttpNotFound();
            }
            return View(mSubClub);
        }

        // GET: MSubClubs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MSubClubs/Create
        [HttpPost]
        public ActionResult Create([Bind(Include = "sIdClub,sIdSubClub,sNombreSubClub,iEstado,dFechaActualizacion,sUsuarioActualizacion")] MSubClub mSubClub)
        {
            if (ModelState.IsValid)
            {
                mSubClub.iEstado = 0;
                mSubClub.dFechaActualizacion = DateTime.Now;
                mSubClub.sUsuarioActualizacion = "SYSTEM";
                db.MSubClubs.Add(mSubClub);
                db.SaveChanges();
                return JavaScript("window.location.reload();");
            }

            return View(mSubClub);
        }

      

        // POST: MSubClubs/Edit/5
        [HttpPost]
        public ActionResult Edit([Bind(Include = "sIdClub,sIdSubClub,sNombreSubClub,iEstado,dFechaActualizacion,sUsuarioActualizacion")] MSubClub mSubClub)
        {
            if (ModelState.IsValid)
            {

                mSubClub.dFechaActualizacion = DateTime.Now;
                mSubClub.sUsuarioActualizacion = "SYSTEM";
                db.Entry(mSubClub).State = EntityState.Modified;
                db.SaveChanges();
                return JavaScript("window.location.reload();");
            }
            return View(mSubClub);
        }


        // POST: MSubClubs/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(string sIdClub, string sIdSubClub) // Recibir ambos valores de la clave primaria
        {
            try
            {
                // Buscar el subclub por su clave primaria compuesta
                MSubClub mSubClub = db.MSubClubs.Find(sIdClub, sIdSubClub);

                if (mSubClub == null)
                {
                    return Json(new { success = false, message = "No se encontró el subclub." });
                }

                // Actualizar el estado del subclub (asumiendo que 1 significa eliminado)
                mSubClub.iEstado = 1;
                mSubClub.dFechaActualizacion = DateTime.Now;
                mSubClub.sUsuarioActualizacion = "SYSTEM";
                db.SaveChanges();

                return Json(new { success = true, message = "Subclub eliminado con éxito" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al eliminar el subclub: " + ex.Message });
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
