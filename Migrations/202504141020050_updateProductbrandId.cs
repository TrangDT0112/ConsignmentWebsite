namespace ConsignmentWebsite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateProductbrandId : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Product", "BrandId", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Product", "BrandId", c => c.Int(nullable: false));
        }
    }
}
