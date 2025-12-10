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

        private bool blokovat = false;
        private string aktualniJazyk = null;
        private string predchoziText = "";

        public ZvyraznovacSyntaxe(RichTextBox textbox)
        {
            this.textbox = textbox;
        }

        public class NastaveniSyntaxe
        {
            public List<string> slovaC { get; set; }
            public List<string> typyC { get; set; }
            public List<string> operatory { get; set; }
            public List<string> cisla { get; set; }
            public Barvy barvy { get; set; }
            public Dictionary<string, List<string>> jazyk { get; set; }
        }

        public class Barvy
        {
            public string slovaC { get; set; }
            public string typyC { get; set; }
            public string operatory { get; set; }
            public string cisla { get; set; }
        }

        public void NacistJson(string soubor)
        {
            if (!File.Exists(soubor))
            {
                return;
            }

            string obsah = File.ReadAllText(soubor);
            nastaveni = JsonConvert.DeserializeObject<NastaveniSyntaxe>(obsah);

            if (nastaveni.barvy == null)
            {
                nastaveni.barvy = new Barvy
                {
                    slovaC = "Purple",
                    typyC = "Red",
                    operatory = "Orange",
                    cisla = "Blue"
                };
            }
        }
        public void ZvyraznitText()
        {
            if (nastaveni == null || aktualniJazyk == null)
            {
                return;
            }

            if (blokovat)
            {
                return;
            }
            blokovat = true;

            if (!nastaveni.jazyk.ContainsKey(aktualniJazyk))
            {
                blokovat = false;
                return;
            }

            var povolene = nastaveni.jazyk[aktualniJazyk];

            int start = textbox.SelectionStart;
            int delka = textbox.SelectionLength;

            textbox.SuspendLayout();

            int radek = textbox.GetLineFromCharIndex(textbox.SelectionStart);

            if (radek >= textbox.Lines.Length)
            {
                radek = textbox.Lines.Length - 1;
            }
                
            if (radek < 0)
            {
                blokovat = false;
                return;
            }

            int startIndex = textbox.GetFirstCharIndexFromLine(radek);
            int delkaRadku = textbox.Lines[radek].Length;
            int konecIndex = startIndex + delkaRadku;

            textbox.Select(startIndex, delkaRadku);
            textbox.SelectionColor = Color.Black;

            if (povolene.Contains("slovaC"))
            {
                Zvyraznit(nastaveni.slovaC, Color.FromName(nastaveni.barvy.slovaC), startIndex, konecIndex);
            }
                
            if (povolene.Contains("typyC"))
            {
                Zvyraznit(nastaveni.typyC, Color.FromName(nastaveni.barvy.typyC), startIndex, konecIndex);
            }
                
            if (povolene.Contains("operatory"))
            {
                ZvyraznitSymbol(nastaveni.operatory, Color.FromName(nastaveni.barvy.operatory), startIndex, konecIndex);
            }
                
            if (povolene.Contains("cisla"))
            {
                Zvyraznit(nastaveni.cisla, Color.FromName(nastaveni.barvy.cisla), startIndex, konecIndex);
            }
                
            textbox.SelectionStart = start;
            textbox.SelectionLength = delka;

            textbox.ResumeLayout();
            blokovat = false;
        }

        private void Zvyraznit(List<string> seznam, Color barva, int startIndex, int konecIndex)
        {
            foreach (var polozka in seznam)
            {
                var nalezeno = Regex.Matches(textbox.Text, $@"\b{polozka}\b");
                foreach (Match m in nalezeno)
                {
                    if (m.Index < startIndex || m.Index > konecIndex)
                    {
                        continue;
                    }

                    textbox.Select(m.Index, m.Length);
                    textbox.SelectionColor = barva;
                }
            }
        }

        private void ZvyraznitSymbol(List<string> seznam, Color barva, int startIndex, int konecIndex)
        {
            
            foreach (var symbol in seznam)
            {
                var nalezeno = Regex.Matches(textbox.Text, Regex.Escape(symbol));
                foreach (Match m in nalezeno)
                {
                    if (m.Index < startIndex || m.Index > konecIndex)
                    {
                        continue;
                    }
                        
                    textbox.Select(m.Index, m.Length);
                    textbox.SelectionColor = barva;
                }
            }
        }

        public void VyberJazyku(string jazyk)
        {
            if (jazyk == null || nastaveni == null || nastaveni.jazyk == null)
            {
                return;
            }

            if (!nastaveni.jazyk.ContainsKey(jazyk))
            {
                return;
            }

            aktualniJazyk = jazyk;
            ZvyraznitText();
        }
    }

}
