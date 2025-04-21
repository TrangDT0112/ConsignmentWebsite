namespace ConsignmentWebsite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatetbOfOrder : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ConsignmentOrder",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ConsignmentCode = c.String(nullable: false),
                        ConsignorName = c.String(nullable: false),
                        Phone = c.String(nullable: false),
                        ReturnAddress = c.String(nullable: false),
                        Email = c.String(),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Quantity = c.Int(nullable: false),
                        PaymentStatus = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        BankAccount = c.String(),
                        BankAccountNumber = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(nullable: false),
                        Modifiedby = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ConsignmentOrder");
        }
    }
}
