namespace ConsignmentWebsite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateChatMesTb : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ChatMessage", "Message", c => c.String(nullable: false));
            AddColumn("dbo.ChatMessage", "SentAt", c => c.DateTime(nullable: false));
            AddColumn("dbo.ChatMessage", "SessionId", c => c.String(nullable: false));
            AlterColumn("dbo.ChatMessage", "Sender", c => c.String(nullable: false));
            DropColumn("dbo.ChatMessage", "UserId");
            DropColumn("dbo.ChatMessage", "MessageText");
            DropColumn("dbo.ChatMessage", "CreatedAt");
            DropColumn("dbo.ChatMessage", "IsRead");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ChatMessage", "IsRead", c => c.Boolean(nullable: false));
            AddColumn("dbo.ChatMessage", "CreatedAt", c => c.DateTime(nullable: false));
            AddColumn("dbo.ChatMessage", "MessageText", c => c.String());
            AddColumn("dbo.ChatMessage", "UserId", c => c.String());
            AlterColumn("dbo.ChatMessage", "Sender", c => c.String());
            DropColumn("dbo.ChatMessage", "SessionId");
            DropColumn("dbo.ChatMessage", "SentAt");
            DropColumn("dbo.ChatMessage", "Message");
        }
    }
}
