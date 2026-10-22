using System.IO;
using System.Web;

namespace Mantenimiento_Liquidaciones.Tests.TestHelpers
{
    // Implementación mínima de HttpPostedFileBase para simular la subida de un Excel
    // en los tests de carga masiva, sin depender del pipeline HTTP real.
    public class FakeHttpPostedFile : HttpPostedFileBase
    {
        private readonly Stream _stream;
        private readonly string _fileName;

        public FakeHttpPostedFile(Stream stream, string fileName)
        {
            _stream = stream;
            _fileName = fileName;
        }

        public override int ContentLength => (int)_stream.Length;

        public override string FileName => _fileName;

        public override Stream InputStream => _stream;
    }
}
