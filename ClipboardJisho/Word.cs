using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClipboardJisho
{
    public class Word
    {
        public string Japanese;
        public List<string> Ruby;
        public List<string> Glossary;

        public Word() { }
        public Word(string japanese)
        {
            Japanese = japanese;
        }

        public Word(List<string> glossary)
        {
            Glossary = glossary;
        }

        public Word(string japanese, List<string> glossary, List<string> ruby)
        {
            Japanese = japanese;
            Glossary = glossary;
            Ruby = ruby;
        }
    }
}
