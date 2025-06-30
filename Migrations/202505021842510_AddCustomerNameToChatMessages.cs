namespace ConsignmentWebsite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCustomerNameToChatMessages : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ChatMessage", "CustomerName", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ChatMessage", "CustomerName");
        }
    }
}
