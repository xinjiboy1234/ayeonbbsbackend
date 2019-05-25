using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ayeonbbsbackend.Migrations
{
    public partial class dbinit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FirstCategories",
                columns: table => new
                {
                    FirstCategoryId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FirstCategoryName = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    CreateDate = table.Column<DateTime>(nullable: true),
                    UpdateDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FirstCategories", x => x.FirstCategoryId);
                });

            migrationBuilder.CreateTable(
                name: "UserInfos",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserName = table.Column<string>(nullable: true),
                    LoginId = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    Avatar = table.Column<string>(nullable: true),
                    NickName = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    IsSupperManager = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    CreateDate = table.Column<DateTime>(nullable: true),
                    UpdateDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInfos", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "SecondCategories",
                columns: table => new
                {
                    SecondCategoryId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SecondCategoryName = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    FirstCategoryId = table.Column<int>(nullable: true),
                    CreateDate = table.Column<DateTime>(nullable: true),
                    UpdateDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecondCategories", x => x.SecondCategoryId);
                    table.ForeignKey(
                        name: "FK_SecondCategories_FirstCategories_FirstCategoryId",
                        column: x => x.FirstCategoryId,
                        principalTable: "FirstCategories",
                        principalColumn: "FirstCategoryId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PostInfos",
                columns: table => new
                {
                    PostId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PostTitle = table.Column<string>(nullable: true),
                    PostContent = table.Column<string>(nullable: true),
                    AuthorUserId = table.Column<int>(nullable: true),
                    SecondCategoryId = table.Column<int>(nullable: true),
                    CreateDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    IsTop = table.Column<int>(nullable: false),
                    Watch = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostInfos", x => x.PostId);
                    table.ForeignKey(
                        name: "FK_PostInfos_UserInfos_AuthorUserId",
                        column: x => x.AuthorUserId,
                        principalTable: "UserInfos",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PostInfos_SecondCategories_SecondCategoryId",
                        column: x => x.SecondCategoryId,
                        principalTable: "SecondCategories",
                        principalColumn: "SecondCategoryId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PostManagers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserInfoUserId = table.Column<int>(nullable: true),
                    SecondCategoryId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostManagers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostManagers_SecondCategories_SecondCategoryId",
                        column: x => x.SecondCategoryId,
                        principalTable: "SecondCategories",
                        principalColumn: "SecondCategoryId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PostManagers_UserInfos_UserInfoUserId",
                        column: x => x.UserInfoUserId,
                        principalTable: "UserInfos",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserPublishCategories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserInfoUserId = table.Column<int>(nullable: true),
                    SecondCategoryId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPublishCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPublishCategories_SecondCategories_SecondCategoryId",
                        column: x => x.SecondCategoryId,
                        principalTable: "SecondCategories",
                        principalColumn: "SecondCategoryId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserPublishCategories_UserInfos_UserInfoUserId",
                        column: x => x.UserInfoUserId,
                        principalTable: "UserInfos",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PostGoods",
                columns: table => new
                {
                    GoodsId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GoodsUserUserId = table.Column<int>(nullable: true),
                    PostInfoPostId = table.Column<int>(nullable: true),
                    CreateDate = table.Column<DateTime>(nullable: true),
                    UpdateDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostGoods", x => x.GoodsId);
                    table.ForeignKey(
                        name: "FK_PostGoods_UserInfos_GoodsUserUserId",
                        column: x => x.GoodsUserUserId,
                        principalTable: "UserInfos",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PostGoods_PostInfos_PostInfoPostId",
                        column: x => x.PostInfoPostId,
                        principalTable: "PostInfos",
                        principalColumn: "PostId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReplyInfos",
                columns: table => new
                {
                    ReplyId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ReplyContent = table.Column<string>(nullable: true),
                    ReplyDate = table.Column<DateTime>(nullable: true),
                    ReplyUserUserId = table.Column<int>(nullable: true),
                    Floor = table.Column<int>(nullable: false),
                    ParentReplyId = table.Column<int>(nullable: false),
                    PostInfoPostId = table.Column<int>(nullable: true),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    RepliedId = table.Column<int>(nullable: true),
                    RepliedUserId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReplyInfos", x => x.ReplyId);
                    table.ForeignKey(
                        name: "FK_ReplyInfos_PostInfos_PostInfoPostId",
                        column: x => x.PostInfoPostId,
                        principalTable: "PostInfos",
                        principalColumn: "PostId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReplyInfos_UserInfos_ReplyUserUserId",
                        column: x => x.ReplyUserUserId,
                        principalTable: "UserInfos",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReplyGoods",
                columns: table => new
                {
                    GoodsId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GoodsUserUserId = table.Column<int>(nullable: true),
                    ReplyInfoReplyId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReplyGoods", x => x.GoodsId);
                    table.ForeignKey(
                        name: "FK_ReplyGoods_UserInfos_GoodsUserUserId",
                        column: x => x.GoodsUserUserId,
                        principalTable: "UserInfos",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReplyGoods_ReplyInfos_ReplyInfoReplyId",
                        column: x => x.ReplyInfoReplyId,
                        principalTable: "ReplyInfos",
                        principalColumn: "ReplyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PostGoods_GoodsUserUserId",
                table: "PostGoods",
                column: "GoodsUserUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PostGoods_PostInfoPostId",
                table: "PostGoods",
                column: "PostInfoPostId");

            migrationBuilder.CreateIndex(
                name: "IX_PostInfos_AuthorUserId",
                table: "PostInfos",
                column: "AuthorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PostInfos_SecondCategoryId",
                table: "PostInfos",
                column: "SecondCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_PostManagers_SecondCategoryId",
                table: "PostManagers",
                column: "SecondCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_PostManagers_UserInfoUserId",
                table: "PostManagers",
                column: "UserInfoUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ReplyGoods_GoodsUserUserId",
                table: "ReplyGoods",
                column: "GoodsUserUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ReplyGoods_ReplyInfoReplyId",
                table: "ReplyGoods",
                column: "ReplyInfoReplyId");

            migrationBuilder.CreateIndex(
                name: "IX_ReplyInfos_PostInfoPostId",
                table: "ReplyInfos",
                column: "PostInfoPostId");

            migrationBuilder.CreateIndex(
                name: "IX_ReplyInfos_ReplyUserUserId",
                table: "ReplyInfos",
                column: "ReplyUserUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SecondCategories_FirstCategoryId",
                table: "SecondCategories",
                column: "FirstCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPublishCategories_SecondCategoryId",
                table: "UserPublishCategories",
                column: "SecondCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPublishCategories_UserInfoUserId",
                table: "UserPublishCategories",
                column: "UserInfoUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PostGoods");

            migrationBuilder.DropTable(
                name: "PostManagers");

            migrationBuilder.DropTable(
                name: "ReplyGoods");

            migrationBuilder.DropTable(
                name: "UserPublishCategories");

            migrationBuilder.DropTable(
                name: "ReplyInfos");

            migrationBuilder.DropTable(
                name: "PostInfos");

            migrationBuilder.DropTable(
                name: "UserInfos");

            migrationBuilder.DropTable(
                name: "SecondCategories");

            migrationBuilder.DropTable(
                name: "FirstCategories");
        }
    }
}
