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
    public class AnexoClubSubClubsController : Controller
    {
        private CLUBS_AX_TESTEntities db = new CLUBS_AX_TESTEntities();

        // GET: AnexoClubSubClubs
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ReadAnexoClubSubClub([DataSourceRequest] DataSourceRequest request, string sIdMayorista, string sRed, string IdTipoFarmacia, string sTipoVenta, string IdTipoSubVenta, string sZona, string dFechaIni, string dFechaFin)
        {
            IQueryable<AnexoClubSubClub> anexoClubSubClubs = db.AnexoClubSubClubs; // Obtener todos los registros inicialmente

            // Aplicar filtros si los parámetros no son nulos ni vacíos
            if (!string.IsNullOrEmpty(sIdMayorista))
            {
                anexoClubSubClubs = anexoClubSubClubs.Where(a => a.sIdMayorista.Contains(sIdMayorista));
            }

            if (!string.IsNullOrEmpty(sRed))
            {
                anexoClubSubClubs = anexoClubSubClubs.Where(a => a.sRed.Contains(sRed));
            }

            if (!string.IsNullOrEmpty(IdTipoFarmacia))
            {
                if (int.TryParse(IdTipoFarmacia, out int idTipoFarmacia))
                {
                    anexoClubSubClubs = anexoClubSubClubs.Where(a => a.IdTipoFarmacia == idTipoFarmacia);
                }
            }

            if (!string.IsNullOrEmpty(sTipoVenta))
            {
                anexoClubSubClubs = anexoClubSubClubs.Where(a => a.sTipoVenta.Contains(sTipoVenta));
            }

            if (!string.IsNullOrEmpty(IdTipoSubVenta))
            {
                if (int.TryParse(IdTipoSubVenta, out int idTipoSubVenta))
                {
                    anexoClubSubClubs = anexoClubSubClubs.Where(a => a.IdTipoSubVenta == idTipoSubVenta);
                }
            }

            if (!string.IsNullOrEmpty(sZona))
            {
                if (int.TryParse(sZona, out int zona))
                {
                    anexoClubSubClubs = anexoClubSubClubs.Where(a => a.sZona == zona);
                }
            }
            if (!string.IsNullOrEmpty(dFechaIni))
            {
                // Fecha Inicio: trae todo lo mayor o igual a esa fecha (sin límite superior)
                if (DateTime.TryParse(dFechaIni, out DateTime fechaIni))
                {
                    anexoClubSubClubs = anexoClubSubClubs.Where(m => m.dFechaIni >= fechaIni);
                }
            }

            if (!string.IsNullOrEmpty(dFechaFin))
            {
                // Fecha Fin: trae todo lo menor o igual a esa fecha (sin límite inferior). Se usa
                // "< fechaFin + 1 día" para incluir toda la jornada del día seleccionado.
                if (DateTime.TryParse(dFechaFin, out DateTime fechaFin))
                {
                    var fechaFinSiguiente = fechaFin.AddDays(1);
                    anexoClubSubClubs = anexoClubSubClubs.Where(m => m.dFechaFin.HasValue && m.dFechaFin < fechaFinSiguiente);
                }
            }

            // Ordenar por dFechaActualizacion descendente (más reciente primero)
            anexoClubSubClubs = anexoClubSubClubs.OrderByDescending(a => a.dFechaActualizacion);

            // Aplicar paginación, ordenación y filtrado utilizando el DataSourceRequest
            DataSourceResult result = anexoClubSubClubs.ToDataSourceResult(request);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

     

        // POST: AnexoClubSubClubs/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create([Bind(Include = "sIdMayorista,sRed,sTipoFarmacia,IdTipoFarmacia,sTipoVenta,IdTipoSubVenta,sZona,sCooperativa,sClub,sSubClub,dFechaIni,dFechaFin,sComentarios,dFechaActualizacion,sUsuarioActualizacion")] AnexoClubSubClub anexoClubSubClub)
        {
            if (ModelState.IsValid)
            {
                anexoClubSubClub.dFechaActualizacion = DateTime.Now;
                anexoClubSubClub.sUsuarioActualizacion = "SYSTEM";
                db.AnexoClubSubClubs.Add(anexoClubSubClub);
                db.SaveChanges();
                return JavaScript("window.location.reload();");
            }

            return View(anexoClubSubClub);
        }

     

        // POST: AnexoClubSubClubs/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit([Bind(Include = "sIdMayorista,sRed,sTipoFarmacia,IdTipoFarmacia,sTipoVenta,IdTipoSubVenta,sZona,sCooperativa,sClub,sSubClub,dFechaIni,dFechaFin,sComentarios,dFechaActualizacion,sUsuarioActualizacion")] AnexoClubSubClub anexoClubSubClub)
        {
            if (ModelState.IsValid)
            {
                anexoClubSubClub.dFechaActualizacion = DateTime.Now;
                anexoClubSubClub.sUsuarioActualizacion = "SYSTEM";
                db.Entry(anexoClubSubClub).State = EntityState.Modified;
                db.SaveChanges();
                return JavaScript("window.location.reload();");
            }
            return View(anexoClubSubClub);
        }

       
        // POST: AnexoClubSubClubs/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(string id)
        {
            AnexoClubSubClub anexoClubSubClub = db.AnexoClubSubClubs.Find(id);
            db.AnexoClubSubClubs.Remove(anexoClubSubClub);
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
        // En AnexoClubSubClubsController.cs

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

                var expectedHeaders = new List<string> { "sIdMayorista", "sRed", "sTipoFarmacia", "IdTipoFarmacia", "sTipoVenta", "IdTipoSubVenta", "sZona", "sCooperativa", "sClub", "sSubClub", "dFechaIni", "dFechaFin", "sComentarios" };
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

                var anexosClubSubclub = new List<AnexoClubSubClub>();
                var erroresValidacion = new List<string>();

                for (int row = 2; row <= rowCount; row++)
                {
                    var anexo = new AnexoClubSubClub();

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

                            case "sRed":
                                anexo.sRed = cellValue?.ToString();
                                if (string.IsNullOrWhiteSpace(anexo.sRed))
                                    erroresValidacion.Add(string.Format(ManejadorErroresCargaMasiva.ObtenerMensajeError(ErroresCargaMasiva.CampoObligatorio), row, header));
                                else if (anexo.sRed.Length > 4)
                                    erroresValidacion.Add(string.Format(ManejadorErroresCargaMasiva.ObtenerMensajeError(ErroresCargaMasiva.LongitudMaximaExcedida), row, header, 4));
                                break;

                            case "sTipoFarmacia":
                                anexo.sTipoFarmacia = cellValue?.ToString();
                                break;

                            case "IdTipoFarmacia":
                                if (int.TryParse(cellValue?.ToString(), out int idTipoFarmacia))
                                    anexo.IdTipoFarmacia = idTipoFarmacia;
                                else
                                    erroresValidacion.Add(string.Format(ManejadorErroresCargaMasiva.ObtenerMensajeError(ErroresCargaMasiva.FormatoNumeroInvalido), row, header));
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

                            case "sZona":
                                if (int.TryParse(cellValue?.ToString(), out int zona))
                                    anexo.sZona = zona;
                                else
                                    erroresValidacion.Add(string.Format(ManejadorErroresCargaMasiva.ObtenerMensajeError(ErroresCargaMasiva.FormatoNumeroInvalido), row, header));
                                break;

                            case "sCooperativa":
                                anexo.sCooperativa = cellValue?.ToString();
                                if (string.IsNullOrWhiteSpace(anexo.sCooperativa))
                                    erroresValidacion.Add(string.Format(ManejadorErroresCargaMasiva.ObtenerMensajeError(ErroresCargaMasiva.CampoObligatorio), row, header));
                                else if (anexo.sCooperativa.Length > 200)
                                    erroresValidacion.Add(string.Format(ManejadorErroresCargaMasiva.ObtenerMensajeError(ErroresCargaMasiva.LongitudMaximaExcedida), row, header, 200));
                                break;

                            case "sClub":
                                anexo.sClub = cellValue?.ToString();
                                if (string.IsNullOrWhiteSpace(anexo.sClub))
                                    erroresValidacion.Add(string.Format(ManejadorErroresCargaMasiva.ObtenerMensajeError(ErroresCargaMasiva.CampoObligatorio), row, header));
                                else if (anexo.sClub.Length > 4)
                                    erroresValidacion.Add(string.Format(ManejadorErroresCargaMasiva.ObtenerMensajeError(ErroresCargaMasiva.LongitudMaximaExcedida), row, header, 4));
                                break;

                            case "sSubClub":
                                anexo.sSubClub = cellValue?.ToString();
                                if (string.IsNullOrWhiteSpace(anexo.sSubClub))
                                    erroresValidacion.Add(string.Format(ManejadorErroresCargaMasiva.ObtenerMensajeError(ErroresCargaMasiva.CampoObligatorio), row, header));
                                else if (anexo.sSubClub.Length > 4)
                                    erroresValidacion.Add(string.Format(ManejadorErroresCargaMasiva.ObtenerMensajeError(ErroresCargaMasiva.LongitudMaximaExcedida), row, header, 4));
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

                            case "sComentarios":
                                anexo.sComentarios = cellValue?.ToString();
                                if (!string.IsNullOrWhiteSpace(anexo.sComentarios) && anexo.sComentarios.Length > 512)
                                    erroresValidacion.Add(string.Format(ManejadorErroresCargaMasiva.ObtenerMensajeError(ErroresCargaMasiva.LongitudMaximaExcedida), row, header, 512));
                                break;
                        }
                    }

                    if (db.AnexoClubSubClubs.Any(acs => acs.sIdMayorista == anexo.sIdMayorista && acs.sRed == anexo.sRed
                                                      && acs.IdTipoFarmacia == anexo.IdTipoFarmacia && acs.IdTipoSubVenta == anexo.IdTipoSubVenta
                                                      && acs.sZona == anexo.sZona && acs.sCooperativa == anexo.sCooperativa
                                                      && acs.sClub == anexo.sClub && acs.sSubClub == anexo.sSubClub
                                                      && acs.dFechaIni == anexo.dFechaIni))
                    {
                        erroresValidacion.Add(string.Format(ManejadorErroresCargaMasiva.ObtenerMensajeError(ErroresCargaMasiva.RegistroDuplicado), row));
                    }

                    anexo.dFechaActualizacion = DateTime.Now;
                    anexo.sUsuarioActualizacion = "usuarioPrueba";
                    anexosClubSubclub.Add(anexo);
                }

                if (erroresValidacion.Any())
                {
                    ViewBag.Mensaje = "Errores de validación en el archivo Excel:<br/>" + string.Join("<br/>", erroresValidacion);
                    ViewBag.Errores = erroresValidacion;
                    ViewBag.Exito = false;
                    return View();
                }

                if (anexosClubSubclub.Count() <= 0)
                {
                    ViewBag.Mensaje = ManejadorErroresCargaMasiva.ObtenerMensajeError(ManejadorErroresCargaMasiva.ErroresCargaMasiva.NoHayRegistrosEnExcel);
                    ViewBag.Exito = false;
                    return View();
                }
                else
                {
                    bool exito = CargarAnexosClubSubClubMasivamente(anexosClubSubclub, out string mensajeError);

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
                var worksheet = package.Workbook.Worksheets.Add("AnexoClubSubClub");
                worksheet.Cells[1, 1].Value = "sIdMayorista";
                worksheet.Cells[1, 2].Value = "sRed";
                worksheet.Cells[1, 3].Value = "sTipoFarmacia";
                worksheet.Cells[1, 4].Value = "IdTipoFarmacia";
                worksheet.Cells[1, 5].Value = "sTipoVenta";
                worksheet.Cells[1, 6].Value = "IdTipoSubVenta";
                worksheet.Cells[1, 7].Value = "sZona";
                worksheet.Cells[1, 8].Value = "sCooperativa";
                worksheet.Cells[1, 9].Value = "sClub";
                worksheet.Cells[1, 10].Value = "sSubClub";
                worksheet.Cells[1, 11].Value = "dFechaIni";
                worksheet.Cells[1, 12].Value = "dFechaFin";
                worksheet.Cells[1, 13].Value = "sComentarios";

                var stream = new MemoryStream(package.GetAsByteArray());
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Plantilla_AnexoClubSubClub.xlsx");
            }
        }

        // Método interno para carga masiva de anexos de club y subclub
        private bool CargarAnexosClubSubClubMasivamente(List<AnexoClubSubClub> anexos, out string mensajeError)
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
                        db.AnexoClubSubClubs.Add(anexo);
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
    }
}
