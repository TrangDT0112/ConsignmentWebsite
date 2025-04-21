namespace ConsignmentWebsite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateSetting : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.SystemSetting", newName: "tb_SettingSystem");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.tb_SettingSystem", newName: "SystemSetting");
        }
    }
}
