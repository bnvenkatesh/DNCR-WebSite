using SD.ACMA.DNCR.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SD.ACMA.POCO.Industry
{
    public class OrderModel
    {
        public int AccountID { get; set; }        
        public int AccountUserID { get; set; }
        public string AdditionalEmailAddress { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedTimeStamp { get; set; }
        public string Description { get; set; }
        public decimal GST { get; set; }
        public List<OrderNoteModel> Notes { get; set; }
        public DateTime OrderExpiryDate { get; set; }
        public int OrderID { get; set; }
        public List<OrderLineModel> OrderLines { get; set; }
        public string OrderNumber { get; set; }
        public SD.ACMA.DNCR.Infrastructure.Enums.OrderStatusEnum OrderStatus { get; set; }
        public List<OrderTransactionModel> OrderTransactions { get; set; }
        public decimal PaymentActivated { get; set; }
        public decimal PaymentOutstanding { get; set; }
        public decimal PaymentReceived { get; set; }
        public decimal PaymentUnallocated { get; set; }
        public List<PaymentModel> Payments { get; set; }

    }
}
