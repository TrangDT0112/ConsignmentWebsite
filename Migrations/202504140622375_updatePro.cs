namespace ConsignmentWebsite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatePro : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Product", "BrandId", c => c.Int(nullable: false));
            DropTable("dbo.Brand");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Brand",
                c => new
                    {
                        BrandId = c.Int(nullable: false, identity: true),
                        BrandName = c.String(nullable: false, maxLength: 250),
                        Description = c.String(),
                        Image = c.String(maxLength: 250),
                        IsPrefer = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(nullable: false),
                        Modifiedby = c.String(),
                    })
                .PrimaryKey(t => t.BrandId);
            
            DropColumn("dbo.Product", "BrandId");
        }
    }
}
