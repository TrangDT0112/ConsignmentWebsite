namespace ConsignmentWebsite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateIdConsOrDetail : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Product", "ConsignmentOrderId", "dbo.ConsignmentOrder");
            DropIndex("dbo.Product", new[] { "ConsignmentOrderId" });
            RenameColumn(table: "dbo.Order_Details", name: "ConsignmentOrderId", newName: "ConsignmentOrder_Id");
            RenameIndex(table: "dbo.Order_Details", name: "IX_ConsignmentOrderId", newName: "IX_ConsignmentOrder_Id");
            AlterColumn("dbo.Product", "ConsignmentOrderId", c => c.Int(nullable: false));
            CreateIndex("dbo.Product", "ConsignmentOrderId");
            AddForeignKey("dbo.Product", "ConsignmentOrderId", "dbo.ConsignmentOrder", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Product", "ConsignmentOrderId", "dbo.ConsignmentOrder");
            DropIndex("dbo.Product", new[] { "ConsignmentOrderId" });
            AlterColumn("dbo.Product", "ConsignmentOrderId", c => c.Int());
            RenameIndex(table: "dbo.Order_Details", name: "IX_ConsignmentOrder_Id", newName: "IX_ConsignmentOrderId");
            RenameColumn(table: "dbo.Order_Details", name: "ConsignmentOrder_Id", newName: "ConsignmentOrderId");
            CreateIndex("dbo.Product", "ConsignmentOrderId");
            AddForeignKey("dbo.Product", "ConsignmentOrderId", "dbo.ConsignmentOrder", "Id");
        }
    }
}
