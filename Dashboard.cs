using System;
using System.Data;
using System.Windows.Forms;

namespace Laboratory3
{
    public partial class Dashboard : Form
    {
        // Use a DataTable to store the results in memory.
        // A real application would use a database.
        private DataTable resultsTable;

        public Dashboard()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Set up the DataTable structure when the form loads
            InitializeResultsTable();

            // Bind the DataTable to the DataGridView
            dgvResults.DataSource = resultsTable;

            // <<< MODIFIED --- Ensures the user selects the entire row for deletion
            dgvResults.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvResults.MultiSelect = false; // Optional: ensure only one row can be deleted at a time
        }

        private void InitializeResultsTable()
        {
            resultsTable = new DataTable();
            resultsTable.Columns.Add("Patient ID", typeof(string));
            resultsTable.Columns.Add("Test Name", typeof(string));
            resultsTable.Columns.Add("Result", typeof(string));
            resultsTable.Columns.Add("Date", typeof(DateTime));
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // 1. Validate Input
            if (string.IsNullOrWhiteSpace(txtPatientID.Text) ||
                string.IsNullOrWhiteSpace(txtTestName.Text) ||
                string.IsNullOrWhiteSpace(txtResult.Text))
            {
                MessageBox.Show("Please fill in all fields before saving.", "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Add data to the DataTable
            try
            {
                resultsTable.Rows.Add(
                    txtPatientID.Text,
                    txtTestName.Text,
                    txtResult.Text,
                    dtpTestDate.Value
                );

                // 3. Clear the input fields
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearForm()
        {
            txtPatientID.Clear();
            txtTestName.Clear();
            txtResult.Clear();
            dtpTestDate.Value = DateTime.Now; // Reset date to today
            txtPatientID.Focus(); // Set focus back to the first field
        }

        // <<< NEW --- This is the event handler for the delete button
        private void btnDelete_Click(object sender, EventArgs e)
        {
            // 1. Check if any row is selected
            if (dgvResults.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a row to delete.", "No Row Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 2. Confirm with the user
            DialogResult confirmation = MessageBox.Show("Are you sure you want to permanently delete this record?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirmation == DialogResult.Yes)
            {
                try
                {
                    // 3. Get the underlying DataRow from the selected DataGridView row
                    // This is the safest way to delete, as it works even if the user has sorted the grid.
                    DataRowView rowView = (DataRowView)dgvResults.SelectedRows[0].DataBoundItem;
                    DataRow rowToDelete = rowView.Row;

                    // 4. Remove the row from the DataTable
                    resultsTable.Rows.Remove(rowToDelete);

                    // Note: If you were using .Delete() instead of .Remove(), 
                    // you would also need to call resultsTable.AcceptChanges()
                    // to commit the deletion. .Remove() does it in one step.
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}