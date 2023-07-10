namespace WebApiArchivos.Models
{
    public class Token
    {
        public string TokenJWT { get; set; }
        public bool Acceso { get; set; }

        //La advertencia se solucionó asignando un valor predeterminado a la propiedades del constructor de la clase "Token" ya que no pueden ser Null. 
        //Esto garantiza que la propiedad tenga un valor válido al salir del constructor y evita la advertencia del compilador.

        public Token() 
        {
            TokenJWT = "default value";
        }
    }
}
