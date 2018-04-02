namespace Unreal.FGO.Repostory.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _0004 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.device_preset", "platform_type", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.device_preset", "platform_type");
        }
    }
}
