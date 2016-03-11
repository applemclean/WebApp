namespace DatabaseService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUsers : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EMail = c.String(nullable: false, maxLength: 300),
                        Name = c.String(nullable: false),
                        Surname = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.EMail, unique: true, name: "IX_Email");
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Users", "IX_Email");
            DropTable("dbo.Users");
        }
    }
}
