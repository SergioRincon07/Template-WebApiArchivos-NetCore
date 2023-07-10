using System.ComponentModel.DataAnnotations;
using static System.Net.Mime.MediaTypeNames;

namespace WebApiArchivos.Models
{
    public class RegistrarArchivo
    {
        public Guid IdArchivo { get; set; }
        public string Nombre { get; set; }
        public string Extension { get; set; }
        public string MimeType { get; set; }
        public string Ruta { get; set; }
        public int IdAlmacenamiento { get; set; }
        public int IdAplicacion { get; set; }
        public long ByteLength { get; set; }

        public RegistrarArchivo()
        {
            Nombre = string.Empty;
            Extension = string.Empty;
            MimeType = string.Empty;
            Ruta = string.Empty;
        }
    }
}
