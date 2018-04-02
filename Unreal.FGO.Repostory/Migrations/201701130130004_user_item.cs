namespace Unreal.FGO.Repostory.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class user_item : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.role_data", "user_item", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.role_data", "user_item");
        }
    }
}
