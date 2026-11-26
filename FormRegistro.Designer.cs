namespace CrudEscuela
{
    partial class FormRegistro
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            pnlCampos = new Panel();
            btnGuardar = new Button();
            btnCancelar = new Button();
            pnlBotones = new Panel();
            pnlBotones.SuspendLayout();
            SuspendLayout();
            // 
            // pnlCampos
            // 
            pnlCampos.AutoScroll = true;
            pnlCampos.Dock = DockStyle.Fill;
            pnlCampos.Location = new Point(0, 0);
            pnlCampos.Name = "pnlCampos";
            pnlCampos.Size = new Size(800, 325);
            pnlCampos.TabIndex = 0;
            pnlCampos.Paint += pnlCampos_Paint;
            // 
            // btnGuardar
            // 
            btnGuardar.Location = new Point(253, 47);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(94, 29);
            btnGuardar.TabIndex = 1;
            btnGuardar.Text = "Guardar";
            btnGuardar.UseVisualStyleBackColor = true;
            btnGuardar.Click += btnGuardar_Click;
            // 
            // btnCancelar
            // 
            btnCancelar.Location = new Point(429, 47);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(94, 29);
            btnCancelar.TabIndex = 2;
            btnCancelar.Text = "Cancelar";
            btnCancelar.UseVisualStyleBackColor = true;
            btnCancelar.Click += btnCancelar_Click;
            // 
            // pnlBotones
            // 
            pnlBotones.Controls.Add(btnGuardar);
            pnlBotones.Controls.Add(btnCancelar);
            pnlBotones.Dock = DockStyle.Bottom;
            pnlBotones.Location = new Point(0, 325);
            pnlBotones.Name = "pnlBotones";
            pnlBotones.Size = new Size(800, 125);
            pnlBotones.TabIndex = 3;
            // 
            // FormRegistro
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(pnlCampos);
            Controls.Add(pnlBotones);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Name = "FormRegistro";
            StartPosition = FormStartPosition.CenterParent;
            Text = "FormRegistro";
            Load += FormRegistro_Load;
            pnlBotones.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlCampos;
        private Button btnGuardar;
        private Button btnCancelar;
        private Panel pnlBotones;
    }
}