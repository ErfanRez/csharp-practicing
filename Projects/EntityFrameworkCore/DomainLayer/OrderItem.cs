using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomainLayer
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int UserProductId { get; set; }
        public int Price { get; set; }
        public int Count { get; set; }
        public string Color { get; set; }



        public virtual Order Order { get; set; }
        public virtual UserProduct UserProduct { get; set; }
    }
}