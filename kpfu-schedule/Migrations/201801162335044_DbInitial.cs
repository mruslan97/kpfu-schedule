namespace kpfu_schedule.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DbInitial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TgUsers",
                c => new
                    {
                        ChatId = c.Long(nullable: false),
                        Username = c.String(),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Group = c.String(),
                    })
                .PrimaryKey(t => t.ChatId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TgUsers");
        }
    }
}
