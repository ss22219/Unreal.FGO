namespace Unreal.FGO.Repostory.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class quest_info : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.role_data", "quest_info", c => c.String());
            AddColumn("dbo.role_data", "svt_info", c => c.String());
            AddColumn("dbo.role_data", "deck_info", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.role_data", "deck_info");
            DropColumn("dbo.role_data", "svt_info");
            DropColumn("dbo.role_data", "quest_info");
        }
    }
}
