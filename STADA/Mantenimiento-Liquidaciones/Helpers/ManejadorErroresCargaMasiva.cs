// Archivo: Helpers/ManejadorErroresCargaMasiva.cs

using System.Collections.Generic;

namespace Mantenimiento_Liquidaciones.Helpers
{
    public static class ManejadorErroresCargaMasiva
    {
        public enum ErroresCargaMasiva
        {
            ArchivoNoSeleccionado,
            ArchivoNoEsExcel,
            HojaDeCalculoNoEncontrada,
            ColumnaFaltanteOIncorrecta,
            FormatoFechaInvalido,
            CampoObligatorio,
            LongitudMaximaExcedida,
            PrecioInvalido,
            IdTipoInvalido,
            FechaFinMenorIgualFechaInicio,
            NoHayRegistrosEnExcel,
            RegistroDuplicado,
            FormatoNumeroInvalido,
            CampoVacioEnActualizacion,

        }

        private static readonly Dictionary<ErroresCargaMasiva, string> _mensajesError = new Dictionary<ErroresCargaMasiva, string>
        {
            { ErroresCargaMasiva.ArchivoNoSeleccionado, "Por favor, selecciona un archivo Excel." },
            { ErroresCargaMasiva.ArchivoNoEsExcel, "El archivo debe ser de tipo Excel (.xlsx)." },
            { ErroresCargaMasiva.HojaDeCalculoNoEncontrada, "El archivo Excel no contiene ninguna hoja de cálculo." },
            { ErroresCargaMasiva.ColumnaFaltanteOIncorrecta, "Falta la columna '{0}' o el nombre es incorrecto en el archivo Excel." },
            { ErroresCargaMasiva.FormatoFechaInvalido, "El campo '{0}' debe ser una fecha válida." },
            { ErroresCargaMasiva.CampoObligatorio, "Fila {0}: El campo '{1}' es obligatorio." },
            { ErroresCargaMasiva.LongitudMaximaExcedida, "Fila {0}: El campo '{1}' no debe ser mayor a {2} dígitos." },
            { ErroresCargaMasiva.PrecioInvalido, "Fila {0}: El campo 'Precio' es obligatorio y debe ser un valor numérico mayor a cero." },
            { ErroresCargaMasiva.IdTipoInvalido, "Fila {0}: El campo 'IdTipo' es obligatorio y debe ser un valor numérico mayor a cero." },
            { ErroresCargaMasiva.FechaFinMenorIgualFechaInicio, "Fila {0}: El campo 'FechaFin' debe ser mayor a FechaIni." },
            { ErroresCargaMasiva.NoHayRegistrosEnExcel, "No se encuentran registros en el excel" },
            { ErroresCargaMasiva.RegistroDuplicado, "Fila {0}: Ya existe un registro con los mismos valores de clave primaria." },
            { ErroresCargaMasiva.FormatoNumeroInvalido, "Fila {0}: Formato de numero no valido" },
            { ErroresCargaMasiva.CampoVacioEnActualizacion, "Fila {0}: El campo '{1}' no puede quedar vacío al actualizar un socio existente." }

        };

        public static string ObtenerMensajeError(ErroresCargaMasiva error, params object[] args)
        {
            if (_mensajesError.ContainsKey(error))
            {
                return (_mensajesError[error]);
            }
            else
            {
                return "Error desconocido.";
            }
        }
    }
}