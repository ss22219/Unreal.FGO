namespace Unreal.FGO.Repostory.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class role_datalast_login : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.role_data", "last_login", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.role_data", "last_login");
        }
    }
}
