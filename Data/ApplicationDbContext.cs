using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Stories.Model;

namespace Stories.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Administrator> Administrators { get; set; }
        public DbSet<AgeRestriction> AgeRestrictions { get; set; }
        public DbSet<AnonymousOnline> AnonymousOnline { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Article_Category> Article_Categories { get; set; }
        public DbSet<Article_Collection> Article_Collections { get; set; }
        public DbSet<Article_Read> Article_Readers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CategoryGroup> CategoryGroups { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<ChatBans> ChatBans { get; set; }
        public DbSet<ChatElevated> ChatElevated { get; set; }
        public DbSet<ChatRooms> ChatRooms { get; set; }
        public DbSet<ChattingTime> ChattingTimes { get; set; }
        public DbSet<ChatRoomAdmins> ChatRoomAdmins { get; set; }
        public DbSet<Club> Clubs { get; set; }
        public DbSet<Club_Article> Club_Articles { get; set; }
        public DbSet<Club_Users> Club_Users { get; set; }
        public DbSet<Collection> Collections { get; set; }
        public DbSet<Collection_Categories> Collection_Categories { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Critic> Critics { get; set; }
        public DbSet<EmailLog> EmailLog { get; set; }
        public DbSet<EmailRecepients> EmailRecepients { get; set; }
        public DbSet<FavoriteAuthor_Users> FavoriteAuthor_Users { get; set; }
        public DbSet<Friends> Friends { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<MessageRecipient> MessageRecipients { get; set; }
        public DbSet<OnlineTime> OnlineTimes { get; set; }
        public DbSet<RecommendedAgeGroup> RecommendedAgeGroups { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Stories.Model.Stars> Stars { get; set; }
        public DbSet<SpamReports> SpamReports { get; set; }
        public DbSet<Theme> Themes { get; set; }
        public DbSet<UserSession> UserSessions { get; set; }
        public DbSet<UserSettings> UserSettings { get; set; }
        public DbSet<UserChatSettings> UserChatSettings { get; set; }
        public DbSet<UserInRoom> UsersInRooms { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Administrator>().ToTable("Administrators");

            modelBuilder.Entity<AgeRestriction>().ToTable("AgeRestriction");

            modelBuilder.Entity<AnonymousOnline>().ToTable("AnonymousOnline");

            modelBuilder.Entity<Article_Category>().ToTable("ArticleCategory")
                .HasOne(i => i.Article)
                .WithMany(j => j.Article_Categories)
                .HasForeignKey(j => j.ArticleId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Article_Category>()
                .HasOne(i => i.Category)
                .WithMany(j => j.Article_Categories)
                .HasForeignKey(j => j.CategoryId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Article_Collection>().ToTable("ArticleCollection")
                .HasOne(i => i.Article)
                .WithMany(j => j.Article_Collections)
                .HasForeignKey(j => j.ArticleId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Article_Collection>()
                .HasOne(i => i.Collection)
                .WithMany(j => j.Article_Collections)
                .HasForeignKey(j => j.CollectionId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Article>().ToTable("Articles")
                .HasOne(i => i.User)
                .WithMany(j => j.Articles)
                .HasForeignKey(j => j.UserId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Article>()
                .HasOne(i => i.RecommendedAgeGroup)
                .WithMany(j => j.Articles)
                .HasForeignKey(j => j.RecommendedAgeGroupId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Article>()
                .HasOne(i => i.AgeRestriction)
                .WithMany(j => j.Articles)
                .HasForeignKey(j => j.AgeRestrictionId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Article_Read>().ToTable("Article_Readers")
                .HasOne(i => i.Article)
                .WithMany(i => i.Article_readers)
                .HasForeignKey(i => i.ArticleId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Article_Read>().ToTable("Article_Readers")
                .HasOne(i => i.User)
                .WithMany(i => i.Article_Readers)
                .HasForeignKey(i => i.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Category>().ToTable("Categories")
                .HasOne(i => i.CategoryGroup)
                .WithMany(j => j.Categories)
                .HasForeignKey(j => j.CategoryGroupId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<CategoryGroup>().ToTable("CategoryGroups");

            modelBuilder.Entity<Chat>().ToTable("Chats")
                .HasOne(i => i.Sender)
                .WithMany(j => j.Chats)
                .HasForeignKey(j => j.SenderId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Chat>()
                .HasOne(i => i.Recepient)
                .WithMany(j => j.Chatting)
                .HasForeignKey(j => j.RecepientId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ChatElevated>().ToTable("ChatElevatedUsers");

            modelBuilder.Entity<ChatBans>().ToTable("ChatUserBans")
                .HasOne(i => i.BannedUser)
                .WithMany(j => j.UsersBanned)
                .HasForeignKey(j => j.UserId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<ChatBans>()
                .HasOne(i => i.AdminUser)
                .WithMany(j => j.UsersAdmins)
                .HasForeignKey(j => j.AdminUserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ChatRooms>().ToTable("ChatRooms");

            modelBuilder.Entity<ChatRoomAdmins>().ToTable("ChatRoomsAdmins");

            modelBuilder.Entity<ChattingTime>().ToTable("ChattingTimes")
                .HasOne(i => i.User)
                .WithMany(j => j.ChattingTimes)
                .HasForeignKey(i => i.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Club>().ToTable("Clubs")
                .HasOne(i => i.Owner)
                .WithMany(i => i.Clubs)
                .HasForeignKey(i => i.OwnerId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Club_Article>().ToTable("Club_Articles")
                .HasOne(i => i.Article)
                .WithMany(i => i.Club_Articles)
                .HasForeignKey(i => i.ArticleId);
            modelBuilder.Entity<Club_Article>()
                .HasOne(i => i.Club)
                .WithMany(i => i.Club_Articles)
                .HasForeignKey(i => i.ClubId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Club_Users>().ToTable("Club_Users")
                .HasOne(i => i.User)
                .WithMany(i => i.Club_Users)
                .HasForeignKey(i => i.UserId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Club_Users>()
                .HasOne(i => i.Club)
                .WithMany(i => i.Club_Users)
                .HasForeignKey(i => i.ClubId)
                .OnDelete(DeleteBehavior.NoAction);


            modelBuilder.Entity<Collection_Categories>().ToTable("CollectionCategories")
                .HasOne(i => i.Collection)
                .WithMany(j => j.Collection_Categories)
                .HasForeignKey(j => j.CollectionId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Collection_Categories>()
                .HasOne(i => i.Category)
                .WithMany(j => j.Collection_Categories)
                .HasForeignKey(j => j.CategoryId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Country>().ToTable("Countries");

            modelBuilder.Entity<Critic>().ToTable("Critics")
                .HasOne(i => i.Article)
                .WithMany(j => j.Critics)
                .HasForeignKey(j => j.ArticleId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Critic>()
                .HasOne(i => i.User)
                .WithMany(j => j.Critics)
                .HasForeignKey(j => j.CriticId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<EmailLog>().ToTable("EmailLogs")
                .HasOne(i => i.User)
                .WithMany(j => j.EmailLogs)
                .HasForeignKey(j => j.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EmailRecepients>().ToTable("EmailRecepients")
                .HasOne(i => i.EmailLog)
                .WithMany(j => j.EmailRecepients)
                .HasForeignKey(j => j.EmailLogId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FavoriteAuthor_Users>().ToTable("FavoriteAuthorUsers")
                .HasOne(i => i.Author)
                .WithMany(j => j.Users)
                .HasForeignKey(j => j.UserId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<FavoriteAuthor_Users>()
                .HasOne(i => i.User)
                .WithMany(i => i.Authors)
                .HasForeignKey(i => i.AuthorId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Friends>().ToTable("Friends")
                .HasOne(i => i.User)
                .WithMany(j => j.Friends)
                .HasForeignKey(j => j.UserId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Friends>()
                .HasOne(i => i.Friend)
                .WithMany(j => j.UserFriends)
                .HasForeignKey(j => j.FriendId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Like>().ToTable("Likes")
                .HasOne(i => i.User)
                .WithMany(j => j.Likes)
                .HasForeignKey(i => i.UserId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Like>()
                .HasOne(i => i.User)
                .WithMany(j => j.Likes)
                .HasForeignKey(j => j.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Message>().ToTable("Messages")
                .HasOne(i => i.User)
                .WithMany(j => j.Messages)
                .HasForeignKey(j => j.SenderId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<MessageRecipient>().ToTable("MessageRecipients")
                .HasOne(i => i.Message)
                .WithMany(j => j.MessageRecipients)
                .HasForeignKey(j => j.MessageId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<MessageRecipient>()
                .HasOne(i => i.User)
                .WithMany(j => j.MessageRecipients)
                .HasForeignKey(j => j.RecepientId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<OnlineTime>().ToTable("OnlineTimes")
                .HasOne(i => i.User)
                .WithMany(i => i.OnlineTimes)
                .HasForeignKey(i => i.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<RecommendedAgeGroup>().ToTable("RecommendedAgeGroups");

            modelBuilder.Entity<Session>().ToTable("Sessions")
                .HasOne(i => i.User)
                .WithMany(j => j.Sessions)
                .HasForeignKey(j => j.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<SpamReports>().ToTable("SpamReports")
                .HasOne(i => i.Message)
                .WithMany(j => j.SpamReports)
                .HasForeignKey(j => j.MessageId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<SpamReports>()
                .HasOne(i => i.User)
                .WithMany(j => j.SpamReports)
                .HasForeignKey(j => j.UserId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<SpamReports>()
                .HasOne(i => i.Administrator)
                .WithMany(j => j.SpamReports)
                .HasForeignKey(j => j.AdminId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Stars>().ToTable("Stars")
                .HasOne(i => i.User)
                .WithMany(i => i.Stars)
                .HasForeignKey(j => j.UserId);
            modelBuilder.Entity<Stars>()
                .HasOne(i => i.Article)
                .WithMany(i => i.Stars)
                .HasForeignKey(j => j.ArticleId);

            modelBuilder.Entity<User>()
                .HasOne(i => i.Country)
                .WithMany(j => j.Users)
                .HasForeignKey(j => j.CountryId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<User>()
                .HasOne(i => i.Administrator)
                .WithMany(j => j.Users)
                .HasForeignKey(j => j.DeactivatedByAdminId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<User>()
                .HasOne(i => i.Theme)
                .WithMany(i => i.Users)
                .HasForeignKey(i => i.ThemeId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<UserSettings>().ToTable("UserSettings")
                .HasOne<User>(i => i.User)
                .WithOne(j => j.UserSettings)
                .HasForeignKey<UserSettings>(j => j.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<UserChatSettings>().ToTable("UserChatSettings")
                .HasOne<User>(i => i.User)
                .WithOne(j => j.UserChatSettings)
                .HasForeignKey<UserChatSettings>(j => j.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<UserSession>().ToTable("UserSessions")
                .HasOne(i => i.User)
                .WithOne(j => j.UserSessions)
                .HasForeignKey<UserSession>(j => j.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<UserInRoom>().ToTable("UsersInRoom");

            modelBuilder.Entity<User>().ToTable(name: "Users");
            modelBuilder.Entity<IdentityUserClaim<string>>(entity => { entity.ToTable("UserClaims"); });
            modelBuilder.Entity<IdentityUserLogin<string>>(entity => { entity.ToTable("UserLogins"); });
            modelBuilder.Entity<IdentityUserRole<string>>(entity => { entity.ToTable("UserRoles"); });
            modelBuilder.Entity<IdentityUserToken<string>>(entity => { entity.ToTable("UserTokens"); });
            modelBuilder.Entity<IdentityRole<string>>(entity => { entity.ToTable("Role"); });
            modelBuilder.Entity<IdentityRoleClaim<string>>(entity => { entity.ToTable("RoleClaims"); });
        }
    }
}