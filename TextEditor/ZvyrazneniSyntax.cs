using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace TextEditor
{
    public class ZvyraznovacSyntaxe
    {
        private RichTextBox textbox;
        private NastaveniSyntaxe nastaveni;

        private bool blokovat = false;
        private string aktualniJazyk = null;

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, bool wParam, IntPtr lParam);

        private const int WM_SETREDRAW = 0x000B;

        private void BeginUpdate()
        {
            SendMessage(textbox.Handle, WM_SETREDRAW, false, IntPtr.Zero);
        }

        private void EndUpdate()
        {
            SendMessage(textbox.Handle, WM_SETREDRAW, true, IntPtr.Zero);
            textbox.Invalidate();
        }

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
                return;

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
            if (nastaveni == null || aktualniJazyk == null || blokovat)
                return;

            if (!nastaveni.jazyk.ContainsKey(aktualniJazyk))
                return;

            blokovat = true;

            int poziceKurzor = textbox.SelectionStart;
            int cisloRadku = textbox.GetLineFromCharIndex(poziceKurzor);

            if (cisloRadku < 0 || cisloRadku >= textbox.Lines.Length)
            {
                blokovat = false;
                return;
            }

            int zacatekRadku = textbox.GetFirstCharIndexFromLine(cisloRadku);
            string textRadku = textbox.Lines[cisloRadku];

            BeginUpdate();

            textbox.Select(zacatekRadku, textRadku.Length);
            textbox.SelectionColor = Color.Black;

            var povoleneKategorie = nastaveni.jazyk[aktualniJazyk];

            if (povoleneKategorie.Contains("slovaC") && nastaveni.slovaC?.Count > 0)
                ZvyraznitRegexem(textRadku, zacatekRadku, $@"\b({string.Join("|", nastaveni.slovaC)})\b", nastaveni.barvy.slovaC);

            if (povoleneKategorie.Contains("typyC") && nastaveni.typyC?.Count > 0)
                ZvyraznitRegexem(textRadku,zacatekRadku, $@"\b({string.Join("|", nastaveni.typyC)})\b", nastaveni.barvy.typyC);

            if (povoleneKategorie.Contains("operatory") && nastaveni.operatory?.Count > 0)
                ZvyraznitRegexem(textRadku,zacatekRadku, $"({string.Join("|", nastaveni.operatory.Select(Regex.Escape))})", nastaveni.barvy.operatory);

            if (povoleneKategorie.Contains("cisla"))
                ZvyraznitRegexem(textRadku, zacatekRadku, @"\b(-?(0b[01]+|0x[\da-fA-F]+|\d+(\.\d+)?([eE]-?\d+)?))\b", nastaveni.barvy.cisla);

            textbox.SelectionStart = poziceKurzor;
            textbox.SelectionLength = 0;

            EndUpdate();
            blokovat = false;
        }

        public void ZvyraznitCelyText()
        {
            if (nastaveni == null || aktualniJazyk == null)
                return;

            blokovat = true;
            BeginUpdate();

            int puvodniPozice = textbox.SelectionStart;

            textbox.SelectAll();
            textbox.SelectionColor = Color.Black;

            var povolene = nastaveni.jazyk[aktualniJazyk];

            if (povolene.Contains("slovaC"))
                Zvyraznit(nastaveni.slovaC, Color.FromName(nastaveni.barvy.slovaC));

            if (povolene.Contains("typyC"))
                Zvyraznit(nastaveni.typyC, Color.FromName(nastaveni.barvy.typyC));

            if (povolene.Contains("operatory"))
                ZvyraznitSymbol(nastaveni.operatory, Color.FromName(nastaveni.barvy.operatory));

            if (povolene.Contains("cisla"))
                ZvyraznitCisla(Color.FromName(nastaveni.barvy.cisla));

            textbox.SelectionStart = puvodniPozice;
            textbox.SelectionLength = 0;

            EndUpdate();
            blokovat = false;
        }

        private void ZvyraznitRegexem(string textRadku, int posun, string vzor, string nazevBarvy)
        {
            Color barva = Color.FromName(nazevBarvy);

            foreach (Match shoda in Regex.Matches(textRadku, vzor))
            {
                textbox.Select(posun + shoda.Index, shoda.Length);
                textbox.SelectionColor = barva;
            }
        }

        private void Zvyraznit(List<string> seznam, Color barva)
        {
            if (seznam == null)
                return;

            foreach (var slovo in seznam)
            {
                foreach (Match m in Regex.Matches(textbox.Text, $@"\b{Regex.Escape(slovo)}\b"))
                {
                    textbox.Select(m.Index, m.Length);
                    textbox.SelectionColor = barva;
                }
            }
        }

        private void ZvyraznitSymbol(List<string> seznam, Color barva)
        {
            if (seznam == null)
                return;

            foreach (var symbol in seznam)
            {
                foreach (Match m in Regex.Matches(textbox.Text, Regex.Escape(symbol)))
                {
                    textbox.Select(m.Index, m.Length);
                    textbox.SelectionColor = barva;
                }
            }
        }

        private void ZvyraznitCisla(Color barva)
        {
            foreach (Match m in Regex.Matches(textbox.Text, @"\b(-?(0b[01]+|0x[\da-fA-F]+|\d+(\.\d+)?([eE]-?\d+)?))\b"))
            {
                textbox.Select(m.Index, m.Length);
                textbox.SelectionColor = barva;
            }
        }

        public void VyberJazyku(string jazyk)
        {
            if (jazyk == null || nastaveni?.jazyk == null)
                return;

            if (!nastaveni.jazyk.ContainsKey(jazyk))
                return;

            aktualniJazyk = jazyk;
            ZvyraznitText();
        }
    }
}