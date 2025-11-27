using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Windows.Forms;

namespace TextEditor
{
    public class ZvyraznovacSyntaxe
    {
        private RichTextBox textbox;
        private NastaveniSyntaxe nastaveni;

        public ZvyraznovacSyntaxe(RichTextBox textbox)
        {
            this.textbox = textbox;
        }

        public class NastaveniSyntaxe
        {
            public List<string> slova { get; set; }
            public List<string> operatory { get; set; }
            public List<string> cisla { get; set; }
            public Barvy barvy { get; set; }
        }

        public class Barvy
        {
            public string slova { get; set; }
            public string operatory { get; set; }
            public string cisla { get; set; }
        }

        public void NacistJson(string soubor)
        {
            if (!File.Exists(soubor))
                return;

            string obsah = File.ReadAllText(soubor);
            nastaveni = JsonConvert.DeserializeObject<NastaveniSyntaxe>(obsah);

            if (nastaveni.barvy == null)
            {
                nastaveni.barvy = new Barvy
                {
                    slova = "Yellow",
                    operatory = "Red",
                    cisla = "Blue"
                };
            }
        }

        public void ZvyraznitText()
        {
            if (nastaveni == null)
            {
                return;
            }

            int start = textbox.SelectionStart;
            int delka = textbox.SelectionLength;

            textbox.SuspendLayout();
            textbox.SelectAll();
            textbox.SelectionColor = Color.Black;

            if (nastaveni.barvy != null)
            {
                if (nastaveni.slova != null)
                    ZvyraznitSlova(nastaveni.slova, Color.FromName(nastaveni.barvy.slova));
            }

            textbox.SelectionStart = start;
            textbox.SelectionLength = delka;
            textbox.ResumeLayout();
        }

        private void ZvyraznitSlova(List<string> slova, Color barva)
        {
            foreach (var slovo in slova)
            {
                var nalezeno = Regex.Matches(textbox.Text, $@"\b{slovo}\b");
                foreach (Match m in nalezeno)
                {
                    textbox.Select(m.Index, m.Length);
                    textbox.SelectionColor = barva;
                }
            }
        }
    }
}