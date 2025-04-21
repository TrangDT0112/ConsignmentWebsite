namespace ConsignmentWebsite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateOrder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductImage", "Order_Id", c => c.Int());
            CreateIndex("dbo.ProductImage", "Order_Id");
            AddForeignKey("dbo.ProductImage", "Order_Id", "dbo.Order", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProductImage", "Order_Id", "dbo.Order");
            DropIndex("dbo.ProductImage", new[] { "Order_Id" });
            DropColumn("dbo.ProductImage", "Order_Id");
        }
    }
}
