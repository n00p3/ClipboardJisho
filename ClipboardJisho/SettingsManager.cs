using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClipboardJisho
{
    static class SettingsManager
    {
        private static Font _japaneseFont;
        private static Font _englishFont;
        private static bool? _alwaysOnTop;
        private static bool? _monitorMousePosition;
        public static Font JapaneseFont
        {
            get { return _japaneseFont ?? InitJapaneseFont(); }
            set { _japaneseFont = value; SetJapaneseFont(value);  }
        }
        public static Font EnglishFont
        {
            get { return _englishFont ?? InitEnglishFont(); }
            set { _englishFont = value; SetEnglishFont(value); }
        }

        public static bool AlwaysOnTop
        {
            get { return _alwaysOnTop ?? InitAlwaysOnTop(); }
            set { _alwaysOnTop = value; SetAlwaysOnTop(value); }
        }
        public static bool MonitorMousePosition
        {
            get { return _monitorMousePosition ?? InitMonitorMousePosition(); }
            set { _monitorMousePosition = value; SetMonitorMousePosition(value); }
        }


        static bool InitAlwaysOnTop()
        {
            RegistryKey writableKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\ClipboardJisho");

            var alwaysOnTop = (int?)writableKey.GetValue("AlwaysOnTop");
            if (alwaysOnTop == null)
            {
                writableKey.SetValue("AlwaysOnTop", 0, RegistryValueKind.DWord);
                alwaysOnTop = 0;
            }

            var ret = alwaysOnTop != 0;
            AlwaysOnTop = ret;
            writableKey.Close();

            return ret;
        }

        static bool InitMonitorMousePosition()
        {
            RegistryKey writableKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\ClipboardJisho");

            var monitorMousePosition = (int?)writableKey.GetValue("MonitorMousePosition");
            if (monitorMousePosition == null)
            {
                writableKey.SetValue("MonitorMousePosition", 0, RegistryValueKind.DWord);
                monitorMousePosition = 0;
            }

            var ret = monitorMousePosition != 0;
            MonitorMousePosition = ret;
            writableKey.Close();

            return ret;
        }

        static void SetAlwaysOnTop(bool onOff)
        {
            RegistryKey writableKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\ClipboardJisho");

            writableKey.SetValue("AlwaysOnTop", (onOff ? 1 : 0), RegistryValueKind.DWord);
            var alwaysOnTop = (onOff ? 1 : 0);

            var ret = alwaysOnTop != 0;
            _alwaysOnTop = ret;
            writableKey.Close();
        }

        static void SetJapaneseFont(Font font)
        {
            RegistryKey writableKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\ClipboardJisho");

            writableKey.SetValue("JapaneseFont", font.Name, RegistryValueKind.String);
            writableKey.SetValue("JapaneseFontSize", font.Size, RegistryValueKind.String);

            _japaneseFont = new Font(font.Name, font.Size);
            writableKey.Close();
        }

        static void SetEnglishFont(Font font)
        {
            RegistryKey writableKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\ClipboardJisho");

            writableKey.SetValue("EnglishFont", font.Name, RegistryValueKind.String);
            writableKey.SetValue("EnglishFontSize", font.Size, RegistryValueKind.String);

            _englishFont = new Font(font.Name, font.Size);
            writableKey.Close();
        }

        static void SetMonitorMousePosition(bool onOff)
        {
            RegistryKey writableKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\ClipboardJisho");

            writableKey.SetValue("MonitorMousePosition", (onOff ? 1 : 0), RegistryValueKind.DWord);
            var monitorMousePosition = (onOff ? 1 : 0);

            var ret = monitorMousePosition != 0;
            _monitorMousePosition = ret;
            writableKey.Close();
        }

        static Font InitJapaneseFont()
        {
            RegistryKey writableKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\ClipboardJisho");

            var japaneseFont = (string)writableKey.GetValue("JapaneseFont");
            if (japaneseFont == null)
            {
                writableKey.SetValue("JapaneseFont", "Meiryo", RegistryValueKind.String);
                japaneseFont = "Meiryo";
            }

            var japaneseFontSize = float.TryParse((string)writableKey.GetValue("JapaneseFontSize"), out var o1) ? (float?)o1 : null;
            if (japaneseFontSize != null)
            {
                writableKey.SetValue("JapaneseFontSize", "16", RegistryValueKind.String);
                japaneseFontSize = 16;
            }

            var font = new Font(japaneseFont, japaneseFontSize ?? 16);

            JapaneseFont = font;
            writableKey.Close();

            return font;
        }

        static Font InitEnglishFont()
        {
            RegistryKey writableKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\ClipboardJisho");

            var englishFont = (string)writableKey.GetValue("EnglishFont");
            if (englishFont == null)
            {
                writableKey.SetValue("EnglishFont", "Segoe UI", RegistryValueKind.String);
                englishFont = "Segoe UI";
            }

            var englishFontSize = float.TryParse((string)writableKey.GetValue("EnglishFontSize"), out var o1) ? (float?)o1 : null;
            if (englishFontSize != null)
            {
                writableKey.SetValue("EnglishFontSize", "16", RegistryValueKind.String);
                englishFontSize = 16;
            }

            var font = new Font(englishFont, englishFontSize ?? 16);

            EnglishFont = font;
            writableKey.Close();

            return font;
        }
    }
}
