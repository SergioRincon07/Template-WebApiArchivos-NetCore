using Newtonsoft.Json.Linq;
using System.Drawing;

namespace WebApiArchivos.Models
{
    public class Jwt
    {
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public string Key { get; set; }
        public string Subject { get; set; }

        //La advertencia se solucionó asignando un valor predeterminado a la propiedades del constructor de la clase "Jwt" ya que no pueden ser Null. 
        //Esto garantiza que la propiedad tenga un valor válido al salir del constructor y evita la advertencia del compilador.
        public Jwt()
        {
            Audience = "default value";
            Issuer = "default value";
            Key = "default value";
            Subject = "default value";
        }
    }
}
