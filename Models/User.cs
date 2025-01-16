using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace coursework.Models
{
    public class User
    {

       public int ID { get; set; }
       public String UserName { get; set; }

      public  String Password { get; set; }

      public  String Address { get; set; }

      public  String ContactNumber { get; set; }

       public List<Transaction> Transactions { get; set; }

       public int AvailableBalance { get; set; }

        public List<string> Tags { get; set; }

        public List<Debt> Debts { get; set; }

    }
}
