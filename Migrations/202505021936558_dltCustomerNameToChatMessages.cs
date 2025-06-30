namespace ConsignmentWebsite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dltCustomerNameToChatMessages : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.ChatMessage", "CustomerName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ChatMessage", "CustomerName", c => c.String(nullable: false));
        }
    }
}
