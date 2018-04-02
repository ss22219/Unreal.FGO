namespace Unreal.FGO.Repostory.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _00002 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.task_data",
                c => new
                    {
                        name = c.String(nullable: false, maxLength: 128),
                        value = c.String(),
                        task_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.name);
            
            AddColumn("dbo.user_task", "expiresTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.user_task", "battlId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.user_task", "battlId");
            DropColumn("dbo.user_task", "expiresTime");
            DropTable("dbo.task_data");
        }
    }
}
