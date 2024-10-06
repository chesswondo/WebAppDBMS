using System.ComponentModel.DataAnnotations;

namespace WebAppDBMS.Models
{
    public class Database
    {
        public Database()
        {
            Tables = new List<Table>();
        }
        public int Id { get; set; }
        [Required(ErrorMessage = "Name should not be empty")]

        public string Name { get; set; }
        public virtual ICollection<Table> Tables { get; set; }
    }
}
