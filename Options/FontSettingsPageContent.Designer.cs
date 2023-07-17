namespace Fontify.Options
{
    partial class FontSettingsPageContent
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SettingsGrid = new System.Windows.Forms.PropertyGrid();
            this.SuspendLayout();
            // 
            // SettingsGrid
            // 
            this.SettingsGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SettingsGrid.Location = new System.Drawing.Point(0, 0);
            this.SettingsGrid.Name = "SettingsGrid";
            this.SettingsGrid.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.SettingsGrid.Size = new System.Drawing.Size(610, 580);
            this.SettingsGrid.TabIndex = 0;
            this.SettingsGrid.ToolbarVisible = false;
            this.SettingsGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.SettingsGrid_PropertyValueChanged);
            // 
            // FontSettingsPageContent
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.SettingsGrid);
            this.Name = "FontSettingsPageContent";
            this.Size = new System.Drawing.Size(610, 580);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PropertyGrid SettingsGrid;
    }
}
