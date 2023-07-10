using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using System.Net;
using WebApiArchivos.DataBaseArchivos.Context;
using WebApiArchivos.Models;
using WebApiArchivos.Services;

namespace WebApiArchivos.Controllers
{
    [Authorize]
    public class ArchivoController : Controller
    {
        private readonly ArchivosContext _context;
        private readonly IConfiguration _configuration;

        public ArchivoController(ArchivosContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        /// <summary>
        /// Envía un archivo para ser guardado.
        /// </summary>
        /// <remarks>
        /// <para> El método GuardarArchivo, recibe un archivo y lo guarda en el sistema.
        /// Primero, obtiene información necesaria, como la ruta de almacenamiento y el tipo de archivo. 
        /// Luego, crea la carpeta si no existe y guarda el archivo en la ubicación especificada. 
        /// Después, registra la información del archivo en la base de datos (spRegistrarArchivo) y devuelve una respuesta 
        /// indicando si la operación fue exitosa o si se produjo un error.
        /// </para>
        /// <para> Para saber el IdAlmacenamiento Ver la tabla Almacenamiento de la base Archivos. </para>
        /// <para> Para Saber el IdAplicacion Ver la tabla Aplicacion de la base Archivos. </para>
        /// </remarks>
        [HttpPost]
        [Route("/SetArchivo")]
        [ProducesResponseType(typeof(ArchivoResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult<ArchivoResponse>> SetArchivoAsync([FromForm] EnvioArchivo setArchivo)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest("Informacion de archivo incompleta");
                
                if (setArchivo.File == null || setArchivo.File.Length == 0)
                    return BadRequest("No se ha proporcionado un archivo.");

                var ArchivoService = new ArchivoService(_context, _configuration);
                var archivoResponse = await ArchivoService.GuardarArchivo(setArchivo);

                return Ok(archivoResponse);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        /// <summary>
        /// Obtiene un archivo.
        /// </summary>
        /// <remarks>
        /// El código proporcionado hace una llamada a la funcion "ObtenerArchivo" 
        /// que se utiliza para obtener un archivo específico a partir de un identificador (IdArchivo). 
        /// La función utiliza un contexto de datos y un procedimiento almacenado (SpGetArchivoEspecifico) para buscar el archivo en cuestión.
        /// </remarks>
        [HttpGet]
        [Route("/GetArchivo")]
        [ProducesResponseType(typeof(FileContentResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult> GetArchivoAsync(string IdArchivo)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest("Informacion de archivo incompleta");

                var ArchivoService = new ArchivoService(_context, _configuration);
                var getArchivoResponse = await ArchivoService.ObtenerArchivo(IdArchivo);

                if (getArchivoResponse.Exitoso == false)
                    return BadRequest(getArchivoResponse.Mensaje);

                if (getArchivoResponse.FileBytes == null)
                    return BadRequest("El archivo no pudo ser encontrado.");

                return File(getArchivoResponse.FileBytes, "application/octet-stream", getArchivoResponse.NombreArchivo);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
