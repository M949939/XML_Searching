using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

namespace ResultMesureSPC
{
    public partial class ErrorInfoForm : Form
    {
        public ErrorInfoForm()
        {
            InitializeComponent();
        }

        // This method populates the ListBox with error details.
        public void AddErrorDetails(Dictionary<string, string> errorDetails)
        {
            ClearExistingContent();

            AddHeaders();

            AddErrorDetailsToListBox(errorDetails);
        }

        private void ClearExistingContent()
        {
            listViewErrorDetails.Items.Clear();
        }

        private void AddHeaders()
        {
            listViewErrorDetails.Items.Add("Field\tError Details");
        }

        private void AddErrorDetailsToListBox(Dictionary<string, string> errorDetails)
        {
            foreach (var detail in errorDetails)
            {
                listViewErrorDetails.Items.Add($"{detail.Key}\t{detail.Value}");
            }
        }

        private void ErrorInfoForm_Load(object sender, EventArgs e)
        {
            listViewErrorDetails.Font = new Font("Courier New", 10F);
        }
    }
}
