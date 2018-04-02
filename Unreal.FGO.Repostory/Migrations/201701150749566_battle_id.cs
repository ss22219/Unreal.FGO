namespace Unreal.FGO.Repostory.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class battle_id : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.role_data", "battle_id", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.role_data", "battle_id");
        }
    }
}
