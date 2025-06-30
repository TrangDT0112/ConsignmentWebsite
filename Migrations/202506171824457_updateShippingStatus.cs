namespace ConsignmentWebsite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateShippingStatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Order", "ShippingStatus", c => c.Int(nullable: false));
            DropColumn("dbo.Order", "TypePaymentVN");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Order", "TypePaymentVN", c => c.String());
            DropColumn("dbo.Order", "ShippingStatus");
        }
    }
}
