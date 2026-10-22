using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
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
    public class AnexoClubSociosController : Controller
    {
        private CLUBS_AX_TESTEntities db = new CLUBS_AX_TESTEntities();

        // GET: AnexoClubSocios
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ReadAnexoClubSocios([DataSourceRequest] DataSourceRequest request, string sIdClub, string sIdRed, string iFormaCobro)
        {
            IQueryable<AnexoClubSocios> anexoClubSocios = db.AnexoClubSocios1; // Obtener todos los registros inicialmente

            // Aplicar filtros si los parámetros no son nulos ni vacíos
            if (!string.IsNullOrEmpty(sIdClub))
            {
                anexoClubSocios = anexoClubSocios.Where(a => a.sIdClub.Contains(sIdClub));
            }

            if (!string.IsNullOrEmpty(sIdRed))
            {
                anexoClubSocios = anexoClubSocios.Where(a => a.sIdRed.Contains(sIdRed));
            }

            if (!string.IsNullOrEmpty(iFormaCobro))
            {
                // Asumiendo que iFormaCobro es de tipo numérico en tu modelo
                if (int.TryParse(iFormaCobro, out int formaCobro))
                {
                    anexoClubSocios = anexoClubSocios.Where(a => a.iFormaCobro == formaCobro);
                }
            }

            // Ordenar por dFechaActualizacion descendente (más reciente primero)
            anexoClubSocios = anexoClubSocios.OrderByDescending(a => a.dFechaActualizacion);

            // Aplicar paginación, ordenación y filtrado utilizando el DataSourceRequest
            DataSourceResult result = anexoClubSocios.ToDataSourceResult(request);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // GET: AnexoClubSocios/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AnexoClubSocios anexoClubSocios = db.AnexoClubSocios1.Find(id);
            if (anexoClubSocios == null)
            {
                return HttpNotFound();
            }
            return View(anexoClubSocios);
        }

       
        // POST: AnexoClubSocios/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create([Bind(Include = "sIdClub,sIdRed,sIdSocio,iFormaCobro,dFechaIni,dFechaFin,dFechaActualizacion,sUsuarioActualizacion")] AnexoClubSocios anexoClubSocios)
        {
            if (ModelState.IsValid)
            {
                anexoClubSocios.dFechaActualizacion = DateTime.Now;
                anexoClubSocios.sUsuarioActualizacion = "SYSTEM";
                db.AnexoClubSocios1.Add(anexoClubSocios);
                db.SaveChanges();
                return JavaScript("window.location.reload();");
            }

            return View(anexoClubSocios);
        }

    
        // POST: AnexoClubSocios/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit([Bind(Include = "sIdClub,sIdRed,sIdSocio,iFormaCobro,dFechaIni,dFechaFin,dFechaActualizacion,sUsuarioActualizacion")] AnexoClubSocios anexoClubSocios)
        {
            if (ModelState.IsValid)
            {
                anexoClubSocios.dFechaActualizacion = DateTime.Now;
                anexoClubSocios.sUsuarioActualizacion = "SYSTEM";
                db.Entry(anexoClubSocios).State = EntityState.Modified;
                db.SaveChanges();
                return JavaScript("window.location.reload();");
            }
            return View(anexoClubSocios);
        }

      

        // POST: AnexoClubSocios/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(string id)
        {
            AnexoClubSocios anexoClubSocios = db.AnexoClubSocios1.Find(id);
            db.AnexoClubSocios1.Remove(anexoClubSocios);
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

        [HttpPost]
        public ActionResult CargarArchivo(HttpPostedFileBase archivoExcel)
        {
            if (archivoExcel == null || archivoExcel.ContentLength == 0)
            {
                ViewBag.Mensaje = ObtenerMensajeError(ErroresCargaMasiva.ArchivoNoSeleccionado);
                ViewBag.Exito = false;
                return View();
            }

            if (!archivoExcel.FileName.EndsWith(".xlsx"))
            {
                ViewBag.Mensaje = ObtenerMensajeError(ErroresCargaMasiva.ArchivoNoEsExcel);
                ViewBag.Exito = false;
                return View();
            }

            using (var package = new ExcelPackage(archivoExcel.InputStream))
            {
                var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                {
                    ViewBag.Mensaje = ObtenerMensajeError(ErroresCargaMasiva.HojaDeCalculoNoEncontrada);
                    ViewBag.Exito = false;
                    return View();
                }

                var rowCount = worksheet.Dimension.Rows;
                var columnCount = worksheet.Dimension.Columns;

                // Validar encabezados de columna
                var expectedHeaders = new List<string> { "sIdClub", "sIdRed", "sIdSocio", "iFormaCobro", "dFechaIni", "dFechaFin" };
                for (int col = 1; col <= columnCount; col++)
                {
                    var header = worksheet.Cells[1, col].Value?.ToString();
                    if (!expectedHeaders.Contains(header))
                    {
                        ViewBag.Mensaje = string.Format(ObtenerMensajeError(ErroresCargaMasiva.ColumnaFaltanteOIncorrecta), header);
                        ViewBag.Exito = false;
                        return View();
                    }
                }

                // Extraer y validar datos
                var anexosClubSocios = new List<AnexoClubSocios>();
                var erroresValidacion = new List<string>();

                for (int row = 2; row <= rowCount; row++)
                {
                    var anexo = new AnexoClubSocios();

                    // Leer y validar cada celda
                    for (int col = 1; col <= columnCount; col++)
                    {
                        var cellValue = worksheet.Cells[row, col].Value;
                        var header = expectedHeaders[col - 1];

                        switch (header)
                        {
                            case "sIdClub":
                                anexo.sIdClub = cellValue?.ToString();
                                if (string.IsNullOrWhiteSpace(anexo.sIdClub))
                                    erroresValidacion.Add(string.Format(ObtenerMensajeError(ErroresCargaMasiva.CampoObligatorio), row, header));
                                else if (anexo.sIdClub.Length > 4)
                                    erroresValidacion.Add(string.Format(ObtenerMensajeError(ErroresCargaMasiva.LongitudMaximaExcedida), row, header, 4));
                                break;

                            case "sIdRed":
                                anexo.sIdRed = cellValue?.ToString();
                                if (string.IsNullOrWhiteSpace(anexo.sIdRed))
                                    erroresValidacion.Add(string.Format(ObtenerMensajeError(ErroresCargaMasiva.CampoObligatorio), row, header));
                                else if (anexo.sIdRed.Length > 4)
                                    erroresValidacion.Add(string.Format(ObtenerMensajeError(ErroresCargaMasiva.LongitudMaximaExcedida), row, header, 4));
                                break;

                            case "sIdSocio":
                                anexo.sIdSocio = cellValue?.ToString();
                                if (string.IsNullOrWhiteSpace(anexo.sIdSocio))
                                    erroresValidacion.Add(string.Format(ObtenerMensajeError(ErroresCargaMasiva.CampoObligatorio), row, header));
                                else if (anexo.sIdSocio.Length > 50)
                                    erroresValidacion.Add(string.Format(ObtenerMensajeError(ErroresCargaMasiva.LongitudMaximaExcedida), row, header, 50));
                                break;

                            case "iFormaCobro":
                                if (int.TryParse(cellValue?.ToString(), out int formaCobro))
                                    anexo.iFormaCobro = formaCobro;
                                else
                                    erroresValidacion.Add(string.Format(ObtenerMensajeError(ErroresCargaMasiva.FormatoNumeroInvalido), header));
                                break;

                            case "dFechaIni":
                                if (double.TryParse(cellValue?.ToString(), out double oaDate))
                                {
                                    try
                                    {
                                        DateTime fechaInicio = DateTime.FromOADate(oaDate);
                                        anexo.dFechaIni = fechaInicio;
                                    }
                                    catch (ArgumentException)
                                    {
                                        erroresValidacion.Add(string.Format(ObtenerMensajeError(ErroresCargaMasiva.FormatoFechaInvalido), header));
                                    }
                                }
                                else
                                {
                                    erroresValidacion.Add(string.Format(ObtenerMensajeError(ErroresCargaMasiva.FormatoFechaInvalido), header));
                                }
                                break;

                            case "dFechaFin":
                                if (cellValue != null && !string.IsNullOrWhiteSpace(cellValue.ToString()))
                                {
                                    if (double.TryParse(cellValue?.ToString(), out double oaDateFin))
                                    {
                                        try
                                        {
                                            DateTime fechaFin = DateTime.FromOADate(oaDateFin);
                                            anexo.dFechaFin = fechaFin;
                                        }
                                        catch (ArgumentException)
                                        {
                                            erroresValidacion.Add(string.Format(ObtenerMensajeError(ErroresCargaMasiva.FormatoFechaInvalido), header));
                                        }
                                    }
                                    else
                                    {
                                        erroresValidacion.Add(string.Format(ObtenerMensajeError(ErroresCargaMasiva.FormatoFechaInvalido), header));
                                    }

                                    if (anexo.dFechaFin <= anexo.dFechaIni)
                                        erroresValidacion.Add(string.Format(ObtenerMensajeError(ErroresCargaMasiva.FechaFinMenorIgualFechaInicio), row));
                                }
                                break;

                            default:
                                break;
                        }
                    }

                    // Validar si ya existe un registro igual en la base de datos
                    if (db.AnexoClubSocios1.Any(acs => acs.sIdClub == anexo.sIdClub && acs.sIdRed == anexo.sIdRed
                                                      && acs.sIdSocio == anexo.sIdSocio && acs.dFechaIni == anexo.dFechaIni))
                    {
                        erroresValidacion.Add(string.Format(ObtenerMensajeError(ErroresCargaMasiva.RegistroDuplicado), row));
                    }

                    anexo.dFechaActualizacion = DateTime.Now;
                    anexo.sUsuarioActualizacion = "usuarioPrueba";
                    anexosClubSocios.Add(anexo);
                }

                if (erroresValidacion.Any())
                {
                    ViewBag.Mensaje = "Errores de validación en el archivo Excel:";
                    ViewBag.Errores = erroresValidacion;
                    ViewBag.Exito = false;
                    return View();
                }

                if (anexosClubSocios.Count() <= 0)
                {
                    ViewBag.Mensaje = ObtenerMensajeError(ErroresCargaMasiva.NoHayRegistrosEnExcel);
                    ViewBag.Exito = false;
                    return View();
                }
                else
                {
                    bool exito = CargarAnexosClubSociosMasivamente(anexosClubSocios, out string mensajeError);

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
                var worksheet = package.Workbook.Worksheets.Add("AnexoClubSocios");
                worksheet.Cells[1, 1].Value = "sIdClub";
                worksheet.Cells[1, 2].Value = "sIdRed";
                worksheet.Cells[1, 3].Value = "sIdSocio";
                worksheet.Cells[1, 4].Value = "iFormaCobro";
                worksheet.Cells[1, 5].Value = "dFechaIni";
                worksheet.Cells[1, 6].Value = "dFechaFin";

                var stream = new MemoryStream(package.GetAsByteArray());
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Plantilla_AnexoClubSocios.xlsx");
            }
        }

        // Método interno para carga masiva de anexos de club y socios
        private bool CargarAnexosClubSociosMasivamente(List<AnexoClubSocios> anexos, out string mensajeError)
        {
            mensajeError = "";

            try
            {
                // Convertir la lista de anexos a un DataTable
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("sIdClub", typeof(string));
                dataTable.Columns.Add("sIdRed", typeof(string));
                dataTable.Columns.Add("sIdSocio", typeof(string));
                dataTable.Columns.Add("iFormaCobro", typeof(int));
                dataTable.Columns.Add("dFechaIni", typeof(DateTime));
                dataTable.Columns.Add("dFechaFin", typeof(DateTime));
                dataTable.Columns.Add("dFechaActualizacion", typeof(DateTime));
                dataTable.Columns.Add("sUsuarioActualizacion", typeof(string));

                foreach (var anexo in anexos)
                {
                    dataTable.Rows.Add(anexo.sIdClub, anexo.sIdRed, anexo.sIdSocio, anexo.iFormaCobro,
                                       anexo.dFechaIni, anexo.dFechaFin, anexo.dFechaActualizacion, anexo.sUsuarioActualizacion);
                }

                // Usar SqlBulkCopy para la inserción masiva
                using (var bulkCopy = new SqlBulkCopy(db.Database.Connection.ConnectionString))
                {
                    bulkCopy.DestinationTableName = "dbo.AnexoClubSocios";
                    bulkCopy.WriteToServer(dataTable);
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
