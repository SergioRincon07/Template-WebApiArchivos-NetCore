namespace WebApiArchivos.Models
{
    public class ArchivoResponse
    {
        public Guid? IdArchivo { get; set; }
        public string? FilePath { get; set; }
        public string? Error { get; set; }
        public Boolean Exitoso { get; set; }
    }
}
