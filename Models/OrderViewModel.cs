using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ConsignmentWebsite.Models
{
    public class OrderViewModel
    {
        [Required(ErrorMessage = "Customer Name cannot be left blank")]
        public string CustomerName { get; set; }
        [Required(ErrorMessage = "Phone number cannot be left blank")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "Address cannot be left blank")]
        public string Address { get; set; }
        public string Email { get; set; }
        public int TypePayment { get; set; }
    }
}