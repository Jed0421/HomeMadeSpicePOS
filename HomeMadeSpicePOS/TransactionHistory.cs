using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HomeMadeSpicePOS
{
    public partial class TransactionHistory : UserControl
    {
        public TransactionHistory()
        {
            InitializeComponent();
        }



        string connectionString = "Data Source=pos.db;Version=3;";

        
        public event EventHandler DataCleared; // NEW EVENT

        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;

        public void UC_ViewTransaction_MouseDown(object sender, MouseEventArgs e)
        {
            dragging = true;
            dragCursorPoint = Cursor.Position;
            dragFormPoint = this.Location;
        }

        public void UC_ViewTransaction_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                Point diff = Point.Subtract(Cursor.Position, new Size(dragCursorPoint));
                this.Location = Point.Add(dragFormPoint, new Size(diff));
            }
        }

        public void UC_ViewTransaction_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }







        private void UC_ViewTransactions_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        string dbConnectionString = "Data Source=pos.db;Version=3;";


        private void btnClear_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Are you sure you want to delete all transaction history? This cannot be undone.",
                "Confirm Clear All",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result == DialogResult.Yes)
            {
                try
                {
                    using (SQLiteConnection conn = new SQLiteConnection(dbConnectionString))
                    {
                        conn.Open();

                        // Delete all records from SalesArchive
                        string deleteQuery = "DELETE FROM SalesArchive";
                        SQLiteCommand deleteCmd = new SQLiteCommand(deleteQuery, conn);
                        deleteCmd.ExecuteNonQuery();

                        // Reset the auto-increment counter
                        string resetQuery = "DELETE FROM sqlite_sequence WHERE name='SalesArchive'";
                        SQLiteCommand resetCmd = new SQLiteCommand(resetQuery, conn);
                        resetCmd.ExecuteNonQuery();
                    }

                    // Refresh the DataGridView
                    LoadAllTransactions();

                    // Notify Form1 that data was cleared
                    DataCleared?.Invoke(this, EventArgs.Empty);

                    MessageBox.Show("All transaction history has been cleared.", "Success");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error clearing transactions:\n" + ex.Message, "Error");
                }
            }
        }

            public void LoadAllTransactions()
            {

            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT * FROM SalesArchive ORDER BY Time DESC";

                    SQLiteDataAdapter da = new SQLiteDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    UC_ViewTransactions.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading transactions:\n" + ex.Message);
            }

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Form1 parentForm = this.FindForm() as Form1;
            if (parentForm != null)
            {
                parentForm.Controls.Remove(this);
                this.Dispose();
            }
        }
    }
    }

