using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using XmlRendererWpf.Models;

namespace XmlRendererWpf
{
    public class Database
    {
        public Database(string connection)
        {
            _dbCLient = new SQLiteConnection(connection);
        }

        #region Private
        private static SQLiteConnection _dbCLient { get;set; }
        #endregion

        #region Public
        public void CreateDatabase(string databaseName)
        {
            try
            {
                SQLiteConnection.CreateFile(databaseName);
            }
            catch (Exception e)
            {
                throw(e.InnerException);
            }
        }

        public void CreateTable(string tableName, Dictionary<string, string> columns)
        {
            try
            {
                var sql = new StringBuilder();
                sql.Append($"create table {tableName} (");

                foreach (var column in columns)
                {
                    sql.Append($"{column.Key} {column.Value},");
                }

                sql.Length--;

                sql.Append(")");
                _dbCLient.Open();
                
                var command = new SQLiteCommand(sql.ToString(), _dbCLient);

                command.ExecuteNonQuery();

                _dbCLient.Close();

            }
            catch (Exception e)
            {
                throw(e.InnerException);
            }
        }

        public void Insert(string tableName, List<InsertData> columns)
        {
            try
            {
                var check = $"SELECT name FROM sqlite_master WHERE type='table' AND name='{tableName}'";
                _dbCLient.Open();
                var checkCmd = new SQLiteCommand(check, _dbCLient);
                var tst = checkCmd.ExecuteReader();
                if (!tst.HasRows)
                {
                    _dbCLient.Close();
                    var table = new Dictionary<string, string>();
                    table.Add("Name", "text");
                    table.Add("Value", "text");
                    CreateTable(tableName, table);
                    _dbCLient.Open();
                }
                

                var sql = new StringBuilder();
                var tmpSb = new StringBuilder();

                sql.Append($"insert into {tableName} values(");

                foreach (var column in columns)
                {
                    sql.Append($"'{column.Value}',");
                }

                sql.Length--;
                sql.Append(")");

                var command = new SQLiteCommand(sql.ToString(), _dbCLient);

                command.ExecuteNonQuery();

                _dbCLient.Close();

            }
            catch (Exception e)
            {
                throw (e.InnerException);
            }

        }

        public List<string> SelectItems()
        {
            var sql = "select Name from xml";
            var cmd = new SQLiteCommand(sql, _dbCLient);
            var itemsList = new List<string>();
            _dbCLient.Open();
            var reader = cmd.ExecuteReader();
            

            while (reader.Read())
            {
                itemsList.Add(reader["Name"].ToString());
            }

            _dbCLient.Close();

            return itemsList;

        }

        public String SelectXmlItem(string xmlName)
        {
            var sql = $"select Value from xml where Name=\"{xmlName}\"";
            var cmd = new SQLiteCommand(sql, _dbCLient);
            
            var sb = new StringBuilder();

            _dbCLient.Open();
            var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                sb.Append(reader["Value"].ToString());
            }

            _dbCLient.Close();

            return sb.ToString();
        }
    
        #endregion



    }
}
