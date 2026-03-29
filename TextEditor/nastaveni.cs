using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class nastaveni : Form
    {
        public Color barva_cislo { get; private set; }
        public Color barva_typy { get; private set; }
        public Color barva_slova { get; private set; }
        public Color barva_operatory { get; private set; }
        public Color barva_retezce { get; private set; }
        public Font vybranyFont { get; private set; }

        public nastaveni(TextEditor.ZvyraznovacSyntaxe.Barvy barvy, Font aktualniFont)
        {
            InitializeComponent();

            if (barvy != null)
            {
                /*Zabarvení tlačítka podle vybrané barvy*/
                button1.BackColor = barvy.cisla;
                button2.BackColor = barvy.typyC;
                button3.BackColor = barvy.slovaC;
                button4.BackColor = barvy.operatory;
                button7.BackColor = barvy.retezce;

                /*Uložení barev do proměnných*/
                barva_cislo = barvy.cisla;
                barva_typy = barvy.typyC;
                barva_slova = barvy.slovaC;
                barva_operatory = barvy.operatory;
                barva_retezce = barvy.retezce;
            }

            /*Výpis fontu při zobrazení nastavení*/
            vybranyFont = aktualniFont;
            label4.Text = aktualniFont.Name + Environment.NewLine + aktualniFont.Style + Environment.NewLine + aktualniFont.Size;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            /*Nastavení barvy pro čísla*/
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                button1.BackColor = colorDialog1.Color;
                this.barva_cislo = colorDialog1.Color;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            /*Nastavení barvy pro datové typy*/
            if (colorDialog2.ShowDialog() == DialogResult.OK)
            { 
                button2.BackColor = colorDialog2.Color;
                this.barva_typy = colorDialog2.Color;
            }  
        }

        private void button3_Click(object sender, EventArgs e)
        {
            /*Nastavení barvy pro klíčová slova*/
            if (colorDialog3.ShowDialog() == DialogResult.OK)
            { 
                button3.BackColor = colorDialog3.Color;
                this.barva_slova = colorDialog3.Color;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            /*Nastavení barvy pro operátory*/
            if(colorDialog4.ShowDialog() == DialogResult.OK)
            {
                button4.BackColor = colorDialog4.Color;
                this.barva_operatory = colorDialog4.Color;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            /*Výběr a výpis fontu*/
            if(fontDialog1.ShowDialog() == DialogResult.OK)
            {
                vybranyFont = fontDialog1.Font;

                label4.Text = vybranyFont.Name + Environment.NewLine + vybranyFont.Style + Environment.NewLine + vybranyFont.Size;
            }
        }

        private void nastaveni_Load(object sender, EventArgs e)
        {
            /*Výpis fontu*/
            label4.Text = vybranyFont.Name + Environment.NewLine + vybranyFont.Style + Environment.NewLine + vybranyFont.Size;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            /*Zavření nastavení*/
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            /*Nastavení barvy pro řetězce*/
            if (colorDialog5.ShowDialog() == DialogResult.OK)
            {
                button7.BackColor = colorDialog5.Color;
                this.barva_retezce = colorDialog5.Color;
            }
        }
    }
}
