using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mantenimiento_Liquidaciones.Controllers
{
    public class TransversalController : Controller
    {

        private CLUBS_AX_TESTEntities db = new CLUBS_AX_TESTEntities();



        // GET: ReadVW_MAT_LIQ
        public JsonResult ReadVW_MAT_LIQ(string mateMaterialId)
        {
            // Llamar al procedimiento almacenado
            var resultado = db.usp_VW_MAT_LIQ_ext_Read(mateMaterialId).ToList();
            return Json(new { Data = resultado }, JsonRequestBehavior.AllowGet);
        }

        // GET: ReadVW_CLI_LIQ
        public JsonResult ReadVW_CLI_LIQ(string KUNNR)
        {
            var resultado = db.usp_VW_CLI_LIQ_ext_Read(KUNNR).ToList();
            return Json(new { Data = resultado }, JsonRequestBehavior.AllowGet);
        }


    }
}
