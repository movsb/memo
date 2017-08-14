using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.ComponentModel;

namespace memo
{
    public class MemoObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void Sync(string prop)
        {
            if(PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
                if (prop != "Modified") {
                    Modified = true;
                }
            }
        }

        private string _title       = "";
        private string _content     = "";
        private bool   _modified    = false;

        public long Id {get;set;}

        public bool Modified
        {
            get
            {
                return _modified;
            }
            set
            {
                _modified = value;
                Sync("Modified");
            }
        }

        public string Title
        {
            get
            {
                return _title;
            }

            set
            {
                _title = value;
                Sync("Title");
            }
        }

        public string Content
        {
            get
            {
                return _content;
            }

            set
            {
                _content = value;
                Sync("Content");
            }
        }
    
        public MemoObject()
        {
            Modified = false;
        }
    }

    public class MemoManager
    {
        private SQLiteConnection _conn;

        public MemoManager()
        {
            var builder = new SQLiteConnectionStringBuilder();
            builder.DataSource = AppDomain.CurrentDomain.BaseDirectory + "/memo.dat";
            _conn = new SQLiteConnection(builder.ConnectionString);
            _conn.Open();

            CreateTables();
        }

        private void CreateTables()
        {
            var sql = "CREATE TABLE IF NOT EXISTS memos ("
                + "id INTEGER PRIMARY KEY"
                + ",title VARCHAR(256) NOT NULL DEFAULT ''"
                + ",content TEXT NOT NULL DEFAULT ''"
                + ")"
                ;

            var cmd = new SQLiteCommand(sql, _conn);
            cmd.ExecuteNonQuery();
        }

        public List<MemoObject> GetAll()
        {
            var sql = "SELECT * FROM memos ORDER BY id DESC";
            var cmd = new SQLiteCommand(sql, _conn);
            var row = cmd.ExecuteReader();

            var lst = new List<MemoObject>();

            while (row.Read()) {
                var m = new MemoObject()
                {
                    Id = Convert.ToInt64(row["id"]),
                    Title = (string)row["title"],
                    Content = (string)row["content"],
                };
                lst.Add(m);
            }

            return lst;
        }

        public long Add(MemoObject M)
        {
            var sql = "INSERT INTO memos (title,content) VALUES (@title,@content)";
            var cmd = new SQLiteCommand(sql, _conn);
            cmd.Parameters.AddWithValue("@title", M.Title);
            cmd.Parameters.AddWithValue("@content", M.Content);
            cmd.ExecuteNonQuery();
            return _conn.LastInsertRowId;
        }

        public void Delete(long id)
        {
            var sql = "DELETE FROM memos WHERE id=" + id;
            var cmd = new SQLiteCommand(sql, _conn);
            cmd.ExecuteNonQuery();
        }

        public void Update(MemoObject M)
        {
            var sql = "UPDATE memos SET title=@title,content=@content WHERE id=" + M.Id;
            var cmd = new SQLiteCommand(sql, _conn);
            cmd.Parameters.AddWithValue("@title", M.Title);
            cmd.Parameters.AddWithValue("@content", M.Content);
            cmd.ExecuteNonQuery();
        }
    }
}
