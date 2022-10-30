using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTracker.Models
{
    public class Category
    {
        [Key]
        public int categoryid { get; set; }

        [Column(TypeName="nvarchar(50)")]
        public string title { get; set; }

        [Column(TypeName = "nvarchar(5)")]
        public string icon { get; set; } = "";

        [Column(TypeName = "nvarchar(10)")]
        public string type { get; set; } = "Expense";

        [NotMapped]
        public string? titlewithicon
        {
            get
            {
                return this.icon + " " + this.title;
            }
        }
    }
}
