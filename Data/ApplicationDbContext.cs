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

        public DbSet<RequestType> RequestTypes => Set<RequestType>();

        public DbSet<Status> Statuses => Set<Status>();

        public DbSet<RequestAssignment> RequestAssignments { get; set; }

        public DbSet<RequestApproval> RequestApprovals => Set<RequestApproval>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<RequestType>().HasData(

                new RequestType
                {
                    RequestTypeId = 1,
                    RequestTypeName = "New Website"
                },

                new RequestType
                {
                    RequestTypeId = 2,
                    RequestTypeName = "Enhancement"
                },

                new RequestType
                {
                    RequestTypeId = 3,
                    RequestTypeName = "Bug Fix"
                },

                new RequestType
                {
                    RequestTypeId = 4,
                    RequestTypeName = "Maintenance"
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