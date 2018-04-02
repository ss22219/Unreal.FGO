namespace Unreal.FGO.Repostory.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.device_preset",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        os = c.String(),
                        model = c.String(),
                        pf_ver = c.String(),
                        ptype = c.String(),
                        dp = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.devices",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        user_id = c.Int(nullable: false),
                        idfa = c.String(),
                        udid = c.String(),
                        deviceid = c.String(),
                        platform_type = c.Int(nullable: false),
                        os = c.String(),
                        model = c.String(),
                        pf_ver = c.String(),
                        ptype = c.String(),
                        dp = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.role_data",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        role_id = c.Int(nullable: false),
                        bilibili_id = c.String(),
                        rguid = c.String(),
                        usk = c.String(),
                        access_token = c.String(),
                        access_token_expires = c.Long(nullable: false),
                        nickname = c.String(),
                        game_user_id = c.String(),
                        face = c.String(),
                        s_face = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.system_info",
                c => new
                    {
                        name = c.String(nullable: false, maxLength: 128),
                        value = c.String(),
                    })
                .PrimaryKey(t => t.name);
            
            CreateTable(
                "dbo.task_error",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        error = c.Int(nullable: false),
                        message = c.String(),
                        source_code = c.String(),
                        source_message = c.String(),
                        source_data = c.String(),
                        create_time = c.DateTime(nullable: false),
                        action = c.Int(nullable: false),
                        task_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.user_role",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        username = c.String(),
                        password = c.String(),
                        create_time = c.DateTime(nullable: false),
                        last_update_time = c.DateTime(nullable: false),
                        device_id = c.Int(nullable: false),
                        user_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.users",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        username = c.String(),
                        password = c.String(),
                        create_time = c.DateTime(nullable: false),
                        last_login_time = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.user_task",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        deckid = c.Int(nullable: false),
                        action = c.String(),
                        current_action = c.Int(nullable: false),
                        excute_rule = c.String(),
                        re_excute_count = c.Int(nullable: false),
                        state = c.Int(nullable: false),
                        error_type = c.Int(nullable: false),
                        enable = c.Boolean(nullable: false),
                        start_time = c.DateTime(),
                        end_time = c.DateTime(),
                        create_time = c.DateTime(nullable: false),
                        last_update_time = c.DateTime(nullable: false),
                        user_role_id = c.Int(nullable: false),
                        user_id = c.Int(nullable: false),
                        useitem = c.Boolean(nullable:false, defaultValue:false),
                        quest_ids = c.String(),
                })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.user_task");
            DropTable("dbo.users");
            DropTable("dbo.user_role");
            DropTable("dbo.task_error");
            DropTable("dbo.system_info");
            DropTable("dbo.role_data");
            DropTable("dbo.devices");
            DropTable("dbo.device_preset");
        }
    }
}
