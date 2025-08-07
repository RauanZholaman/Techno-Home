using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Techno_Home.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    categoryID = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Category__23CAF1F8A0BFAAFB", x => x.categoryID);
                });

            migrationBuilder.CreateTable(
                name: "Patrons",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "int", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Salt = table.Column<string>(type: "varchar(32)", unicode: false, maxLength: 32, nullable: true),
                    HashedPW = table.Column<string>(type: "varchar(64)", unicode: false, maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Patrons__1788CCAC2DF458CF", x => x.UserID);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsAdmin = table.Column<bool>(type: "bit", nullable: false),
                    Salt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HashedPw = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Users__1788CC4C3AA39A38", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Source",
                columns: table => new
                {
                    sourceid = table.Column<int>(type: "int", nullable: false),
                    Source_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    externalLink = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CategoryID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Source__5ABF0FB8217B99A7", x => x.sourceid);
                    table.ForeignKey(
                        name: "FK__Source__Category__4AB81AF0",
                        column: x => x.CategoryID,
                        principalTable: "Category",
                        principalColumn: "categoryID");
                });

            migrationBuilder.CreateTable(
                name: "SubCategory",
                columns: table => new
                {
                    SubCategoryID = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CategoryID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SubCateg__26BE5BF9CF19DD0C", x => x.SubCategoryID);
                    table.ForeignKey(
                        name: "FK__SubCatego__Categ__398D8EEE",
                        column: x => x.CategoryID,
                        principalTable: "Category",
                        principalColumn: "categoryID");
                });

            migrationBuilder.CreateTable(
                name: "TO",
                columns: table => new
                {
                    customerID = table.Column<int>(type: "int", nullable: false),
                    PatronId = table.Column<int>(type: "int", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    StreetAddress = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PostCode = table.Column<int>(type: "int", nullable: true),
                    Suburb = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    State = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CardNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CardOwner = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Expiry = table.Column<string>(type: "varchar(5)", unicode: false, maxLength: 5, nullable: true),
                    CVV = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TO__B611CB9DC15F216C", x => x.customerID);
                    table.ForeignKey(
                        name: "FK__TO__PatronId__403A8C7D",
                        column: x => x.PatronId,
                        principalTable: "Patrons",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    BrandName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CategoryID = table.Column<int>(type: "int", nullable: false),
                    SubCategoryID = table.Column<int>(type: "int", nullable: true),
                    Released = table.Column<DateOnly>(type: "date", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime", nullable: true),
                    ImagePath = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    Price = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    ImageData = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    LastUpdatedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Product__3214EC2792542439", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Product_Users_LastUpdatedBy",
                        column: x => x.LastUpdatedBy,
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK__Product__Categor__4316F928",
                        column: x => x.CategoryID,
                        principalTable: "Category",
                        principalColumn: "categoryID");
                    table.ForeignKey(
                        name: "FK__Product__SubCate__440B1D61",
                        column: x => x.SubCategoryID,
                        principalTable: "SubCategory",
                        principalColumn: "SubCategoryID");
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    OrderID = table.Column<int>(type: "int", nullable: false),
                    customer = table.Column<int>(type: "int", nullable: true),
                    StreetAddress = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PostCode = table.Column<int>(type: "int", nullable: true),
                    Suburb = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    State = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Orders__C3905BAF1B12E68C", x => x.OrderID);
                    table.ForeignKey(
                        name: "FK__Orders__customer__47DBAE45",
                        column: x => x.customer,
                        principalTable: "TO",
                        principalColumn: "customerID");
                });

            migrationBuilder.CreateTable(
                name: "Stocktake",
                columns: table => new
                {
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    SourceId = table.Column<int>(type: "int", nullable: true),
                    ProductId = table.Column<int>(type: "int", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: true),
                    Price = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Stocktak__727E838BDF0BC290", x => x.ItemId);
                    table.ForeignKey(
                        name: "FK__Stocktake__Produ__4E88ABD4",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK__Stocktake__Sourc__4D94879B",
                        column: x => x.SourceId,
                        principalTable: "Source",
                        principalColumn: "sourceid");
                });

            migrationBuilder.CreateTable(
                name: "ProductsInOrders",
                columns: table => new
                {
                    OrderId = table.Column<int>(type: "int", nullable: true),
                    ProductId = table.Column<int>(type: "int", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "FK__ProductsI__Order__5070F446",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderID");
                    table.ForeignKey(
                        name: "FK__ProductsI__Produ__5165187F",
                        column: x => x.ProductId,
                        principalTable: "Stocktake",
                        principalColumn: "ItemId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_customer",
                table: "Orders",
                column: "customer");

            migrationBuilder.CreateIndex(
                name: "IX_Product_CategoryID",
                table: "Product",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_Product_LastUpdatedBy",
                table: "Product",
                column: "LastUpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Product_SubCategoryID",
                table: "Product",
                column: "SubCategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductsInOrders_OrderId",
                table: "ProductsInOrders",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductsInOrders_ProductId",
                table: "ProductsInOrders",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Source_CategoryID",
                table: "Source",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_Stocktake_ProductId",
                table: "Stocktake",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Stocktake_SourceId",
                table: "Stocktake",
                column: "SourceId");

            migrationBuilder.CreateIndex(
                name: "IX_SubCategory_CategoryID",
                table: "SubCategory",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_TO_PatronId",
                table: "TO",
                column: "PatronId");

            migrationBuilder.CreateIndex(
                name: "UQ__Users__A9D105345770D551",
                table: "Users",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductsInOrders");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Stocktake");

            migrationBuilder.DropTable(
                name: "TO");

            migrationBuilder.DropTable(
                name: "Product");

            migrationBuilder.DropTable(
                name: "Source");

            migrationBuilder.DropTable(
                name: "Patrons");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "SubCategory");

            migrationBuilder.DropTable(
                name: "Category");
        }
    }
}
