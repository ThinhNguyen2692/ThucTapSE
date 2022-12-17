namespace TicketDesk.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateticket : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Tickets", "TicketType", c => c.String());
            AlterColumn("dbo.Tickets", "Category", c => c.String());
            AlterColumn("dbo.Tickets", "CreatedBy", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Tickets", "CreatedBy", c => c.String(nullable: false, maxLength: 256));
            AlterColumn("dbo.Tickets", "Category", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Tickets", "TicketType", c => c.String(nullable: false, maxLength: 50));
        }
    }
}
