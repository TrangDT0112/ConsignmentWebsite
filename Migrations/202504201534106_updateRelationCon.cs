namespace ConsignmentWebsite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateRelationCon : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Order_Details", "ConsignmentOrderId", c => c.Int());
            AlterColumn("dbo.Product", "ConsignmentOrderId", c => c.Int());
            CreateIndex("dbo.Product", "ConsignmentOrderId");
            CreateIndex("dbo.Order_Details", "ConsignmentOrderId");
            AddForeignKey("dbo.Order_Details", "ConsignmentOrderId", "dbo.ConsignmentOrder", "Id");
            AddForeignKey("dbo.Product", "ConsignmentOrderId", "dbo.ConsignmentOrder", "Id");
            DropColumn("dbo.Product", "ConsignmentCode");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Product", "ConsignmentCode", c => c.String());
            DropForeignKey("dbo.Product", "ConsignmentOrderId", "dbo.ConsignmentOrder");
            DropForeignKey("dbo.Order_Details", "ConsignmentOrderId", "dbo.ConsignmentOrder");
            DropIndex("dbo.Order_Details", new[] { "ConsignmentOrderId" });
            DropIndex("dbo.Product", new[] { "ConsignmentOrderId" });
            AlterColumn("dbo.Product", "ConsignmentOrderId", c => c.Int(nullable: false));
            DropColumn("dbo.Order_Details", "ConsignmentOrderId");
        }
    }
}
