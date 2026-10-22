using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Mantenimiento_Liquidaciones;
using Mantenimiento_Liquidaciones.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OfficeOpenXml;

namespace Mantenimiento_Liquidaciones.Tests.Controllers
{
    // Tests de los ajustes de MMateriales (tickets MV 19052026 / MV 18052026):
    //  1) "Finalizar vigencia" (individual y masivo) ya acepta una fecha concreta en vez de
    //     usar siempre DateTime.Now.
    //  2) El [Bind(Include=...)] de Edit incluye sDescripcionMaterial y DescuentoAdelantadoClub
    //     (antes se perdían en cualquier edición). El binding real de MVC solo se ejerce a
    //     través del pipeline HTTP, así que aquí se verifica por reflexión que el atributo
    //     los declara — se verificó además manualmente en vivo contra SQL Server local.
    //  3) La plantilla de carga masiva incluye la columna "Descuento adelantado".
    //  4) CargarDesdeExcel: cambia el tipo de material (iIdTipo) y guarda el descuento
    //     adelantado en un registro existente, y devuelve un JSON de error limpio en vez de
    //     una excepción sin controlar cuando el archivo no es válido.
    //
    // Requieren una base de datos local CLUBS_AX_TEST accesible con
    // "data source=localhost;integrated security=True" (ver App.config de este proyecto).
    // No cubren regresión general del controlador, solo estos ajustes.
    [TestClass]
    public class MMaterialesControllerTest
    {
        private const string IdClubPrueba = "ZTST";

        [TestCleanup]
        public void LimpiarMaterialesDePrueba()
        {
            using (var db = new CLUBS_AX_TESTEntities())
            {
                var materialesDePrueba = db.MMateriales1.Where(m => m.sIdClub == IdClubPrueba).ToList();
                if (materialesDePrueba.Any())
                {
                    db.MMateriales1.RemoveRange(materialesDePrueba);
                    db.SaveChanges();
                }
            }
        }

        [TestMethod]
        public void Edit_BindIncludeContieneDescripcionMaterialYDescuentoAdelantado()
        {
            var metodo = typeof(MMaterialesController).GetMethod(
                "Edit",
                BindingFlags.Public | BindingFlags.Instance,
                null,
                new[] { typeof(MMateriales) },
                null);

            Assert.IsNotNull(metodo, "No se encontró la acción Edit(MMateriales).");

            var bindAttribute = metodo.GetParameters()[0]
                .GetCustomAttributes(typeof(BindAttribute), false)
                .Cast<BindAttribute>()
                .FirstOrDefault();

            Assert.IsNotNull(bindAttribute, "El parámetro de Edit debería tener [Bind(Include=...)].");
            StringAssert.Contains(bindAttribute.Include, "sDescripcionMaterial");
            StringAssert.Contains(bindAttribute.Include, "DescuentoAdelantadoClub");
        }

        [TestMethod]
        public void DeleteConfirmed_UsaLaFechaIndicada_EnVezDeHoy()
        {
            using (var db = new CLUBS_AX_TESTEntities())
            {
                db.MMateriales1.Add(new MMateriales
                {
                    sIdClub = IdClubPrueba,
                    sCodSAPMaterial = "SAP001",
                    dFechaIni = new DateTime(2025, 1, 1),
                    sCodNacional = "111111",
                    fPrecio = 10m,
                    iIdTipo = 1
                });
                db.SaveChanges();
            }

            var fechaFinEsperada = new DateTime(2025, 6, 15);
            using (var controller = new MMaterialesController())
            {
                var resultado = controller.DeleteConfirmed("ZTST", "01/01/2025 00:00:00", "111111", "2025-06-15") as JsonResult;
                Assert.IsNotNull(resultado);
            }

            using (var db = new CLUBS_AX_TESTEntities())
            {
                var material = db.MMateriales1.FirstOrDefault(m => m.sIdClub == IdClubPrueba && m.sCodNacional == "111111");
                Assert.IsNotNull(material);
                Assert.AreEqual(fechaFinEsperada, material.dFechaFin);
            }
        }

        [TestMethod]
        public void MassDelete_UsaLaFechaIndicada_EnVezDeHoy()
        {
            using (var db = new CLUBS_AX_TESTEntities())
            {
                db.MMateriales1.Add(new MMateriales
                {
                    sIdClub = IdClubPrueba,
                    sCodSAPMaterial = "SAP002",
                    dFechaIni = new DateTime(2025, 1, 1),
                    sCodNacional = "222222",
                    fPrecio = 10m,
                    iIdTipo = 1
                });
                db.SaveChanges();
            }

            var fechaFinEsperada = new DateTime(2025, 7, 20);
            var seleccionados = new System.Collections.Generic.List<MaterialDto>
            {
                new MaterialDto { sIdClub = IdClubPrueba, dFechaIni = "01/01/2025 00:00:00", sCodNacional = "222222" }
            };

            using (var controller = new MMaterialesController())
            {
                var resultado = controller.MassDelete(seleccionados, "2025-07-20") as JsonResult;
                Assert.IsNotNull(resultado);
            }

            using (var db = new CLUBS_AX_TESTEntities())
            {
                var material = db.MMateriales1.FirstOrDefault(m => m.sIdClub == IdClubPrueba && m.sCodNacional == "222222");
                Assert.IsNotNull(material);
                Assert.AreEqual(fechaFinEsperada, material.dFechaFin);
            }
        }

