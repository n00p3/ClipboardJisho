using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ClipboardJisho
{
    class DBAdapter
    {
        public string tempPath { get; private set; }
        public SQLiteConnection connection { get; private set; }
        public DBAdapter()
        {
            var dict = Properties.Resources.JMdict_eng;

            var tempPath = Path.Combine(Path.GetTempPath(), "ClipboardJisho.sqlite");
            this.tempPath = tempPath;
            File.WriteAllBytes(tempPath, dict);

            try
            {
                connection = new SQLiteConnection($"DataSource={tempPath}");
                connection.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Cannot connect to sqlite. ({ex.Message})");
                Environment.Exit(-1);
            }
        }

        public async Task<Word> FindDefinition(string word)
        {
            return await Task.Run(() =>
            {
                //using (var command = new SQLiteCommand("SELECT * FROM entry WHERE kanji = @word or reading like @like LIMIT 1", connection))
                using (var command = new SQLiteCommand(@"
                    SELECT * FROM entry 
                    WHERE kanji = @word
                        or reading like @before
                        or reading like @after
                        or reading like @middle
                        LIMIT 5
                    ", connection))
                {
                    command.Parameters.Add(new SQLiteParameter("@word", word));
                    command.Parameters.AddWithValue("@middle", "%," + word + ",%");
                    command.Parameters.AddWithValue("@after", "%," + word);
                    command.Parameters.AddWithValue("@before", word + ",%");
                    var dataAdapter = new SQLiteDataAdapter(command);
                    var dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);

                    var ret = new Word
                    {
                        Glossary = dataTable.AsEnumerable().Select(row => row.Field<string>("gloss")).ToList(),
                        Japanese = word,
                        Ruby = dataTable.AsEnumerable().Select(row => row.Field<string>("reading")).ToList()
                    };
                    return ret;
                }
            });
        }

        ~DBAdapter()
        {
            try
            {
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Disposing sqlite connection error: {ex.Message}");
            }
        }
    }
}
