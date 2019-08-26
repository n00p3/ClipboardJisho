using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
        private static int? _jpMaxLen;
        private static int? _engMaxLen;
        private static bool? _monitorMousePosition;
        private static Point? _windowPosition;
        private static System.Windows.Size? _windowSize;
        private static readonly string _filterFile = "filter.txt";
        private static List<string> _filterBuffor; // To avoid reading file unnecessarily.
        public static string FilterContent
        {
            get
            {
                if (!File.Exists(_filterFile))
                    File.WriteAllText(_filterFile,
@"# Lines starting with `#` are ignored.
# Ignore particles and one letter words:
^.$

^ます$
^ない$
^って$
");

                return File.ReadAllText(_filterFile);
            }
            set
            {
                File.WriteAllText(_filterFile, value);
                _filterBuffor = value
                        .Split('\n')
                        .Where(it => !it.Trim().StartsWith("#") && it.Trim() != "")
                        .Select(it => it.Trim())
                        .ToList();
            }
        }

        public static List<string> BufferedFilter
        {
            get
            {
                if (_filterBuffor == null)
                    _filterBuffor = FilterContent
                        .Split('\n')
                        .Where(it => !it.Trim().StartsWith("#") && it.Trim() != "")
                        .Select(it => it.Trim())
                        .ToList();

                return _filterBuffor;
            }
        }

        public static int JpMaxLen
        {
            get { return _jpMaxLen ?? InitJpMaxLen(); }
            set { _jpMaxLen = value; SetJpMaxLen(value); }
        }
        public static int EngMaxLen
        {
            get { return _engMaxLen ?? InitEngMaxLen(); }
            set { _engMaxLen = value; SetEngMaxLen(value); }
        }

        public static Point WindowPosition
        {
            get { return _windowPosition ?? InitWindowPosition(); }
            set { _windowPosition = value; SetWindowPosition(value); }
        }
        public static System.Windows.Size WindowSize
        {
            get { return _windowSize ?? InitWindowSize(); }
            set { _windowSize = value; SetWindowSize(value); }
        }

        public static Font JapaneseFont
        {
            get { return _japaneseFont ?? InitJapaneseFont(); }
            set { _japaneseFont = value; SetJapaneseFont(value); }
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

        static int InitJpMaxLen()
        {
            RegistryKey writableKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\ClipboardJisho");

            var jpMaxLen = (string)writableKey.GetValue("JpMaxLen");
            if (jpMaxLen == null)
            {
                writableKey.SetValue("JpMaxLen", 20, RegistryValueKind.String);
                jpMaxLen = "20";
            }

            _jpMaxLen = Convert.ToInt32(jpMaxLen);

            writableKey.Close();

            return Convert.ToInt32(jpMaxLen);
        }

        static int InitEngMaxLen()
        {
            RegistryKey writableKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\ClipboardJisho");

            var engMaxLen = (string)writableKey.GetValue("EngMaxLen");
            if (engMaxLen == null)
            {
                writableKey.SetValue("EngMaxLen", 200, RegistryValueKind.String);
                engMaxLen = "200";
            }

            _engMaxLen = Convert.ToInt32(engMaxLen);

            writableKey.Close();

            return Convert.ToInt32(engMaxLen);
        }

        static Point InitWindowPosition()
        {
            RegistryKey writableKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\ClipboardJisho");

            var windowX = (string)writableKey.GetValue("WindowX");
            var windowY = (string)writableKey.GetValue("WindowY");
            if (windowX == null || windowY == null)
            {
                writableKey.SetValue("windowX", 100, RegistryValueKind.String);
                writableKey.SetValue("windowY", 100, RegistryValueKind.String);
                windowX = "100";
                windowY = "100";
            }

            var pos = new Point(Convert.ToInt32(windowX), Convert.ToInt32(windowY));

            WindowPosition = pos;

            writableKey.Close();

            return pos;
        }

        static System.Windows.Size InitWindowSize()
        {
            RegistryKey writableKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\ClipboardJisho");

            var windowW = (string)writableKey.GetValue("WindowW");
            var windowH = (string)writableKey.GetValue("WindowH");
            if (windowW == null || windowH == null)
            {
                writableKey.SetValue("windowW", 300, RegistryValueKind.String);
                writableKey.SetValue("windowH", 800, RegistryValueKind.String);
                windowW = "300";
                windowH = "800";
            }
            var size = new System.Windows.Size(Convert.ToInt32(windowW), Convert.ToInt32(windowH));

            WindowSize = size;

            writableKey.Close();

            return size;
        }

        static void SetWindowPosition(Point point)
        {
            RegistryKey writableKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\ClipboardJisho");

            writableKey.SetValue("WindowX", point.X, RegistryValueKind.String);
            writableKey.SetValue("WindowY", point.Y, RegistryValueKind.String);

            _windowPosition = point;
            writableKey.Close();
        }

        static void SetJpMaxLen(int len)
        {
            RegistryKey writableKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\ClipboardJisho");

            writableKey.SetValue("JpMaxLen", len, RegistryValueKind.String);

            _jpMaxLen = len;
            writableKey.Close();
        }

        static void SetEngMaxLen(int len)
        {
            RegistryKey writableKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\ClipboardJisho");

            writableKey.SetValue("EngMaxLen", len, RegistryValueKind.String);

            _engMaxLen = len;
            writableKey.Close();
        }

        static void SetWindowSize(System.Windows.Size size)
        {
            RegistryKey writableKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\ClipboardJisho");

            writableKey.SetValue("WindowW", size.Width, RegistryValueKind.String);
            writableKey.SetValue("WindowH", size.Height, RegistryValueKind.String);

            _windowSize = size;
            writableKey.Close();
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
