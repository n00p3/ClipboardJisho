using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
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
        private bool isMouseOverWindow = false;
        private string lastClipboardContent;

        public MainWindow()
        {
            InitializeComponent();

            lastClipboardContent = System.Windows.Clipboard.GetText();

            db = new DBAdapter();

            var mpara = new NMeCab.MeCabParam();
            tagger = NMeCab.MeCabTagger.Create(mpara);
            ClipboardMonitor();

            ClipboardNotification.ClipboardUpdate += ClipboardChanged;

            Left = SettingsManager.WindowPosition.X;
            Top = SettingsManager.WindowPosition.Y;
            Width = SettingsManager.WindowSize.Width;
            Height = SettingsManager.WindowSize.Height;


            Task.Run(() =>
            {
                while (true)
                {
                    Thread.Sleep(50);

                    Dispatcher.Invoke(() =>
                    {
                        try
                        {

                            if (lastClipboardContent != System.Windows.Clipboard.GetText())
                            {
                                lastClipboardContent = System.Windows.Clipboard.GetText();
                                ClipboardMonitor();
                            }
                        }
                        catch (Exception ex)
                        {
                            //System.Windows.MessageBox.Show($"Error reading clipboard ({ex.Message})");
                        }
                    });

                    if (isMouseOverWindow)
                        continue;

                    Dispatcher.Invoke(() =>
                    {
                        if (!SettingsManager.MonitorMousePosition)
                            return;

                        var maxH = MyScrollViewer.ScrollableHeight;
                        var moveTo = maxH * MousePositionPercentage();
                        MyScrollViewer.ScrollToVerticalOffset(moveTo);
                    });

                }

            });
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
            var index = 0;
            var wordsList = new List<Tuple<int, CardControl>>();

            while (node != null)
            {
                if (node.CharType > 0)
                {
                    var baseForm = "";
                    try
                    {
                        baseForm = node.Feature.Split(',')[6];
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        continue;
                    }

                    var shouldFilter = SettingsManager.BufferedFilter.Any(regex => Regex.IsMatch(baseForm, regex));
                    if (!shouldFilter)
                    {
                        db.FindDefinition(baseForm, index).ContinueWith(result =>
                        {
                            //var alreadyExists = (from CardControl child in MainGrid.Children
                            //                     where child.Kanji == result.Result.Item2.Japanese
                            //                     select child.Kanji).Count() > 0;

                            //if (alreadyExists)
                            //return;

                            var temp = new CardControl(result.Result.Item2, this);
                            if (result.Result.Item2.Glossary.Count > 0 && result.Result.Item2.Japanese != null)
                                wordsList.Add(new Tuple<int, CardControl>(result.Result.Item1, temp));
                            //MainGrid.Children.Add(temp);


                            MainGrid.Children.Clear();
                            var sorted = from row in wordsList
                                         orderby row.Item1
                                         select row;

                            foreach (var row in sorted)
                            {
                                Console.WriteLine(row.Item1 + " " + row.Item2.LabelTategaki.Text);
                                MainGrid.Children.Add(row.Item2);
                            }
                        }, TaskScheduler.FromCurrentSynchronizationContext());
                    }
                    else
                    {
                        Console.WriteLine($"Filtering word {baseForm}");
                    }

                    index++;
                }


                node = node.Next;
            }


        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            (new SettingsWindow()).ShowDialog();
            Topmost = SettingsManager.AlwaysOnTop;
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

        static Point GetMousePositionWindowsForms()
        {
            System.Drawing.Point point = System.Windows.Forms.Control.MousePosition;
            return new Point(point.X, point.Y);
        }

        /// <summary>
        /// Mouse position (Y) in percentages.
        /// </summary>
        /// <returns> Double in range 0..1 </returns>
        static double MousePositionPercentage()
        {
            var pos = GetMousePositionWindowsForms();
            var maxH = Screen.PrimaryScreen.Bounds.Height;
            return pos.Y / (maxH - 1);
        }

        private void Window_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            isMouseOverWindow = true;
        }

        private void Window_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            isMouseOverWindow = false;
        }
    }
}
