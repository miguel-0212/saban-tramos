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
    public class MMayoristasController : Controller
    {
        private CLUBS_AX_TESTEntities db = new CLUBS_AX_TESTEntities();

        // GET: MMayoristas
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ReadMMayorista([DataSourceRequest] DataSourceRequest request, string sMayoristaAgrupador, string IdTipoFarmaciaPorDefecto)
        {
            // Inicia la consulta
            IQueryable<MMayorista> query = db.MMayoristas;

            // Aplicar filtros si se proporcionan valores
            if (!string.IsNullOrEmpty(sMayoristaAgrupador))
            {
                query = query.Where(m => m.sMayoristaAgrupador.Contains(sMayoristaAgrupador));
            }

            if (!string.IsNullOrEmpty(IdTipoFarmaciaPorDefecto))
            {
                int tipoFarmacia;
                // Intenta convertir el string a int
                if (int.TryParse(IdTipoFarmaciaPorDefecto, out tipoFarmacia))
                {
                    query = query.Where(m => m.IdTipoFarmaciaPorDefecto == tipoFarmacia);
                }
            }

            // Llamar al stored procedure y mapear los resultados a una lista de 
            var lstMMayoristas = query.OrderByDescending(m => m.dFechaActualizacion).ToList();

            // Aplicar paginación, ordenación y filtrado utilizando el DataSourceRequest
            DataSourceResult result = lstMMayoristas.ToDataSourceResult(request);

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        // GET: MMayoristas/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MMayorista mMayorista = db.MMayoristas.Find(id);
            if (mMayorista == null)
            {
                return HttpNotFound();
            }
            return View(mMayorista);
        }

        // GET: MMayoristas/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MMayoristas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create([Bind(Include = "sIdMayoristaAgrupador,sMayoristaAgrupador,IdTipoFarmaciaPorDefecto,dFechaActualizacion,sUsuarioActualizacion")] MMayorista mMayorista)
        {
            if (ModelState.IsValid)
            {
                mMayorista.dFechaActualizacion = DateTime.Now;
                mMayorista.sUsuarioActualizacion = "SYSTEM";
                db.MMayoristas.Add(mMayorista);
                db.SaveChanges();
                return JavaScript("window.location.reload();");
            }

            return View(mMayorista);
        }

        // POST: MMayoristas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit([Bind(Include = "sIdMayoristaAgrupador,sMayoristaAgrupador,IdTipoFarmaciaPorDefecto,dFechaActualizacion,sUsuarioActualizacion")] MMayorista mMayorista)
        {
            if (ModelState.IsValid)
            {
                mMayorista.dFechaActualizacion = DateTime.Now;
                mMayorista.sUsuarioActualizacion = "SYSTEM";
                db.Entry(mMayorista).State = EntityState.Modified;
                db.SaveChanges();
                return JavaScript("window.location.reload();");
            }
            return View(mMayorista);
        }


        // POST: MMayoristas/Delete/5
        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(string sIdMayoristaAgrupador)
        {
            var response = new { success = false, message = "Error al eliminar el registro" }; // Respuesta inicial de error

            // Busca el registro por su identificador
            MMayorista mMayorista = db.MMayoristas.Find(sIdMayoristaAgrupador);

            if (mMayorista != null)
            {
                db.MMayoristas.Remove(mMayorista); // Elimina el registro
                db.SaveChanges(); // Guarda los cambios
                response = new { success = true, message = "Registro eliminado correctamente" }; // Respuesta de éxito
            }
            else
            {
                response = new { success = false, message = "Registro no encontrado" }; // Registro no encontrado
            }

            return Json(response); // Retorna la respuesta como JSON
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
