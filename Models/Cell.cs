using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace WebAppDBMS.Models
{
    public class Cell
    {
        public Cell()
        {
        }
        public int Id { get; set; }
        public string Value { get; set; }
        public int RowId { get; set; }
        public int ColumnID { get; set; }
        public virtual Row Row { get; set; }
        public virtual Column Column { get; set; }
    }
}
