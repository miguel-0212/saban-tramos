using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Windows.Media.Media3D;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Mantenimiento_Liquidaciones;
using Mantenimiento_Liquidaciones.Helpers;
using OfficeOpenXml;
using static Mantenimiento_Liquidaciones.Helpers.ManejadorErroresCargaMasiva;

namespace Mantenimiento_Liquidaciones.Controllers
{

    public class MaterialDto
    {
        public string sIdClub { get; set; }
        public string dFechaIni { get; set; } // Añadir este campo para la fecha inicial
        public string sCodNacional { get; set; }
    }
    public class MMaterialesController : Controller
    {
        private CLUBS_AX_TESTEntities db = new CLUBS_AX_TESTEntities();

        // GET: MMateriales
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ReadMMateriales([DataSourceRequest] DataSourceRequest request, string sIdClub, string sCodNacional, string sCodSAPMaterial, string dFechaIni, string dFechaFin)
        {
            // Obtener la lista de materiales
            var lstMateriales = db.MMateriales1.AsQueryable();

            // Aplicar filtros según los parámetros
            if (!string.IsNullOrEmpty(sIdClub))
            {
                lstMateriales = lstMateriales.Where(m => m.sIdClub.Contains(sIdClub));
            }

            if (!string.IsNullOrEmpty(sCodNacional))
            {
                lstMateriales = lstMateriales.Where(m => m.sCodNacional.Contains(sCodNacional));
            }

            if (!string.IsNullOrEmpty(sCodSAPMaterial))
            {
                lstMateriales = lstMateriales.Where(m => m.sCodSAPMaterial.Contains(sCodSAPMaterial));
            }

            if (!string.IsNullOrEmpty(dFechaIni))
            {
                // Fecha Inicio: trae todo lo mayor o igual a esa fecha (sin límite superior)
                if (DateTime.TryParse(dFechaIni, out DateTime fechaIni))
                {
                    lstMateriales = lstMateriales.Where(m => m.dFechaIni >= fechaIni);
                }
            }

            if (!string.IsNullOrEmpty(dFechaFin))
            {
                // Fecha Fin: trae todo lo menor o igual a esa fecha (sin límite inferior). Se usa
                // "< fechaFin + 1 día" para incluir toda la jornada del día seleccionado, no solo
                // registros con hora exacta 00:00:00.
                if (DateTime.TryParse(dFechaFin, out DateTime fechaFin))
                {
                    var fechaFinSiguiente = fechaFin.AddDays(1);
                    lstMateriales = lstMateriales.Where(m => m.dFechaFin.HasValue && m.dFechaFin < fechaFinSiguiente);
                }
            }

            // Ordenar (sin limitar a un número fijo de registros: ToDataSourceResult pagina sobre la consulta completa)
            lstMateriales = lstMateriales.OrderByDescending(m => m.dFechaActualizacion);

            // Aplicar paginación, ordenación y filtrado utilizando el DataSourceRequest
            DataSourceResult result = lstMateriales.ToDataSourceResult(request);

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



        

        // POST: MMateriales/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create([Bind(Include = "sIdClub,sCodSAPMaterial,dFechaIni,dFechaFin,sCodNacional,fPrecio,iIdTipo,dFechaActualizacion,sUsuarioActualizacion,sDescripcionMaterial,DescuentoAdelantadoClub ")] MMateriales mMateriales)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    mMateriales.dFechaActualizacion = DateTime.Now;
                    mMateriales.sUsuarioActualizacion = "SYSTEM";
                    db.MMateriales1.Add(mMateriales);
                    db.SaveChanges();
                    return JavaScript("window.location.reload();");
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var validationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            Console.WriteLine("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                        }
                    }
                }
            }

