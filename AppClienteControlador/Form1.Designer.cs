
namespace AppClienteControlador
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btn_conectar = new System.Windows.Forms.Button();
            this.btn_desconectar = new System.Windows.Forms.Button();
            this.txt_ip = new System.Windows.Forms.TextBox();
            this.txt_puerto = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.lbl_estado = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.cbx_datos = new System.Windows.Forms.ComboBox();
            this.btn_consultar = new System.Windows.Forms.Button();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.btn_subirVolumen = new System.Windows.Forms.Button();
            this.btn_bajarVolumen = new System.Windows.Forms.Button();
            this.btn_silenciar = new System.Windows.Forms.Button();
            this.btn_apagar = new System.Windows.Forms.Button();
            this.btn_reiniciar = new System.Windows.Forms.Button();
            this.btn_cerrarSesion = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.timer_mouse = new System.Windows.Forms.Timer(this.components);
            this.btn_detenerMouse = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(27, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(23, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "IP: ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(175, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 15);
            this.label2.TabIndex = 1;
            this.label2.Text = "Puerto";
            // 
            // btn_conectar
            // 
            this.btn_conectar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(120)))), ((int)(((byte)(220)))));
            this.btn_conectar.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(120)))), ((int)(((byte)(220)))));
            this.btn_conectar.FlatAppearance.BorderSize = 0;
            this.btn_conectar.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btn_conectar.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_conectar.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btn_conectar.Location = new System.Drawing.Point(284, 31);
            this.btn_conectar.Name = "btn_conectar";
            this.btn_conectar.Size = new System.Drawing.Size(82, 23);
            this.btn_conectar.TabIndex = 2;
            this.btn_conectar.Text = "Conectar";
            this.btn_conectar.UseVisualStyleBackColor = false;
            this.btn_conectar.Click += new System.EventHandler(this.btn_conectar_Click);
            // 
            // btn_desconectar
            // 
            this.btn_desconectar.Location = new System.Drawing.Point(284, 60);
            this.btn_desconectar.Name = "btn_desconectar";
            this.btn_desconectar.Size = new System.Drawing.Size(82, 23);
            this.btn_desconectar.TabIndex = 3;
            this.btn_desconectar.Text = "Desconectar";
            this.btn_desconectar.UseVisualStyleBackColor = true;
            this.btn_desconectar.Click += new System.EventHandler(this.btn_desconectar_Click);
            // 
            // txt_ip
            // 
            this.txt_ip.Location = new System.Drawing.Point(56, 31);
            this.txt_ip.Name = "txt_ip";
            this.txt_ip.Size = new System.Drawing.Size(100, 22);
            this.txt_ip.TabIndex = 4;
            this.txt_ip.Text = "127.0.0.1";
            // 
            // txt_puerto
            // 
            this.txt_puerto.Location = new System.Drawing.Point(219, 31);
            this.txt_puerto.Name = "txt_puerto";
            this.txt_puerto.Size = new System.Drawing.Size(59, 22);
            this.txt_puerto.TabIndex = 5;
            this.txt_puerto.Text = "8000";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.lbl_estado);
            this.groupBox1.Controls.Add(this.txt_ip);
            this.groupBox1.Controls.Add(this.btn_desconectar);
            this.groupBox1.Controls.Add(this.txt_puerto);
            this.groupBox1.Controls.Add(this.btn_conectar);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(3, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(456, 100);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Conexion";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(90, 69);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(38, 15);
            this.label4.TabIndex = 7;
            this.label4.Text = "label4";
            // 
            // lbl_estado
            // 
            this.lbl_estado.AutoSize = true;
            this.lbl_estado.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_estado.Location = new System.Drawing.Point(30, 69);
            this.lbl_estado.Name = "lbl_estado";
            this.lbl_estado.Size = new System.Drawing.Size(48, 15);
            this.lbl_estado.TabIndex = 6;
            this.lbl_estado.Text = "Estado: ";
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Image = ((System.Drawing.Image)(resources.GetObject("button1.Image")));
            this.button1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button1.Location = new System.Drawing.Point(130, 15);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(91, 32);
            this.button1.TabIndex = 8;
            this.button1.Text = "Controlar";
            this.button1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(4, 111);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(131, 15);
            this.label3.TabIndex = 7;
            this.label3.Text = "Informacion del Equipo";
            // 
            // cbx_datos
            // 
            this.cbx_datos.FormattingEnabled = true;
            this.cbx_datos.Location = new System.Drawing.Point(141, 109);
            this.cbx_datos.Name = "cbx_datos";
            this.cbx_datos.Size = new System.Drawing.Size(182, 21);
            this.cbx_datos.TabIndex = 8;
            // 
            // btn_consultar
            // 
            this.btn_consultar.Location = new System.Drawing.Point(330, 106);
            this.btn_consultar.Name = "btn_consultar";
            this.btn_consultar.Size = new System.Drawing.Size(75, 23);
            this.btn_consultar.TabIndex = 9;
            this.btn_consultar.Text = "Consultar";
            this.btn_consultar.UseVisualStyleBackColor = true;
            this.btn_consultar.Click += new System.EventHandler(this.btn_consultar_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(7, 135);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(452, 139);
            this.richTextBox1.TabIndex = 10;
            this.richTextBox1.Text = "";
            // 
            // btn_subirVolumen
            // 
            this.btn_subirVolumen.Image = ((System.Drawing.Image)(resources.GetObject("btn_subirVolumen.Image")));
            this.btn_subirVolumen.Location = new System.Drawing.Point(88, 14);
            this.btn_subirVolumen.Name = "btn_subirVolumen";
            this.btn_subirVolumen.Size = new System.Drawing.Size(36, 32);
            this.btn_subirVolumen.TabIndex = 11;
            this.btn_subirVolumen.UseVisualStyleBackColor = true;
            this.btn_subirVolumen.Click += new System.EventHandler(this.btn_subirVolumen_Click);
            // 
            // btn_bajarVolumen
            // 
            this.btn_bajarVolumen.Image = ((System.Drawing.Image)(resources.GetObject("btn_bajarVolumen.Image")));
            this.btn_bajarVolumen.Location = new System.Drawing.Point(49, 14);
            this.btn_bajarVolumen.Name = "btn_bajarVolumen";
            this.btn_bajarVolumen.Size = new System.Drawing.Size(35, 33);
            this.btn_bajarVolumen.TabIndex = 12;
            this.btn_bajarVolumen.UseVisualStyleBackColor = true;
            this.btn_bajarVolumen.Click += new System.EventHandler(this.btn_bajarVolumen_Click);
            // 
            // btn_silenciar
            // 
            this.btn_silenciar.Image = ((System.Drawing.Image)(resources.GetObject("btn_silenciar.Image")));
            this.btn_silenciar.Location = new System.Drawing.Point(6, 15);
            this.btn_silenciar.Name = "btn_silenciar";
            this.btn_silenciar.Size = new System.Drawing.Size(37, 32);
            this.btn_silenciar.TabIndex = 13;
            this.btn_silenciar.UseVisualStyleBackColor = true;
            this.btn_silenciar.Click += new System.EventHandler(this.btn_silenciar_Click);
            // 
            // btn_apagar
            // 
            this.btn_apagar.Image = ((System.Drawing.Image)(resources.GetObject("btn_apagar.Image")));
            this.btn_apagar.Location = new System.Drawing.Point(410, 14);
            this.btn_apagar.Name = "btn_apagar";
            this.btn_apagar.Size = new System.Drawing.Size(33, 32);
            this.btn_apagar.TabIndex = 14;
            this.btn_apagar.UseVisualStyleBackColor = true;
            this.btn_apagar.Click += new System.EventHandler(this.btn_apagar_Click);
            // 
            // btn_reiniciar
            // 
            this.btn_reiniciar.Image = ((System.Drawing.Image)(resources.GetObject("btn_reiniciar.Image")));
            this.btn_reiniciar.Location = new System.Drawing.Point(329, 15);
            this.btn_reiniciar.Name = "btn_reiniciar";
            this.btn_reiniciar.Size = new System.Drawing.Size(33, 30);
            this.btn_reiniciar.TabIndex = 15;
            this.btn_reiniciar.UseVisualStyleBackColor = true;
            this.btn_reiniciar.Click += new System.EventHandler(this.btn_reiniciar_Click);
            // 
            // btn_cerrarSesion
            // 
            this.btn_cerrarSesion.Image = ((System.Drawing.Image)(resources.GetObject("btn_cerrarSesion.Image")));
            this.btn_cerrarSesion.Location = new System.Drawing.Point(368, 14);
            this.btn_cerrarSesion.Name = "btn_cerrarSesion";
            this.btn_cerrarSesion.Size = new System.Drawing.Size(39, 32);
            this.btn_cerrarSesion.TabIndex = 16;
            this.btn_cerrarSesion.UseVisualStyleBackColor = true;
            this.btn_cerrarSesion.Click += new System.EventHandler(this.btn_cerrarSesion_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btn_detenerMouse);
            this.groupBox2.Controls.Add(this.button1);
            this.groupBox2.Controls.Add(this.btn_silenciar);
            this.groupBox2.Controls.Add(this.btn_reiniciar);
            this.groupBox2.Controls.Add(this.btn_cerrarSesion);
            this.groupBox2.Controls.Add(this.btn_bajarVolumen);
            this.groupBox2.Controls.Add(this.btn_subirVolumen);
            this.groupBox2.Controls.Add(this.btn_apagar);
            this.groupBox2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(7, 281);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(452, 53);
            this.groupBox2.TabIndex = 17;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Acciones";
            // 
            // timer_mouse
            // 
            this.timer_mouse.Interval = 40;
            this.timer_mouse.Tick += new System.EventHandler(this.timer_mouse_Tick);
            // 
            // btn_detenerMouse
            // 
            this.btn_detenerMouse.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_detenerMouse.Image = ((System.Drawing.Image)(resources.GetObject("btn_detenerMouse.Image")));
            this.btn_detenerMouse.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btn_detenerMouse.Location = new System.Drawing.Point(228, 15);
            this.btn_detenerMouse.Name = "btn_detenerMouse";
            this.btn_detenerMouse.Size = new System.Drawing.Size(88, 32);
            this.btn_detenerMouse.TabIndex = 17;
            this.btn_detenerMouse.Text = "Detener";
            this.btn_detenerMouse.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btn_detenerMouse.UseVisualStyleBackColor = true;
            this.btn_detenerMouse.Click += new System.EventHandler(this.button2_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.ClientSize = new System.Drawing.Size(471, 335);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.btn_consultar);
            this.Controls.Add(this.cbx_datos);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form1";
            this.Text = "Cliente";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btn_conectar;
        private System.Windows.Forms.Button btn_desconectar;
        private System.Windows.Forms.TextBox txt_ip;
        private System.Windows.Forms.TextBox txt_puerto;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lbl_estado;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ComboBox cbx_datos;
        private System.Windows.Forms.Button btn_consultar;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button btn_subirVolumen;
        private System.Windows.Forms.Button btn_bajarVolumen;
        private System.Windows.Forms.Button btn_silenciar;
        private System.Windows.Forms.Button btn_apagar;
        private System.Windows.Forms.Button btn_reiniciar;
        private System.Windows.Forms.Button btn_cerrarSesion;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btn_detenerMouse;
        private System.Windows.Forms.Timer timer_mouse;
    }

}

