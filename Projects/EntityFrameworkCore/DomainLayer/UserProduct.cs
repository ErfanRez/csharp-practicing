using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace DomainLayer
{
    public class UserProduct
    {
        private Product _product;
        private ILazyLoader _lazyLoader;

        public UserProduct()
        {

        }
        public UserProduct(ILazyLoader lazyLoader)
        {
            _lazyLoader = lazyLoader;
        }
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int SellerId { get; set; }
        public int Price { get; set; }

        public string Color { get; set; }


        public Product Product
        {
            get
                => _lazyLoader.Load(this, ref _product);
            set
            =>
                _product = value;
        }

        public User User { get; set; }

        public List<OrderItem> OrderItems { get; set; }
    }
}