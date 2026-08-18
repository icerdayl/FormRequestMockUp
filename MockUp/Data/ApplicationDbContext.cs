using Microsoft.EntityFrameworkCore;
using RequestForm.Models;

namespace RequestForm.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Request> Requests => Set<Request>();

        public DbSet<TicketType> TicketTypes => Set<TicketType>();

        public DbSet<Status> Statuses => Set<Status>();

        public DbSet<RequestAssignment> RequestAssignments { get; set; }

        public DbSet<RequestApproval> RequestApprovals => Set<RequestApproval>();

        public DbSet<Feature> Features => Set<Feature>();

        public DbSet<SubTask> SubTasks => Set<SubTask>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // SubTask has two paths back to Request: directly, and
            // through Feature. If both cascade-deleted, SQL Server
            // rejects the schema ("multiple cascade paths"). The
            // Feature -> SubTask cascade (default, since FeatureId
            // is required) already deletes a Request's SubTasks
            // transitively when the Request is deleted via its
            // Features, so the direct Request -> SubTask FK only
            // needs to Restrict, not cascade.
            modelBuilder.Entity<SubTask>()
                .HasOne(s => s.Request)
                .WithMany()
                .HasForeignKey(s => s.RequestId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TicketType>().HasData(

                new TicketType
                {
                    TicketTypeId = 1,
                    TicketTypeName = "Bug"
                },

                new TicketType
                {
                    TicketTypeId = 2,
                    TicketTypeName = "Feature Request"
                },

                new TicketType
                {
                    TicketTypeId = 3,
                    TicketTypeName = "Enhancement"
                },

                new TicketType
                {
                    TicketTypeId = 4,
                    TicketTypeName = "Maintenance"
                },

                new TicketType
                {
                    TicketTypeId = 5,
                    TicketTypeName = "Technical Support"
                },

                new TicketType
                {
                    TicketTypeId = 6,
                    TicketTypeName = "Change Request"
                }

            );

            modelBuilder.Entity<Status>().HasData(

                new Status
                {
                    StatusId = 1,
                    StatusName = "Pending"
                },

                new Status
                {
                    StatusId = 2,
                    StatusName = "Approved by Help Desk"
                },

                new Status
                {
                    StatusId = 3,
                    StatusName = "Approved by Supervisor"
                },

                new Status
                {
                    StatusId = 4,
                    StatusName = "Approved by Manager"
                },

                new Status
                {
                    StatusId = 5,
                    StatusName = "Rejected"
                },

                new Status
                {
                    StatusId = 6,
                    StatusName = "In Progress"
                },

                new Status
                {
                    StatusId = 7,
                    StatusName = "Completed"
                }
            );
        }
    }
}