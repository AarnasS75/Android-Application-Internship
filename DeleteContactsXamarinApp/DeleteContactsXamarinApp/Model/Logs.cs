using SQLite;

namespace HGB.Model
{
    public class Logs
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string deviceno { get; set; }
        public string phoneno { get; set; }
        public string direction { get; set; }
        public string dt { get; set; }
        public string action { get; set; }
    }
}