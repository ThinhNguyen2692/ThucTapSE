namespace TicketDesk.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateticketdate : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Tickets", "TargetDate", c => c.DateTimeOffset(nullable: false, precision: 7));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Tickets", "TargetDate", c => c.DateTimeOffset(precision: 7));
        }
    }
}