            return View(mMateriales);
        }

      
        // POST: MMateriales/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit([Bind(Include = "sIdClub,sCodSAPMaterial,dFechaIni,dFechaFin,sCodNacional,fPrecio,iIdTipo,dFechaActualizacion,sUsuarioActualizacion,sDescripcionMaterial,DescuentoAdelantadoClub")] MMateriales mMateriales)
        {
            if (ModelState.IsValid)
            {
                mMateriales.dFechaActualizacion = DateTime.Now;
                mMateriales.sUsuarioActualizacion = "SYSTEM";
                db.Entry(mMateriales).State = EntityState.Modified;
                db.SaveChanges();
                return JavaScript("window.location.reload();");
            }
            return View(mMateriales);
        }

        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(string sIdClub, string dFechaIni, string sCodNacional, string dFechaFinVigencia)
        {

            var response = new { success = false, message = "Error al eliminar el registro" }; // Respuesta inicial de error

            DateTime? fechaIni = TryParseDate(dFechaIni);

            MMateriales mMateriales = db.MMateriales1.Find(sIdClub, fechaIni, sCodNacional);

            if (mMateriales != null)
            {
                // Si el usuario eligió una fecha de fin de vigencia concreta, se respeta; si no, se usa hoy.
                mMateriales.dFechaFin = TryParseDate(dFechaFinVigencia) ?? DateTime.Now;
                mMateriales.dFechaActualizacion = DateTime.Now;
                mMateriales.sUsuarioActualizacion = "SYSTEM";

                db.Entry(mMateriales).State = EntityState.Modified;
                db.SaveChanges();
                response = new { success = true, message = "Registro actualizado correctamente" };

            }
            else
            {
                response = new { success = false, message = "Registro no encontrado" };
            }

            return Json(response);
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
                var expectedHeaders = new List<string> { "sIdClub", "sCodSAPMaterial", "dFechaIni", "dFechaFin", "sCodNacional", "fPrecio", "iIdTipo" };
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
                var materiales = new List<MMateriales>();
                var erroresValidacion = new List<string>();

                for (int row = 2; row <= rowCount; row++)
                {
                    var material = new MMateriales();

                    // Leer y validar cada celda
                    for (int col = 1; col <= columnCount; col++)
                    {
                        var cellValue = worksheet.Cells[row, col].Value;
                        var header = expectedHeaders[col - 1];

                        switch (header)
                        {
                            case "sIdClub":
                                material.sIdClub = cellValue?.ToString();
                                if (string.IsNullOrWhiteSpace(material.sIdClub))
                                    erroresValidacion.Add(string.Format(ObtenerMensajeError(ErroresCargaMasiva.CampoObligatorio, row,header), row, header));
                                else if (material.sIdClub.Length > 4)
                                    erroresValidacion.Add(string.Format(ObtenerMensajeError(ErroresCargaMasiva.LongitudMaximaExcedida, row), row, header, 4));
                                break;

                            case "sCodSAPMaterial":
                                material.sCodSAPMaterial = cellValue?.ToString();
                                if (string.IsNullOrWhiteSpace(material.sCodSAPMaterial))
                                    erroresValidacion.Add(string.Format(ObtenerMensajeError(ErroresCargaMasiva.CampoObligatorio), row, header));
                                else if (material.sCodSAPMaterial.Length > 18)
                                    erroresValidacion.Add(string.Format(ObtenerMensajeError(ErroresCargaMasiva.LongitudMaximaExcedida), row, header, 18));
                                break;

                            case "dFechaIni":
                                if (double.TryParse(cellValue?.ToString(), out double oaDate))
                                {
                                    try
                                    {
                                        DateTime fechaIni = DateTime.FromOADate(oaDate);
                                        material.dFechaIni = fechaIni;
                                    }
                                    catch (ArgumentException)
                                    {
                                        erroresValidacion.Add(string.Format(ObtenerMensajeError(ErroresCargaMasiva.FormatoFechaInvalido), header));
                                    }
                                }
                                else
                                    erroresValidacion.Add(string.Format(ObtenerMensajeError(ErroresCargaMasiva.FormatoFechaInvalido),row, header));
                                break;

                            case "dFechaFin":
                                if (cellValue != null && !string.IsNullOrWhiteSpace(cellValue.ToString()))
                                {
                                    if (double.TryParse(cellValue?.ToString(), out double oaDate1))
                                    {
                                        try
                                        {
                                            DateTime dFechaFin = DateTime.FromOADate(oaDate1);
                                            material.dFechaFin = dFechaFin;
                                        }
                                        catch (ArgumentException)
                                        {
                                            erroresValidacion.Add(string.Format(ObtenerMensajeError(ErroresCargaMasiva.FormatoFechaInvalido), header));
                                        }
                                    }
                                    else
                                        erroresValidacion.Add(string.Format(ObtenerMensajeError(ErroresCargaMasiva.FormatoFechaInvalido), header));

                                    if (material.dFechaFin <= material.dFechaIni)
                                        erroresValidacion.Add(string.Format(ObtenerMensajeError(ErroresCargaMasiva.FechaFinMenorIgualFechaInicio), row));
                                }
                                break;

                            case "sCodNacional":
                                material.sCodNacional = cellValue?.ToString();
                                if (string.IsNullOrWhiteSpace(material.sCodNacional))
                                    erroresValidacion.Add(string.Format(ObtenerMensajeError(ErroresCargaMasiva.CampoObligatorio), row, header));
                                else if (material.sCodNacional.Length > 6)
                                    erroresValidacion.Add(string.Format(ObtenerMensajeError(ErroresCargaMasiva.LongitudMaximaExcedida), row, header, 6));
                                break;

                            case "fPrecio":
                                if (decimal.TryParse(cellValue?.ToString(), out decimal precio))
                                    material.fPrecio = precio;
                                else
                                    erroresValidacion.Add(string.Format(ObtenerMensajeError(ErroresCargaMasiva.FormatoNumeroInvalido), header));

                                if (material.fPrecio <= 0)
                                    erroresValidacion.Add(string.Format(ObtenerMensajeError(ErroresCargaMasiva.PrecioInvalido), row));
                                break;

                            case "iIdTipo":
                                if (int.TryParse(cellValue?.ToString(), out int idTipo))
                                    material.iIdTipo = idTipo;
                                else
                                    erroresValidacion.Add(string.Format(ObtenerMensajeError(ErroresCargaMasiva.FormatoNumeroInvalido), header));

                                if (material.iIdTipo <= 0)
                                    erroresValidacion.Add(string.Format(ObtenerMensajeError(ErroresCargaMasiva.IdTipoInvalido), row));
                                break;

                            default:
                                break;
                        }
                    }

                    // Validar si ya existe un registro igual en la base de datos
                    //if (db.MMateriales1.Any(m => m.sIdClub == material.sIdClub && m.sCodNacional == material.sCodNacional && m.dFechaIni == material.dFechaIni))
                    //{
                    //    erroresValidacion.Add(string.Format(ObtenerMensajeError(ErroresCargaMasiva.RegistroDuplicado), row));
                    //}

                    material.dFechaActualizacion = DateTime.Now;
                    material.sUsuarioActualizacion = "SYSTEM"; // O el nombre de usuario real si lo tienes disponible
                    materiales.Add(material);
                }

                if (erroresValidacion.Any())
                {
                    ViewBag.Mensaje = "Errores de validación en el archivo Excel:";
                    ViewBag.Errores = erroresValidacion;
                    ViewBag.Exito = false;
                    return View();
                }
                if (materiales.Count() <= 0)
                {
                    ViewBag.Mensaje = ObtenerMensajeError(ErroresCargaMasiva.NoHayRegistrosEnExcel);
                    ViewBag.Exito = false;
                    return View();
                }
                else
                {
                    bool exito = CargarMaterialesMasivamente(materiales, out string mensajeError);

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
            // Generar un Excel vacío
            using (ExcelPackage package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Materiales");
                worksheet.Cells[1, 1].Value = "Id Club"; // Asegúrate de que las cabeceras coincidan
                worksheet.Cells[1, 2].Value = "Código SAP Material";
                worksheet.Cells[1, 3].Value = "Fecha Inicial";
                worksheet.Cells[1, 4].Value = "Fecha Fin";
                worksheet.Cells[1, 5].Value = "Código Nacional";
                worksheet.Cells[1, 6].Value = "Precio";
                worksheet.Cells[1, 7].Value = "Id Clasificación";
                worksheet.Cells[1, 8].Value = "Descuento adelantado";

                // Guardar el archivo
                var fileStream = new MemoryStream();
                package.SaveAs(fileStream);
                fileStream.Position = 0;

                return File(fileStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Plantilla_MMateriales.xlsx");
            }
        }

        public ActionResult DescargarPlantillaCompletoOLD()
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Materiales");
                worksheet.Cells[1, 1].Value = "sIdClub";
                worksheet.Cells[1, 2].Value = "sCodSAPMaterial";
                worksheet.Cells[1, 3].Value = "dFechaIni";
                worksheet.Cells[1, 4].Value = "dFechaFin";
                worksheet.Cells[1, 5].Value = "sCodNacional";
                worksheet.Cells[1, 6].Value = "fPrecio";
                worksheet.Cells[1, 7].Value = "iIdTipo";

                var stream = new MemoryStream(package.GetAsByteArray());
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Plantilla_Materiales.xlsx");
            }
        }

        [HttpPost]
        public ActionResult CargarDesdeExcel(HttpPostedFileBase file)
        {
            var errors = new List<string>();
            var materiales = new List<MMateriales>();

            try
            {
                if (file != null && file.ContentLength > 0)
                {
                    using (var package = new ExcelPackage(file.InputStream))
                    {
                        var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                        if (worksheet == null || worksheet.Dimension == null)
                        {
                            errors.Add("El archivo Excel no contiene ninguna hoja con datos.");
                            return Json(new { message = "Carga masiva no completada revisar errores.", errors });
                        }
                        int rowCount = worksheet.Dimension.Rows;

                        for (int row = 2; row <= rowCount; row++)
                        {

                            var material = new MMateriales
                            {
                                sIdClub = worksheet.Cells[row, 1].Text,
                                sCodSAPMaterial = worksheet.Cells[row, 2].Text,
                                sCodNacional = worksheet.Cells[row, 5].Text,
                                fPrecio = decimal.TryParse(worksheet.Cells[row, 6].Text, out decimal precio) ? precio : (decimal?)null,
                                iIdTipo = int.TryParse(worksheet.Cells[row, 7].Text, out int idTipo) ? idTipo : (int?)null,
                                DescuentoAdelantadoClub = decimal.TryParse(worksheet.Cells[row, 8].Text, out decimal descuento) ? descuento : (decimal?)null
                            };

                            string fechaIniString = worksheet.Cells[row, 3].Text;
                            DateTime? fechaIni = TryParseDate(fechaIniString);
                            if (fechaIni.HasValue)
                            {
                                material.dFechaIni = fechaIni.Value;
                            }
                            else
                            {
                                errors.Add($"La fecha de inicio en la fila {row} no es válida: {fechaIniString}");
                            }

                            // Intentar parsear dFechaFin
                            string fechaFinString = worksheet.Cells[row, 4].Text; // Asegúrate de que esto sea correcto
                            DateTime? fechaFin = TryParseDate(fechaFinString);
                            if (fechaFin.HasValue || string.IsNullOrWhiteSpace(fechaFinString))
                            {
                                material.dFechaFin = fechaFin;
                            }
                            else
                            {
                                errors.Add($"La fecha de fin en la fila {row} no es válida: {fechaFinString}");
                            }
                            // Validaciones
                            if (string.IsNullOrWhiteSpace(material.sIdClub) || material.sIdClub.Length > 4)
                                errors.Add($"Error en fila {row}: 'Id Club' es inválido.");

                            if (string.IsNullOrWhiteSpace(material.sCodNacional) || material.sCodNacional.Length > 6)
                                errors.Add($"Error en fila {row}: 'Código Nacional' es inválido.");

                            if (string.IsNullOrWhiteSpace(material.sCodSAPMaterial) || material.sCodSAPMaterial.Length > 18)
                                errors.Add($"Error en fila {row}: 'Código SAP Material' es inválido.");

                            materiales.Add(material);
                        }
                    }
                    if (errors.Count == 0)
                    {
                        foreach (var material in materiales)
                        {
                            var existingMaterial = db.MMateriales1
                                .FirstOrDefault(m => m.sIdClub == material.sIdClub && m.sCodNacional == material.sCodNacional && m.dFechaIni == material.dFechaIni);

                            if (existingMaterial != null)
                            {
                                // Actualizar el registro existente
                                existingMaterial.sCodSAPMaterial = material.sCodSAPMaterial;
                                existingMaterial.dFechaFin = material.dFechaFin;
                                existingMaterial.fPrecio = material.fPrecio;
                                existingMaterial.iIdTipo = material.iIdTipo;
                                existingMaterial.DescuentoAdelantadoClub = material.DescuentoAdelantadoClub;
                                existingMaterial.dFechaActualizacion = DateTime.Today;
                                existingMaterial.sUsuarioActualizacion = "SYSTEM";

                            }
                            else
                            {
                                material.dFechaActualizacion = DateTime.Today;
                                material.sUsuarioActualizacion = "SYSTEM";
                                db.MMateriales1.Add(material);
                            }
                        }

                        // Guardar cambios en la base de datos
                        db.SaveChanges();
                        return Json(new { message = "Carga masiva completada exitosamente.", errors });
                    }

                    return Json(new { message = "Carga masiva no completada revisar errores.", errors });
                }
                else
                {
                    errors.Add("No se seleccionó ningún archivo.");
                    return Json(new { message = "Carga masiva no completada revisar errores.", errors });
                }
            }
            catch (Exception ex)
            {
                errors.Add(ex.Message);
                if (ex.InnerException != null)
                {
                    errors.Add(ex.InnerException.Message);
                }
                return Json(new { message = "Error durante la carga masiva: " + ex.Message, errors });
            }
        }

        [HttpGet]
        public ActionResult DescargarPlantillaCompleto()
        {
            // Obtener todos los registros de MMateriales
            var materiales = db.MMateriales1.Take(1000).ToList();

            using (var package = new ExcelPackage())
            {
                // Crear una nueva hoja de trabajo
                var worksheet = package.Workbook.Worksheets.Add("MMateriales");

                // Establecer encabezados
                worksheet.Cells[1, 1].Value = "Id Club";
                worksheet.Cells[1, 2].Value = "Código SAP Material";
                worksheet.Cells[1, 3].Value = "Fecha Inicio";
                worksheet.Cells[1, 4].Value = "Fecha Fin";
                worksheet.Cells[1, 5].Value = "Código Nacional";
                worksheet.Cells[1, 6].Value = "Precio";
                worksheet.Cells[1, 7].Value = "Id Tipo";

                // Llenar los datos
                int row = 2; // Empezar a llenar desde la segunda fila
                foreach (var material in materiales)
                {
                    worksheet.Cells[row, 1].Value = material.sIdClub;
                    worksheet.Cells[row, 2].Value = material.sCodSAPMaterial;
                    worksheet.Cells[row, 3].Value = material.dFechaIni.ToString("MM/dd/yyyy");
                    worksheet.Cells[row, 4].Value = material.dFechaFin.HasValue ? material.dFechaFin.Value.ToString("MM/dd/yyyy") : "";
                    worksheet.Cells[row, 5].Value = material.sCodNacional;
                    worksheet.Cells[row, 6].Value = material.fPrecio;
                    worksheet.Cells[row, 7].Value = material.iIdTipo;
                    row++;
                }

                // Ajustar el ancho de las columnas
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Generar el archivo Excel en un flujo de memoria
                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0; // Restablecer la posición del flujo al principio

                // Retornar el archivo como descarga
                var fileName = "Plantilla_MMateriales.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }


        private DateTime? TryParseDate(string dateString)
        {
            // Array de formatos de fecha que deseas probar
            var formats = new[]
            {
                "MM/dd/yy H:mm:ss",
                "MM/dd/yyyy H:mm:ss",
                "dd/MM/yy H:mm:ss",
                "dd/MM/yyyy H:mm:ss",
                "MM/dd/yy HH:mm:ss",
                "MM/dd/yyyy HH:mm:ss",
                "dd/MM/yy HH:mm:ss",
                "dd/MM/yyyy HH:mm:ss",
                "MM/dd/yy H:mm",
                "MM/dd/yyyy H:mm",
                "dd/MM/yy H:mm",
                "dd/MM/yyyy H:mm",
                "MM/dd/yy HH:mm",
                "MM/dd/yyyy HH:mm",
                "dd/MM/yy HH:mm",
                "dd/MM/yyyy HH:mm",
                "MM/dd/yy",
                "MM/dd/yyyy",
                "dd/MM/yy",
                "dd/MM/yyyy",
                "yyyy-MM-dd HH:mm:ss",
                "yyyy-MM-dd H:mm:ss",
                "yyyy-MM-dd HH:mm",
                "yyyy-MM-dd H:mm",
                "yyyy-MM-dd"
            };

            foreach (var format in formats)
            {
                if (DateTime.TryParseExact(dateString, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateValue))
                {
                    return dateValue; // Retorna la fecha si se convierte correctamente
                }
            }

            // Si ningún formato funcionó, retornar nulo
            return null;
        }
        // Método interno para carga masiva de materiales
        private bool CargarMaterialesMasivamente(List<MMateriales> materiales, out string mensajeError)
        {
            mensajeError = "";

            try
            {
                foreach (var material in materiales)
                {
                    if (ModelState.IsValid)
                    {
                        material.dFechaActualizacion = DateTime.Now;
                        material.sUsuarioActualizacion = "SYSTEM";

                        db.usp_MMateriales_Insert(material.sIdClub, material.sCodSAPMaterial, material.dFechaIni, material.dFechaFin,
                        material.sCodNacional, material.fPrecio, material.iIdTipo, material.dFechaActualizacion, material.sUsuarioActualizacion);
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

        [HttpPost]
        public JsonResult MassDelete(List<MaterialDto> selectedMaterials, string dFechaFinVigencia)
        {
            var response = new { success = false, message = "Error al eliminar los registros" }; // Respuesta inicial de error
            var updatedCount = 0; // Contador para los registros actualizados

            try
            {
                // Si el usuario eligió una fecha de fin de vigencia concreta, se respeta para todos los
                // registros seleccionados; si no, se usa hoy.
                DateTime fechaFin = TryParseDate(dFechaFinVigencia) ?? DateTime.Now;

                foreach (var material in selectedMaterials)
                {
                    DateTime? fechaIni = TryParseDate(material.dFechaIni); // Asegúrate de que dFechaIni esté en el DTO

                    // Busca el material en la base de datos por sus identificadores
                    MMateriales mMateriales = db.MMateriales1.Find(material.sIdClub, fechaIni, material.sCodNacional);
                    if (mMateriales != null)
                    {
                        // Actualiza la fecha de fin y otros campos necesarios
                        mMateriales.dFechaFin = fechaFin;
                        mMateriales.dFechaActualizacion = DateTime.Now;
                        mMateriales.sUsuarioActualizacion = "SYSTEM";

                        // Marca el estado como modificado
                        db.Entry(mMateriales).State = EntityState.Modified;
                        updatedCount++; // Incrementa el contador de registros actualizados
                    }
                }

                // Guarda los cambios en la base de datos solo si hay registros actualizados
                if (updatedCount > 0)
                {
                    db.SaveChanges();
                    response = new { success = true, message = $"{updatedCount} registros actualizados correctamente." };
                }
                else
                {
                    response = new { success = false, message = "No se encontraron registros para actualizar." };
                }
            }
            catch (Exception ex)
            {
                response = new { success = false, message = "Ocurrió un error al eliminar los registros: " + ex.Message };
            }

            return Json(response);
        }

    }
}
