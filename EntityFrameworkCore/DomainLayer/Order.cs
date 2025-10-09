using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomainLayer
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime FinallyDate { get; set; }


        public virtual User User { get; set; }
        public virtual List<OrderItem> OrderItems { get; set; }
        public virtual OrderAddress OrderAddress { get; set; }
    }

    public enum OrderStatus
    {
        IsPay,
        Canceled,
        Finally
    }
}