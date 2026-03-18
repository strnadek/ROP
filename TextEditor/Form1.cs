using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp1;

namespace TextEditor
{
    public partial class Form1 : Form
    {
        ZvyraznovacSyntaxe zvyraznovac;
        string aktualniSoubor = null;
        private bool nacitamSoubor = false;

        public Form1()
        {
            InitializeComponent();

            zvyraznovac = new ZvyraznovacSyntaxe(richTextBox1);

            /*Načtení seznamu*/
            zvyraznovac.NacistJson("../../seznam.json");

            /*Událost při změne v textboxu*/
            richTextBox1.TextChanged += RichTextBox1_TextChanged;

            comboBox1.SelectedIndex = 0;

            /*Defaultní nastavení fontu a barvy textu*/
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

            if (WindowsFormsApp1.Properties.Settings.Default.SvetlyRezim)
            {
                zvyraznovac.ZvyraznitCelyText(Color.Black);
            }
            else if (WindowsFormsApp1.Properties.Settings.Default.TmavyRezim)
            {
                zvyraznovac.ZvyraznitCelyText(Color.WhiteSmoke);
            }
        }

        private void RichTextBox1_TextChanged(object sender, EventArgs e)
        {
            if (WindowsFormsApp1.Properties.Settings.Default.SvetlyRezim)
            {
                zvyraznovac.ZvyraznitText(Color.Black);
            }
            else if (WindowsFormsApp1.Properties.Settings.Default.TmavyRezim)
            {
                zvyraznovac.ZvyraznitText(Color.WhiteSmoke);
            }
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

            if (WindowsFormsApp1.Properties.Settings.Default.SvetlyRezim)
            {
                zvyraznovac.ZvyraznitCelyText(Color.Black);
            }
            else if (WindowsFormsApp1.Properties.Settings.Default.TmavyRezim)
            {
                zvyraznovac.ZvyraznitCelyText(Color.WhiteSmoke);
            }
        }

        private void OtevritSoubor()
        {
            /*Kontrola jestli je vybraný jazyk*/
            if (comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Nejprve vyberte jazyk");
                return;
            }

            /*Otevření dialogu, kde si uživatel vybírá soubor, který chce editovat v aplikaci*/
            using (OpenFileDialog otevrit = new OpenFileDialog())
            {
                otevrit.Filter = "Textové soubory|*.txt|C#|*.cs|PHP|*.php|JavaScript|*.js";
                otevrit.Title = "Otevřít soubor";

                if (otevrit.ShowDialog() == DialogResult.OK)
                {
                    nacitamSoubor = true;

                    aktualniSoubor = otevrit.FileName;
                    richTextBox1.Text = File.ReadAllText(aktualniSoubor);

                    nacitamSoubor = false;

                    if (WindowsFormsApp1.Properties.Settings.Default.SvetlyRezim)
                    {
                        zvyraznovac.ZvyraznitCelyText(Color.Black);
                    }
                    else if (WindowsFormsApp1.Properties.Settings.Default.TmavyRezim)
                    {
                        zvyraznovac.ZvyraznitCelyText(Color.WhiteSmoke);
                    }
                }
            }
        }

