using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Mantenimiento_Liquidaciones;
using Mantenimiento_Liquidaciones.Helpers;
using OfficeOpenXml;
using static Mantenimiento_Liquidaciones.Helpers.ManejadorErroresCargaMasiva;

namespace Mantenimiento_Liquidaciones.Controllers
{
    public class MClubController : Controller
    {
        private CLUBS_AX_TESTEntities db = new CLUBS_AX_TESTEntities();

        // GET: MClub
        public ActionResult Index()
        {
            return View();

        }
        public ActionResult ReadMClub([DataSourceRequest] DataSourceRequest request, string sIdRed, string sIdClub, string sNombreClub)
        {
            IQueryable<MClub> clubs = db.MClubs; // Obtener todos los clubs inicialmente

            // Aplicar filtros si los parámetros no son nulos ni vacíos
            if (!string.IsNullOrEmpty(sIdRed))
            {
                clubs = clubs.Where(c => c.sIdRed.Contains(sIdRed));
            }

            if (!string.IsNullOrEmpty(sIdClub))
            {
                clubs = clubs.Where(c => c.sIdClub.Contains(sIdClub));
            }

            if (!string.IsNullOrEmpty(sNombreClub))
            {
                clubs = clubs.Where(c => c.sNombreClub.Contains(sNombreClub));
            }

            // Ordenar y tomar los últimos 100 registros
            clubs = clubs.OrderByDescending(m => m.dFechaActualizacion);

            // Aplicar paginación, ordenación y filtrado utilizando el DataSourceRequest
            DataSourceResult result = clubs.ToDataSourceResult(request);

            // JsonResult.MaxJsonLength por defecto es ~2 MB (JavaScriptSerializer). Con
            // "Exportar a Excel" (AllPages, que pide todo de una vez) el JSON puede superar ese
            // límite y devolver 500. Se sube el límite solo para esta acción.
            return new JsonResult
            {
                Data = result,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = int.MaxValue
            };
        }

        public ActionResult ReadMClubActivo([DataSourceRequest] DataSourceRequest request)
        {
            IQueryable<MClub> clubs = db.MClubs; // Obtener todos los clubs inicialmente

            clubs = clubs.Where(c => c.iEstado.Equals(0));
            clubs = clubs.OrderByDescending(m => m.dFechaActualizacion);
            DataSourceResult result = clubs.ToDataSourceResult(request);
            return new JsonResult
            {
                Data = result,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = int.MaxValue
            };
        }



        // GET: MClub/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MClub mClub = db.MClubs.Find(id);
            if (mClub == null)
            {
                return HttpNotFound();
            }
            return View(mClub);
        }

     
        // POST: MClub/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create([Bind(Include = "sIdClub,sIdRed,sNombreClub,iEstado,dFechaActualizacion,sUsuarioActualizacion")] MClub mClub)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // El Id Club es autonumérico a partir de 0500 (MV 27052026); se ignora
                    // cualquier valor que llegue del formulario para evitar colisiones.
                    mClub.sIdClub = CalcularSiguienteIdClub();
                    mClub.sUsuarioActualizacion = "SYSTEM";
                    mClub.dFechaActualizacion = DateTime.Now;
                    db.MClubs.Add(mClub);
                    db.SaveChanges();

