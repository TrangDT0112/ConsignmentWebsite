namespace ConsignmentWebsite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addForeignkey : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.tb_Review", "productId");
            AddForeignKey("dbo.tb_Review", "productId", "dbo.Product", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.tb_Review", "productId", "dbo.Product");
            DropIndex("dbo.tb_Review", new[] { "productId" });
        }
    }
}
