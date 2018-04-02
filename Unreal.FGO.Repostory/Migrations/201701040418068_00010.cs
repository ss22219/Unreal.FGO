namespace Unreal.FGO.Repostory.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _00010 : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.task_data");
            AddColumn("dbo.task_data", "id", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.task_data", "name", c => c.String());
            AddPrimaryKey("dbo.task_data", "id");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.task_data");
            AlterColumn("dbo.task_data", "name", c => c.String(nullable: false, maxLength: 128));
            DropColumn("dbo.task_data", "id");
            AddPrimaryKey("dbo.task_data", "name");
        }
    }
}
