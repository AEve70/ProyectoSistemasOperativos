using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace AppServidor.Servicios
{
    public static class AccionesRemotas
    {
        //Se tomara la captura en un formato entendible base64
        public static string TomarScreenshot()
        {
            try
            {
                Rectangle bounds = Screen.PrimaryScreen.Bounds;

                using (Bitmap bmp = new Bitmap(bounds.Width, bounds.Height))
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.CopyFromScreen(bounds.X, bounds.Y, 0, 0, bounds.Size);

                    using (MemoryStream ms = new MemoryStream())
                    {
                        bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        return Convert.ToBase64String(ms.ToArray());
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error al capturar pantalla: " + e.Message);
            }
        }


        public static string MostrarMensaje(String texto)
        {
            try
            {
                MessageBox.Show(texto, "Mensaje desde el Controlador",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                return "Mensaje mostrado correctamente";
            }
            catch (Exception ex)
            {
                throw new Exception("Error mostrando mensaje: " + ex.Message);
            }
        }
    }
}
