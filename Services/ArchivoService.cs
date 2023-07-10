using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Newtonsoft.Json.Linq;
using WebApiArchivos.DataBaseArchivos.Context;
using WebApiArchivos.DataBaseArchivos.Models;
using WebApiArchivos.Models;

namespace WebApiArchivos.Services
{
    public class ArchivoService
    {
        private readonly ArchivosContext _context;
        private readonly IConfiguration _configuration;

        public ArchivoService(ArchivosContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        public async Task<ArchivoResponse> GuardarArchivo(EnvioArchivo setArchivo)
        {
            try
            {
                List<SpGetAlmacenamientoResult> ListAlmacenamiento = await _context.GetProcedures().SpGetAlmacenamientoAsync(setArchivo.IdAlmacenamiento);

                if (ListAlmacenamiento.Count > 0)
                {
                    string rutaOrigen = ListAlmacenamiento[0].RutaFisica ?? string.Empty;
                    string rutaArchivo = ListAlmacenamiento[0].Raiz;
                    string nombreCarpetaCunstom = "\\" + setArchivo.NombreCarpeta;
                    string filePath = string.Empty;
                    long bytelenght = 0;

                    string contentType = new FileExtensionContentTypeProvider().TryGetContentType(setArchivo.File.FileName, out var result) ? result : "N/A";

                    if (ListAlmacenamiento[0].TieneFecha == true)
                    {
                        string datePath = "\\" + DateTime.Now.ToString("yyyy-MM");
                        filePath = Path.Combine(rutaOrigen + rutaArchivo + datePath + nombreCarpetaCunstom, setArchivo.File.FileName);
                    }
                    else
                    {
                        filePath = Path.Combine(rutaOrigen + rutaArchivo + nombreCarpetaCunstom, setArchivo.File.FileName);
                    }

                    Directory.CreateDirectory(Path.GetDirectoryName(filePath) ?? string.Empty);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await setArchivo.File.CopyToAsync(stream);
                        stream.Flush(); // Asegúrate de que los datos se escriban en el archivo antes de obtener el tamaño
                        bytelenght = stream.Length;
                    }

                    var registrarArchivo = new RegistrarArchivo
                    {
                        IdArchivo = Guid.NewGuid(),
                        Nombre = setArchivo.File.FileName,
                        Extension = setArchivo.File.FileName.Split('.').Last(),
                        MimeType = contentType,
                        Ruta = filePath,
                        IdAlmacenamiento = setArchivo.IdAlmacenamiento,
                        IdAplicacion = 1,
                        ByteLength = bytelenght,
                    };

                    await _context.GetProcedures().spRegistrarArchivoAsync(registrarArchivo.IdArchivo,
                                                                            registrarArchivo.Nombre,
                                                                            registrarArchivo.Extension,
                                                                            registrarArchivo.MimeType,
                                                                            registrarArchivo.Ruta,
                                                                            registrarArchivo.IdAlmacenamiento,
                                                                            registrarArchivo.IdAplicacion,
                                                                            registrarArchivo.ByteLength);

                    var setArchivoResponse = new ArchivoResponse
                    {
                        IdArchivo = registrarArchivo.IdArchivo,
                        FilePath = filePath,
                        Error = null,
                        Exitoso = true,
                    };

                    return setArchivoResponse;
                }
                else
                {
                    var setArchivoResponse = new ArchivoResponse
                    {
                        IdArchivo = null,
                        FilePath = null,
                        Error = "No se encontró en la base el Almacenamiento especificado",
                        Exitoso = false,
                    };

                    return setArchivoResponse;
                }

            }
            catch (Exception ex)
            {
                var setArchivoResponse = new ArchivoResponse
                {
                    IdArchivo = null,
                    FilePath = null,
                    Error = ex.Message,
                    Exitoso = false,
                };

                return setArchivoResponse;
            }

        }

        public async Task<ObtenerArchivo> ObtenerArchivo(string IdArchivo)
        {
            try
            {
                List<SpGetArchivoEspecificoResult> ListArchivo = await _context.GetProcedures().SpGetArchivoEspecificoAsync(IdArchivo);
                var responseGetArchivo = new ObtenerArchivo();
                if (ListArchivo.Count == 0)
                {
                    responseGetArchivo.FileBytes = null;
                    responseGetArchivo.Exitoso = false;
                    responseGetArchivo.Mensaje = "No se encontró el archivo";
                    responseGetArchivo.NombreArchivo = null;
                    return responseGetArchivo;
                }

                byte[] fileBytes = await File.ReadAllBytesAsync(ListArchivo[0].Ruta);

                responseGetArchivo.FileBytes = fileBytes;
                responseGetArchivo.Exitoso = true;
                responseGetArchivo.Mensaje = null;
                responseGetArchivo.NombreArchivo= ListArchivo[0].Nombre;
                return responseGetArchivo;

            }
            catch (Exception ex)
            {
                var responseGetArchivo = new ObtenerArchivo
                {
                    FileBytes = null,
                    Exitoso = false,
                    Mensaje = ex.Message,
                    NombreArchivo = null,
                };

                return responseGetArchivo;
            }
        }
    }
}
