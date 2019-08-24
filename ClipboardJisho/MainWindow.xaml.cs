﻿using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            Task.Run(() =>
            {
                while (true)
                {
                    Thread.Sleep(50);

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
