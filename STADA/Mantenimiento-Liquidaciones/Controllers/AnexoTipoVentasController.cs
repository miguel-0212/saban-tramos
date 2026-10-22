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
    public class AnexoTipoVentasController : Controller
    {
        private CLUBS_AX_TESTEntities db = new CLUBS_AX_TESTEntities();

        // GET: AnexoTipoVentas
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult ReadAnexoTipoVenta([DataSourceRequest] DataSourceRequest request, string idMayorista, string idTipoSubVenta, string sNotas)
        {
            IQueryable<AnexoTipoVenta> anexoTipoVentas = db.AnexoTipoVentas; // Obtener todos los registros inicialmente

            // Aplicar filtros si los parámetros no son nulos ni vacíos
            if (!string.IsNullOrEmpty(idMayorista))
            {
                anexoTipoVentas = anexoTipoVentas.Where(a => a.sIdMayorista.Contains(idMayorista));
            }

            if (!string.IsNullOrEmpty(idTipoSubVenta))
            {
                if (int.TryParse(idTipoSubVenta, out int idTipoSubVentaInt))
                {
                    anexoTipoVentas = anexoTipoVentas.Where(a => a.IdTipoSubVenta == idTipoSubVentaInt);
                }
            }

            if (!string.IsNullOrEmpty(sNotas))
            {
                anexoTipoVentas = anexoTipoVentas.Where(a => a.sNotas.Contains(sNotas));
            }

            // Ordenar por dFechaActualizacion descendente (más reciente primero)
            anexoTipoVentas = anexoTipoVentas.OrderByDescending(a => a.dFechaActualizacion);

            // Aplicar paginación, ordenación y filtrado utilizando el DataSourceRequest
            DataSourceResult result = anexoTipoVentas.ToDataSourceResult(request);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

      

        // POST: AnexoTipoVentas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create([Bind(Include = "sIdMayorista,sCodigo,sTipoVenta,IdTipoSubVenta,dFechaIni,dFechaFin,sNotas,dFechaActualizacion,sUsuarioActualizacion")] AnexoTipoVenta anexoTipoVenta)
        {
            if (ModelState.IsValid)
            {
                anexoTipoVenta.dFechaActualizacion = DateTime.Now;
                anexoTipoVenta.sUsuarioActualizacion = "SYSTEM";
                db.AnexoTipoVentas.Add(anexoTipoVenta);
                db.SaveChanges();
                return JavaScript("window.location.reload();");
            }

            return View(anexoTipoVenta);
        }


        // POST: AnexoTipoVentas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit([Bind(Include = "sIdMayorista,sCodigo,sTipoVenta,IdTipoSubVenta,dFechaIni,dFechaFin,sNotas,dFechaActualizacion,sUsuarioActualizacion")] AnexoTipoVenta anexoTipoVenta)
        {
            if (ModelState.IsValid)
            {
                anexoTipoVenta.dFechaActualizacion = DateTime.Now;
                anexoTipoVenta.sUsuarioActualizacion = "SYSTEM";
                db.Entry(anexoTipoVenta).State = EntityState.Modified;
                db.SaveChanges();
                return JavaScript("window.location.reload();");
            }
            return View(anexoTipoVenta);
        }

        

        // POST: AnexoTipoVentas/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(string id)
        {
            AnexoTipoVenta anexoTipoVenta = db.AnexoTipoVentas.Find(id);
            db.AnexoTipoVentas.Remove(anexoTipoVenta);
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

                var expectedHeaders = new List<string> { "sIdMayorista", "sCodigo", "sTipoVenta", "IdTipoSubVenta", "dFechaIni", "dFechaFin", "sNotas" };
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

                var anexosTipoVenta = new List<AnexoTipoVenta>();
                var erroresValidacion = new List<string>();

                for (int row = 2; row <= rowCount; row++)
                {
                    var anexo = new AnexoTipoVenta();

                    for (int col = 1; col <= columnCount; col++)
                    {
                        var cellValue = worksheet.Cells[row, col].Value;
                        var header = expectedHeaders[col - 1];

                        switch (header)
                        {
                            case "sIdMayorista":
                                anexo.sIdMayorista = cellValue?.ToString();
                                if (string.IsNullOrWhiteSpace(anexo.sIdMayorista))
                                    erroresValidacion.Add(string.Format(ManejadorErroresCargaMasiva.ObtenerMensajeError(ErroresCargaMasiva.CampoObligatorio), row, header));
                                else if (anexo.sIdMayorista.Length > 10)
                                    erroresValidacion.Add(string.Format(ManejadorErroresCargaMasiva.ObtenerMensajeError(ErroresCargaMasiva.LongitudMaximaExcedida), row, header, 10));
                                break;

                            case "sCodigo":
                                anexo.sCodigo = cellValue?.ToString();
                                if (string.IsNullOrWhiteSpace(anexo.sCodigo))
                                    erroresValidacion.Add(string.Format(ManejadorErroresCargaMasiva.ObtenerMensajeError(ErroresCargaMasiva.CampoObligatorio), row, header));
                                else if (anexo.sCodigo.Length > 64)
                                    erroresValidacion.Add(string.Format(ManejadorErroresCargaMasiva.ObtenerMensajeError(ErroresCargaMasiva.LongitudMaximaExcedida), row, header, 64));
                                break;

                            case "sTipoVenta":
                                anexo.sTipoVenta = cellValue?.ToString();
                                break;

                            case "IdTipoSubVenta":
                                if (int.TryParse(cellValue?.ToString(), out int idTipoSubVenta))
                                    anexo.IdTipoSubVenta = idTipoSubVenta;
                                else
                                    erroresValidacion.Add(string.Format(ManejadorErroresCargaMasiva.ObtenerMensajeError(ErroresCargaMasiva.FormatoNumeroInvalido), row, header));
                                break;

                            case "dFechaIni":
                                if (DateTime.TryParse(cellValue?.ToString(), out DateTime fechaIni))
                                    anexo.dFechaIni = fechaIni;
                                else
                                    erroresValidacion.Add(string.Format(ManejadorErroresCargaMasiva.ObtenerMensajeError(ErroresCargaMasiva.FormatoFechaInvalido), row, header));
                                break;

                            case "dFechaFin":
                                if (cellValue != null && !string.IsNullOrWhiteSpace(cellValue.ToString()))
                                {
                                    if (DateTime.TryParse(cellValue?.ToString(), out DateTime fechaFin))
                                        anexo.dFechaFin = fechaFin;
                                    else
                                        erroresValidacion.Add(string.Format(ManejadorErroresCargaMasiva.ObtenerMensajeError(ErroresCargaMasiva.FormatoFechaInvalido), row, header));

                                    if (anexo.dFechaFin <= anexo.dFechaIni)
                                        erroresValidacion.Add(string.Format(ManejadorErroresCargaMasiva.ObtenerMensajeError(ErroresCargaMasiva.FechaFinMenorIgualFechaInicio), row));
                                }
                                break;

                            case "sNotas":
                                anexo.sNotas = cellValue?.ToString();
                                break;
                        }
                    }

                    // Validar si ya existe un registro igual en la base de datos
                    if (db.AnexoTipoVentas.Any(a => a.sIdMayorista == anexo.sIdMayorista && a.sCodigo == anexo.sCodigo && a.dFechaIni == anexo.dFechaIni))
                    {
                        erroresValidacion.Add(string.Format(ManejadorErroresCargaMasiva.ObtenerMensajeError(ErroresCargaMasiva.RegistroDuplicado), row));
                    }

                    anexo.dFechaActualizacion = DateTime.Now;
                    anexo.sUsuarioActualizacion = "usuarioPrueba"; // Reemplazar con el nombre de usuario real si está disponible
                    anexosTipoVenta.Add(anexo);
                }

                if (erroresValidacion.Any())
                {
                    ViewBag.Mensaje = "Errores de validación en el archivo Excel:<br/>" + string.Join("<br/>", erroresValidacion);
                    ViewBag.Errores = erroresValidacion;
                    ViewBag.Exito = false;
                    return View();
                }

                if (anexosTipoVenta.Count() <= 0)
                {
                    ViewBag.Mensaje = ManejadorErroresCargaMasiva.ObtenerMensajeError(ManejadorErroresCargaMasiva.ErroresCargaMasiva.NoHayRegistrosEnExcel);
                    ViewBag.Exito = false;
                    return View();
                }
                else
                {
                    bool exito = CargarAnexosTipoVentaMasivamente(anexosTipoVenta, out string mensajeError);

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

        public ActionResult DescargarPlantilla()
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("AnexoTipoVenta");
                worksheet.Cells[1, 1].Value = "sIdMayorista";
                worksheet.Cells[1, 2].Value = "sCodigo";
                worksheet.Cells[1, 3].Value = "sTipoVenta";
                worksheet.Cells[1, 4].Value = "IdTipoSubVenta";
                worksheet.Cells[1, 5].Value = "dFechaIni";
                worksheet.Cells[1, 6].Value = "dFechaFin";
                worksheet.Cells[1, 7].Value = "sNotas";

                var stream = new MemoryStream(package.GetAsByteArray());
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Plantilla_AnexoTipoVenta.xlsx");
            }
        }

        // Método interno para carga masiva de anexos de tipo de venta
        private bool CargarAnexosTipoVentaMasivamente(List<AnexoTipoVenta> anexos, out string mensajeError)
        {
            mensajeError = "";

            try
            {
                foreach (var anexo in anexos)
                {
                    if (ModelState.IsValid)
                    {
                        anexo.dFechaActualizacion = DateTime.Now;
                        anexo.sUsuarioActualizacion = "usuarioPrueba"; // Reemplazar con el nombre de usuario real si está disponible

                        // Insertar el anexo utilizando Entity Framework
                        db.AnexoTipoVentas.Add(anexo);
                        db.SaveChanges();
                    }
                    else
                    {
                        var errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                        mensajeError = "Errores de validación:\n" + string.Join("\n", errores);
                        return false;
                    }
                }

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
