namespace ConsignmentWebsite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateCheckOutCOD : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Order", "TypePaymentVN", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Order", "TypePaymentVN");
        }
    }
}
