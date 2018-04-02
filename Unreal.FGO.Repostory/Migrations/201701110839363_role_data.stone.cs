namespace Unreal.FGO.Repostory.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class role_datastone : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.role_data", "stone", c => c.Int(nullable: false));
            AddColumn("dbo.user_role", "registed", c => c.Boolean(nullable: false));
            AddColumn("dbo.user_role", "inited", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.user_role", "inited");
            DropColumn("dbo.user_role", "registed");
            DropColumn("dbo.role_data", "stone");
        }
    }
}
