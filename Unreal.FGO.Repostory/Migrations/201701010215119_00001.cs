namespace Unreal.FGO.Repostory.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _00001 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.user_task", "followerId", c => c.String());
            AlterColumn("dbo.user_task", "deckid", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.user_task", "deckid", c => c.Int(nullable: false));
            DropColumn("dbo.user_task", "followerId");
        }
    }
}
