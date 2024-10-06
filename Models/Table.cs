using System.ComponentModel.DataAnnotations;

namespace WebAppDBMS.Models
{
    public class Table
    {
        public Table()
        {
            Columns = new List<Column>();
            Rows = new List<Row>();
        }
        public int Id { get; set; }
        [Required(ErrorMessage = "Name should not be empty")]

        public string Name { get; set; }
        public int DatabaseId { get; set; }
        public Database Database { get; set; }
        public virtual ICollection<Column> Columns { get; set; }
        public virtual ICollection<Row> Rows { get; set; }
    }
}
