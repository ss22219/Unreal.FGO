namespace Unreal.FGO.Repostory.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _0009 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.task_data", "value", c => c.String(maxLength: 2500));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.task_data", "value", c => c.String());
        }
    }
}
