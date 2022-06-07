using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stories.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AgeRestriction",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AgeFrom = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgeRestriction", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AnonymousOnline",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConnectionId = table.Column<string>(type: "varchar(30)", nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClientAgent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Connected = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Disconnected = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnonymousOnline", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CategoryGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CountryName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    FlagFilePath = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    PhonePrefix = table.Column<int>(type: "int", nullable: false),
                    Timezone = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChatElevatedUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatElevatedUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RecommendedAgeGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LowestAge = table.Column<int>(type: "int", nullable: false),
                    HighestAge = table.Column<int>(type: "int", nullable: false),
                    AgeGroupName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecommendedAgeGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Themes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(64)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Themes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CategoryGroupId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Categories_CategoryGroups_CategoryGroupId",
                        column: x => x.CategoryGroupId,
                        principalTable: "CategoryGroups",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Administrators",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ActivatedBy = table.Column<int>(type: "int", nullable: true),
                    ActivatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeactivationReason = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    DeactivatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeactivatedBy = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Administrators", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DisplayedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    RegistrationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastSeen = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CountryId = table.Column<int>(type: "int", nullable: true),
                    DeactivatedByAdminId = table.Column<int>(type: "int", nullable: true),
                    DeactivatedSince = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeactivatedUntil = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    Info = table.Column<string>(type: "nvarchar(max)", maxLength: 10000, nullable: true),
                    Facebook = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Twitter = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Microsoft = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Google = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ThemeId = table.Column<int>(type: "int", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Administrators_DeactivatedByAdminId",
                        column: x => x.DeactivatedByAdminId,
                        principalTable: "Administrators",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Users_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Users_Themes_ThemeId",
                        column: x => x.ThemeId,
                        principalTable: "Themes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Articles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Prolog = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Epilog = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ArticleAdded = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ArticlePublished = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false),
                    IsBanned = table.Column<bool>(type: "bit", nullable: false),
                    BannedBy = table.Column<int>(type: "int", nullable: true),
                    AgeRestrictionId = table.Column<int>(type: "int", nullable: false),
                    RecommendedAgeGroupId = table.Column<int>(type: "int", nullable: true),
                    LastChange = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FirstPublished = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BannedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Articles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Articles_AgeRestriction_AgeRestrictionId",
                        column: x => x.AgeRestrictionId,
                        principalTable: "AgeRestriction",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Articles_RecommendedAgeGroups_RecommendedAgeGroupId",
                        column: x => x.RecommendedAgeGroupId,
                        principalTable: "RecommendedAgeGroups",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Articles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Clubs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClubName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    ClubDescription = table.Column<string>(type: "nvarchar(2560)", maxLength: 2560, nullable: false),
                    ClubCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OwnerId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    isPublic = table.Column<bool>(type: "bit", nullable: false),
                    isActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clubs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Clubs_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Collections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CollectionName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CollectionDescription = table.Column<string>(type: "nvarchar(max)", maxLength: 4096, nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Collections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Collections_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EmailLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Subject = table.Column<string>(type: "varchar(512)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailLogs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FavoriteAuthorUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    AuthorId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavoriteAuthorUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FavoriteAuthorUsers_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FavoriteAuthorUsers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Friends",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    FriendId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Confirmed = table.Column<bool>(type: "bit", nullable: false),
                    Declined = table.Column<bool>(type: "bit", nullable: false),
                    Blocked = table.Column<bool>(type: "bit", nullable: false),
                    RequestTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AnsweringAction = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Friends", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Friends_Users_FriendId",
                        column: x => x.FriendId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Friends_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ChatRooms",
                columns: table => new
                {
                    RoomName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    RoomType = table.Column<string>(type: "varchar(16)", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatRooms", x => x.RoomName);
                    table.ForeignKey(
                        name: "FK_ChatRooms_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Chats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SenderId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ChatroomName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    RecepientId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Message = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsAdminMessage = table.Column<bool>(type: "bit", nullable: false),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Chats_Users_RecepientId",
                        column: x => x.RecepientId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Chats_Users_SenderId",
                        column: x => x.SenderId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ChattingTimes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TimeInChatSuma = table.Column<TimeSpan>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChattingTimes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChattingTimes_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ChatUserBans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    AdminUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    GroupName = table.Column<string>(type: "varchar(256)", nullable: true),
                    BanForAllChat = table.Column<bool>(type: "bit", nullable: false),
                    BannedUntil = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReasonForBan = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatUserBans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatUserBans_Users_AdminUserId",
                        column: x => x.AdminUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ChatUserBans_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SenderId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Subject = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    MessageText = table.Column<string>(type: "nvarchar(max)", maxLength: 4096, nullable: false),
                    SentTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Messages_Users_SenderId",
                        column: x => x.SenderId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OnlineTimes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TimeOnline = table.Column<TimeSpan>(type: "time", nullable: false),
                    Days = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OnlineTimes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OnlineTimes_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConnectionId = table.Column<string>(type: "varchar(64)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    IpAddress = table.Column<string>(type: "varchar(16)", nullable: true),
                    ClientAgent = table.Column<string>(type: "varchar(1024)", nullable: true),
                    Connected = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Disconnected = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sessions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserClaims_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserChatSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    NameFontColor = table.Column<string>(type: "varchar(64)", nullable: true),
                    ChatFontColor = table.Column<string>(type: "varchar(64)", nullable: true),
                    ChatBackgroundColor = table.Column<string>(type: "varchar(64)", nullable: true),
                    MessageNotification = table.Column<bool>(type: "bit", nullable: false),
                    PrivateMessageNotification = table.Column<bool>(type: "bit", nullable: false),
                    SystemMessageNotification = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserChatSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserChatSettings_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_UserLogins_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ActiveSessionsCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSessions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    EmailPrivacy = table.Column<int>(type: "int", nullable: false),
                    NamePrivacy = table.Column<int>(type: "int", nullable: false),
                    FrindsPrivacy = table.Column<int>(type: "int", nullable: false),
                    PhonePrivacy = table.Column<int>(type: "int", nullable: false),
                    SocialMediaLinksPrivacy = table.Column<int>(type: "int", nullable: false),
                    CountryPrivacy = table.Column<int>(type: "int", nullable: false),
                    BirthDatePrivacy = table.Column<int>(type: "int", nullable: false),
                    PictureFilePrivacy = table.Column<int>(type: "int", nullable: false),
                    LastSeenPrivacy = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSettings_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_UserTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserUser",
                columns: table => new
                {
                    UserRecepientId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserSenderId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserUser", x => new { x.UserRecepientId, x.UserSenderId });
                    table.ForeignKey(
                        name: "FK_UserUser_Users_UserRecepientId",
                        column: x => x.UserRecepientId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserUser_Users_UserSenderId",
                        column: x => x.UserSenderId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Article_Readers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArticleId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    isAuthenticated = table.Column<bool>(type: "bit", nullable: false),
                    IPAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReadAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Article_Readers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Article_Readers_Articles_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Articles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Article_Readers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ArticleCategory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArticleId = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArticleCategory_Articles_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Articles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ArticleCategory_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Critics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArticleId = table.Column<int>(type: "int", nullable: false),
                    CriticAdded = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CriticId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CriticMessage = table.Column<string>(type: "nvarchar(2560)", maxLength: 2560, nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Critics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Critics_Articles_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Articles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Critics_Users_CriticId",
                        column: x => x.CriticId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Stars",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArticleId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    StarsCount = table.Column<int>(type: "int", nullable: false),
                    Change = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stars", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Stars_Articles_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Articles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Stars_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Club_Articles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArticleId = table.Column<int>(type: "int", nullable: false),
                    ClubId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Club_Articles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Club_Articles_Articles_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Articles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Club_Articles_Clubs_ClubId",
                        column: x => x.ClubId,
                        principalTable: "Clubs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Club_Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ClubId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Club_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Club_Users_Clubs_ClubId",
                        column: x => x.ClubId,
                        principalTable: "Clubs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Club_Users_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ArticleCollection",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArticleId = table.Column<int>(type: "int", nullable: false),
                    CollectionId = table.Column<int>(type: "int", nullable: false),
                    OrderInCollection = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleCollection", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArticleCollection_Articles_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Articles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ArticleCollection_Collections_CollectionId",
                        column: x => x.CollectionId,
                        principalTable: "Collections",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CollectionCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CollectionId = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectionCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CollectionCategories_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CollectionCategories_Collections_CollectionId",
                        column: x => x.CollectionId,
                        principalTable: "Collections",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EmailRecepients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmailLogId = table.Column<int>(type: "int", nullable: false),
                    Sent = table.Column<bool>(type: "bit", nullable: false),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Error = table.Column<string>(type: "varchar(1024)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailRecepients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailRecepients_EmailLogs_EmailLogId",
                        column: x => x.EmailLogId,
                        principalTable: "EmailLogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChatRoomsAdmins",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RoomName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    AdminHash = table.Column<string>(type: "varchar(128)", nullable: true),
                    isElevated = table.Column<bool>(type: "bit", nullable: false),
                    ChatRoomsRoomName = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatRoomsAdmins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatRoomsAdmins_ChatRooms_ChatRoomsRoomName",
                        column: x => x.ChatRoomsRoomName,
                        principalTable: "ChatRooms",
                        principalColumn: "RoomName");
                });

            migrationBuilder.CreateTable(
                name: "UsersInRoom",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoomName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ConnectionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayedName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    IsAdmin = table.Column<bool>(type: "bit", nullable: false),
                    IsOwner = table.Column<bool>(type: "bit", nullable: false),
                    JoiningTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LeavingTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChatRoomsRoomName = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersInRoom", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsersInRoom_ChatRooms_ChatRoomsRoomName",
                        column: x => x.ChatRoomsRoomName,
                        principalTable: "ChatRooms",
                        principalColumn: "RoomName");
                    table.ForeignKey(
                        name: "FK_UsersInRoom_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MessageRecipients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MessageId = table.Column<int>(type: "int", nullable: false),
                    RecepientId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    WasRead = table.Column<bool>(type: "bit", nullable: false),
                    ReadTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WasDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DontShowInBin = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageRecipients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageRecipients_Messages_MessageId",
                        column: x => x.MessageId,
                        principalTable: "Messages",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MessageRecipients_Users_RecepientId",
                        column: x => x.RecepientId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SpamReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MessageId = table.Column<int>(type: "int", nullable: false),
                    ReportedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    AdminId = table.Column<int>(type: "int", nullable: false),
                    WasSpam = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpamReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SpamReports_Administrators_AdminId",
                        column: x => x.AdminId,
                        principalTable: "Administrators",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SpamReports_Messages_MessageId",
                        column: x => x.MessageId,
                        principalTable: "Messages",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SpamReports_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Likes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ArticleId = table.Column<int>(type: "int", nullable: false),
                    Article_CategoryId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Likes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Likes_ArticleCategory_Article_CategoryId",
                        column: x => x.Article_CategoryId,
                        principalTable: "ArticleCategory",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Likes_Articles_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Articles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Likes_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Administrators_UserId",
                table: "Administrators",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Article_Readers_ArticleId",
                table: "Article_Readers",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_Article_Readers_UserId",
                table: "Article_Readers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ArticleCategory_ArticleId",
                table: "ArticleCategory",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_ArticleCategory_CategoryId",
                table: "ArticleCategory",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ArticleCollection_ArticleId",
                table: "ArticleCollection",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_ArticleCollection_CollectionId",
                table: "ArticleCollection",
                column: "CollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_AgeRestrictionId",
                table: "Articles",
                column: "AgeRestrictionId");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_RecommendedAgeGroupId",
                table: "Articles",
                column: "RecommendedAgeGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_UserId",
                table: "Articles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_CategoryGroupId",
                table: "Categories",
                column: "CategoryGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Club_Articles_ArticleId",
                table: "Club_Articles",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_Club_Articles_ClubId",
                table: "Club_Articles",
                column: "ClubId");

            migrationBuilder.CreateIndex(
                name: "IX_Club_Users_ClubId",
                table: "Club_Users",
                column: "ClubId");

            migrationBuilder.CreateIndex(
                name: "IX_Club_Users_UserId",
                table: "Club_Users",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Clubs_OwnerId",
                table: "Clubs",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_CollectionCategories_CategoryId",
                table: "CollectionCategories",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CollectionCategories_CollectionId",
                table: "CollectionCategories",
                column: "CollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_Collections_UserId",
                table: "Collections",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Critics_ArticleId",
                table: "Critics",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_Critics_CriticId",
                table: "Critics",
                column: "CriticId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailLogs_UserId",
                table: "EmailLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailRecepients_EmailLogId",
                table: "EmailRecepients",
                column: "EmailLogId");

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteAuthorUsers_AuthorId",
                table: "FavoriteAuthorUsers",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteAuthorUsers_UserId",
                table: "FavoriteAuthorUsers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Friends_FriendId",
                table: "Friends",
                column: "FriendId");

            migrationBuilder.CreateIndex(
                name: "IX_Friends_UserId",
                table: "Friends",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatRooms_UserId",
                table: "ChatRooms",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatRoomsAdmins_ChatRoomsRoomName",
                table: "ChatRoomsAdmins",
                column: "ChatRoomsRoomName");

            migrationBuilder.CreateIndex(
                name: "IX_Chats_RecepientId",
                table: "Chats",
                column: "RecepientId");

            migrationBuilder.CreateIndex(
                name: "IX_Chats_SenderId",
                table: "Chats",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_ChattingTimes_UserId",
                table: "ChattingTimes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatUserBans_AdminUserId",
                table: "ChatUserBans",
                column: "AdminUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatUserBans_UserId",
                table: "ChatUserBans",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_Article_CategoryId",
                table: "Likes",
                column: "Article_CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_ArticleId",
                table: "Likes",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_UserId",
                table: "Likes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageRecipients_MessageId",
                table: "MessageRecipients",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageRecipients_RecepientId",
                table: "MessageRecipients",
                column: "RecepientId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SenderId",
                table: "Messages",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_OnlineTimes_UserId",
                table: "OnlineTimes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleClaims_RoleId",
                table: "RoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_UserId",
                table: "Sessions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SpamReports_AdminId",
                table: "SpamReports",
                column: "AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_SpamReports_MessageId",
                table: "SpamReports",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_SpamReports_UserId",
                table: "SpamReports",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Stars_ArticleId",
                table: "Stars",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_Stars_UserId",
                table: "Stars",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserClaims_UserId",
                table: "UserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserChatSettings_UserId",
                table: "UserChatSettings",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogins_UserId",
                table: "UserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "Users",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CountryId",
                table: "Users",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_DeactivatedByAdminId",
                table: "Users",
                column: "DeactivatedByAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_ThemeId",
                table: "Users",
                column: "ThemeId");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "Users",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_UserId",
                table: "UserSessions",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UserSettings_UserId",
                table: "UserSettings",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UsersInRoom_ChatRoomsRoomName",
                table: "UsersInRoom",
                column: "ChatRoomsRoomName");

            migrationBuilder.CreateIndex(
                name: "IX_UsersInRoom_UserId",
                table: "UsersInRoom",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserUser_UserSenderId",
                table: "UserUser",
                column: "UserSenderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Administrators_Users_UserId",
                table: "Administrators",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Administrators_Users_UserId",
                table: "Administrators");

            migrationBuilder.DropTable(
                name: "AnonymousOnline");

            migrationBuilder.DropTable(
                name: "Article_Readers");

            migrationBuilder.DropTable(
                name: "ArticleCollection");

            migrationBuilder.DropTable(
                name: "Club_Articles");

            migrationBuilder.DropTable(
                name: "Club_Users");

            migrationBuilder.DropTable(
                name: "CollectionCategories");

            migrationBuilder.DropTable(
                name: "Critics");

            migrationBuilder.DropTable(
                name: "EmailRecepients");

            migrationBuilder.DropTable(
                name: "FavoriteAuthorUsers");

            migrationBuilder.DropTable(
                name: "Friends");

            migrationBuilder.DropTable(
                name: "ChatElevatedUsers");

            migrationBuilder.DropTable(
                name: "ChatRoomsAdmins");

            migrationBuilder.DropTable(
                name: "Chats");

            migrationBuilder.DropTable(
                name: "ChattingTimes");

            migrationBuilder.DropTable(
                name: "ChatUserBans");

            migrationBuilder.DropTable(
                name: "Likes");

            migrationBuilder.DropTable(
                name: "MessageRecipients");

            migrationBuilder.DropTable(
                name: "OnlineTimes");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "RoleClaims");

            migrationBuilder.DropTable(
                name: "Sessions");

            migrationBuilder.DropTable(
                name: "SpamReports");

            migrationBuilder.DropTable(
                name: "Stars");

            migrationBuilder.DropTable(
                name: "UserClaims");

            migrationBuilder.DropTable(
                name: "UserChatSettings");

            migrationBuilder.DropTable(
                name: "UserLogins");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "UserSessions");

            migrationBuilder.DropTable(
                name: "UserSettings");

            migrationBuilder.DropTable(
                name: "UsersInRoom");

            migrationBuilder.DropTable(
                name: "UserTokens");

            migrationBuilder.DropTable(
                name: "UserUser");

            migrationBuilder.DropTable(
                name: "Clubs");

            migrationBuilder.DropTable(
                name: "Collections");

            migrationBuilder.DropTable(
                name: "EmailLogs");

            migrationBuilder.DropTable(
                name: "ArticleCategory");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "ChatRooms");

            migrationBuilder.DropTable(
                name: "Articles");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "AgeRestriction");

            migrationBuilder.DropTable(
                name: "RecommendedAgeGroups");

            migrationBuilder.DropTable(
                name: "CategoryGroups");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Administrators");

            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropTable(
                name: "Themes");
        }
    }
}
