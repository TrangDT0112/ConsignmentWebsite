using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ConsignmentWebsite.Models.EF
{
    [Table("ConsignmentOrder")]

    public class ConsignmentOrder :CommonAbstract
    {
        public ConsignmentOrder()
        {
            this.Product = new HashSet<Product>();
        }
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required(ErrorMessage = "Consignment code cannot be left blank")]
        public string ConsignmentCode { get; set; }
        [Required(ErrorMessage = "Consignor name cannot be left blank")]
        public string ConsignorName { get; set; }
        [Required(ErrorMessage = "Phone cannot be left blank")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "Address cannot be left blank")]
        public string ReturnAddress { get; set; }
        public string Email { get; set; }
        public decimal TotalAmount { get; set; }
        public int Quantity { get; set; }
        public int PaymentStatus { get; set; }
        public int Status { get; set; }
        public string BankAccount { get; set; }
        public string BankAccountNumber { get; set; }
        public virtual ICollection<Product> Product { get; set; }


    }
}