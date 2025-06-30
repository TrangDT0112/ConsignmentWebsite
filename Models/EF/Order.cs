namespace ConsignmentWebsite.Models.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Order")]
    public class Order : CommonAbstract
    {
        public Order()
        {
            this.Order_Details = new HashSet<Order_Details>();
            this.ProductImages = new HashSet<ProductImage>();
        }
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Code { get; set; }
        [Required(ErrorMessage = "Code cannot be left blank")]
        public string CustomerName { get; set; }
        [Required(ErrorMessage = "Customer name number cannot be left blank")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "Phone cannot be left blank")]
        public string Address { get; set; }
        public string Email { get; set; }
        public decimal TotalAmount { get; set; }
        public int Quantity { get; set; }
        public int TypePayment { get; set; }
        public int Status { get; set; }
        public string CustomerId { get; set; }
        public string PaymentStatus { get; set; }
        public int ShippingStatus { get; set; }

        public virtual ICollection<Order_Details> Order_Details { get; set; }
        public virtual ICollection<ProductImage> ProductImages { get; set; }
    }
}
