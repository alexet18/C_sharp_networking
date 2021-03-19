using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientSide
{
    public partial class BidOnOffer : Form
    {
        public BidOnOffer(int currentprice)
        {
            InitializeComponent();
            btn_ok.DialogResult = DialogResult.OK;
            btn_cancel.DialogResult = DialogResult.Cancel;

            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.label1.Text = "Current price: " + currentprice;


           
        }
    }
}
