namespace Unreal.FGO.Repostory.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _0007 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.user_task", "expires_time", c => c.DateTime(nullable: false));
            DropColumn("dbo.user_task", "expiresTime");
        }
        
        public override void Down()
        {
            AddColumn("dbo.user_task", "expiresTime", c => c.DateTime(nullable: false));
            DropColumn("dbo.user_task", "expires_time");
        }
    }
}
