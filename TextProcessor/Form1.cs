using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using System.Text.RegularExpressions;



namespace TextProcessor
{
    public partial class Form1 : Form
    {
        private SqlConnection sqlConnection;
       
        public Form1()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterParent;
        }

        private void ckdToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["wordAutocomplete"].ConnectionString);
          
        }

        private void fastColoredTextBox1_TextChanged(object sender, FastColoredTextBoxNS.TextChangedEventArgs e)
        {
             this.sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["wordAutocomplete"].ConnectionString);
             this.sqlConnection.Open();
             
            var linecomand = $"SELECT * FROM [Words] ORDER BY quanity DESC";

            SqlCommand command = new SqlCommand(linecomand, this.sqlConnection);
            SqlDataReader reader = command.ExecuteReader();

            
            List<string> word = new List<string>();

            int i=0;
            while ((reader.Read()) && (i != 5)){
                word.Add(reader.GetString(1));
                i++;
            }

            autocompleteMenu1.Items = word.ToArray();

            reader.Close();
            this.sqlConnection.Close();
           
        }

        private void ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Создать словарь

            this.sqlConnection.Open();
            SqlCommand createdir = new SqlCommand("DROP TABLE [Words]  CREATE TABLE [Words] (Id INT NOT NULL PRIMARY KEY IDENTITY(1,1), word nvarchar(50) not null, quanity int  null  )", sqlConnection);
            createdir.ExecuteNonQuery();
            this.sqlConnection.Close();
            
        }

        private void ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //Обновление словаря

            this.sqlConnection.Open();
            string word = fastColoredTextBox1.Text;
            word = Regex.Replace(word, "[!\"#$%&'()*+,-./:;<=>?@\\[\\]^_`{|}~]", string.Empty);
            string[] words = word.Split(new char[] {' '});

            for (int i = 0; i < words.Length; i++)
            {
                if(words[i].Length >= 3)
                {
                    int quanityWord = 0;
                    foreach (string s in words)
                    {
                        if (s == words[i]) quanityWord++;
                    }

                    var linecomand = $"SELECT word  FROM Words where word=N'{words[i]}'";
                    SqlCommand command = new SqlCommand(linecomand, this.sqlConnection);
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows) 
                    {
                        reader.Close();

                        var linecomand2 = $"SELECT word,quanity  FROM Words where word=N'{words[i]}' AND [quanity]>=3 AND quanity + '{quanityWord}' AS summa>=3";
                        SqlCommand command2 = new SqlCommand(linecomand, this.sqlConnection);
                        SqlDataReader reader2 = command.ExecuteReader();
                        if (reader2.HasRows)
                        {
                            reader2.Close();
                            SqlCommand up = new SqlCommand($"UPDATE Words SET quanity=quanity +N'{quanityWord}' WHERE word=N'{words[i]}'", sqlConnection);
                            up.ExecuteNonQuery();
                            
                        }
                    }
                    else if (quanityWord >= 3)
                    {
                        reader.Close();
                         SqlCommand ins = new SqlCommand($"INSERT INTO [Words] (word,quanity) VALUES(N'{words[i]}', N'{ (int)Math.Sqrt( quanityWord)}')", sqlConnection);
                         ins.ExecuteNonQuery();
                        
                    }
                }
            }
            this.sqlConnection.Close();
            
        }

        private void ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            //Очистить словарь

            this.sqlConnection.Open();
            SqlCommand delWords = new SqlCommand("DELETE FROM [Words]", sqlConnection);
            delWords.ExecuteNonQuery();
            this.sqlConnection.Close();
            
        }

    }

}