                    // Refrescar la página después de guardar
                    TempData["SuccessMessage"] = "Club creado con éxito."; // Puedes usar TempData o ViewBag para pasar un mensaje a la vista
                    return JavaScript("window.location.reload();");
                }
                else
                {
                    return View(mClub);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al crear el club: " + ex.Message);
                return View(mClub);
            }
        }

        // POST: MClub/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit([Bind(Include = "sIdClub,sIdRed,sNombreClub,iEstado,dFechaActualizacion,sUsuarioActualizacion")] MClub mClub)
        {
            if (ModelState.IsValid)
            {
                mClub.dFechaActualizacion = DateTime.Now;
                mClub.sUsuarioActualizacion = "SYSTEM";
                db.Entry(mClub).State = EntityState.Modified;
                db.SaveChanges();
                return JavaScript("window.location.reload();");
            }
            return View(mClub);
        }

        // GET: MClub/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MClub mClub = db.MClubs.Find(id);
            if (mClub == null)
            {
                return HttpNotFound();
            }
            return View(mClub);
        }

        // POST: MClub/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(string sIdClub, string sIdRed )
        {
            MClub mClub = db.MClubs.Find(sIdClub, sIdRed);
            mClub.iEstado = 1;
            mClub.dFechaActualizacion = DateTime.Now;
            mClub.sUsuarioActualizacion = "SYSTEM";
            db.Entry(mClub).State = EntityState.Modified;
            db.SaveChanges();
            return JavaScript("window.location.reload();");
        }

        [HttpPost]
        public ActionResult MassDelete(List<string> ids)
        {
            try
            {
                var clubsToUpdate = db.MClubs.Where(c => ids.Contains(c.sIdClub)).ToList();

                foreach (var club in clubsToUpdate)
                {
                    club.iEstado = 1; 
                }
                db.SaveChanges();
  

                return Json(new { success = true, message = "Los registros han sido eliminados correctamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al eliminar los registros: " + ex.Message });
            }
        }


        [HttpPost]
        public ActionResult CargarArchivo(HttpPostedFileBase archivoExcel)
        {
            if (archivoExcel == null || archivoExcel.ContentLength == 0)
            {
                ViewBag.Mensaje = ManejadorErroresCargaMasiva.ObtenerMensajeError(ErroresCargaMasiva.ArchivoNoSeleccionado);
                ViewBag.Exito = false;
                return View();
            }

            if (!archivoExcel.FileName.EndsWith(".xlsx"))
            {
                ViewBag.Mensaje = ManejadorErroresCargaMasiva.ObtenerMensajeError(ErroresCargaMasiva.ArchivoNoEsExcel);
                ViewBag.Exito = false;
                return View();
            }

            using (var package = new ExcelPackage(archivoExcel.InputStream))
            {
                var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                {
                    ViewBag.Mensaje = ManejadorErroresCargaMasiva.ObtenerMensajeError(ErroresCargaMasiva.HojaDeCalculoNoEncontrada);
                    ViewBag.Exito = false;
                    return View();
                }

                var rowCount = worksheet.Dimension.Rows;
                var columnCount = worksheet.Dimension.Columns;

                // Validar encabezados de columna
                var expectedHeaders = new List<string> { "sIdClub", "sIdRed", "sNombreClub" };
                for (int col = 1; col <= columnCount; col++)
                {
                    var header = worksheet.Cells[1, col].Value?.ToString();
                    if (!expectedHeaders.Contains(header))
                    {
                        ViewBag.Mensaje = string.Format(ManejadorErroresCargaMasiva.ObtenerMensajeError(ErroresCargaMasiva.ColumnaFaltanteOIncorrecta), header);
                        ViewBag.Exito = false;
                        return View();
                    }
                }

                // Extraer y validar datos
                var clubs = new List<MClub>();
                var erroresValidacion = new List<string>();

                for (int row = 2; row <= rowCount; row++)
                {
                    var club = new MClub();

                    // Leer y validar cada celda
                    for (int col = 1; col <= columnCount; col++)
                    {
                        var cellValue = worksheet.Cells[row, col].Value;
                        var header = expectedHeaders[col - 1];

                        switch (header)
                        {
                            case "sIdClub":
                                club.sIdClub = cellValue?.ToString();
                                if (string.IsNullOrWhiteSpace(club.sIdClub))
                                    erroresValidacion.Add(string.Format(ManejadorErroresCargaMasiva.ObtenerMensajeError(ErroresCargaMasiva.CampoObligatorio), row, header));
                                else if (club.sIdClub.Length > 4)
                                    erroresValidacion.Add(string.Format(ManejadorErroresCargaMasiva.ObtenerMensajeError(ErroresCargaMasiva.LongitudMaximaExcedida), row, header, 4));
                                break;

                            case "sIdRed":
                                club.sIdRed = cellValue?.ToString();
                                if (string.IsNullOrWhiteSpace(club.sIdRed))
                                    erroresValidacion.Add(string.Format(ManejadorErroresCargaMasiva.ObtenerMensajeError(ErroresCargaMasiva.CampoObligatorio), row, header));
                                else if (club.sIdRed.Length > 4)
                                    erroresValidacion.Add(string.Format(ManejadorErroresCargaMasiva.ObtenerMensajeError(ErroresCargaMasiva.LongitudMaximaExcedida), row, header, 4));
                                break;

                            case "sNombreClub":
                                club.sNombreClub = cellValue?.ToString();
                                if (string.IsNullOrWhiteSpace(club.sNombreClub))
                                    erroresValidacion.Add(string.Format(ManejadorErroresCargaMasiva.ObtenerMensajeError(ErroresCargaMasiva.CampoObligatorio), row, header));
                                else if (club.sNombreClub.Length > 100)
                                    erroresValidacion.Add(string.Format(ManejadorErroresCargaMasiva.ObtenerMensajeError(ErroresCargaMasiva.LongitudMaximaExcedida), row, header, 100));
                                break;

                           
                            default:
                                break;
                        }
                    }

                    // Validar si ya existe un registro igual en la base de datos
                    if (db.MClubs.Any(c => c.sIdClub == club.sIdClub && c.sIdRed == club.sIdRed))
                    {
                        erroresValidacion.Add($"Fila {row}: Ya existe un club con el mismo IdClub y IdRed.");
                    }

                    clubs.Add(club);
                }

                if (erroresValidacion.Any())
                {
                    ViewBag.Mensaje = "Errores de validación en el archivo Excel:<br/>" + string.Join("<br/>", erroresValidacion);
                    ViewBag.Exito = false;
                    return View();
                }

                if (clubs.Count() <= 0)
                {
                    ViewBag.Mensaje = ManejadorErroresCargaMasiva.ObtenerMensajeError(ManejadorErroresCargaMasiva.ErroresCargaMasiva.NoHayRegistrosEnExcel);
                    ViewBag.Exito = false;
                    return View();
                }
                else
                {
                    bool exito = CargarClubsMasivamente(clubs, out string mensajeError);

                    if (exito)
                    {
                        ViewBag.Mensaje = "Carga masiva completada con éxito.";
                        ViewBag.Exito = true;
                    }
                    else
                    {
                        ViewBag.Mensaje = mensajeError;
                        ViewBag.Exito = false;
                    }
                }

                return View();
            }
        }

        // Método interno para carga masiva de clubs
        private bool CargarClubsMasivamente(List<MClub> clubs, out string mensajeError)
        {
            mensajeError = "";

            try
            {
                foreach (var club in clubs)
                {
                    if (ModelState.IsValid)
                    {
                        club.iEstado = 0;
                        club.dFechaActualizacion = DateTime.Today;
                        club.sUsuarioActualizacion = "SYSTEM";
                        db.MClubs.Add(club);
                    }
                    else
                    {
                        var errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                        mensajeError = "Errores de validación:\n" + string.Join("\n", errores);
                        return false;
                    }
                }

                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                mensajeError = "Error durante la carga masiva: " + ex.Message;
                if (ex.InnerException != null)
                {
                    mensajeError += "\nInner Exception: " + ex.InnerException.Message;
                }
                return false;
            }
        }

        [HttpGet]
        public ActionResult CargarArchivo()
        {
            return View();
        }

        public ActionResult DescargarPlantilla()
        {

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Clubs");
                worksheet.Cells[1, 1].Value = "sIdClub";
                worksheet.Cells[1, 2].Value = "sIdRed";
                worksheet.Cells[1, 3].Value = "sNombreClub";


                var stream = new MemoryStream(package.GetAsByteArray());
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Plantilla_Clubs.xlsx");
            }
        }

        public ActionResult CargarArchivoModal()
        {
            return PartialView("_CargarArchivo");
        }

        // MV 27052026: Id Club autonumérico a partir de 0500. Los códigos por debajo de 500 son
        // históricos/manuales, y 9185-9999 están reservados para clubs especiales (no socios,
        // captación, errores...), así que el autonumérico solo avanza dentro de [500, 9184].
        [HttpGet]
        public JsonResult ObtenerSiguienteIdClub()
        {
            return Json(new { siguienteId = CalcularSiguienteIdClub() }, JsonRequestBehavior.AllowGet);
        }

        private string CalcularSiguienteIdClub()
        {
            const int idInicial = 500;
            const int idMaximoAutonumerico = 9184;

            var maxEnRango = db.MClubs
                .Select(c => c.sIdClub)
                .AsEnumerable()
                .Where(id => id != null && id.Length == 4 && id.All(char.IsDigit))
                .Select(int.Parse)
                .Where(n => n >= idInicial && n <= idMaximoAutonumerico)
                .DefaultIfEmpty(idInicial - 1)
                .Max();

            int siguiente = Math.Max(idInicial, maxEnRango + 1);
            if (siguiente > idMaximoAutonumerico)
            {
                throw new InvalidOperationException("Se alcanzó el límite de Ids autonuméricos disponibles para Clubs (9184).");
            }

            return siguiente.ToString("D4");
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
