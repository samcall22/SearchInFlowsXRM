namespace SearchInFlows
{
    partial class MyPluginControl
    {
        /// <summary> 
        /// Necessary designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed of; false otherwise.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de componentes


        /// <summary>
        /// Method required to support the Designer. You cannot modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnFetchSolutions = new System.Windows.Forms.Button();
            this.lvSolutions = new System.Windows.Forms.ListView();
            this.colName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colUniqueName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnFetchFlows = new System.Windows.Forms.Button();
            this.rtbResults = new System.Windows.Forms.RichTextBox();
            this.txtSearchTerm = new System.Windows.Forms.MaskedTextBox();
            this.lblTextSearch = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnFetchSolutions
            // 
            this.btnFetchSolutions.Location = new System.Drawing.Point(5, 5);
            this.btnFetchSolutions.Margin = new System.Windows.Forms.Padding(4);
            this.btnFetchSolutions.Name = "btnFetchSolutions";
            this.btnFetchSolutions.Size = new System.Drawing.Size(160, 28);
            this.btnFetchSolutions.TabIndex = 0;
            this.btnFetchSolutions.Text = "Load solutions";
            this.btnFetchSolutions.UseVisualStyleBackColor = true;
            this.btnFetchSolutions.Click += new System.EventHandler(this.btnFetchSolutions_Click);
            // 
            // lvSolutions
            // 
            this.lvSolutions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lvSolutions.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colName,
            this.colUniqueName});
            this.lvSolutions.FullRowSelect = true;
            this.lvSolutions.HideSelection = false;
            this.lvSolutions.Location = new System.Drawing.Point(5, 42);
            this.lvSolutions.Margin = new System.Windows.Forms.Padding(4);
            this.lvSolutions.MultiSelect = false;
            this.lvSolutions.Name = "lvSolutions";
            this.lvSolutions.Size = new System.Drawing.Size(390, 507);
            this.lvSolutions.TabIndex = 1;
            this.lvSolutions.UseCompatibleStateImageBehavior = false;
            this.lvSolutions.View = System.Windows.Forms.View.Details;
            // 
            // colName
            // 
            this.colName.Text = "Nombre de la Solución";
            this.colName.Width = 180;
            // 
            // colUniqueName
            // 
            this.colUniqueName.Text = "Nombre Único";
            this.colUniqueName.Width = 180;
            // 
            // btnFetchFlows
            // 
            this.btnFetchFlows.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFetchFlows.Location = new System.Drawing.Point(1156, 5);
            this.btnFetchFlows.Margin = new System.Windows.Forms.Padding(4);
            this.btnFetchFlows.Name = "btnFetchFlows";
            this.btnFetchFlows.Size = new System.Drawing.Size(200, 28);
            this.btnFetchFlows.TabIndex = 4;
            this.btnFetchFlows.Text = "Search in flows";
            this.btnFetchFlows.UseVisualStyleBackColor = true;
            this.btnFetchFlows.Click += new System.EventHandler(this.btnFetchFlows_Click);
            // 
            // rtbResults
            // 
            this.rtbResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbResults.Location = new System.Drawing.Point(404, 42);
            this.rtbResults.Margin = new System.Windows.Forms.Padding(4);
            this.rtbResults.Name = "rtbResults";
            this.rtbResults.ReadOnly = true;
            this.rtbResults.Size = new System.Drawing.Size(952, 507);
            this.rtbResults.TabIndex = 5;
            this.rtbResults.Text = "";
            // 
            // txtSearchTerm
            // 
            this.txtSearchTerm.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSearchTerm.Location = new System.Drawing.Point(510, 8);
            this.txtSearchTerm.Name = "txtSearchTerm";
            this.txtSearchTerm.Size = new System.Drawing.Size(639, 22);
            this.txtSearchTerm.TabIndex = 3;
            this.txtSearchTerm.Text = "Text to seach";
            // 
            // lblTextSearch
            // 
            this.lblTextSearch.AutoSize = true;
            this.lblTextSearch.Location = new System.Drawing.Point(404, 11);
            this.lblTextSearch.Name = "lblTextSearch";
            this.lblTextSearch.Size = new System.Drawing.Size(94, 16);
            this.lblTextSearch.TabIndex = 2;
            this.lblTextSearch.Text = "Text to search:";
            // 
            // MyPluginControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblTextSearch);
            this.Controls.Add(this.txtSearchTerm);
            this.Controls.Add(this.rtbResults);
            this.Controls.Add(this.btnFetchFlows);
            this.Controls.Add(this.lvSolutions);
            this.Controls.Add(this.btnFetchSolutions);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "MyPluginControl";
            this.Size = new System.Drawing.Size(1360, 554);
            this.OnCloseTool += new System.EventHandler(this.MyPluginControl_OnCloseTool);
            this.Load += new System.EventHandler(this.MyPluginControl_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnFetchSolutions;
        private System.Windows.Forms.ListView lvSolutions;
        private System.Windows.Forms.Button btnFetchFlows;
        private System.Windows.Forms.RichTextBox rtbResults;
        private System.Windows.Forms.ColumnHeader colName;
        private System.Windows.Forms.ColumnHeader colUniqueName;
        private System.Windows.Forms.MaskedTextBox txtSearchTerm;
        private System.Windows.Forms.Label lblTextSearch;
        
    }
}