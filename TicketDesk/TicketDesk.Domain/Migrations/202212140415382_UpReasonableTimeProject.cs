namespace TicketDesk.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpReasonableTimeProject : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Projects", "ReasonableTime", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Projects", "ReasonableTime");
        }
    }
}
