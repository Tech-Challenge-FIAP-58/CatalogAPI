using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIAP.FCG.CATALOG.Core.Inputs
{
    public class PaymentProcessedDto
    {
        public int OrderId { get; set; }
        public string PaymentStatus { get; set; }
    }
}
