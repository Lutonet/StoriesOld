using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Stories.Data;
using Stories.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Stories.Services
{
    public class MigratorHostedService : IHostedService
    {
        // We need to inject the IServiceProvider so we can create
        // the scoped service, MyDbContext
        private readonly IServiceProvider _serviceProvider;

        public MigratorHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // Create a new scope to retrieve scoped services
            using (var scope = _serviceProvider.CreateScope())
            {
                // Get the DbContext instance
                var myDbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                //Do the migration asynchronously
                await myDbContext.Database.MigrateAsync();
                List<ChatRooms> roomsToDelete = await myDbContext.ChatRooms.Where(s => s.RoomName != "Living Room").ToListAsync();
                myDbContext.ChatRooms.RemoveRange(roomsToDelete);
                List<ChatRoomAdmins> adminsToDelete = await myDbContext.ChatRoomAdmins.ToListAsync();
                myDbContext.ChatRoomAdmins.RemoveRange(adminsToDelete);
                List<UserInRoom> usersToDelete = await myDbContext.UsersInRooms.ToListAsync();
                myDbContext.UsersInRooms.RemoveRange(usersToDelete);
                List<ChatElevated> chatToDelete = await myDbContext.ChatElevated.ToListAsync();
                myDbContext.ChatElevated.RemoveRange(chatToDelete);
                await myDbContext.SaveChangesAsync();

                List<AnonymousOnline> finishAnonymous = await myDbContext.AnonymousOnline.Where(s => s.IsActive == true).ToListAsync();
                foreach (var anonymous in finishAnonymous)
                {
                    anonymous.IsActive = false;
                }
                await myDbContext.SaveChangesAsync();

                List<Session> sessions = await myDbContext.Sessions.Where(s => s.IsActive == true).ToListAsync();
                foreach (var session in sessions)
                {
                    session.IsActive = false;
                }
                await myDbContext.SaveChangesAsync();

                if (!await myDbContext.Themes.AnyAsync())
                {
                    var themes = new Theme []
                    {
                        new Theme {Name="Light", Description="Default Light Theme"},
                        new Theme {Name="Dark", Description="Default Dark Theme"}
                    };
                    
                    try
                    {
                        await myDbContext.Themes.AddRangeAsync(themes);
                        await myDbContext.SaveChangesAsync();
                        Console.WriteLine("Themes Table created");
                    }
                    catch (Exception ex)
                    {
                        Console.Write(ex.ToString());
                    }
                }
            }
        }

        // noop
        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}