        [TestMethod]
        public void DescargarPlantilla_IncluyeColumnaDescuentoAdelantado()
        {
            using (var controller = new MMaterialesController())
            {
                var resultado = controller.DescargarPlantilla() as FileStreamResult;
                Assert.IsNotNull(resultado);

                using (var package = new ExcelPackage(resultado.FileStream))
                {
                    var worksheet = package.Workbook.Worksheets.First();
                    Assert.AreEqual(8, worksheet.Dimension.Columns, "La plantilla debería tener 8 columnas.");
                    Assert.AreEqual("Descuento adelantado", worksheet.Cells[1, 8].Text);
                }
            }
        }

        private static HttpPostedFileBase CrearExcelMateriales(string sIdClub, string sCodSAPMaterial, string fechaIni,
            string fechaFin, string sCodNacional, double precio, int idTipo, double? descuento)
        {
            using (var package = new ExcelPackage())
            {
                var ws = package.Workbook.Worksheets.Add("Materiales");
                string[] headers = { "Id Club", "Codigo SAP Material", "Fecha Inicial", "Fecha Fin", "Codigo Nacional", "Precio", "Id Clasificacion", "Descuento adelantado" };
                for (int c = 1; c <= headers.Length; c++)
                {
                    ws.Cells[1, c].Value = headers[c - 1];
                }

                ws.Cells[2, 1].Value = sIdClub;
                ws.Cells[2, 2].Value = sCodSAPMaterial;
                ws.Cells[2, 3].Value = fechaIni;
                ws.Cells[2, 4].Value = fechaFin ?? "";
                ws.Cells[2, 5].Value = sCodNacional;
                ws.Cells[2, 6].Value = precio;
                ws.Cells[2, 7].Value = idTipo;
                if (descuento.HasValue)
                {
                    ws.Cells[2, 8].Value = descuento.Value;
                }

                var stream = new MemoryStream(package.GetAsByteArray());
                return new Mantenimiento_Liquidaciones.Tests.TestHelpers.FakeHttpPostedFile(stream, "materiales.xlsx");
            }
        }

        [TestMethod]
        public void CargarDesdeExcel_ActualizaTipoMaterialYDescuentoAdelantado_ParaRegistroExistente()
        {
            using (var db = new CLUBS_AX_TESTEntities())
            {
                db.MMateriales1.Add(new MMateriales
                {
                    sIdClub = IdClubPrueba,
                    sCodSAPMaterial = "SAP003",
                    dFechaIni = new DateTime(2025, 1, 1),
                    sCodNacional = "333333",
                    fPrecio = 3.11m,
                    iIdTipo = 1
                });
                db.SaveChanges();
            }

            var archivo = CrearExcelMateriales(IdClubPrueba, "SAP003", "01/01/2025", null, "333333", 3.11, 99, 4.25);

            using (var controller = new MMaterialesController())
            {
                var resultado = controller.CargarDesdeExcel(archivo) as JsonResult;
                Assert.IsNotNull(resultado);
            }

            using (var db = new CLUBS_AX_TESTEntities())
            {
                var material = db.MMateriales1.FirstOrDefault(m => m.sIdClub == IdClubPrueba && m.sCodNacional == "333333");
                Assert.IsNotNull(material);
                Assert.AreEqual(99, material.iIdTipo, "El cambio de Tipo Material no se guardó.");
                Assert.AreEqual(4.25m, material.DescuentoAdelantadoClub, "El descuento adelantado no se guardó.");
            }
        }

        [TestMethod]
        public void CargarDesdeExcel_DevuelveJsonDeError_CuandoElArchivoNoTieneDatos()
        {
            HttpPostedFileBase archivoVacio;
            using (var package = new ExcelPackage())
            {
                package.Workbook.Worksheets.Add("Materiales");
                var stream = new MemoryStream(package.GetAsByteArray());
                archivoVacio = new Mantenimiento_Liquidaciones.Tests.TestHelpers.FakeHttpPostedFile(stream, "vacio.xlsx");
            }

            using (var controller = new MMaterialesController())
            {
                // No debe lanzar una excepción sin controlar: debe devolver un JsonResult con el error.
                var resultado = controller.CargarDesdeExcel(archivoVacio) as JsonResult;
                Assert.IsNotNull(resultado, "CargarDesdeExcel debería devolver un JSON de error, no lanzar una excepción sin controlar.");
            }
        }
    }
}
