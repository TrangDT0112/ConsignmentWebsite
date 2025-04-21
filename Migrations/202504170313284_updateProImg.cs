namespace ConsignmentWebsite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateProImg : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductImage", "IsSold", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductImage", "IsSold");
        }
    }
}
