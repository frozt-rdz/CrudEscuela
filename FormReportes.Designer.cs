namespace CrudEscuela
{
    partial class FormReportes
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
            cmbReporte = new ComboBox();
            dgvReporte = new DataGridView();
            ((System.ComponentModel.ISupportInitialize)dgvReporte).BeginInit();
            SuspendLayout();
            // 
            // cmbReporte
            // 
            cmbReporte.FormattingEnabled = true;
            cmbReporte.Location = new Point(12, 23);
            cmbReporte.Name = "cmbReporte";
            cmbReporte.Size = new Size(228, 28);
            cmbReporte.TabIndex = 0;
            cmbReporte.SelectedIndexChanged += cmbReporte_SelectedIndexChanged;
            // 
            // dgvReporte
            // 
            dgvReporte.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvReporte.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvReporte.Location = new Point(12, 93);
            dgvReporte.Name = "dgvReporte";
            dgvReporte.RowHeadersWidth = 51;
            dgvReporte.Size = new Size(758, 298);
            dgvReporte.TabIndex = 1;
            // 
            // FormReportes
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(782, 403);
            Controls.Add(dgvReporte);
            Controls.Add(cmbReporte);
            Name = "FormReportes";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Reportes";
            Load += FormReportes_Load;
            ((System.ComponentModel.ISupportInitialize)dgvReporte).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private ComboBox cmbReporte;
        private DataGridView dgvReporte;
    }
}