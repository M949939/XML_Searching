namespace ResultMesureSPC
{
    partial class ErrorInfoForm
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
            this.listViewErrorDetails = new System.Windows.Forms.ListBox();
            this.labelTitle = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // listViewErrorDetails
            // 
            this.listViewErrorDetails.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.listViewErrorDetails.FormattingEnabled = true;
            this.listViewErrorDetails.HorizontalScrollbar = true;
            this.listViewErrorDetails.ItemHeight = 16;
            this.listViewErrorDetails.Location = new System.Drawing.Point(0, 54);
            this.listViewErrorDetails.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.listViewErrorDetails.Name = "listViewErrorDetails";
            this.listViewErrorDetails.Size = new System.Drawing.Size(885, 404);
            this.listViewErrorDetails.TabIndex = 1;
            // 
            // labelTitle
            // 
            this.labelTitle.AutoSize = true;
            this.labelTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelTitle.Font = new System.Drawing.Font("Arial", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTitle.ForeColor = System.Drawing.Color.DarkBlue;
            this.labelTitle.Location = new System.Drawing.Point(0, 0);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(345, 46);
            this.labelTitle.TabIndex = 0;
            this.labelTitle.Text = "Error Information";
            // 
            // ErrorInfoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(885, 458);
            this.Controls.Add(this.labelTitle);
            this.Controls.Add(this.listViewErrorDetails);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "ErrorInfoForm";
            this.Text = "Error Information";
            this.Load += new System.EventHandler(this.ErrorInfoForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }




        #endregion

        private System.Windows.Forms.ListBox listViewErrorDetails;
        private System.Windows.Forms.Label labelTitle;
        // private Label labelTitle;
    }
}
