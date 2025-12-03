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
        string jazyk;

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
            if (comboBox1.SelectedItem == null || string.IsNullOrEmpty(comboBox1.Text))
            {
                MessageBox.Show("Vyberte prosím jazyk");
                richTextBox1.Text = string.Empty;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            if (comboBox1.SelectedIndex == 0)
            {
                jazyk = "C#";
                zvyraznovac.VyberJazyku(jazyk);
            }
            else if (comboBox1.SelectedIndex == 1)
            {
                jazyk = "PHP";
                zvyraznovac.VyberJazyku(jazyk);
            }
            else if (comboBox1.SelectedIndex == 2)
            {
                jazyk = "JavaScript";
                zvyraznovac.VyberJazyku(jazyk);
            }
        }
    }
}
