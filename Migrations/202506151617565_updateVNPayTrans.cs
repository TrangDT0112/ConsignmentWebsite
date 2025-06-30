namespace ConsignmentWebsite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateVNPayTrans : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Order", "PaymentStatus", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Order", "PaymentStatus");
        }
    }
}
