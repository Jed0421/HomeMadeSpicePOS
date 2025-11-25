using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
            this.MaximumSize = new System.Drawing.Size(this.Width, 5000);
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

        private void btnEspresso_Click(object sender, EventArgs e)
        {
            AddToCart("Espresso", 10m);
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

        private void guna2Button14_Click(object sender, EventArgs e)
        {
            AddToCart("Caramel", 20m);
        }
    }

}
