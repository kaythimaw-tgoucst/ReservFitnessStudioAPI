using FitnessStudio.Application.Interfaces.Gateways;
using FitnessStudio.Infrastructure.DataAccess.Entities;
using FitnessStudio.Infrastructure.DataAccessWriteOnly;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FitnessStudio.Infrastructure.Seed
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<FitnessStudioWriteDbContext>();
            var dateTimeProvider = scope.ServiceProvider.GetRequiredService<IDateTimeProvider>();

            if (await context.TUsers.AnyAsync() || await context.TBusinessStudios.AnyAsync())
            {
                return;
            }

            var now = dateTimeProvider.GetCurrentDateUTC();

            var company = new TCompany
            {
                Id = Guid.NewGuid(),
                Name = "Infinity Fitness Group",
                CreatedOn = now
            };

            var company2 = new TCompany
            {
                Id = Guid.NewGuid(),
                Name = "Anyone Fitness Group",
                CreatedOn = now
            };

            var studios = new List<TBusinessStudio>
            {
                new() { Id = Guid.NewGuid(), CompanyId = company.Id, Name = "Infinity Fitness Branch 1", CreatedOn = now },
                new() { Id = Guid.NewGuid(), CompanyId = company.Id, Name = "Infinity Fitness Branch 2", CreatedOn = now },
                new() { Id = Guid.NewGuid(), CompanyId = company2.Id, Name = "Anyone Fitness", CreatedOn = now }
            };

            var users = Enumerable.Range(1, 10)
                .Select(i => new TUser
                {
                    Id = Guid.NewGuid(),
                    Name = $"User {i}",
                    Email = $"kaythimaw.user{i}@fitness.local",
                    CreatedOn = now
                })
                .ToList();

            var loginUser = new TUser
            {
                Id = Guid.NewGuid(),
                Name = "Kay Thimaw Fitness",
                Email = "kaythimaw.fitness@yopmail.com",
                CreatedOn = now
            };

            var userBusinessStudioMemberships = new List<TUserBusinessStudioMembership>();
            userBusinessStudioMemberships.AddRange(users.Select(u => new TUserBusinessStudioMembership
            {
                UserId = u.Id,
                BusinessStudioId = studios[0].Id,
                Role = "Member",
                IsActive = true,
                CreatedOn = now
            }));

            users.Add(loginUser);

            // user1 under another studio
            userBusinessStudioMemberships.Add(new TUserBusinessStudioMembership
            {
                UserId = users[0].Id,
                BusinessStudioId = studios[1].Id,
                Role = "Member",
                IsActive = true,
                CreatedOn = now
            });

            // kaythimaw.fitness@yopmail.com under all businesses/studios
            userBusinessStudioMemberships.AddRange(studios.Select(s => new TUserBusinessStudioMembership
            {
                UserId = loginUser.Id,
                BusinessStudioId = s.Id,
                Role = "Member",
                IsActive = true,
                CreatedOn = now
            }));

            // user2 under another studio
            userBusinessStudioMemberships.Add(new TUserBusinessStudioMembership
            {
                UserId = users[1].Id,
                BusinessStudioId = studios[2].Id,
                Role = "Member",
                IsActive = true,
                CreatedOn = now
            });

            // user3 under all studios
            userBusinessStudioMemberships.AddRange(
                new TUserBusinessStudioMembership
                {
                    UserId = users[2].Id,
                    BusinessStudioId = studios[1].Id,
                    Role = "Member",
                    IsActive = true,
                    CreatedOn = now
                },
                new TUserBusinessStudioMembership
                {
                    UserId = users[2].Id,
                    BusinessStudioId = studios[2].Id,
                    Role = "Member",
                    IsActive = true,
                    CreatedOn = now
                });

            var schedules = new List<TTimetableSchedule>
            {
                // studios[0] - 5
                new() { Id = Guid.NewGuid(), BusinessStudioId = studios[0].Id, ClassName = "Gym", InstructorName = "John Doe", StartTime = now.AddDays(1).Date.AddHours(7), EndTime = now.AddDays(1).Date.AddHours(8), Capacity = 2, CreatedOn = now },
                new() { Id = Guid.NewGuid(), BusinessStudioId = studios[0].Id, ClassName = "Yoga", InstructorName = "Alice Tan", StartTime = now.AddDays(1).Date.AddHours(18), EndTime = now.AddDays(1).Date.AddHours(19), Capacity = 3, CreatedOn = now },
                new() { Id = Guid.NewGuid(), BusinessStudioId = studios[0].Id, ClassName = "Cardio", InstructorName = "Marcus Lim", StartTime = now.AddDays(2).Date.AddHours(12), EndTime = now.AddDays(2).Date.AddHours(13), Capacity = 4, CreatedOn = now },
                new() { Id = Guid.NewGuid(), BusinessStudioId = studios[0].Id, ClassName = "Pilates", InstructorName = "Siti Noor", StartTime = now.AddDays(3).Date.AddHours(9), EndTime = now.AddDays(3).Date.AddHours(10), Capacity = 5, CreatedOn = now },
                new() { Id = Guid.NewGuid(), BusinessStudioId = studios[0].Id, ClassName = "Zumba", InstructorName = "Daniel Low", StartTime = now.AddDays(4).Date.AddHours(20), EndTime = now.AddDays(4).Date.AddHours(21), Capacity = 5, CreatedOn = now },

                // studios[1] - 4
                new() { Id = Guid.NewGuid(), BusinessStudioId = studios[1].Id, ClassName = "Gym", InstructorName = "Priya Nair", StartTime = now.AddDays(1).Date.AddHours(8), EndTime = now.AddDays(1).Date.AddHours(9), Capacity = 2, CreatedOn = now },
                new() { Id = Guid.NewGuid(), BusinessStudioId = studios[1].Id, ClassName = "Cardio", InstructorName = "Ben Chua", StartTime = now.AddDays(2).Date.AddHours(18), EndTime = now.AddDays(2).Date.AddHours(19), Capacity = 3, CreatedOn = now },
                new() { Id = Guid.NewGuid(), BusinessStudioId = studios[1].Id, ClassName = "Kickboxing", InstructorName = "Ella Ng", StartTime = now.AddDays(3).Date.AddHours(7), EndTime = now.AddDays(3).Date.AddHours(8), Capacity = 4, CreatedOn = now },
                new() { Id = Guid.NewGuid(), BusinessStudioId = studios[1].Id, ClassName = "Dance", InstructorName = "Tom Goh", StartTime = now.AddDays(5).Date.AddHours(17), EndTime = now.AddDays(5).Date.AddHours(18), Capacity = 5, CreatedOn = now },

                // studios[2] - 2
                new() { Id = Guid.NewGuid(), BusinessStudioId = studios[2].Id, ClassName = "Gym", InstructorName = "Iris Tan", StartTime = now.AddDays(2).Date.AddHours(19), EndTime = now.AddDays(2).Date.AddHours(20), Capacity = 10, CreatedOn = now },
                new() { Id = Guid.NewGuid(), BusinessStudioId = studios[2].Id, ClassName = "Zumba", InstructorName = "Nora Lim", StartTime = now.AddDays(6).Date.AddHours(10), EndTime = now.AddDays(6).Date.AddHours(11), Capacity = 20, CreatedOn = now }
            };

            var packages = new List<TPackage>
            {
                new() { Id = Guid.NewGuid(), UserId = users[0].Id, BusinessStudioId = studios[0].Id, TotalCredits = 10, RemainingCredits = 8, ExpiryDate = now.AddMonths(2), CreatedOn = now },
                new() { Id = Guid.NewGuid(), UserId = users[1].Id, BusinessStudioId = studios[1].Id, TotalCredits = 20, RemainingCredits = 20, ExpiryDate = now.AddMonths(3), CreatedOn = now },
                new() { Id = Guid.NewGuid(), UserId = users[2].Id, BusinessStudioId = studios[2].Id, TotalCredits = 15, RemainingCredits = 14, ExpiryDate = now.AddMonths(1), CreatedOn = now },
                new() { Id = Guid.NewGuid(), UserId = users[3].Id, BusinessStudioId = studios[0].Id, TotalCredits = 8, RemainingCredits = 8, ExpiryDate = now.AddMonths(1), CreatedOn = now }
            };

            context.TCompanies.AddRange(company, company2);
            context.TBusinessStudios.AddRange(studios);
            context.TUsers.AddRange(users);
            context.TUserBusinessStudioMemberships.AddRange(userBusinessStudioMemberships);
            context.TTimetableSchedules.AddRange(schedules);
            context.TPackages.AddRange(packages);

            await context.SaveChangesAsync();
        }
    }
}
