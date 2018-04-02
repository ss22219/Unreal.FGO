namespace Unreal.FGO.Repostory.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _00003 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.users", "token", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.users", "token");
        }
    }
}
