using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTracker.Models
{
    public class Transaction
    {
        [Key]
        public int transactionid { get; set; }

        [Range(1,int.MaxValue,ErrorMessage ="Please select category")]
        public int categoryid { get; set; }
        public Category? category { get; set; } /*no column for this*/

        [Range(1,int.MaxValue,ErrorMessage ="Amount should be greater then 0")]
        public int amount { get; set; }

        [Column(TypeName = "nvarchar(75)")]
        public string? note { get; set; }

        public DateTime date { get; set; }=DateTime.Now;

        [NotMapped]
        public string categorytitlewithicon
        {
            get
            {
                return category==null?"":category.icon+" "+category.title;
            }
        }
        [NotMapped]
        public string? formattedamount
        {
            get
            {
                return ((category == null || category.type == "expense") ? "-" : "+" )+ amount.ToString("C0");
            }
        }
    }
}
