using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClipboardJisho
{
    /// <summary>
    /// Interaction logic for CardControl.xaml
    /// </summary>
    public partial class CardControl : UserControl
    {
        public string Kanji { get; private set; }
        public List<string> Gloss { get; private set; }
        public List<string> Ruby { get; private set; }


        public CardControl(Word word, MainWindow parent)
        {
            InitializeComponent();
            LabelTategaki.FontFamily = new FontFamily(SettingsManager.JapaneseFont.Name);
            LabelTategaki.FontSize = SettingsManager.JapaneseFont.Size;

            LabelRuby.FontFamily = new FontFamily(SettingsManager.JapaneseFont.Name);
            LabelRuby.FontSize = SettingsManager.JapaneseFont.Size;

            LabelGloss.FontFamily = new FontFamily(SettingsManager.EnglishFont.Name);
            LabelGloss.FontSize = SettingsManager.EnglishFont.Size;

            if (word.Glossary?.Count == 0 || word.Glossary == null)
                return;

            Kanji = word.Japanese;
            Gloss = word.Glossary;
            Ruby = word.Ruby;

            LabelTategaki.Text = word.Japanese;
            var tempGloss = "";
            for (var i = 0; i < word.Glossary.Count - 1; i++)
            {
                tempGloss += $"{i + 1}. {word.Glossary[i]}\n";
            }
            tempGloss += $"{word.Glossary.Count}. {Gloss[word.Glossary.Count - 1]}";
            if (tempGloss.Length > SettingsManager.EngMaxLen)
                tempGloss = tempGloss.Substring(0, SettingsManager.EngMaxLen) + "...";

            LabelGloss.Text = tempGloss;

            var rubyStr = String.Join("、", Ruby);
            if (rubyStr.Length > SettingsManager.JpMaxLen)
                rubyStr = rubyStr.Substring(0, SettingsManager.JpMaxLen) + "…";
            LabelRuby.Text = String.Join("、", rubyStr);
        }
    }
}
