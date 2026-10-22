using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Mantenimiento_Liquidaciones;
using Mantenimiento_Liquidaciones.Controllers;
using Mantenimiento_Liquidaciones.Tests.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OfficeOpenXml;

namespace Mantenimiento_Liquidaciones.Tests.Controllers
{
    // Tests de los ajustes del ticket MV 28052026 (Maestra de Socios):
    //  1) ReadMSocios ya no limita el grid a 100 registros.
    //  2) La carga masiva actualiza socios existentes (upsert) sin depender del
    //     procedimiento almacenado usp_MSocios_Insert, que no existe en la base de datos
    //     (antes fallaba con "No se encontró el procedimiento almacenado...").
    //  3) La carga masiva rechaza y avisa una fila de actualización con campos vacíos,
    //     sin modificar el registro existente.
    //
    // Requieren una base de datos local CLUBS_AX_TEST accesible con
    // "data source=localhost;integrated security=True" (ver App.config de este proyecto).
    // No cubren regresión general del controlador, solo estos 3 ajustes.
    [TestClass]
    public class MSociosControllerTest
    {
        private const string PrefijoSociosTest = "TEST_MV28052026_";

        [TestCleanup]
        public void LimpiarSociosDePrueba()
        {
            using (var db = new CLUBS_AX_TESTEntities())
            {
                var sociosDePrueba = db.MSocios.Where(s => s.sIdSocio.StartsWith(PrefijoSociosTest)).ToList();
                if (sociosDePrueba.Any())
                {
                    db.MSocios.RemoveRange(sociosDePrueba);
                    db.SaveChanges();
                }
            }
        }

        private static string NuevoIdSocioDePrueba()
        {
            return PrefijoSociosTest + Guid.NewGuid().ToString("N").Substring(0, 8);
        }

        private static HttpPostedFileBase CrearExcelSocios(params object[] fila)
        {
            using (var package = new ExcelPackage())
            {
                var ws = package.Workbook.Worksheets.Add("Socios");
                string[] headers = { "sIdSocio", "sCodSAP", "sNIF", "sNombreFcia", "sDireccion", "sCP", "sPoblacion" };
                for (int c = 0; c < headers.Length; c++)
                {
                    ws.Cells[1, c + 1].Value = headers[c];
                }

                for (int c = 0; c < fila.Length; c++)
                {
                    ws.Cells[2, c + 1].Value = fila[c];
                }

                var stream = new MemoryStream(package.GetAsByteArray());
                return new FakeHttpPostedFile(stream, "socios.xlsx");
            }
        }

        [TestMethod]
        public void ReadMSocios_DevuelveMasDe100Registros_CuandoHayMasDe100EnBaseDeDatos()
        {
            long totalEnBd;
            using (var db = new CLUBS_AX_TESTEntities())
            {
                totalEnBd = db.MSocios.LongCount();
            }
            Assert.IsTrue(totalEnBd > 100,
                "Este test requiere una BD local con más de 100 socios para verificar el fix del límite fijo.");

            using (var controller = new MSociosController())
            {
                var request = new DataSourceRequest { Page = 1, PageSize = 500 };
                var jsonResult = controller.ReadMSocios(request, null, null, null, null, null, null, null, null) as JsonResult;

                Assert.IsNotNull(jsonResult);
                var data = (DataSourceResult)jsonResult.Data;
                Assert.IsTrue(data.Total > 100,
                    $"Se esperaba Total > 100, se obtuvo {data.Total}. ¿Sigue el Take(100) fijo en ReadMSocios?");
            }
        }

        [TestMethod]
        public void CargarArchivo_InsertaSocioNuevo_SinUsarProcedimientoAlmacenadoInexistente()
        {
            string idSocio = NuevoIdSocioDePrueba();
            var archivo = CrearExcelSocios(idSocio, "SAP001", "12345678A", "Farmacia Nueva", "Calle Test 1", "28001", "Madrid");

            using (var controller = new MSociosController())
            {
                var result = controller.CargarArchivo(archivo) as ViewResult;

                Assert.IsNotNull(result);
                Assert.AreEqual(true, result.ViewBag.Exito, "Mensaje: " + result.ViewBag.Mensaje);
            }

            using (var db = new CLUBS_AX_TESTEntities())
            {
                var socio = db.MSocios.FirstOrDefault(s => s.sIdSocio == idSocio);
                Assert.IsNotNull(socio, "El socio nuevo debería haberse insertado.");
                Assert.AreEqual("Madrid", socio.sPoblacion);
            }
        }

        [TestMethod]
        public void CargarArchivo_ActualizaSocioExistente_CuandoTodosLosCamposVienenCompletos()
        {
            string idSocio = NuevoIdSocioDePrueba();
            using (var db = new CLUBS_AX_TESTEntities())
            {
                db.MSocios.Add(new MSocios
                {
                    sIdSocio = idSocio,
                    sCodSAP = "SAP000",
                    sNIF = "00000000A",
                    sNombreFcia = "Nombre Original",
                    sDireccion = "Direccion Original",
                    sCP = "00000",
                    sPoblacion = "Poblacion Original"
                });
                db.SaveChanges();
            }

            var archivo = CrearExcelSocios(idSocio, "SAP999", "99999999Z", "Nombre Corregido", "Direccion Corregida", "28080", "Madrid");

            using (var controller = new MSociosController())
            {
                var result = controller.CargarArchivo(archivo) as ViewResult;

                Assert.IsNotNull(result);
                Assert.AreEqual(true, result.ViewBag.Exito, "Mensaje: " + result.ViewBag.Mensaje);
            }

            using (var db = new CLUBS_AX_TESTEntities())
            {
                var socio = db.MSocios.FirstOrDefault(s => s.sIdSocio == idSocio);
                Assert.IsNotNull(socio);
                Assert.AreEqual("Nombre Corregido", socio.sNombreFcia);
                Assert.AreEqual("28080", socio.sCP);
                Assert.AreEqual("Madrid", socio.sPoblacion);
            }
        }

        [TestMethod]
        public void CargarArchivo_MarcaErrorYNoModifica_CuandoCampoVieneVacioAlActualizarSocioExistente()
        {
            string idSocio = NuevoIdSocioDePrueba();
            using (var db = new CLUBS_AX_TESTEntities())
            {
                db.MSocios.Add(new MSocios
                {
                    sIdSocio = idSocio,
                    sCodSAP = "SAP000",
                    sNIF = "00000000A",
                    sNombreFcia = "Nombre Original",
                    sDireccion = "Direccion Original",
                    sCP = "00000",
                    sPoblacion = "Poblacion Original"
                });
                db.SaveChanges();
            }

            // sCP viene vacío: debe marcarse como error y NO debe modificar el registro existente.
            var archivo = CrearExcelSocios(idSocio, "SAP999", "99999999Z", "Nombre Corregido", "Direccion Corregida", "", "Madrid");

            using (var controller = new MSociosController())
            {
                var result = controller.CargarArchivo(archivo) as ViewResult;

                Assert.IsNotNull(result);
                Assert.AreEqual(false, result.ViewBag.Exito);
                StringAssert.Contains((string)result.ViewBag.Mensaje, "sCP");
            }

            using (var db = new CLUBS_AX_TESTEntities())
            {
                var socio = db.MSocios.FirstOrDefault(s => s.sIdSocio == idSocio);
                Assert.IsNotNull(socio);
                Assert.AreEqual("Nombre Original", socio.sNombreFcia,
                    "El registro no debía modificarse: el archivo tenía una fila con error de validación.");
            }
        }
    }
}
