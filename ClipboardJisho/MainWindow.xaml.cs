using MahApps.Metro.Controls;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        NMeCab.MeCabTagger tagger;
        DBAdapter db;
        public int test = 100;

        public MainWindow()
        {
            InitializeComponent();

            //for (var i = 0; i < 2; i++)
            //{
            //    MainGrid.Children.Add(new CardControl("漢字", new List<string> { "kanji", "nananana"}, this));
            //}

            //var t = new CardControl("漢字", new List<string> { "kanji", "nanang sdgsdgdsgsdgdsg sdgdsgsdgsdgdsgsd gsdgsdgdsgsdgdsgds gsdgdsgdsgsdg dsgdana", "cscsc", "cscscs" }, this);
            //MainGrid.Children.Add(t);
            //MainGrid.Children.Add(new CardControl("漢字漢字漢字", new List<string> { "kanji" }, this));
            //MainGrid.Children.Add(new CardControl("漢字", new List<string> { "kanji", "nananana" }, this));

            db = new DBAdapter();
            //db.FindDefinition("漢字");

            //var mpara = new NMeCab.MeCabParam();
            //var tagger = NMeCab.MeCabTagger.Create(mpara);
            //var node = tagger.ParseToNode("今日はいい天気ですね");

            //while (node != null)
            //{
            //    if (node.CharType > 0)
            //        Console.WriteLine($"{node.Surface}, {node.Feature}");

            //    node = node.Next;
            //}
            var mpara = new NMeCab.MeCabParam();
            tagger = NMeCab.MeCabTagger.Create(mpara);

            ClipboardMonitor();

            //var notifier = new ClipboardNotification();
            ClipboardNotification.ClipboardUpdate += ClipboardChanged;

        }

        void ClipboardChanged(object sender, EventArgs e)
        {
            ClipboardMonitor();
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void CardControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        void ClipboardMonitor()
        {

            var node = tagger.ParseToNode(ClipboardNotification.Content);

            MainGrid.Children.Clear();

            while (node != null)
            {
                if (node.CharType > 0)
                {

                    db.FindDefinition(node.Surface).ContinueWith(word =>
                    {

                        var alreadyExists = (from CardControl child in MainGrid.Children
                                             where child.Kanji == word.Result.Japanese
                                             select child.Kanji).Count() > 0;

                        if (alreadyExists)
                            return;

                        var temp = new CardControl(word.Result, this);
                        if (word.Result.Glossary.Count > 0 && word.Result.Japanese != null)
                            MainGrid.Children.Add(temp);


                    }, TaskScheduler.FromCurrentSynchronizationContext());

                }


                node = node.Next;
            }


        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            (new SettingsWindow()).ShowDialog();
        }
    }
}
