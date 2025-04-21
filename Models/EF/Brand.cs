namespace ConsignmentWebsite.Models.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Web.Mvc;

    [Table("Brand")]
    public class Brand : CommonAbstract
    {
        public Brand()
        {
            this.Products = new HashSet<Product>();
        }
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [StringLength(250)]
        public string BrandName { get; set; }
        public string Description { get; set; }

        [StringLength(250)]
        public string Image { get; set; }
        public bool IsPrefer { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}
