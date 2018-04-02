namespace Unreal.FGO.Repostory.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class user_roleuser_game : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.role_data", "user_game", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.role_data", "user_game");
        }
    }
}
