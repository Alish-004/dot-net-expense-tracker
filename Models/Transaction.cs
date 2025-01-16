
namespace coursework.Models
{

    public class Transaction
    {
        public int Id { get; set; }
        public DateTime? Date { get; set; }
        public int Amount { get; set; }
        public string Description { get; set; }
        public string ExpenseTag { get; set; }

        public string TransactionType { get; set; }
    }
}

