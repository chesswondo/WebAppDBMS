namespace WebAppDBMS.Models
{
    public class TableViewModel
    {
        public Table Table { get; set; }
        public List<Column> FilteredColumns { get; set; }
        public ICollection<Row> Rows { get; set; }
    }
}
