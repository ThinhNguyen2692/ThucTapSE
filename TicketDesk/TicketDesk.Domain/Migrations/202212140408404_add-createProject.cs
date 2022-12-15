namespace TicketDesk.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addcreateProject : DbMigration
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
