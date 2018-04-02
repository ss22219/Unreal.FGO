namespace Unreal.FGO.Repostory.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class cookie : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.role_data", "cookie", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.role_data", "cookie");
        }
    }
}
