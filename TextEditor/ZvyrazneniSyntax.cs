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

        /*BeginUpdate*/
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
            public List<string> slovaPHP { get; set; }
            public List<string> typyPHP { get; set; }
            public List<string> slovaJS { get; set; }
            public List<string> typyJS { get; set; }
            public List<string> operatory { get; set; }
            public Barvy barvy { get; set; }
            public Dictionary<string, List<string>> jazyk { get; set; }
        }

        public class Barvy
        {
            public Color slovaC { get; set; }
            public Color typyC { get; set; }
            public Color slovaPHP { get; set; }
            public Color typyPHP { get; set; }
            public Color slovaJS { get; set; }
            public Color typyJS { get; set; }
            public Color operatory { get; set; }
            public Color cisla { get; set; }
            public Color retezce { get; set; }
        }

        public Barvy ZiskejBarvy()
        {
            /*Získaní uložených barev*/
            return nastaveni?.barvy;
        }

        public void NacistJson(string soubor)
        {
            /*Naèítání seznamu z JSON*/
            if (!File.Exists(soubor))
                return;

            string obsah = File.ReadAllText(soubor);
            nastaveni = JsonConvert.DeserializeObject<NastaveniSyntaxe>(obsah);
        }

        public void ZvyraznitText(Color barva)
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
            textbox.SelectionColor = barva;

            var povoleneKategorie = nastaveni.jazyk[aktualniJazyk];

            if (povoleneKategorie.Contains("slovaC") && nastaveni.slovaC?.Count > 0)
                ZvyraznitRegexem(textRadku, zacatekRadku, $@"\b({string.Join("|", nastaveni.slovaC)})\b", nastaveni.barvy.slovaC);

            if (povoleneKategorie.Contains("typyC") && nastaveni.typyC?.Count > 0)
                ZvyraznitRegexem(textRadku, zacatekRadku, $@"\b({string.Join("|", nastaveni.typyC)})\b", nastaveni.barvy.typyC);

            if(povoleneKategorie.Contains("slovaPHP") && nastaveni.slovaPHP.Count > 0)
                ZvyraznitRegexem(textRadku, zacatekRadku, $@"\b({string.Join("|", nastaveni.slovaPHP)})\b", nastaveni.barvy.slovaPHP);

            if(povoleneKategorie.Contains("typyPHP") && nastaveni.typyPHP.Count > 0) 
                ZvyraznitRegexem(textRadku, zacatekRadku, $@"\b({string.Join("|", nastaveni.typyPHP)})\b", nastaveni.barvy.typyPHP);

            if (povoleneKategorie.Contains("slovaJS") && nastaveni.slovaJS.Count > 0)
                ZvyraznitRegexem(textRadku, zacatekRadku, $@"\b({string.Join("|", nastaveni.slovaJS)})\b", nastaveni.barvy.slovaJS);

            if (povoleneKategorie.Contains("typyJS") && nastaveni.typyJS.Count > 0)
                ZvyraznitRegexem(textRadku, zacatekRadku, $@"\b({string.Join("|", nastaveni.typyJS)})\b", nastaveni.barvy.typyJS);

            if (povoleneKategorie.Contains("operatory") && nastaveni.operatory?.Count > 0)
                ZvyraznitRegexem(textRadku, zacatekRadku, $"({string.Join("|", nastaveni.operatory.Select(Regex.Escape))})", nastaveni.barvy.operatory);

            if (povoleneKategorie.Contains("cisla"))
                ZvyraznitRegexem(textRadku, zacatekRadku, @"\b(-?(0b[01]+|0x[\da-fA-F]+|\d+(\.\d+)?([eE]-?\d+)?))\b", nastaveni.barvy.cisla);

            ZvyraznitRegexem(textRadku, zacatekRadku, "\".*?\"", nastaveni.barvy.retezce);

            textbox.SelectionStart = poziceKurzor;
            textbox.SelectionLength = 0;

            EndUpdate();
            blokovat = false;
        }

        public void ZvyraznitCelyText(Color barva)
        {
            /*Zvýraznit celý text pøi vložení textu nebo otevøení souboru*/
            textbox.SelectAll();
            textbox.SelectionColor = barva;
            textbox.SelectionFont = textbox.Font;

            if (nastaveni == null || aktualniJazyk == null)
                return;

            blokovat = true;
            BeginUpdate();

            int puvodniPozice = textbox.SelectionStart;

            textbox.SelectAll();
            textbox.SelectionColor = barva;

            var povolene = nastaveni.jazyk[aktualniJazyk];

            if (povolene.Contains("slovaC"))
                Zvyraznit(nastaveni.slovaC, nastaveni.barvy.slovaC);

            if (povolene.Contains("typyC"))
                Zvyraznit(nastaveni.typyC, nastaveni.barvy.typyC);

            if (povolene.Contains("slovaPHP"))
                Zvyraznit(nastaveni.slovaPHP, nastaveni.barvy.slovaPHP);

            if (povolene.Contains("typyPHP"))
                Zvyraznit(nastaveni.typyPHP, nastaveni.barvy.typyPHP);

            if (povolene.Contains("slovaJS"))
                Zvyraznit(nastaveni.slovaJS, nastaveni.barvy.slovaJS);

            if (povolene.Contains("typyJS"))
                Zvyraznit(nastaveni.typyJS, nastaveni.barvy.typyJS);

            if (povolene.Contains("operatory"))
                ZvyraznitSymbol(nastaveni.operatory, nastaveni.barvy.operatory);

            if (povolene.Contains("cisla"))
                ZvyraznitCisla(nastaveni.barvy.cisla);

            ZvyraznitRegexem("\".*?\"", nastaveni.barvy.retezce);

            textbox.SelectionStart = puvodniPozice;
            textbox.SelectionLength = 0;

            EndUpdate();
            blokovat = false;
        }

        private void ZvyraznitRegexem(string vzor, Color b)
        {
            foreach (Match m in Regex.Matches(textbox.Text, vzor))
            {
                textbox.Select(m.Index, m.Length);
                textbox.SelectionColor = b;
            }
        }

        private void ZvyraznitRegexem(string textRadku, int posun, string vzor, Color b)
        {
            foreach (Match m in Regex.Matches(textRadku, vzor))
            {
                textbox.Select(posun + m.Index, m.Length);
                textbox.SelectionColor = b;
            }
        }

        private void Zvyraznit(List<string> seznam, Color barva)
        {
            /*Zvýraznìní*/
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
            /*Zvýraznìní symbolù (operátorù)*/
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
            /*Zvýraznìní èísel*/
            foreach (Match m in Regex.Matches(textbox.Text, @"\b(-?(0b[01]+|0x[\da-fA-F]+|\d+(\.\d+)?([eE]-?\d+)?))\b"))
            {
                textbox.Select(m.Index, m.Length);
                textbox.SelectionColor = barva;
            }
        }

        public void VyberJazyku(string jazyk)
        {
            /*Výbìr programovacího jazyku*/
            if (jazyk == null || nastaveni?.jazyk == null)
                return;

            if (!nastaveni.jazyk.ContainsKey(jazyk))
                return;

            aktualniJazyk = jazyk;
        }

        public void NastavBarvy(Color cisla, Color typy, Color slova, Color operatory, Color retezce)
        {
            /*Nastavení barev*/
            if (nastaveni == null)
                return;

            nastaveni.barvy.cisla = cisla;
            nastaveni.barvy.typyC = typy;
            nastaveni.barvy.typyPHP = typy;
            nastaveni.barvy.typyJS = typy;
            nastaveni.barvy.slovaC = slova;
            nastaveni.barvy.slovaPHP = slova;
            nastaveni.barvy.slovaJS = slova;
            nastaveni.barvy.operatory = operatory;
            nastaveni.barvy.retezce = retezce;
        }
    }
}