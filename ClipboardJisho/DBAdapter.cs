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
                using (var command = new SQLiteCommand("SELECT * FROM entry WHERE kanji = @word LIMIT 3", connection))
                {
                    command.Parameters.Add(new SQLiteParameter("@word", word));
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
                connection.Close();
            }
        }
    }
