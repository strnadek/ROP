using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TextEditor
{
    public partial class Form1 : Form
    {
        ZvyraznovacSyntaxe zvyraznovac;

        public Form1()
        {
            InitializeComponent();

            zvyraznovac = new ZvyraznovacSyntaxe(richTextBox1);

            zvyraznovac.NacistJson("../../seznam.json");

            richTextBox1.TextChanged += RichTextBox1_TextChanged;
        }

        private void RichTextBox1_TextChanged(object sender, EventArgs e)
        {

            zvyraznovac.ZvyraznitText();
        }
    }
}
