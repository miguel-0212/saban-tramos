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
    public class MAbono_IVAController : Controller
    {
        private CLUBS_AX_TESTEntities db = new CLUBS_AX_TESTEntities();

        // GET: MAbono_IVA
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ReadMAbono_IVA([DataSourceRequest] DataSourceRequest request,string sAbonoIVA, string sIdDivision, string sIdRed)
        {
            var abonosIva = db.MAbono_IVA.AsQueryable();

            if (!string.IsNullOrEmpty(sAbonoIVA))
            {
                abonosIva = abonosIva.Where(a => a.sAbonoIVA.Contains(sAbonoIVA));
            }

            if (!string.IsNullOrEmpty(sIdDivision))
            {
                abonosIva = abonosIva.Where(a => a.sIdDivision.Contains(sIdDivision));
            }

            if (!string.IsNullOrEmpty(sIdRed))
            {
                abonosIva = abonosIva.Where(a => a.sIdRed.Contains(sIdRed));
            }

            // Aplicar paginación, ordenación y filtrado utilizando el DataSourceRequest
            DataSourceResult result = abonosIva.ToDataSourceResult(request);

            return new JsonResult
            {
                Data = result,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = int.MaxValue
            };
        }
        public ActionResult ReadMAbono_IVAold([DataSourceRequest] DataSourceRequest request,
            string sAbonoIVA, string sIdDivision, string sIdRed)
        {
            var abonosIva = db.MAbono_IVA.OrderBy(a => a.dFechaActualizacion).ToList();

            return Json(new { Data = abonosIva }, JsonRequestBehavior.AllowGet);
        }

        // GET: MAbono_IVA/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MAbono_IVA mAbono_IVA = db.MAbono_IVA.Find(id);
            if (mAbono_IVA == null)
            {
                return HttpNotFound();
            }
            return View(mAbono_IVA);
        }

        // GET: MAbono_IVA/Create
        public ActionResult Create()
        {
            return PartialView("_Create");
        }

        // POST: MAbono_IVA/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        // POST: MAbono_IVA/Create
        [HttpPost]
        public ActionResult Create([DataSourceRequest] DataSourceRequest request,
            [Bind(Include = "sIdAbonoIVA,sAbonoIVA,sIdDivision,sIdRed,fIVA,iEstado,dFechaActualizacion,sUsuarioActualizacion")] MAbono_IVA mAbono_IVA)
        
            {
            if (ModelState.IsValid)
            {
                mAbono_IVA.dFechaActualizacion = DateTime.Now;
                mAbono_IVA.iEstado = 1;
                db.MAbono_IVA.Add(mAbono_IVA);
                db.SaveChanges();
                return JavaScript("window.location.reload();");
            }
            else
            {
                // Retornar un JsonResult con los errores de validación
                var errores = ModelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );

                return Json(new
                {
                    success = false,
                    errors = errores
                });
            }
        }

        // GET: MAbono_IVA/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MAbono_IVA mAbono_IVA = db.MAbono_IVA.Find(id);
            if (mAbono_IVA == null)
            {
                return HttpNotFound();
            }
            return View(mAbono_IVA);
        }

        // POST: MAbono_IVA/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit([DataSourceRequest] DataSourceRequest request,
            [Bind(Include = "sIdAbonoIVA,sAbonoIVA,sIdDivision,sIdRed,fIVA,iEstado,dFechaActualizacion,sUsuarioActualizacion")] MAbono_IVA mAbono_IVA)
        {
            if (ModelState.IsValid)
            {
                mAbono_IVA.dFechaActualizacion = DateTime.Now;
                db.Entry(mAbono_IVA).State = EntityState.Modified;
                db.SaveChanges();
                return JavaScript("window.location.reload();");
            }
            return View(mAbono_IVA);
        }


        // POST: MAbono_IVA/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed([DataSourceRequest] DataSourceRequest request,
           [Bind(Include = "sIdAbonoIVA,sAbonoIVA,sIdDivision,sIdRed,fIVA,iEstado,dFechaActualizacion,sUsuarioActualizacion")] MAbono_IVA mAbono_IVA)
        {
            var entidad = db.MAbono_IVA.Find(mAbono_IVA.sIdAbonoIVA);
            if (entidad != null)
            {
                db.MAbono_IVA.Remove(entidad);
                db.SaveChanges();
            }
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


