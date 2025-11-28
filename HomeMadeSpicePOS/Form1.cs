using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HomeMadeSpicePOS
{
    public partial class Form1 : Form
    {
        string connectionString = "Data Source=pos.db;Version=3;";

        public Form1()
        {
            InitializeComponent();
            InitializeDatabase();    
            
            LoadSalesHistory();
            LoadTotalSales();
            LoadBestSellingItems();
            LoadSalesHistoryFromDatabase();
       
        }
        void LoadSalesHistoryFromDatabase()
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                SQLiteDataAdapter da = new SQLiteDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dataGridViewSalesHistory.DataSource = dt;
            }
        }


        private void LoadBestSellingItems()
        {
            var itemTotals = new Dictionary<string, int>();

            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                string query = "SELECT Items FROM Sales";
                SQLiteCommand cmd = new SQLiteCommand(query, conn);
                SQLiteDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string itemsText = reader["Items"].ToString();

                    // Example: "Carbonara x2, Baked Mac x1, Tuna Pasta x1"
                    var items = itemsText.Split(',');

                    foreach (var entry in items)
                    {
                        var parts = entry.Trim().Split('x');

                        if (parts.Length == 2)
                        {
                            string name = parts[0].Trim();
                            int qty = int.Parse(parts[1].Trim());

                            if (!itemTotals.ContainsKey(name))
                                itemTotals[name] = 0;

                            itemTotals[name] += qty;
                        }
                    }
                }
            }

            // Sort descending & take top 3
            var bestThree = itemTotals
                .OrderByDescending(i => i.Value)
                .Take(3);

            StringBuilder sb = new StringBuilder();
            int rank = 1;

            foreach (var item in bestThree)
            {
                sb.AppendLine($"{rank}. {item.Key} ({item.Value} sold)");
                rank++;
            }

            lblBestSellingDashboard.Text = sb.ToString();
        }

        private void LoadTotalSales()
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                // Changed from Sales to SalesArchive
                string query = "SELECT SUM(Total) FROM SalesArchive";
                SQLiteCommand cmd = new SQLiteCommand(query, conn);

                object result = cmd.ExecuteScalar();
                decimal total = result == DBNull.Value ? 0 : Convert.ToDecimal(result);

                lblTotalSalesDashboard.Text = "₱" + total.ToString("N2");
            }
        }
        private void InitializeDatabase()
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                string query = @"
            CREATE TABLE IF NOT EXISTS Sales (
                ID INTEGER PRIMARY KEY AUTOINCREMENT,
                Items TEXT NOT NULL,
                Total DECIMAL(10,2) NOT NULL,
                Time TEXT NOT NULL
            );
        ";

                SQLiteCommand cmd = new SQLiteCommand(query, conn);
                cmd.ExecuteNonQuery();
            }
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.Sizable;

            this.MaximumSize = new System.Drawing.Size(5000, 5000);

            this.Height = 2500;
        }
        Dictionary<string, OrderItem> cart = new Dictionary<string, OrderItem>();

        public class OrderItem
        {
            public string Name { get; set; }
            public int Qty { get; set; }
            public decimal Price { get; set; }

            public decimal Total => Qty * Price;
        }

        private void btnEspresso_Click(object sender, EventArgs e)
        {
            AddToCart("Cappucino", 75m);
        }
        private void AddToCart(string name, decimal price)
        {
            if (cart.ContainsKey(name))
            {
                cart[name].Qty++;
            }
            else
            {
                cart[name] = new OrderItem
                {
                    Name = name,
                    Qty = 1,
                    Price = price
                };
            }

            RefreshOrderPanel();
        }
        private void RefreshOrderPanel()
        {
            panelCurrentOrder.Controls.Clear();
            int y = 10;

            foreach (var item in cart.Values)
            {

                Panel row = new Panel();
                row.Width = panelCurrentOrder.Width - 25;
                row.Height = 70;   // Increased height
                row.Left = 10;
                row.Top = y;
                row.BackColor = Color.FromArgb(50, 50, 50);
                row.BorderStyle = BorderStyle.FixedSingle;

                // Larger font
                Font font = new Font("Segoe UI", 12, FontStyle.Bold);

                Label lblQty = new Label();
                lblQty.Text = item.Qty.ToString();
                lblQty.Width = 50;
                lblQty.Left = 10;
                lblQty.Top = 20;
                lblQty.ForeColor = Color.White;
                lblQty.Font = font;

                Label lblName = new Label();
                lblName.Text = item.Name;
                lblName.Width = 220;
                lblName.Left = 70;
                lblName.Top = 20;
                lblName.ForeColor = Color.White;
                lblName.Font = font;

                Label lblPrice = new Label();
                lblPrice.Text = "₱" + item.Total.ToString("0.00");
                lblPrice.Width = 100;
                lblPrice.Left = 300;
                lblPrice.Top = 20;
                lblPrice.ForeColor = Color.White;
                lblPrice.Font = font;

                Button btnPlus = new Button();
                btnPlus.Text = "+";
                btnPlus.Width = 40;
                btnPlus.Height = 30;
                btnPlus.Left = 410;
                btnPlus.Top = 18;
                btnPlus.Tag = item.Name;
                btnPlus.Font = new Font("Segoe UI", 12, FontStyle.Bold);
                btnPlus.Click += BtnPlus_Click;

                btnPlus.FlatStyle = FlatStyle.Flat;
                btnPlus.FlatAppearance.BorderSize = 0;
                btnPlus.BackColor = Color.White;

                btnPlus.HandleCreated += (s, e) => MakeRounded(btnPlus, 15);

                Button btnMinus = new Button();
                btnMinus.Text = "-";
                btnMinus.Width = 40;
                btnMinus.Height = 30;
                btnMinus.Left = 455;
                btnMinus.Top = 18;
                btnMinus.Tag = item.Name;
                btnMinus.Font = new Font("Segoe UI", 12, FontStyle.Bold);
                btnMinus.Click += BtnMinus_Click;

                btnMinus.FlatStyle = FlatStyle.Flat;
                btnMinus.FlatAppearance.BorderSize = 0;
                btnMinus.BackColor = Color.White;

                btnMinus.HandleCreated += (s, e) => MakeRounded(btnMinus, 15);

                row.Controls.Add(lblQty);
                row.Controls.Add(lblName);
                row.Controls.Add(lblPrice);
                row.Controls.Add(btnPlus);
                row.Controls.Add(btnMinus);

                panelCurrentOrder.Controls.Add(row);

                y += 75; // Adjust spacing between rows
            }

            UpdateTotalsPanel();
        }
        private void MakeRounded(Control c, int radius)
        {
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            path.StartFigure();
            path.AddArc(new Rectangle(0, 0, radius, radius), 180, 90);
            path.AddArc(new Rectangle(c.Width - radius, 0, radius, radius), 270, 90);
            path.AddArc(new Rectangle(c.Width - radius, c.Height - radius, radius, radius), 0, 90);
            path.AddArc(new Rectangle(0, c.Height - radius, radius, radius), 90, 90);
            path.CloseFigure();
            c.Region = new Region(path);


        }


        private void UpdateTotalsPanel()
        {
            decimal total = CalculateTotal();
            lblTotal.Text = "₱" + total.ToString("0.00");

            // Call the robust method to handle cash parsing and change display
            CalculateAndDisplayChange();
        }

        private void BtnPlus_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            string name = btn.Tag.ToString();

            cart[name].Qty++;
            RefreshOrderPanel();
        }
        private void BtnMinus_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            string name = btn.Tag.ToString();

            cart[name].Qty--;

            if (cart[name].Qty <= 0)
                cart.Remove(name);

            RefreshOrderPanel();
        }
        private decimal CalculateTotal()
        {
            return cart.Values.Sum(item => item.Total);
        }


        private void guna2Button14_Click(object sender, EventArgs e)
        {
            AddToCart("Caramel", 75m);
        }

        private void txtCash_TextChanged(object sender, EventArgs e)
        {
           UpdateTotalsPanel();
        }

        private void btnCheckOut_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtCash.Text))
                {
                    MessageBox.Show("Please enter cash amount.");
                    return;
                }

                if (!decimal.TryParse(txtCash.Text, out decimal cashPaid))
                {
                    MessageBox.Show("Invalid cash amount.");
                    return;
                }

                if (cart.Count == 0)
                {
                    MessageBox.Show("Cart is empty.");
                    return;
                }

                decimal total = CalculateTotal();
                if (cashPaid < total)
                {
                    MessageBox.Show("Not enough cash.");
                    return;
                }

                string transactionTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                // --- CREATE ONE SUMMARY STRING FOR THE WHOLE ORDER ---
                string itemsSummary = string.Join(", ",
                    cart.Values.Select(i => $"{i.Name} x{i.Qty}")
                );

                // --- SAVE ONE ROW ONLY ---
                using (SQLiteConnection conn = new SQLiteConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                INSERT INTO Sales (Items, Total, Time)
                VALUES (@items, @total, @time);
            ";

                    SQLiteCommand cmd = new SQLiteCommand(query, conn);
                    cmd.Parameters.AddWithValue("@items", itemsSummary);
                    cmd.Parameters.AddWithValue("@total", total);
                    cmd.Parameters.AddWithValue("@time", transactionTime);
                    cmd.ExecuteNonQuery();
                }

                // Refresh dashboard
                LoadSalesHistoryFromDatabase();
                LoadTotalSales();
                LoadBestSellingItems();

                // Receipt
                StringBuilder receipt = new StringBuilder();
                receipt.AppendLine("*************************************************");
                receipt.AppendLine("************** HOME MADE SPICE **************");
                receipt.AppendLine("*************************************************");
                receipt.AppendLine(" ");

                foreach (var item in cart.Values)
                    receipt.AppendLine($"{item.Name}\t{item.Qty}\t₱{item.Price:0.00}\t₱{item.Total:0.00}");

                decimal change = cashPaid - total;

                receipt.AppendLine("*************************************************");
                receipt.AppendLine($"Total:\t\t₱{total:0.00}");
                receipt.AppendLine($"Cash:\t\t₱{cashPaid:0.00}");
                receipt.AppendLine($"Change:\t\t₱{change:0.00}");
                receipt.AppendLine("*************************************************");
                receipt.AppendLine("***************** THANK YOU! ******************");
                receipt.AppendLine("*************************************************");
                receipt.AppendLine("");

                MessageBox.Show(receipt.ToString(), "Receipt");

                // Reset cart
                cart.Clear();
                txtCash.Clear();
                lblChange.Text = "₱0.00";
                RefreshOrderPanel();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Checkout Error:\n" + ex.Message);
            }
        }






        private List<SaleLineItem> rawSalesLog = new List<SaleLineItem>();

        public class SaleLineItem
        {
            public int ID { get; set; }        // Transaction ID
            public string Item { get; set; }
            public int Quantity { get; set; }
            public DateTime Time { get; set; }
            public decimal Total { get; set; }
        }

        public class TransactionSummary
        {
            public int ID { get; set; }         // Transaction ID
            public string Items { get; set; }   // Summarized item list
            public DateTime Time { get; set; }  // Completion Time
            public decimal Total { get; set; }  // Transaction Total
        }
        

        private void btnTuna_Click(object sender, EventArgs e)
        {
            AddToCart("Tuna Pasta", 60m);
        }

        private void btnSaltedEgg_Click(object sender, EventArgs e)
        {
            AddToCart("Salted Egg Pasta", 60m);
        }

        private void btnBakedMac_Click(object sender, EventArgs e)
        {
            AddToCart("Baked Mac", 75m);
        }

        private void btnChkenMushroom_Click(object sender, EventArgs e)
        {
            AddToCart("Chicken and Mushroom Alfredo", 75m);
        }

        private void btnCarbonaraBacon_Click(object sender, EventArgs e)
        {
            AddToCart("Carbonara with Bacon bits", 75m);
        }

        private void btnChikenPesto_Click(object sender, EventArgs e)
        {
            AddToCart("Chicken Penne Pesto", 85m);
        }

        private void btnOreo_Click(object sender, EventArgs e)
        {
            AddToCart("Oreo", 130m);
        }

        private void btnJavachip_Click(object sender, EventArgs e)
        {
            AddToCart("Java Chip", 130m);
        }

        private void btnDarkChocolate_Click(object sender, EventArgs e)
        {
            AddToCart("Dark Chocolate", 130m);
        }

        private void btnStrawberry_Click(object sender, EventArgs e)
        {
            AddToCart("Strawberry", 130m);
        }

        private void btnCoffeeJelly_Click(object sender, EventArgs e)
        {
            AddToCart("Coffee Jelly", 130m);
        }

        private void btnCookiesCream_Click(object sender, EventArgs e)
        {
            AddToCart("Cookies & Cream", 145m);
        }

        private void btnLatte_Click(object sender, EventArgs e)
        {
            AddToCart("Latte", 75m);
        }

        private void btnMacchiato_Click(object sender, EventArgs e)
        {
            AddToCart("Macchiato", 75m);
        }

        private void btnVanila_Click(object sender, EventArgs e)
        {
            AddToCart("Vanilla", 75m);
        }

        private void btnMocha_Click(object sender, EventArgs e)
        {
            AddToCart("Mocha", 75m);
        }

        private void btnCancelOrder_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Are you sure you want to cancel the entire order?",
                "Cancel Order",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.No)
                return;

            cart.Clear();

            panelCurrentOrder.Controls.Clear();

            lblTotal.Text = "₱0.00";
            lblChange.Text = "₱0.00";
            txtCash.Text = "";

  

            MessageBox.Show("Order has been cancelled.", "Order Reset");

            RefreshOrderPanel();
        }
        
        private void CalculateAndDisplayChange()
        {
            // Define variables using 'decimal' for accurate financial calculations
            decimal total = 0.00m;
            decimal cashPaid = 0.00m;
            decimal change = 0.00m;

            // --- STEP 1: Safely Parse the Total Price ---
            // Remove the currency symbol (₱) and try to parse the number.
            // Use NumberStyles.Currency to correctly handle commas, periods, etc.
            string totalText = lblTotal.Text.Replace("₱", "").Trim();

            if (!decimal.TryParse(
                    totalText,
                    System.Globalization.NumberStyles.Currency,
                    System.Globalization.CultureInfo.InvariantCulture, // Use invariant culture for reliable parsing
                    out total))
            {
                // If the Total Label text is NOT a valid number, display the error.
                lblChange.Text = "TOTAL ERROR";
                lblChange.ForeColor = System.Drawing.Color.Red;
                return; // Stop execution
            }

            // --- STEP 2: Safely Parse the Cash Paid ---
            // Only parse the simple text from the input box.
            if (decimal.TryParse(txtCash.Text, out cashPaid))
            {
                // --- STEP 3: Perform Calculation (Only if both inputs are valid numbers) ---
                change = cashPaid - total;

                // --- STEP 4: Format and Display the Change ---

                // Check if cash paid is less than total
                if (change < 0)
                {
                    // Display the amount due as a negative number
                    lblChange.Text = "₱" + change.ToString("N2");
                    lblChange.ForeColor = System.Drawing.Color.Red;
                }
                else
                {
                    // Display the change due back
                    lblChange.Text = "₱" + change.ToString("N2");
                    lblChange.ForeColor = System.Drawing.Color.Green;
                }
            }
            else
            {
                // This handles cases where txtCash is empty, or has non-numeric text (like 'dfsbgf' in your image)
                lblChange.Text = "₱0.00"; // Safe default when input is invalid
                lblChange.ForeColor = System.Drawing.Color.Black;
            }
        }
        
    
        



        private void label25_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void panelCurrentOrder_Paint(object sender, PaintEventArgs e)
        {

        }

        private void lblChange_Click(object sender, EventArgs e)
        {
            
        }

        private void txtCash_TextChanged_1(object sender, EventArgs e)
        {
            CalculateAndDisplayChange();
        }

        private void guna2GradientPanel6_Paint(object sender, PaintEventArgs e)
        {

        }

        private void lblBestSellingDashboard_Click(object sender, EventArgs e)
        {

        }

        private void btnResetDB_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
        "Are you sure you want to delete the entire Sales table? This cannot be undone.",
        "Confirm Reset",
        MessageBoxButtons.YesNo,
        MessageBoxIcon.Warning
    );

            if (result == DialogResult.Yes)
            {
                using (var conn = new SQLiteConnection(connectionString))
                {
                    conn.Open();

                    string query = "DROP TABLE IF EXISTS Sales";
                    SQLiteCommand cmd = new SQLiteCommand(query, conn);
                    cmd.ExecuteNonQuery();
                }

                // IMPORTANT: Recreate the new format table
                InitializeDatabase();

                // Reload empty table
                LoadSalesHistoryFromDatabase();
                LoadTotalSales();
                LoadBestSellingItems();

                MessageBox.Show("Sales table has been reset and recreated.");
            }
        }


        private void dataGridViewSalesHistory_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void txtSearchBar_TextChanged(object sender, EventArgs e)
        {

        }



    }

}
