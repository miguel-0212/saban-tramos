using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mantenimiento_Liquidaciones.Models
{
    public class IndexModels
    {
        public class Index
        {
            public int iIdUser { get; set; }
            public string sCodRole { get; set; }
            public int iIdUserRole { get; set; }
            public string sNameUsuario { get; set; }
            public int? iIdOffer { get; set; }
        }

    }
}