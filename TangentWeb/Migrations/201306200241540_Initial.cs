namespace TangentWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TangentItems",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        ImageUrl = c.String(),
                        Title = c.String(),
                        Text = c.String(),
                        Date = c.String(),
                        Complete = c.Boolean(nullable: false),
                        PosterId = c.String(),
                        PosterImageUrl = c.String(),
                        DeviceToken = c.String(),
                        OriginalUrl = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TangentItems");
        }
    }
}
