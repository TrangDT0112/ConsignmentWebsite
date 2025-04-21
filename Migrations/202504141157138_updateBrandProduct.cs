namespace ConsignmentWebsite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateBrandProduct : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Product", "BrandId");
            AddForeignKey("dbo.Product", "BrandId", "dbo.Brand", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Product", "BrandId", "dbo.Brand");
            DropIndex("dbo.Product", new[] { "BrandId" });
        }
    }
}
