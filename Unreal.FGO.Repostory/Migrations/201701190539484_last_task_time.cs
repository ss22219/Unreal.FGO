namespace Unreal.FGO.Repostory.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class last_task_time : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.user_role", "last_task_time", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.user_role", "last_task_time");
        }
    }
}
