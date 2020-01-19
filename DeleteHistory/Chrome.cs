using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeleteHistory
{
    class Chrome
    {
        public List<URL> URLs = new List<URL>();
        public List<string> IDs = new List<string>();
        public IEnumerable<URL> GetHistory(string urlToDetele)
        {
            // Get Current Users App Data
            string documentsFolder = Environment.GetFolderPath
            (Environment.SpecialFolder.ApplicationData);
            string[] tempstr = documentsFolder.Split('\\');
            string tempstr1 = "";
            documentsFolder += "\\Google\\Chrome\\User Data\\Default";
            if (tempstr[tempstr.Length - 1] != "Local")
            {
                for (int i = 0; i < tempstr.Length - 1; i++)
                {
                    tempstr1 += tempstr[i] + "\\";
                }
                documentsFolder = tempstr1 + "Local\\Google\\Chrome\\User Data\\Default";
            }


            // Check if directory exists
            if (Directory.Exists(documentsFolder))
            {
                return ExtractUserHistory(documentsFolder, urlToDetele);

            }
            return null;
        }


        IEnumerable<URL> ExtractUserHistory(string folder, string urlToDetele)
        {
            bool isFetched = false;
            DataTable visitsDT = new DataTable();
            // Get User history info
            DataTable historyDT = ExtractFromTable("urls", folder, out isFetched);

            // Get visit Time/Data info
            if (isFetched)
            {
                visitsDT = ExtractFromTable("visits", folder, out isFetched);
            }

            if (isFetched)
            {
                // Loop each history entry
                Console.WriteLine("[INFO] Fetching ID... ");
                foreach (DataRow row in historyDT.Rows)
                {

                    // Obtain URL and Title strings
                    string id = row["id"].ToString();
                    string url = row["url"].ToString();
                    if (url.Contains(urlToDetele))
                    {
                        Console.WriteLine("Url is going to delete: " + id + " - " + url);
                        IDs.Add(id);
                    }
                    string title = row["title"].ToString();

                    // Create new Entry
                    URL u = new URL(url.Replace('\'', ' '), title.Replace('\'', ' '), "Google Chrome");

                    // Add entry to list
                    URLs.Add(u);
                }
                if (IDs.Count > 0)
                {
                    Console.WriteLine("[INFO] Done");
                    // Clear URL History
                    Console.Write("[CONFIRM] Are you sure you want to delete these? y/n: ");
                    string confirm = Console.ReadLine();
                    if (confirm.Equals("y") || confirm.Equals("Y"))
                    {
                        Console.WriteLine("[INFO] Delete urls and vits...");
                        DeleteFromTable("urls", folder, "id");
                        DeleteFromTable("visits", folder, "url");
                        Console.WriteLine("[INFO] Done");
                    }
                }
                else
                {
                    Console.WriteLine("[INFO] We can't find any matching url.");
                }
            }

            return URLs;
        }

        void DeleteFromTable(string table, string folder, string column)
        {
            try
            {
                SQLiteConnection sql_con;
                SQLiteCommand sql_cmd;

                // FireFox database file
                string dbPath = folder + "\\History";

                // If file exists
                if (File.Exists(dbPath))
                {
                    if (IDs.Count > 0)
                    {
                        // Data connection
                        sql_con = new SQLiteConnection("Data Source=" + dbPath + ";Version=3;New=False;Compress=True;");

                        // Open the Conn
                        sql_con.Open();

                        // Delete Query
                        string CommandText = "delete from " + table + " where " + column + " in " + GetIDs();

                        // Create command
                        sql_cmd = new SQLiteCommand(CommandText, sql_con);

                        sql_cmd.ExecuteNonQuery();

                        // Clean up
                        sql_con.Close();
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Close your Google Chrome first and close chrome.exe if it showed in Task Manager");
            }
        }

        string GetIDs()
        {
            string result = "(";
            for (int i = 0; i < IDs.Count; i++)
            {
                if (i == 0)
                {
                    result += IDs[0];
                }
                else
                {
                    result += ", " + IDs[i];
                }
            }
            result += ")";
            return result;
        }

        DataTable ExtractFromTable(string table, string folder, out bool isFetched)
        {
            isFetched = false;
            SQLiteConnection sql_con;
            SQLiteCommand sql_cmd;
            SQLiteDataAdapter DB;
            DataTable DT = new DataTable();

            // FireFox database file
            string dbPath = folder + "\\History";

            Console.WriteLine("[INFO] Fetching " + table + "... ");
            // If file exists
            if (File.Exists(dbPath))
            {
                try
                {
                    // Data connection
                    sql_con = new SQLiteConnection("Data Source=" + dbPath + ";Version=3;New=False;Compress=True;");

                    // Open the Connection
                    sql_con.Open();
                    sql_cmd = sql_con.CreateCommand();

                    // Select Query
                    string CommandText = "select * from " + table;

                    // Populate Data Table
                    DB = new SQLiteDataAdapter(CommandText, sql_con);
                    DB.Fill(DT);

                    // Clean up
                    sql_con.Close();
                    isFetched = true;
                    Console.WriteLine("[INFO] Done ");
                }
                catch (Exception)
                {
                    Console.WriteLine("[ERROR] Close your Google Chrome first and close chrome.exe if it showed in Task Manager");
                }
            }
            return DT;
        }
    }
}
