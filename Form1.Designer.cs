namespace CrudEscuela
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            cmbTabla = new ComboBox();
            dgvDatos = new DataGridView();
            btnInsertar = new Button();
            btnModificar = new Button();
            btnEliminar = new Button();
            label1 = new Label();
            txtBuscar = new TextBox();
            btnBuscar = new Button();
            btnLimpiar = new Button();
            lblResultados = new Label();
            ((System.ComponentModel.ISupportInitialize)dgvDatos).BeginInit();
            SuspendLayout();
            // 
            // cmbTabla
            // 
            cmbTabla.FormattingEnabled = true;
            cmbTabla.Location = new Point(105, 63);
            cmbTabla.Name = "cmbTabla";
            cmbTabla.Size = new Size(250, 28);
            cmbTabla.TabIndex = 0;
            cmbTabla.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            // 
            // dgvDatos
            // 
            dgvDatos.AllowUserToResizeColumns = false;
            dgvDatos.AllowUserToResizeRows = false;
            dgvDatos.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvDatos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvDatos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvDatos.Location = new Point(12, 235);
            dgvDatos.Name = "dgvDatos";
            dgvDatos.RowHeadersWidth = 51;
            dgvDatos.Size = new Size(858, 266);
            dgvDatos.TabIndex = 1;
            dgvDatos.CellContentClick += dataGridView1_CellContentClick;
            // 
            // btnInsertar
            // 
            btnInsertar.Location = new Point(105, 99);
            btnInsertar.Name = "btnInsertar";
            btnInsertar.Size = new Size(109, 28);
            btnInsertar.TabIndex = 2;
            btnInsertar.Text = "Insertar";
            btnInsertar.UseVisualStyleBackColor = true;
            btnInsertar.Click += btnInsertar_Click;
            // 
            // btnModificar
            // 
            btnModificar.Location = new Point(105, 133);
            btnModificar.Name = "btnModificar";
            btnModificar.Size = new Size(109, 28);
            btnModificar.TabIndex = 3;
            btnModificar.Text = "Modificar";
            btnModificar.UseVisualStyleBackColor = true;
            btnModificar.Click += btnModificar_Click;
            // 
            // btnEliminar
            // 
            btnEliminar.Location = new Point(105, 167);
            btnEliminar.Name = "btnEliminar";
            btnEliminar.Size = new Size(109, 29);
            btnEliminar.TabIndex = 4;
            btnEliminar.Text = "Eliminar";
            btnEliminar.UseVisualStyleBackColor = true;
            btnEliminar.Click += button3_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 16.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(343, 38);
            label1.TabIndex = 5;
            label1.Text = "Administración de tablas";
            label1.Click += label1_Click;
            // 
            // txtBuscar
            // 
            txtBuscar.Location = new Point(385, 63);
            txtBuscar.Name = "txtBuscar";
            txtBuscar.Size = new Size(374, 27);
            txtBuscar.TabIndex = 7;
            // 
            // btnBuscar
            // 
            btnBuscar.Location = new Point(385, 98);
            btnBuscar.Name = "btnBuscar";
            btnBuscar.Size = new Size(94, 29);
            btnBuscar.TabIndex = 8;
            btnBuscar.Text = "Buscar";
            btnBuscar.UseVisualStyleBackColor = true;
            btnBuscar.Click += btnBuscar_Click;
            // 
            // btnLimpiar
            // 
            btnLimpiar.Location = new Point(485, 99);
            btnLimpiar.Name = "btnLimpiar";
            btnLimpiar.Size = new Size(94, 29);
            btnLimpiar.TabIndex = 9;
            btnLimpiar.Text = "Limpiar";
            btnLimpiar.UseVisualStyleBackColor = true;
            btnLimpiar.Click += btnLimpiar_Click;
            // 
            // lblResultados
            // 
            lblResultados.AutoSize = true;
            lblResultados.Location = new Point(598, 103);
            lblResultados.Name = "lblResultados";
            lblResultados.Size = new Size(0, 20);
            lblResultados.TabIndex = 10;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(882, 513);
            Controls.Add(lblResultados);
            Controls.Add(btnLimpiar);
            Controls.Add(btnBuscar);
            Controls.Add(txtBuscar);
            Controls.Add(label1);
            Controls.Add(btnEliminar);
            Controls.Add(btnModificar);
            Controls.Add(btnInsertar);
            Controls.Add(dgvDatos);
            Controls.Add(cmbTabla);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)dgvDatos).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ComboBox cmbTabla;
        private DataGridView dgvDatos;
        private Button btnInsertar;
        private Button btnModificar;
        private Button btnEliminar;
        private Label label1;
        private PictureBox pictureBox1;
        private TextBox txtBuscar;
        private Button btnBuscar;
        private Button btnLimpiar;
        private Label lblResultados;
    }
}
