namespace ConsignmentWebsite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateBrand : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Brand", "IsPrefer", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Brand", "IsPrefer");
        }
    }
}
