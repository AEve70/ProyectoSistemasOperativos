using System;

namespace Compartido
{
    public class MensajesIO
    {
        public string Comando { get; set; }
        public bool Ok { get; set; }
        public object Datos { get; set; }
        public string Mensaje { get; set; }
        public string Enviador { get; set; }
        public string Tiempo { get; set; }

        public MensajesIO() => Tiempo = DateTime.Now.ToString("o");

        public MensajesIO(string comando, bool ok = true, object datos = null, string mensaje = "", string enviador = "")
        {
            Comando = comando;
            Ok = ok;
            Datos = datos;
            Mensaje = mensaje;
            Enviador = enviador;
            Tiempo = DateTime.Now.ToString("o");
        }
    }
}
