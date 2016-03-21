using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCenter.Common.Portable;
using UCenter.CouchBase.Attributes;
using UCenter.CouchBase.Entities;

namespace GF.UCenter.CouchBase.Entities
{
    [DocumentType("Order")]
    public class OrderEntity : BaseEntity<OrderEntity>
    {
        public string AppId { get; set; }
        public string AccountId { get; set; }
        public string OrderNo { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public string OrderData { get; set; }
    }
}
