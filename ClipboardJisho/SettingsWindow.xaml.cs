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
using System.Windows.Shapes;

namespace ClipboardJisho
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            JapaneseFontName.Content = SettingsManager.JapaneseFont.Name + ", " + SettingsManager.JapaneseFont.Size;
            EnglishFontName.Content = SettingsManager.EnglishFont.Name + ", " + SettingsManager.EnglishFont.Size;
            AlwaysOnTopCheck.IsChecked = SettingsManager.AlwaysOnTop;
            MonitorMouseCheck.IsChecked = SettingsManager.MonitorMousePosition;
            JpMaxLen.Value = SettingsManager.JpMaxLen;
            EngMaxLen.Value = SettingsManager.EngMaxLen;
        }

        private void JapaneseFontButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FontDialog();
            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.Cancel)
            {
                SettingsManager.JapaneseFont = new System.Drawing.Font(dialog.Font.Name, dialog.Font.Size);
            }

            JapaneseFontName.Content = SettingsManager.JapaneseFont.Name + ", " + SettingsManager.JapaneseFont.Size;

        }

        private void AlwaysOnTopCheck_Checked(object sender, RoutedEventArgs e)
        {
            SettingsManager.AlwaysOnTop = true;
        }

        private void AlwaysOnTopCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            SettingsManager.AlwaysOnTop = false;
        }

        private void MonitorMouseCheck_Checked(object sender, RoutedEventArgs e)
        {
            SettingsManager.MonitorMousePosition = true;
        }

        private void MonitorMouseCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            SettingsManager.MonitorMousePosition = false;
        }

        private void EnglishFontButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FontDialog();
            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.Cancel)
            {
                SettingsManager.EnglishFont = new System.Drawing.Font(dialog.Font.Name, dialog.Font.Size);
            }

            EnglishFontName.Content = SettingsManager.EnglishFont.Name + ", " + SettingsManager.EnglishFont.Size;
        }

        private void JpMaxLen_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (e.OldValue == 0)
                return;

            SettingsManager.JpMaxLen = Convert.ToInt32(e.NewValue);
        }

        private void EngMaxLen_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (e.OldValue == 0)
                return;

            SettingsManager.EngMaxLen = Convert.ToInt32(e.NewValue);
        }
    }
}
