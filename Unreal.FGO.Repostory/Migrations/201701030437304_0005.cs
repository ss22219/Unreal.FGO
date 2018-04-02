namespace Unreal.FGO.Repostory.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _0005 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.user_task", "name", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.user_task", "name");
        }
    }
}
