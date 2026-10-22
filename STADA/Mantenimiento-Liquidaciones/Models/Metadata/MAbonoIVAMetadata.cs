using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace Mantenimiento_Liquidaciones.Models.Metadata
{
    [MetadataType(typeof(MAbono_IVAMetadata))]
    public partial class MAbono_IVA
    {
        // Esta clase puede estar vacía, ya que las validaciones estarán en la clase MAbonoIVAMetadata.
    }

    public class MAbono_IVAMetadata
    {
        [Required(ErrorMessage = "El campo Id Abono IVA es obligatorio.")]
        public string sIdAbonoIVA { get; set; }

        [Required(ErrorMessage = "El campo Descripción Abono es obligatorio.")]
        public string sAbonoIVA { get; set; }

        [Required(ErrorMessage = "El campo División es obligatorio.")]
        public string sIdDivision { get; set; }

        [Required(ErrorMessage = "El campo Red es obligatorio.")]
        public string sIdRed { get; set; }

        [Required(ErrorMessage = "El campo IVA es obligatorio.")]
        [Range(0, double.MaxValue, ErrorMessage = "El campo IVA debe ser un número válido.")]
        public decimal fIVA { get; set; }

        [Required(ErrorMessage = "El campo Estado es obligatorio.")]
        public int iEstado { get; set; }

        public DateTime? dFechaActualizacion { get; set; }
        public string sUsuarioActualizacion { get; set; }
    }
}
