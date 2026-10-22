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
    public class MFormasCobroesController : Controller
    {
        private CLUBS_AX_TESTEntities db = new CLUBS_AX_TESTEntities();

        // GET: MFormasCobroes
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ReadMFormasCobro([DataSourceRequest] DataSourceRequest request, string sFormaCobro)
        {

            IQueryable<MFormasCobro> LstMFormasCobros = db.MFormasCobroes; // Obtener todos los clubs inicialmente
            // Llamar al stored procedure y mapear los resultados a una lista de MAbono_IVA
            if (!string.IsNullOrEmpty(sFormaCobro))
            {
                LstMFormasCobros = LstMFormasCobros.Where(c => c.sFormaCobro.Contains(sFormaCobro));
            }
            LstMFormasCobros = LstMFormasCobros.OrderByDescending(m => m.dFechaActualizacion).Take(100);
            DataSourceResult result = LstMFormasCobros.ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        
        // GET: MFormasCobroes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MFormasCobro mFormasCobro = db.MFormasCobroes.Find(id);
            if (mFormasCobro == null)
            {
                return HttpNotFound();
            }
            return View(mFormasCobro);
        }

        // POST: MFormasCobroes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create([Bind(Include = "IdFormaCobro,sFormaCobro,iEstado,dFechaActualizacion,sUsuarioActualizacion")] MFormasCobro mFormasCobro)
        {
            if (ModelState.IsValid)
            {
                mFormasCobro.dFechaActualizacion = DateTime.Today;
                mFormasCobro.sUsuarioActualizacion = "SYSTEM";
                db.MFormasCobroes.Add(mFormasCobro);
                db.SaveChanges();
                return JavaScript("window.location.reload();");
            }

            return View(mFormasCobro);
        }

   
        // POST: MFormasCobroes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit([Bind(Include = "IdFormaCobro,sFormaCobro,iEstado,dFechaActualizacion,sUsuarioActualizacion")] MFormasCobro mFormasCobro)
        {
            if (ModelState.IsValid)
            {
                mFormasCobro.dFechaActualizacion = DateTime.Today;
                db.Entry(mFormasCobro).State = EntityState.Modified;
                db.SaveChanges();
                return JavaScript("window.location.reload();");
            }
            return View(mFormasCobro);
        }

       
        // POST: MFormasCobroes/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            MFormasCobro mFormasCobro = db.MFormasCobroes.Find(id);
            mFormasCobro.iEstado = 1;
            mFormasCobro.dFechaActualizacion = DateTime.Now;
            mFormasCobro.sUsuarioActualizacion = "SYSTEM";
            db.MFormasCobroes.Remove(mFormasCobro);
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
