namespace TicketDesk.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateProjectUser10 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.demoes",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ProjectId = c.Int(nullable: false),
                        UserId = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.demoes");
        }
    }
}
