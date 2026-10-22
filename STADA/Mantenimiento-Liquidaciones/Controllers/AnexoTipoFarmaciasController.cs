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
    public class AnexoTipoFarmaciasController : Controller
    {
        private CLUBS_AX_TESTEntities db = new CLUBS_AX_TESTEntities();

        // GET: AnexoTipoFarmacias
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ReadAnexoTipoFarmacia([DataSourceRequest] DataSourceRequest request, string sIdMayorista, string sNIF, string IdTipoFarmacia)
        {
            IQueryable<AnexoTipoFarmacia> anexoTipoFarmacias = db.AnexoTipoFarmacias; // Obtener todos los registros inicialmente

            // Aplicar filtros si los parámetros no son nulos ni vacíos
            if (!string.IsNullOrEmpty(sIdMayorista))
            {
                anexoTipoFarmacias = anexoTipoFarmacias.Where(a => a.sIdMayorista.Contains(sIdMayorista));
            }

            if (!string.IsNullOrEmpty(sNIF))
            {
                anexoTipoFarmacias = anexoTipoFarmacias.Where(a => a.sNIF.Contains(sNIF));
            }

            if (!string.IsNullOrEmpty(IdTipoFarmacia))
            {
                // Asumiendo que IdTipoFarmacia es de tipo numérico en tu modelo
                if (int.TryParse(IdTipoFarmacia, out int idTipoFarmacia))
                {
                    anexoTipoFarmacias = anexoTipoFarmacias.Where(a => a.IdTipoFarmacia == idTipoFarmacia);
                }
            }

            // Ordenar por dFechaActualizacion descendente (más reciente primero)
            anexoTipoFarmacias = anexoTipoFarmacias.OrderByDescending(a => a.dFechaActualizacion);

            // Aplicar paginación, ordenación y filtrado utilizando el DataSourceRequest
            DataSourceResult result = anexoTipoFarmacias.ToDataSourceResult(request);

            return Json(result, JsonRequestBehavior.AllowGet);
        }


     
        // POST: AnexoTipoFarmacias/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create([Bind(Include = "sIdMayorista,sNIF,sTipoFarmacia,IdTipoFarmacia,sCliente,dFechaIni,dFechaFin,dFechaActualizacion,sUsuarioActualizacion")] AnexoTipoFarmacia anexoTipoFarmacia)
        {
            if (ModelState.IsValid)
            {
                anexoTipoFarmacia.dFechaActualizacion = DateTime.Now;
                anexoTipoFarmacia.sUsuarioActualizacion = "SYSTEM";
                db.AnexoTipoFarmacias.Add(anexoTipoFarmacia);
                db.SaveChanges();
                return JavaScript("window.location.reload();");
            }

            return View(anexoTipoFarmacia);
        }

        
        // POST: AnexoTipoFarmacias/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit([Bind(Include = "sIdMayorista,sNIF,sTipoFarmacia,IdTipoFarmacia,sCliente,dFechaIni,dFechaFin,dFechaActualizacion,sUsuarioActualizacion")] AnexoTipoFarmacia anexoTipoFarmacia)
        {
            if (ModelState.IsValid)
            {
                anexoTipoFarmacia.dFechaActualizacion = DateTime.Now;
                anexoTipoFarmacia.sUsuarioActualizacion = "SYSTEM";
                db.Entry(anexoTipoFarmacia).State = EntityState.Modified;
                db.SaveChanges();
                return JavaScript("window.location.reload();");
            }
            return View(anexoTipoFarmacia);
        }

      
        // POST: AnexoTipoFarmacias/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(string id)
        {
            AnexoTipoFarmacia anexoTipoFarmacia = db.AnexoTipoFarmacias.Find(id);
            db.AnexoTipoFarmacias.Remove(anexoTipoFarmacia);
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

                var expectedHeaders = new List<string> { "sIdMayorista", "sNIF", "sTipoFarmacia", "IdTipoVenta", "IdTipoFarmacia", "sCliente", "dFechaIni", "dFechaFin" };
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

                var anexosTipoFarmacia = new List<AnexoTipoFarmacia>();
                var erroresValidacion = new List<string>();

                for (int row = 2; row <= rowCount; row++)
                {
                    var anexo = new AnexoTipoFarmacia();

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

                            case "sNIF":
                                anexo.sNIF = cellValue?.ToString();
                                if (string.IsNullOrWhiteSpace(anexo.sNIF))
                                    erroresValidacion.Add(string.Format(ManejadorErroresCargaMasiva.ObtenerMensajeError(ErroresCargaMasiva.CampoObligatorio), row, header));
                                else if (anexo.sNIF.Length > 50)
                                    erroresValidacion.Add(string.Format(ManejadorErroresCargaMasiva.ObtenerMensajeError(ErroresCargaMasiva.LongitudMaximaExcedida), row, header, 50));
                                break;

                            case "sTipoFarmacia":
                                anexo.sTipoFarmacia = cellValue?.ToString();
                                break;

                            case "IdTipoVenta":
                                if (int.TryParse(cellValue?.ToString(), out int idTipoVenta))
                                    anexo.IdTipoVenta = idTipoVenta;
                                else
                                    erroresValidacion.Add(string.Format(ManejadorErroresCargaMasiva.ObtenerMensajeError(ErroresCargaMasiva.FormatoNumeroInvalido), row, header));
                                break;

                            case "IdTipoFarmacia":
                                if (int.TryParse(cellValue?.ToString(), out int idTipoFarmacia))
                                    anexo.IdTipoFarmacia = idTipoFarmacia;
                                else
                                    erroresValidacion.Add(string.Format(ManejadorErroresCargaMasiva.ObtenerMensajeError(ErroresCargaMasiva.FormatoNumeroInvalido), row, header));
                                break;

                            case "sCliente":
                                if (double.TryParse(cellValue?.ToString(), out double cliente))
                                    anexo.sCliente = (float)cliente;
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
                        }
                    }

                    // Validar si ya existe un registro igual en la base de datos
                    if (db.AnexoTipoFarmacias.Any(a => a.sIdMayorista == anexo.sIdMayorista && a.sNIF == anexo.sNIF
                                                     && a.IdTipoVenta == anexo.IdTipoVenta && a.dFechaIni == anexo.dFechaIni))
                    {
                        erroresValidacion.Add(string.Format(ManejadorErroresCargaMasiva.ObtenerMensajeError(ErroresCargaMasiva.RegistroDuplicado), row));
                    }

                    anexo.dFechaActualizacion = DateTime.Now;
                    anexo.sUsuarioActualizacion = "usuarioPrueba"; // Reemplazar con el nombre de usuario real si está disponible
                    anexosTipoFarmacia.Add(anexo);
                }

                if (erroresValidacion.Any())
                {
                    ViewBag.Mensaje = "Errores de validación en el archivo Excel:<br/>" + string.Join("<br/>", erroresValidacion);
                    ViewBag.Errores = erroresValidacion;
                    ViewBag.Exito = false;
                    return View();
                }

                if (anexosTipoFarmacia.Count() <= 0)
                {
                    ViewBag.Mensaje = ManejadorErroresCargaMasiva.ObtenerMensajeError(ManejadorErroresCargaMasiva.ErroresCargaMasiva.NoHayRegistrosEnExcel);
                    ViewBag.Exito = false;
                    return View();
                }
                else
                {
                    bool exito = CargarAnexosTipoFarmaciaMasivamente(anexosTipoFarmacia, out string mensajeError);

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
                var worksheet = package.Workbook.Worksheets.Add("AnexoTipoFarmacia");
                worksheet.Cells[1, 1].Value = "sIdMayorista";
                worksheet.Cells[1, 2].Value = "sNIF";
                worksheet.Cells[1, 3].Value = "sTipoFarmacia";
                worksheet.Cells[1, 4].Value = "IdTipoVenta";
                worksheet.Cells[1, 5].Value = "IdTipoFarmacia";
                worksheet.Cells[1, 6].Value = "sCliente";
                worksheet.Cells[1, 7].Value = "dFechaIni";
                worksheet.Cells[1, 8].Value = "dFechaFin";

                var stream = new MemoryStream(package.GetAsByteArray());
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Plantilla_AnexoTipoFarmacia.xlsx");
            }
        }

        // Método interno para carga masiva de anexos de tipo de farmacia
        private bool CargarAnexosTipoFarmaciaMasivamente(List<AnexoTipoFarmacia> anexos, out string mensajeError)
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
                        db.AnexoTipoFarmacias.Add(anexo);
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
