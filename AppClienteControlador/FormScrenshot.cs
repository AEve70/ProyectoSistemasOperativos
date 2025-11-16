using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace AppClienteControlador
{
    public partial class FormScrenshot : Form
    {
        public FormScrenshot()
        {
            InitializeComponent();
            pb_captura.SizeMode = PictureBoxSizeMode.Zoom;
        }

        private void FormScrenshot_Load(object sender, EventArgs e)
        {

        }

        private void pb_captura_Click(object sender, EventArgs e)
        {

        }

        public void mostrarCaptura(Image img)
        {
           pb_captura.Image  = img;
        }
    }
}
