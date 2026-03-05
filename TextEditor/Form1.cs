using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using WindowsFormsApp1;

namespace TextEditor
{
    public partial class Form1 : Form
    {
        ZvyraznovacSyntaxe zvyraznovac;
        string jazyk;
        string aktualniSoubor = null;
        private bool nacitamSoubor = false;
        

        public Form1()
        {
            InitializeComponent();

            zvyraznovac = new ZvyraznovacSyntaxe(richTextBox1);

            zvyraznovac.NacistJson("../../seznam.json");

            richTextBox1.TextChanged += RichTextBox1_TextChanged;

            comboBox1.SelectedIndex = 0;

            richTextBox1.Font = WindowsFormsApp1.Properties.Settings.Default.Font ?? new Font("Consolas", 10);
            richTextBox1.ForeColor = Color.Black;
            richTextBox1.Rtf = richTextBox1.Text;

            /*Nastavení fontu*/
            if (WindowsFormsApp1.Properties.Settings.Default.Font != null)
            {
                richTextBox1.Font = WindowsFormsApp1.Properties.Settings.Default.Font;
            }

            /*Nastavení barev*/
            zvyraznovac.NastavBarvy(
                WindowsFormsApp1.Properties.Settings.Default.BarvaCisla,
                WindowsFormsApp1.Properties.Settings.Default.BarvaTypy,
                WindowsFormsApp1.Properties.Settings.Default.BarvaSlova,
                WindowsFormsApp1.Properties.Settings.Default.BarvaOperatory
                );

            zvyraznovac.ZvyraznitCelyText();
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
            if (comboBox1.SelectedIndex == -1)
                return;

            if (comboBox1.SelectedIndex == 0)
                zvyraznovac.VyberJazyku("C#");
            else if (comboBox1.SelectedIndex == 1)
                zvyraznovac.VyberJazyku("PHP");
            else if (comboBox1.SelectedIndex == 2)
                zvyraznovac.VyberJazyku("JavaScript");

            zvyraznovac.ZvyraznitCelyText();
        }

        private void OtevritSoubor()
        {
            if (comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Nejprve vyberte jazyk");
                return;
            }

            using (OpenFileDialog otevrit = new OpenFileDialog())
            {
                otevrit.Filter = "Textové soubory|*.txt|C#|*.cs|PHP|*.php|JavaScript|*.js|Všechny soubory|*.*";
                otevrit.Title = "Otevřít soubor";

                if (otevrit.ShowDialog() == DialogResult.OK)
                {
                    nacitamSoubor = true;

                    aktualniSoubor = otevrit.FileName;
                    richTextBox1.Text = File.ReadAllText(aktualniSoubor);

                    nacitamSoubor = false;

                    zvyraznovac.ZvyraznitCelyText();
                }
            }
        }

        private void UlozitSoubor()
        {
            if (string.IsNullOrWhiteSpace(richTextBox1.Text))
            {
                MessageBox.Show("Není co uložit");
                return;
            }

            if (!string.IsNullOrEmpty(aktualniSoubor))
            {
                File.WriteAllText(aktualniSoubor, richTextBox1.Text);
                MessageBox.Show("Soubor byl uložen");
                return;
            }

            using (SaveFileDialog ulozit = new SaveFileDialog())
            {
                ulozit.Filter = "Textové soubory|*.txt|C#|*.cs|PHP|*.php|JavaScript|*.js|Všechny soubory|*.*";
                ulozit.Title = "Uložit soubor";

                if (ulozit.ShowDialog() == DialogResult.OK)
                {
                    aktualniSoubor = ulozit.FileName;
                    File.WriteAllText(aktualniSoubor, richTextBox1.Text);
                    MessageBox.Show("Soubor byl uložen");
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OtevritSoubor();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            UlozitSoubor();
        }

        private void richTextBox1_TextChanged_1(object sender, EventArgs e)
        {
            if (nacitamSoubor)
                return;

            if (comboBox1.SelectedItem == null)
                return;

            zvyraznovac.ZvyraznitText();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var aktualniBarvy = zvyraznovac.ZiskejBarvy();
            nastaveni nastaveni = new nastaveni(aktualniBarvy, richTextBox1.Font);

            if (nastaveni.ShowDialog() == DialogResult.OK)
            {
                richTextBox1.Font = nastaveni.vybranyFont;

                zvyraznovac.NastavBarvy(
                    nastaveni.barva_cislo,
                    nastaveni.barva_typy,
                    nastaveni.barva_slova,
                    nastaveni.barva_operatory
                );

                zvyraznovac.ZvyraznitCelyText();
            }

            /*Uložení barev a fontu*/
            WindowsFormsApp1.Properties.Settings.Default.BarvaCisla = nastaveni.barva_cislo;
            WindowsFormsApp1.Properties.Settings.Default.BarvaTypy = nastaveni.barva_typy;
            WindowsFormsApp1.Properties.Settings.Default.BarvaSlova = nastaveni.barva_slova;
            WindowsFormsApp1.Properties.Settings.Default.BarvaOperatory = nastaveni.barva_operatory;

            WindowsFormsApp1.Properties.Settings.Default.Font = nastaveni.vybranyFont;

            WindowsFormsApp1.Properties.Settings.Default.Save();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            zvyraznovac.ZvyraznitCelyText();
        }
    }
}
