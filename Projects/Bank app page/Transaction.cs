using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class Transaction
    {
        public string Sender { get; set; }
        public string Amount { get; set; }
        public string Date { get; set; }
        public string Type { get; set; }

        public Transaction(string amount, string type, string date, string sender)
        {
            Amount = amount;
            Type = type;
            Date = date;
            Sender = sender;
        }
    }
}
