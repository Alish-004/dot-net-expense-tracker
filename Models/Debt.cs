using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace coursework.Models
{
    public class Debt
    { 
       public int Id { get; set; }
    public int Amount { get; set; }
    public string Description { get; set; }

    public string Source { get; set; }  // Make sure this field is not null or empty

    public DateTime? Date { get; set; }
    public bool Paid { get; set; } = false;
}
}
