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
    public class AnexoCambioMaterialesController : Controller
    {
        private CLUBS_AX_TESTEntities db = new CLUBS_AX_TESTEntities();

        // GET: AnexoCambioMateriales
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ReadAnexoCambioMaterial([DataSourceRequest] DataSourceRequest request, string CN_old, string CN_NEW, string SAP_old, string SAP_new)
        {
            IQueryable<AnexoCambioMaterial> anexoCambioMateriales = db.AnexoCambioMaterials; // Obtener todos los registros inicialmente

            // Aplicar filtros si los parámetros no son nulos ni vacíos
            if (!string.IsNullOrEmpty(CN_old))
            {
                anexoCambioMateriales = anexoCambioMateriales.Where(a => a.CN_old.Contains(CN_old));
            }

            if (!string.IsNullOrEmpty(CN_NEW))
            {
                anexoCambioMateriales = anexoCambioMateriales.Where(a => a.CN_NEW.Contains(CN_NEW));
            }

            if (!string.IsNullOrEmpty(SAP_old))
            {
                anexoCambioMateriales = anexoCambioMateriales.Where(a => a.SAP_old.Contains(SAP_old));
            }

            if (!string.IsNullOrEmpty(SAP_new))
            {
                anexoCambioMateriales = anexoCambioMateriales.Where(a => a.SAP_new.Contains(SAP_new));
            }

            // Ordenar por dFechaActualizacion descendente (más reciente primero)
            anexoCambioMateriales = anexoCambioMateriales.OrderByDescending(a => a.dFechaActualizacion);

            // Aplicar paginación, ordenación y filtrado utilizando el DataSourceRequest
            DataSourceResult result = anexoCambioMateriales.ToDataSourceResult(request);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // GET: AnexoCambioMateriales/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AnexoCambioMaterial anexoCambioMaterial = db.AnexoCambioMaterials.Find(id);
            if (anexoCambioMaterial == null)
            {
                return HttpNotFound();
            }
            return View(anexoCambioMaterial);
        }



        // POST: AnexoCambioMateriales/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create([Bind(Include = "CN_old,CN_NEW,SAP_old,SAP_new,dFechaInicio,dFechaFin,dFechaActualizacion,sUsuarioActualizacion")] AnexoCambioMaterial anexoCambioMaterial)
        {
            if (ModelState.IsValid)
            {
                anexoCambioMaterial.dFechaActualizacion = DateTime.Now;
                anexoCambioMaterial.sUsuarioActualizacion = "SYSTEM";
                db.AnexoCambioMaterials.Add(anexoCambioMaterial);
                db.SaveChanges();
                return JavaScript("window.location.reload();");
            }

            return View(anexoCambioMaterial);
        }



        // POST: AnexoCambioMateriales/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit([Bind(Include = "CN_old,CN_NEW,SAP_old,SAP_new,dFechaInicio,dFechaFin,dFechaActualizacion,sUsuarioActualizacion")] AnexoCambioMaterial anexoCambioMaterial)
        {
            if (ModelState.IsValid)
            {
                anexoCambioMaterial.dFechaActualizacion = DateTime.Now;
                anexoCambioMaterial.sUsuarioActualizacion = "SYSTEM";
                db.Entry(anexoCambioMaterial).State = EntityState.Modified;
                db.SaveChanges();
                return JavaScript("window.location.reload();");
            }
            return View(anexoCambioMaterial);
        }


        // POST: AnexoCambioMateriales/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(string id)
        {
            AnexoCambioMaterial anexoCambioMaterial = db.AnexoCambioMaterials.Find(id);
            db.AnexoCambioMaterials.Remove(anexoCambioMaterial);
            db.SaveChanges();
            return JavaScript("window.location.reload();");
        }
        [HttpPost]
        public ActionResult CargarArchivo(HttpPostedFileBase archivoExcel)
        {
            if (archivoExcel == null || archivoExcel.ContentLength == 0)
            {
                ViewBag.Mensaje = ManejadorErroresCargaMasiva.ObtenerMensajeError(ManejadorErroresCargaMasiva.ErroresCargaMasiva.ArchivoNoSeleccionado);
                ViewBag.Exito = false;
                return View();
            }

            if (!archivoExcel.FileName.EndsWith(".xlsx"))
            {
                ViewBag.Mensaje = ManejadorErroresCargaMasiva.ObtenerMensajeError(ManejadorErroresCargaMasiva.ErroresCargaMasiva.ArchivoNoEsExcel);
                ViewBag.Exito = false;
                return View();
            }

            using (var package = new ExcelPackage(archivoExcel.InputStream))
            {
                var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                {
                    ViewBag.Mensaje = ManejadorErroresCargaMasiva.ObtenerMensajeError(ManejadorErroresCargaMasiva.ErroresCargaMasiva.HojaDeCalculoNoEncontrada);
                    ViewBag.Exito = false;
                    return View();
                }

                var rowCount = worksheet.Dimension.Rows;
                var columnCount = worksheet.Dimension.Columns;

                // Validar encabezados de columna
                var expectedHeaders = new List<string> { "CN_old", "CN_NEW", "SAP_old", "SAP_new", "dFechaInicio", "dFechaFin" };
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
                var cambiosMateriales = new List<AnexoCambioMaterial>();
                var erroresValidacion = new List<string>();

                for (int row = 2; row <= rowCount; row++)
                {
                    var cambioMaterial = new AnexoCambioMaterial();

                    // Leer y validar cada celda
                    for (int col = 1; col <= columnCount; col++)
                    {
                        var cellValue = worksheet.Cells[row, col].Value;
                        var header = expectedHeaders[col - 1];

                        switch (header)
                        {
                            case "CN_old":
                                cambioMaterial.CN_old = cellValue?.ToString();
                                if (string.IsNullOrWhiteSpace(cambioMaterial.CN_old))
                                    erroresValidacion.Add(string.Format(ManejadorErroresCargaMasiva.ObtenerMensajeError(ErroresCargaMasiva.CampoObligatorio), row, header));
                                else if (cambioMaterial.CN_old.Length > 6)
                                    erroresValidacion.Add(string.Format(ManejadorErroresCargaMasiva.ObtenerMensajeError(ErroresCargaMasiva.LongitudMaximaExcedida), row, header, 6));
                                break;

                            case "CN_NEW":
                                cambioMaterial.CN_NEW = cellValue?.ToString();
                                if (!string.IsNullOrWhiteSpace(cambioMaterial.CN_NEW) && cambioMaterial.CN_NEW.Length > 6)
                                    erroresValidacion.Add(string.Format(ManejadorErroresCargaMasiva.ObtenerMensajeError(ErroresCargaMasiva.LongitudMaximaExcedida), row, header, 6));
                                break;

                            case "SAP_old":
                                cambioMaterial.SAP_old = cellValue?.ToString();
                                if (!string.IsNullOrWhiteSpace(cambioMaterial.SAP_old) && cambioMaterial.SAP_old.Length > 8)
                                    erroresValidacion.Add(string.Format(ManejadorErroresCargaMasiva.ObtenerMensajeError(ErroresCargaMasiva.LongitudMaximaExcedida), row, header, 8));
                                break;

                            case "SAP_new":
                                cambioMaterial.SAP_new = cellValue?.ToString();
                                if (!string.IsNullOrWhiteSpace(cambioMaterial.SAP_new) && cambioMaterial.SAP_new.Length > 8)
                                    erroresValidacion.Add(string.Format(ManejadorErroresCargaMasiva.ObtenerMensajeError(ErroresCargaMasiva.LongitudMaximaExcedida), row, header, 8));
                                break;

                            case "dFechaInicio":
                                if (DateTime.TryParse(cellValue?.ToString(), out DateTime fechaInicio))
                                    cambioMaterial.dFechaInicio = fechaInicio;
                                else
                                    erroresValidacion.Add(string.Format(ManejadorErroresCargaMasiva.ObtenerMensajeError(ErroresCargaMasiva.FormatoFechaInvalido), header));
                                break;

                            case "dFechaFin":
                                if (cellValue != null && !string.IsNullOrWhiteSpace(cellValue.ToString()))
                                {
                                    if (DateTime.TryParse(cellValue?.ToString(), out DateTime fechaFin))
                                        cambioMaterial.dFechaFin = fechaFin;
                                    else
                                        erroresValidacion.Add(string.Format(ManejadorErroresCargaMasiva.ObtenerMensajeError(ErroresCargaMasiva.FormatoFechaInvalido), header));

                                    if (cambioMaterial.dFechaFin <= cambioMaterial.dFechaInicio)
                                        erroresValidacion.Add(string.Format(ManejadorErroresCargaMasiva.ObtenerMensajeError(ErroresCargaMasiva.FechaFinMenorIgualFechaInicio), row));
                                }
                                break;

                            default:
                                break;
                        }
                    }

                    // Validar si ya existe un registro igual en la base de datos
                    if (db.AnexoCambioMaterials.Any(acm => acm.CN_old == cambioMaterial.CN_old && acm.dFechaInicio == cambioMaterial.dFechaInicio))
                    {
                        erroresValidacion.Add(string.Format(ManejadorErroresCargaMasiva.ObtenerMensajeError(ErroresCargaMasiva.RegistroDuplicado), row));
                    }

                    cambioMaterial.dFechaActualizacion = DateTime.Now;
                    cambioMaterial.sUsuarioActualizacion = "usuarioPrueba"; // Reemplazar con el nombre de usuario real si está disponible
                    cambiosMateriales.Add(cambioMaterial);
                }

                if (erroresValidacion.Any())
                {
                    ViewBag.Mensaje = "Errores de validación en el archivo Excel:<br/>" + string.Join("<br/>", erroresValidacion);
                    ViewBag.Errores = erroresValidacion;
                    ViewBag.Exito = false;
                    return View();
                }

                if (cambiosMateriales.Count() <= 0)
                {
                    ViewBag.Mensaje = ManejadorErroresCargaMasiva.ObtenerMensajeError(ManejadorErroresCargaMasiva.ErroresCargaMasiva.NoHayRegistrosEnExcel);
                    ViewBag.Exito = false;
                    return View();
                }
                else
                {
                    bool exito = CargarAnexoCambioMaterialMasivamente(cambiosMateriales, out string mensajeError);

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

        private bool CargarAnexoCambioMaterialMasivamente(List<AnexoCambioMaterial> cambiosMateriales, out string mensajeError)
        {
            mensajeError = "";

            try
            {
                foreach (var cambio in cambiosMateriales)
                {
                    if (ModelState.IsValid)
                    {
                        // Insertar el cambio de material utilizando db.SaveChanges()
                        db.AnexoCambioMaterials.Add(cambio);
                        db.SaveChanges();
                    }
                    else
                    {
                        var errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                        mensajeError = "Errores de validación:\n" + string.Join("\n", errores);
                        return false;
                    }
                }

                return true; // Indicar que la carga fue exitosa
            }
            catch (Exception ex)
            {
                // Capturar cualquier excepción y construir un mensaje de error
                mensajeError = "Error durante la carga masiva: " + ex.Message;
                if (ex.InnerException != null)
                {
                    mensajeError += "\nInner Exception: " + ex.InnerException.Message;
                }
                return false; // Indicar que hubo errores
            }
        }

        public ActionResult DescargarPlantilla()
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("AnexoCambioMaterial");
                worksheet.Cells[1, 1].Value = "CN_old";
                worksheet.Cells[1, 2].Value = "CN_NEW";
                worksheet.Cells[1, 3].Value = "SAP_old";
                worksheet.Cells[1, 4].Value = "SAP_new";
                worksheet.Cells[1, 5].Value = "dFechaInicio";
                worksheet.Cells[1, 6].Value = "dFechaFin";

                var stream = new MemoryStream(package.GetAsByteArray());
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Plantilla_AnexoCambioMaterial.xlsx");
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

        [HttpGet]
        public ActionResult CargarArchivo()
        {
            return View();
        }
    }
}
