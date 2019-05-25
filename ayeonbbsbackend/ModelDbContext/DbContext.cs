using Microsoft.EntityFrameworkCore;
using ayeonbbsbackend.Models;

namespace ayeonbbsbackend.ModelDbContext
{
    public class DataContext: DbContext
    {
        public DataContext(DbContextOptions<DataContext> options): base(options)
        {
        }

        public DbSet<UserInfo> UserInfos { get; set; }
        public DbSet<SecondCategory> SecondCategories { get; set; }
        public DbSet<FirstCategory> FirstCategories { get; set; }
        public DbSet<PostInfo> PostInfos { get; set; }
        public DbSet<PostGood> PostGoods { get; set; }
        public DbSet<ReplyGood> ReplyGoods { get; set; }
        public DbSet<ReplyInfo> ReplyInfos { get; set; }
        public DbSet<PostManager> PostManagers { get; set; }
        public DbSet<UserPublishCategory> UserPublishCategories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}