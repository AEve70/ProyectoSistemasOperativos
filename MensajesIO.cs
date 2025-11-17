using System;

namespace Compartido
{   //Clase para pasar informacion entre el cliente y servidor
    //Se usa estructura JSON ya que es más sencillo para enviar informacion (en este proyecto se usan objetos anonimos)
    //Esta clase es compartida entre la appcliente y appservidor
    public class MensajesIO
    {
        public string Comando { get; set; }
        public bool Ok { get; set; }
        public object Datos { get; set; }
        public string Mensaje { get; set; }
        public string Enviador { get; set; }
        public string Tiempo { get; set; }

        public MensajesIO() => Tiempo = DateTime.Now.ToString("o");
        //Estructura del JSON comando, su estado, los datos que se van enviar, el mensaje, y quien envia, el timer es para monitorear cuando tarda
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
