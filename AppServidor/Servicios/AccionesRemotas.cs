using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

/* Proyecto Sistemas Operativos
 * Sebastián Alfaro Arias C4C212
 * Alejandro Córdoba Solís C4E463 
 * Evelyn Martinez Hernández C34617
 */

namespace AppServidor.Servicios
{
    //Clase que contiene metodos de accion remota como Controlar Mouse y Mandar Mensaje
    public static class AccionesRemotas
    {
        //En este metodo obtenemos la pantalla, la copiamos en un mapa de bits y se convierte en base64 que son los datos de imagen.
        public static string TomarScreenshot()
        {
            try
            {
                Rectangle bounds = Screen.PrimaryScreen.Bounds;

                using (Bitmap bmp = new Bitmap(bounds.Width, bounds.Height))
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    // Obtener o dibujar pantalla
                    g.CopyFromScreen(bounds.X,bounds.Y, 0,0,bounds.Size,CopyPixelOperation.SourceCopy);

                    using (MemoryStream ms = new MemoryStream())
                    {
                        bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        return Convert.ToBase64String(ms.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                
                return $"ERROR_SCREENSHOT:{ex.Message}";
            }
        }
        //Metodo para enviar mensaje
        //Simplemente se le muestra un cuadro de texto al usuario con un mensaje
        public static string MostrarMensaje(string texto)
        {
            try
            {
                MessageBox.Show(texto, "Mensaje desde el Controlador", MessageBoxButtons.OK, MessageBoxIcon.Information);

                return "Mensaje mostrado correctamente";
            }
            catch (Exception ex)
            {
                return $"ERROR_MSG:{ex.Message}";
            }
        }
    }
}
