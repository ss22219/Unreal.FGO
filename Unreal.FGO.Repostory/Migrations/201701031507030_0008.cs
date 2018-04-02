namespace Unreal.FGO.Repostory.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _0008 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.task_log",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        task_id = c.Int(nullable: false),
                        create_time = c.DateTime(nullable: false),
                        message = c.String(),
                        action = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.task_log");
        }
    }
}
