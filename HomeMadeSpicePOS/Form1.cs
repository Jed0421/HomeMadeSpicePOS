using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HomeMadeSpicePOS
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            
        }
     

        private void Form1_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.Sizable;

            // Tiyakin na walang maximum limit
            this.MaximumSize = new System.Drawing.Size(5000, 5000);

            // Palakihin ang taas ng form sa 2500 pixels (I-override ang lahat)
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

        private void btnEspresso_Click(object sender, EventArgs e)//cappucino
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
                row.Height = 40;
                row.Left = 10;
                row.Top = y;
                row.BackColor = Color.FromArgb(50, 50, 50);
                row.BorderStyle = BorderStyle.FixedSingle;

                Label lblQty = new Label();
                lblQty.Text = item.Qty.ToString();
                lblQty.Width = 40;
                lblQty.Left = 10;
                lblQty.Top = 10;
                lblQty.ForeColor = Color.White;

                Label lblName = new Label();
                lblName.Text = item.Name;
                lblName.Width = 180;
                lblName.Left = 60;
                lblName.Top = 10;
                lblName.ForeColor = Color.White;

                Label lblPrice = new Label();
                lblPrice.Text = "₱" + item.Total.ToString("0.00");
                lblPrice.Width = 80;
                lblPrice.Left = 250;
                lblPrice.Top = 10;
                lblPrice.ForeColor = Color.White;

                Button btnPlus = new Button();
                btnPlus.Text = "+";
                btnPlus.Width = 30;
                btnPlus.Left = 340;
                btnPlus.Top = 5;
                btnPlus.Tag = item.Name;
                btnPlus.Click += BtnPlus_Click;

                Button btnMinus = new Button();
                btnMinus.Text = "-";
                btnMinus.Width = 30;
                btnMinus.Left = 375;
                btnMinus.Top = 5;
                btnMinus.Tag = item.Name;
                btnMinus.Click += BtnMinus_Click;

                row.Controls.Add(lblQty);
                row.Controls.Add(lblName);
                row.Controls.Add(lblPrice);
                row.Controls.Add(btnPlus);
                row.Controls.Add(btnMinus);

                panelCurrentOrder.Controls.Add(row);

                y += 45;
            }
            UpdateTotalsPanel();
        }
        private void UpdateTotalsPanel()
        {
            decimal total = CalculateTotal();
            lblTotal.Text = "₱" + total.ToString("0.00");

            if (decimal.TryParse(txtCash.Text, out decimal cash))
            {
                decimal change = cash - total;
                lblChange.Text = "₱" + change.ToString("0.00");
            }
            else
            {
                lblChange.Text = "₱0.00";
            }
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


        private void guna2Button14_Click(object sender, EventArgs e)//Caramel
        {
            AddToCart("Caramel", 75m);
        }

        private void txtCash_TextChanged(object sender, EventArgs e)
        {
           UpdateTotalsPanel();
        }

        private void btnCheckOut_Click(object sender, EventArgs e)
        {

            if (cart.Count == 0)
            {
                MessageBox.Show("Your cart is empty!", "Warning");
                return;
            }

            int idCounter = 1;
            string timeNow = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            // Add to DataGridView
            foreach (var item in cart.Values)
            {
                dataGridView1.Rows.Add(
                    idCounter,
                    item.Name,
                    item.Qty,
                    timeNow,
                    "₱" + item.Total.ToString("0.00")
                );

                idCounter++;
            }

            // Build Receipt Text
            StringBuilder receipt = new StringBuilder();
            receipt.AppendLine("        HOME MADE SPICE");
            receipt.AppendLine("--------------------------------------");
            receipt.AppendLine("Item               Qty        Total");
            receipt.AppendLine("--------------------------------------");

            foreach (var item in cart.Values)
            {
                receipt.AppendLine(
                    $"{item.Name.PadRight(15)} {item.Qty.ToString().PadRight(5)} ₱{item.Total.ToString("0.00")}"
                );
            }

            receipt.AppendLine("--------------------------------------");
            receipt.AppendLine($"TOTAL:                    {lblTotal.Text}");
            receipt.AppendLine($"CASH:                     ₱{txtCash.Text}");
            receipt.AppendLine($"CHANGE:                   {lblChange.Text}");
            receipt.AppendLine("--------------------------------------");
            receipt.AppendLine("     Thank you for your purchase!");

            // Show Receipt
            MessageBox.Show(receipt.ToString(), "Receipt", MessageBoxButtons.OK);

            // Clear cart after Checkout
            cart.Clear();
            txtCash.Clear();
            lblChange.Text = "₱0.00";
            RefreshOrderPanel();



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
            // Ask first (optional)
            DialogResult result = MessageBox.Show(
                "Are you sure you want to cancel the entire order?",
                "Cancel Order",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.No)
                return;

            // Clear the cart dictionary
            cart.Clear();

            // Clear the order panel UI
            panelCurrentOrder.Controls.Clear();

            // Reset payment fields
            lblTotal.Text = "₱0.00";
            lblChange.Text = "₱0.00";
            txtCash.Text = "";

            // OPTIONAL: clear the DataGridView (uncomment if you want)
            // dataGridView1.Rows.Clear();

            MessageBox.Show("Order has been cancelled.", "Order Reset");

            // Refresh UI
            RefreshOrderPanel();
        }




    }

}
