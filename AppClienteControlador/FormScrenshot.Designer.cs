
namespace AppClienteControlador
{
    partial class FormScrenshot
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.pb_captura = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pb_captura)).BeginInit();
            this.SuspendLayout();
            // 
            // pb_captura
            // 
            this.pb_captura.Location = new System.Drawing.Point(3, 2);
            this.pb_captura.Name = "pb_captura";
            this.pb_captura.Size = new System.Drawing.Size(466, 335);
            this.pb_captura.TabIndex = 0;
            this.pb_captura.TabStop = false;
            this.pb_captura.Click += new System.EventHandler(this.pb_captura_Click);
            // 
            // FormScrenshot
            // 
            this.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.ClientSize = new System.Drawing.Size(471, 336);
            this.Controls.Add(this.pb_captura);
            this.Name = "FormScrenshot";
            this.Text = "Captura Tomada";
            this.Load += new System.EventHandler(this.FormScrenshot_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pb_captura)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pb_captura;
    }
}