        private void UlozitSoubor()
        {
            /*Nic se neukládá v případě, že textbox je prádzný*/
            if (string.IsNullOrWhiteSpace(richTextBox1.Text))
            {
                MessageBox.Show("Není co uložit");
                return;
            }

            /*Uložení souboru*/
            if (File.Exists(aktualniSoubor))
            {
                if (!string.IsNullOrEmpty(aktualniSoubor))
                {
                    if (File.Exists(aktualniSoubor))
                    {
                        File.WriteAllText(aktualniSoubor, richTextBox1.Text);
                        MessageBox.Show("Soubor byl uložen");
                        return;
                    }
                }
            }
            else
            {
                using (SaveFileDialog ulozit = new SaveFileDialog())
                {
                    ulozit.Filter = "Textové soubory|*.txt|C#|*.cs|PHP|*.php|JavaScript|*.js";
                    ulozit.Title = "Uložit soubor";

                    if (ulozit.ShowDialog() == DialogResult.OK)
                    {
                        aktualniSoubor = ulozit.FileName;
                        File.WriteAllText(aktualniSoubor, richTextBox1.Text);
                        MessageBox.Show("Soubor byl uložen");
                    }
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

            if (WindowsFormsApp1.Properties.Settings.Default.SvetlyRezim)
            {
                zvyraznovac.ZvyraznitText(Color.Black);
            }
            else if (WindowsFormsApp1.Properties.Settings.Default.TmavyRezim)
            {
                zvyraznovac.ZvyraznitText(Color.WhiteSmoke);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var aktualniBarvy = zvyraznovac.ZiskejBarvy();
            nastaveni nastaveni = new nastaveni(aktualniBarvy, richTextBox1.Font);

            /*Zobrazení nastavení*/
            if (nastaveni.ShowDialog() == DialogResult.OK)
            {
                richTextBox1.Font = nastaveni.vybranyFont;

                zvyraznovac.NastavBarvy(
                    nastaveni.barva_cislo,
                    nastaveni.barva_typy,
                    nastaveni.barva_slova,
                    nastaveni.barva_operatory
                );

                if (WindowsFormsApp1.Properties.Settings.Default.SvetlyRezim)
                {
                    zvyraznovac.ZvyraznitCelyText(Color.Black);
                }
                else if (WindowsFormsApp1.Properties.Settings.Default.TmavyRezim)
                {
                    zvyraznovac.ZvyraznitCelyText(Color.WhiteSmoke);
                }
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
            if (WindowsFormsApp1.Properties.Settings.Default.SvetlyRezim)
            {
                button4.Text = "🌙";

                richTextBox1.BackColor = Color.White;

                zvyraznovac.ZvyraznitCelyText(Color.Black);
            }
            else if (WindowsFormsApp1.Properties.Settings.Default.TmavyRezim)
            {
                button4.Text = "☀️";

                richTextBox1.BackColor = Color.FromArgb(30, 30, 30);

                zvyraznovac.ZvyraznitCelyText(Color.WhiteSmoke);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            /*Tmavý a světlý režim*/
            if(button4.Text == "🌙")
            {
                WindowsFormsApp1.Properties.Settings.Default.SvetlyRezim = true;
                WindowsFormsApp1.Properties.Settings.Default.TmavyRezim = false;

                WindowsFormsApp1.Properties.Settings.Default.Save();
            }
            else if(button4.Text == "☀️")
            {
                WindowsFormsApp1.Properties.Settings.Default.TmavyRezim = true;
                WindowsFormsApp1.Properties.Settings.Default.SvetlyRezim = false;

                WindowsFormsApp1.Properties.Settings.Default.Save();
            }

            if (!WindowsFormsApp1.Properties.Settings.Default.TmavyRezim)
            {
                WindowsFormsApp1.Properties.Settings.Default.TmavyRezim = true;
                WindowsFormsApp1.Properties.Settings.Default.SvetlyRezim = false;

                WindowsFormsApp1.Properties.Settings.Default.Save();

                button4.Text = "☀️";

                richTextBox1.BackColor = Color.FromArgb(30, 30, 30);

                zvyraznovac.ZvyraznitCelyText(Color.WhiteSmoke);
            }
            else if (!WindowsFormsApp1.Properties.Settings.Default.SvetlyRezim)
            {
                WindowsFormsApp1.Properties.Settings.Default.SvetlyRezim = true;
                WindowsFormsApp1.Properties.Settings.Default.TmavyRezim = false;

                WindowsFormsApp1.Properties.Settings.Default.Save();

                button4.Text = "🌙";

                richTextBox1.BackColor = Color.White;

                zvyraznovac.ZvyraznitCelyText(Color.Black);
            }
        }
    }
}
