using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DomainLayer
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Family { get; set; }
        public string Email { get; set; }
        public DateTime CreationDate { get; set; }
        public string FullName => $"{Name} {Family}";

        public virtual List<UserProduct> Products { get; set; }
        public virtual List<Order> Orders { get; set; }

    }
}