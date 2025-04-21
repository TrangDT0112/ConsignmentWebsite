namespace ConsignmentWebsite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateProductCategory : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ProductCategory", "Alias", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ProductCategory", "Alias", c => c.String(nullable: false, maxLength: 150));
        }
    }
}
