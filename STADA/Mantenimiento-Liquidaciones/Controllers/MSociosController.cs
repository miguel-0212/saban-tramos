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
    public class MSociosController : Controller
    {
        private CLUBS_AX_TESTEntities db = new CLUBS_AX_TESTEntities();

        // GET: MSocios
        public ActionResult Index()
        {
            return View();
        }



        public ActionResult ReadMSocios([DataSourceRequest] DataSourceRequest request, string sIdSocio, string sCodSAP, string sNIF, string sNombreFcia, string sDireccion, string sCP, string sPoblacion, string dFechaActualizacion)
        {
            IQueryable<MSocios> socios = db.MSocios; // Obtener todos los socios inicialmente

            // Aplicar filtros si los parámetros no son nulos ni vacíos
            if (!string.IsNullOrEmpty(sCodSAP))
            {
                socios = socios.Where(s => s.sCodSAP.Contains(sCodSAP));
            }

            if (!string.IsNullOrEmpty(sIdSocio))
            {
                socios = socios.Where(s => s.sIdSocio.Contains(sIdSocio));
            }

            if (!string.IsNullOrEmpty(sNIF))
            {
                socios = socios.Where(s => s.sNIF.Contains(sNIF));
            }

            if (!string.IsNullOrEmpty(sNombreFcia))
            {
                socios = socios.Where(s => s.sNombreFcia.Contains(sNombreFcia));
            }

            if (!string.IsNullOrEmpty(sDireccion))
            {
                socios = socios.Where(s => s.sDireccion.Contains(sDireccion));
            }

            if (!string.IsNullOrEmpty(sCP))
            {
                socios = socios.Where(s => s.sCP.Contains(sCP));
            }

            if (!string.IsNullOrEmpty(sPoblacion))
            {
                socios = socios.Where(s => s.sPoblacion.Contains(sPoblacion));
            }
            if (!string.IsNullOrEmpty(dFechaActualizacion))
            {
                if (DateTime.TryParse(dFechaActualizacion, out DateTime fechaIni))
                {
                    var fechaIniSiguiente = fechaIni.AddDays(1); // Calcular fuera del filtro
                    socios = socios.Where(m => m.dFechaActualizacion >= fechaIni && m.dFechaActualizacion < fechaIniSiguiente);
                }
            }

            // Ordenar (sin limitar a un número fijo de registros: ToDataSourceResult pagina sobre la consulta completa)
            socios = socios.OrderByDescending(m => m.dFechaActualizacion);

            // Aplicar paginación, ordenación y filtrado utilizando el DataSourceRequest
            DataSourceResult result = socios.ToDataSourceResult(request);

            // JsonResult.MaxJsonLength por defecto es ~2 MB (JavaScriptSerializer). Con muchos
            // registros (p. ej. "Exportar a Excel" con AllPages, que pide todo de una vez) el JSON
            // puede superar ese límite y devolver 500. Se sube el límite solo para esta acción.
            return new JsonResult
            {
                Data = result,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = int.MaxValue
            };
        }

        
        // POST: MSocios/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create([Bind(Include = "sIdSocio,sCodSAP,sNIF,sNombreFcia,sDireccion,sCP,sPoblacion,iVirtualId,dFechaActualizacion,sUsuarioActualizacion")] MSocios mSocios)
        {
            if (ModelState.IsValid)
            {
                mSocios.dFechaActualizacion = DateTime.Now;
                mSocios.sUsuarioActualizacion = "SYSTEM";
                db.MSocios.Add(mSocios);
                db.SaveChanges();
                return JavaScript("window.location.reload();");
            }

            return View(mSocios);
        }

        [HttpGet]
        public ActionResult CargarArchivo()
        {
            return View();
        }

        

        // POST: MSocios/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit([Bind(Include = "sIdSocio,sCodSAP,sNIF,sNombreFcia,sDireccion,sCP,sPoblacion,iVirtualId,dFechaActualizacion,sUsuarioActualizacion")] MSocios mSocios)
        {
            if (ModelState.IsValid)
            {
                mSocios.dFechaActualizacion = DateTime.Now;
                mSocios.sUsuarioActualizacion = "SYSTEM";
                db.Entry(mSocios).State = EntityState.Modified;
                db.SaveChanges();
                return JavaScript("window.location.reload();");
            }
            return View(mSocios);
        }


        // POST: MSocios/Delete/5
        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(string sIdSocio)
        {
            var response = new { success = false, message = "Error al eliminar el socio" }; // Respuesta inicial

            try
            {
                // Buscar el socio por su ID
                MSocios mSocios = db.MSocios.Find(sIdSocio);

                if (mSocios != null)
                {
                    db.MSocios.Remove(mSocios);
                    db.SaveChanges();

                    // Actualizar la respuesta en caso de éxito
                    response = new { success = true, message = "Socio eliminado con éxito" };
                }
                else
                {
                    response = new { success = false, message = "Socio no encontrado" };
                }
            }
            catch (Exception ex)
            {
                response = new { success = false, message = "Error al eliminar el socio: " + ex.Message };
            }

            return Json(response);
        }

        // POST: MSocios/MassDelete
        [HttpPost]
        public JsonResult MassDelete(List<string> sIdSocios)
        {
            var response = new { success = false, message = "Error al eliminar los socios" }; // Respuesta inicial
            var errores = new List<string>(); // Para capturar errores individuales
            var eliminados = 0; // Contador de registros eliminados

            try
            {
                // Verificar si se proporcionaron socios para eliminar
                if (sIdSocios == null || !sIdSocios.Any())
                {
                    return Json(new { success = false, message = "No se proporcionaron socios para eliminar" });
                }

                foreach (var sIdSocio in sIdSocios)
                {
                    // Buscar cada socio por su ID
                    MSocios mSocios = db.MSocios.Find(sIdSocio);

                    if (mSocios != null)
                    {
                        // Eliminar el socio encontrado
                        db.MSocios.Remove(mSocios);
                        eliminados++;
                    }
                    else
                    {
                        // Si no se encuentra un socio, agregar un mensaje de error
                        errores.Add($"Socio con ID {sIdSocio} no encontrado.");
                    }
                }

                // Guardar los cambios en la base de datos solo si hubo eliminaciones
                if (eliminados > 0)
                {
                    db.SaveChanges();
                }

                // Construir la respuesta basada en el resultado
                if (errores.Any())
                {
                    response = new { success = false, message = "Algunos socios no se encontraron o no se pudieron eliminar: " + string.Join(", ", errores) };
                }
                else
                {
                    response = new { success = true, message = $"{eliminados} socio(s) eliminado(s) con éxito" };
                }
            }
            catch (Exception ex)
            {
                response = new { success = false, message = "Error al eliminar los socios: " + ex.Message };
            }

            return Json(response);
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
                var expectedHeaders = new List<string> { "sIdSocio", "sCodSAP", "sNIF", "sNombreFcia", "sDireccion", "sCP", "sPoblacion" };
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
                var socios = new List<MSocios>();
                var erroresValidacion = new List<string>();

                for (int row = 2; row <= rowCount; row++)
                {
                    var socio = new MSocios();

                    // Leer y validar cada celda
                    for (int col = 1; col <= columnCount; col++)
                    {
                        var cellValue = worksheet.Cells[row, col].Value;
                        var header = expectedHeaders[col - 1];

                        switch (header)
                        {
                            case "sIdSocio":
                                socio.sIdSocio = cellValue?.ToString();
                                if (string.IsNullOrWhiteSpace(socio.sIdSocio))
                                    erroresValidacion.Add(string.Format(ManejadorErroresCargaMasiva.ObtenerMensajeError(ErroresCargaMasiva.CampoObligatorio), row, header));
                                else if (socio.sIdSocio.Length > 50)
                                    erroresValidacion.Add(string.Format(ManejadorErroresCargaMasiva.ObtenerMensajeError(ErroresCargaMasiva.LongitudMaximaExcedida), row, header, 50));
                                break;

                            case "sCodSAP":
                                socio.sCodSAP = cellValue?.ToString();
                                if (!string.IsNullOrWhiteSpace(socio.sCodSAP) && socio.sCodSAP.Length > 10)
                                    erroresValidacion.Add(string.Format(ManejadorErroresCargaMasiva.ObtenerMensajeError(ErroresCargaMasiva.LongitudMaximaExcedida), row, header, 10));
                                break;

                            case "sNIF":
                                socio.sNIF = cellValue?.ToString();
                                if (!string.IsNullOrWhiteSpace(socio.sNIF) && socio.sNIF.Length > 11)
                                    erroresValidacion.Add(string.Format(ManejadorErroresCargaMasiva.ObtenerMensajeError(ErroresCargaMasiva.LongitudMaximaExcedida), row, header, 11));
                                break;

                            case "sNombreFcia":
                                socio.sNombreFcia = cellValue?.ToString();
                                if (!string.IsNullOrWhiteSpace(socio.sNombreFcia) && socio.sNombreFcia.Length > 250)
                                    erroresValidacion.Add(string.Format(ManejadorErroresCargaMasiva.ObtenerMensajeError(ErroresCargaMasiva.LongitudMaximaExcedida), row, header, 250));
                                break;

                            case "sDireccion":
                                socio.sDireccion = cellValue?.ToString();
                                if (!string.IsNullOrWhiteSpace(socio.sDireccion) && socio.sDireccion.Length > 250)
                                    erroresValidacion.Add(string.Format(ManejadorErroresCargaMasiva.ObtenerMensajeError(ErroresCargaMasiva.LongitudMaximaExcedida), row, header, 250));
                                break;

                            case "sCP":
                                socio.sCP = cellValue?.ToString();
                                if (!string.IsNullOrWhiteSpace(socio.sCP) && socio.sCP.Length > 5)
                                    erroresValidacion.Add(string.Format(ManejadorErroresCargaMasiva.ObtenerMensajeError(ErroresCargaMasiva.LongitudMaximaExcedida), row, header, 5));
                                break;

                            case "sPoblacion":
                                socio.sPoblacion = cellValue?.ToString();
                                if (!string.IsNullOrWhiteSpace(socio.sPoblacion) && socio.sPoblacion.Length > 100)
                                    erroresValidacion.Add(string.Format(ManejadorErroresCargaMasiva.ObtenerMensajeError(ErroresCargaMasiva.LongitudMaximaExcedida), row, header, 100));
                                break;

                            default:
                                break;
                        }
                    }

                    // Si el socio ya existe, la fila es una actualización: los campos editables no pueden venir vacíos
                    bool socioExistente = db.MSocios.Any(s => s.sIdSocio == socio.sIdSocio);
                    if (socioExistente)
                    {
                        var camposEditables = new Dictionary<string, string>
                        {
                            { "sCodSAP", socio.sCodSAP },
                            { "sNIF", socio.sNIF },
                            { "sNombreFcia", socio.sNombreFcia },
                            { "sDireccion", socio.sDireccion },
                            { "sCP", socio.sCP },
                            { "sPoblacion", socio.sPoblacion }
                        };

                        foreach (var campo in camposEditables)
                        {
                            if (string.IsNullOrWhiteSpace(campo.Value))
                            {
                                erroresValidacion.Add(string.Format(ManejadorErroresCargaMasiva.ObtenerMensajeError(ErroresCargaMasiva.CampoVacioEnActualizacion), row, campo.Key));
                            }
                        }
                    }

                    socios.Add(socio);
                }

                if (erroresValidacion.Any())
                {
                    ViewBag.Mensaje = "Errores de validación en el archivo Excel:<br/>" + string.Join("<br/>", erroresValidacion);
                    ViewBag.Exito = false;
                    return View();
                }

                if (socios.Count() <= 0)
                {
                    ViewBag.Mensaje = ManejadorErroresCargaMasiva.ObtenerMensajeError(ErroresCargaMasiva.NoHayRegistrosEnExcel);
                    ViewBag.Exito = false;
                    return View();
                }
                else
                {
                    bool exito = CargarSociosMasivamente(socios, out string mensajeError);

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
                var worksheet = package.Workbook.Worksheets.Add("Socios");
                worksheet.Cells[1, 1].Value = "sIdSocio";
                worksheet.Cells[1, 2].Value = "sCodSAP";
                worksheet.Cells[1, 3].Value = "sNIF";
                worksheet.Cells[1, 4].Value = "sNombreFcia";
                worksheet.Cells[1, 5].Value = "sDireccion";
                worksheet.Cells[1, 6].Value = "sCP";
                worksheet.Cells[1, 7].Value = "sPoblacion";

                var stream = new MemoryStream(package.GetAsByteArray());
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "Plantilla_Socios.xlsx");
            }
        }

        // Método interno para carga masiva de socios
        private bool CargarSociosMasivamente(List<MSocios> socios, out string mensajeError)
        {
            mensajeError = "";

            try
            {
                foreach (var socio in socios)
                {
                    if (!ModelState.IsValid)
                    {
                        var errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                        mensajeError = "Errores de validación:\n" + string.Join("\n", errores);
                        return false;
                    }

                    var socioExistente = db.MSocios.FirstOrDefault(s => s.sIdSocio == socio.sIdSocio);
                    if (socioExistente != null)
                    {
                        // Actualización: el socio ya existe (los campos vacíos ya se rechazaron en la validación previa)
                        socioExistente.sCodSAP = socio.sCodSAP;
                        socioExistente.sNIF = socio.sNIF;
                        socioExistente.sNombreFcia = socio.sNombreFcia;
                        socioExistente.sDireccion = socio.sDireccion;
                        socioExistente.sCP = socio.sCP;
                        socioExistente.sPoblacion = socio.sPoblacion;
                        socioExistente.dFechaActualizacion = DateTime.Now;
                        socioExistente.sUsuarioActualizacion = "SYSTEM";
                    }
                    else
                    {
                        // Alta: socio nuevo
                        socio.dFechaActualizacion = DateTime.Now;
                        socio.sUsuarioActualizacion = "SYSTEM";
                        db.MSocios.Add(socio);
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
