namespace WebAppDBMS.Models
{
    public class Row
    {
        public Row()
        {
            Cells = new List<Cell>();
        }
        public int Id { get; set; }
        public int TableId { get; set; }
        public virtual Table Table { get; set; }

        public List<Cell> Cells { get; set; }
    }
}
