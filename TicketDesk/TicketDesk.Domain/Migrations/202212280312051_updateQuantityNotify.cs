namespace TicketDesk.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateQuantityNotify : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserSettings", "NotifyQuantity", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserSettings", "NotifyQuantity");
        }
    }
}
