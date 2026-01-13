using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIAP.FCG.CATALOG.Core.Inputs
{
    public class PaymentProcessesdDto
    {
        public bool Approved { get; set; }
        public int UserId { get; set; }
        public int GameId { get; set; }
        public decimal Price { get; set; }
    }
}
