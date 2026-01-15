using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIAP.FCG.CATALOG.Core.Inputs
{
    public class PaymentProcessedDto
    {

        public int orderId { get; set; }
        public int paymentId  { get; set; }
        public decimal amount { get; set; }
        public int status { get; set; }

        public string? reason { get; set; }

        /*
        public int OrderId { get; set; }
        public string PaymentStatus { get; set; }*/
    }
}
