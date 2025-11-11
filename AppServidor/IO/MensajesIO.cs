using System;

public class MensajesIO
{
    
    public string Comando { get; set; }       // Ej: "GET_SYSINFO"
    public bool Ok { get; set; }              // True = correcto
    public object Datos { get; set; }         // Cuerpo del mensaje (entrada o salida)
    public string Mensaje { get; set; }       // Texto de respuesta o error
    public string Enviador { get; set; }      // "Cliente" o "Servidor"
    public string Tiempo { get; set; }        // Fecha y hora en formato ISO

   
    public MensajesIO()
    {
        Tiempo = DateTime.Now.ToString("o");
    }

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

