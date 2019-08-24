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

            db = new DBAdapter();
          
            var mpara = new NMeCab.MeCabParam();
            tagger = NMeCab.MeCabTagger.Create(mpara);

            ClipboardMonitor();

            ClipboardNotification.ClipboardUpdate += ClipboardChanged;

            Left = SettingsManager.WindowPosition.X;
            Top = SettingsManager.WindowPosition.Y;
            Width = SettingsManager.WindowSize.Width;
            Height = SettingsManager.WindowSize.Height;

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

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Ignore on init.
            if (e.PreviousSize == new Size(0, 0))
                return;

            SettingsManager.WindowSize = e.NewSize;

        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            SettingsManager.WindowPosition = new System.Drawing.Point(Convert.ToInt32(Left), Convert.ToInt32(Top));
        }
    }
}
