namespace Unreal.FGO.Repostory.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class chaptered : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.user_role", "platform", c => c.Int(nullable: false));
            AddColumn("dbo.user_role", "chaptered", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.user_role", "chaptered");
            DropColumn("dbo.user_role", "platform");
        }
    }
}
