using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DomainLayer
{
    public class Product
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string ImageName { get; set; }
        public string Description { get; set; }
        public List<string> Tags { get; set; }

        public virtual List<UserProduct> UserProducts { get; set; }
    }
}