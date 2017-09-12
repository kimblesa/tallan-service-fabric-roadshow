using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AzureRoadshow.VideoStore.Models.EF
{
    public partial class VideoStoreContext : DbContext
    {
        public VideoStoreContext() : base() { }

        public VideoStoreContext(DbContextOptions<VideoStoreContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            #warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
            //optionsBuilder.UseSqlServer(@"##myConnectionString###");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Actor>(entity =>
            {
                entity.Property(e => e.ActorName)
                    .IsRequired()
                    .HasColumnType("varchar(50)");
            });

            modelBuilder.Entity<ActorToVideo>(entity =>
            {
                entity.HasKey(e => e.RelationId)
                    .HasName("PK__ActorToV__E2DA16B5A861BF56");

                entity.HasOne(d => d.Actor)
                    .WithMany(p => p.ActorToVideo)
                    .HasForeignKey(d => d.ActorId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Actor_Video_ToActor");

                entity.HasOne(d => d.Video)
                    .WithMany(p => p.ActorToVideo)
                    .HasForeignKey(d => d.VideoId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Actor_Video_ToVideo");
            });

            modelBuilder.Entity<Address>(entity =>
            {
                entity.Property(e => e.AddressType)
                    .IsRequired()
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.Road)
                    .IsRequired()
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.State)
                    .IsRequired()
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.Town)
                    .IsRequired()
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.ZipCode)
                    .IsRequired()
                    .HasColumnType("varchar(10)");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Address)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Address_ToCustomer");
            });
            
            modelBuilder.Entity<BillingInformation>(entity =>
            {
                entity.HasKey(e => e.BillingId)
                    .HasName("PK__BillingI__F1656DF308E782D6");

                entity.Property(e => e.CreditCard)
                    .IsRequired()
                    .HasColumnType("varchar(16)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(50)");

                entity.HasOne(d => d.Address)
                    .WithMany(p => p.BillingInformation)
                    .HasForeignKey(d => d.AddressId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_BillingInformation_ToAddress");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.Property(e => e.Email).HasColumnType("varchar(50)");

                entity.Property(e => e.UserId).HasMaxLength(500);
            });

            modelBuilder.Entity<FormatToVideo>(entity =>
            {
                entity.HasKey(e => e.RelationId)
                    .HasName("PK__FormatTo__E2DA16B5855750FC");

                entity.HasOne(d => d.Format)
                    .WithMany(p => p.FormatToVideo)
                    .HasForeignKey(d => d.FormatId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Format_Video_ToMovieFormat");

                entity.HasOne(d => d.Inventory)
                    .WithMany(p => p.FormatToVideo)
                    .HasForeignKey(d => d.InventoryId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_FormatToVideo_ToInventory");

                entity.HasOne(d => d.Video)
                    .WithMany(p => p.FormatToVideo)
                    .HasForeignKey(d => d.VideoId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Format_Video_ToVideo");
            });

            modelBuilder.Entity<Inventory>(entity =>
            {
                entity.Property(e => e.Category).HasColumnType("varchar(50)");

                entity.Property(e => e.Description).HasColumnType("varchar(50)");

                entity.Property(e => e.Image).HasColumnType("varchar(200)");

                entity.Property(e => e.Price).HasColumnType("decimal");
            });

            modelBuilder.Entity<Item>(entity =>
            {
                entity.HasKey(e => e.InventoryId)
                    .HasName("PK__Item__F5FDE6B3BA2A4019");

                entity.Property(e => e.InventoryId).ValueGeneratedNever();

                entity.Property(e => e.Description).HasColumnType("varchar(500)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(50)");

                entity.HasOne(d => d.Inventory)
                    .WithOne(p => p.Item)
                    .HasForeignKey<Item>(d => d.InventoryId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Item_ToInventory");
            });

            modelBuilder.Entity<Producer>(entity =>
            {
                entity.Property(e => e.ProducerName)
                    .IsRequired()
                    .HasColumnType("varchar(50)");
            });

            modelBuilder.Entity<ProducerToVideo>(entity =>
            {
                entity.HasKey(e => e.RelationId)
                    .HasName("PK__Producer__E2DA16B505A3475B");

                entity.HasOne(d => d.Producer)
                    .WithMany(p => p.ProducerToVideo)
                    .HasForeignKey(d => d.ProducerId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Producer_Video_ToProducer");

                entity.HasOne(d => d.Video)
                    .WithMany(p => p.ProducerToVideo)
                    .HasForeignKey(d => d.VideoId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Producer_Video_ToVideo");
            });

            modelBuilder.Entity<RentalHistory>(entity =>
            {
                entity.HasKey(e => e.TransactionId)
                    .HasName("PK__RentalHi__55433A6BE1F07039");

                entity.Property(e => e.TransactionId).ValueGeneratedNever();

                entity.Property(e => e.EndDate).HasColumnType("date");

                entity.Property(e => e.StartDate).HasColumnType("date");

                entity.Property(e => e.VideoId)
                    .IsRequired()
                    .HasColumnType("varchar(10)");

                entity.HasOne(d => d.Transaction)
                    .WithOne(p => p.RentalHistory)
                    .HasForeignKey<RentalHistory>(d => d.TransactionId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_RentalHistory_ToTransactionHistory");
            });

            modelBuilder.Entity<Tag>(entity =>
            {
                entity.Property(e => e.TagDescription)
                    .IsRequired()
                    .HasColumnType("varchar(50)");
            });

            modelBuilder.Entity<TagToVideo>(entity =>
            {
                entity.HasKey(e => e.RelationId)
                    .HasName("PK__TagToVid__E2DA16B5C000CB6C");

                entity.HasOne(d => d.Tag)
                    .WithMany(p => p.TagToVideo)
                    .HasForeignKey(d => d.TagId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Tag_Video_ToTag");

                entity.HasOne(d => d.Video)
                    .WithMany(p => p.TagToVideo)
                    .HasForeignKey(d => d.VideoId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Tag_Video_ToVideo");
            });

            modelBuilder.Entity<TransactionHistory>(entity =>
            {
                entity.HasKey(e => e.TransactionId)
                    .HasName("PK__Transact__55433A6B30B5F8CA");

                entity.Property(e => e.Date).HasColumnType("date");
            });

            modelBuilder.Entity<Video>(entity =>
            {
                entity.Property(e => e.Description).HasColumnType("varchar(500)");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasColumnType("varchar(50)");
            });

            modelBuilder.Entity<VideoFormat>(entity =>
            {
                entity.HasKey(e => e.FormatId)
                    .HasName("PK__VideoFor__5D3DCB59447DBEDE");

                entity.Property(e => e.FormatName)
                    .IsRequired()
                    .HasColumnType("varchar(50)");
            });

            modelBuilder.Entity<VideoReview>(entity =>
            {
                entity.HasKey(e => e.ReviewId)
                    .HasName("PK__VideoRev__74BC79CE6C0B2E21");

                entity.Property(e => e.Review).HasColumnType("varchar(500)");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.VideoReview)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_VideoReview_ToCustomer");

                entity.HasOne(d => d.Inventory)
                    .WithMany(p => p.VideoReview)
                    .HasForeignKey(d => d.InventoryId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_VideoReview_ToInventory");
            });
        }

        public virtual DbSet<Actor> Actor { get; set; }
        public virtual DbSet<ActorToVideo> ActorToVideo { get; set; }
        public virtual DbSet<Address> Address { get; set; }
        public virtual DbSet<BillingInformation> BillingInformation { get; set; }
        public virtual DbSet<Customer> Customer { get; set; }
        public virtual DbSet<FormatToVideo> FormatToVideo { get; set; }
        public virtual DbSet<Inventory> Inventory { get; set; }
        public virtual DbSet<Item> Item { get; set; }
        public virtual DbSet<Producer> Producer { get; set; }
        public virtual DbSet<ProducerToVideo> ProducerToVideo { get; set; }
        public virtual DbSet<RentalHistory> RentalHistory { get; set; }
        public virtual DbSet<Tag> Tag { get; set; }
        public virtual DbSet<TagToVideo> TagToVideo { get; set; }
        public virtual DbSet<TransactionHistory> TransactionHistory { get; set; }
        public virtual DbSet<Video> Video { get; set; }
        public virtual DbSet<VideoFormat> VideoFormat { get; set; }
        public virtual DbSet<VideoReview> VideoReview { get; set; }
    }
}