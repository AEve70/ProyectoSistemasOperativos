using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

/* Proyecto Sistemas Operativos
 * Sebastián Alfaro Arias C4C212
 * Alejandro Córdoba Solís C4E463 
 * Evelyn Martinez Hernández C34617
 */

namespace AppClienteControlador
{ 
    //Form para mostrar la captura de pantalla
    public partial class FormScrenshot : Form
    {
        public FormScrenshot()
        {
            InitializeComponent();
            pb_captura.Dock = DockStyle.Fill;
            pb_captura.SizeMode = PictureBoxSizeMode.Zoom;
            pb_captura.BackColor = Color.Black;
        }

        private void FormScrenshot_Load(object sender, EventArgs e)
        {

        }

        private void pb_captura_Click(object sender, EventArgs e)
        {

        }

        //La captura se mostrará en el picturebox integrado al forms
        public void mostrarCaptura(Image img)
        {
           pb_captura.Image  = img;
        }
    }
}
