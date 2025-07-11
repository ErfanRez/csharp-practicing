using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomainLayer
{
    public class OrderAddress
    {
        public int Id { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public int OrderId { get; set; }

        public virtual Order Order { get; set; }
    }
}