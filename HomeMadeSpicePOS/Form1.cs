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

    }
}
