namespace Unreal.FGO.Repostory.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _0006 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.task_data", "user_role_id", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.task_data", "user_role_id");
        }
    }
}
