namespace TicketDesk.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateProjectUser111 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.demo2",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        demoID = c.Int(nullable: false),
                        UserId = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.ID);
            
            AddColumn("dbo.demoes", "demo2_ID", c => c.Int());
            CreateIndex("dbo.demoes", "demo2_ID");
            AddForeignKey("dbo.demoes", "demo2_ID", "dbo.demo2", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.demoes", "demo2_ID", "dbo.demo2");
            DropIndex("dbo.demoes", new[] { "demo2_ID" });
            DropColumn("dbo.demoes", "demo2_ID");
            DropTable("dbo.demo2");
        }
    }
}
