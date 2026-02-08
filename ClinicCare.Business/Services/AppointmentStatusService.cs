using ClinicCare.DataAccess.Data;
using ClinicCare.DataAccess.Repositories.Interfaces;
using ClinicCare.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ClinicCare.Business.Services
{
    public class AppointmentStatusService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AppointmentStatusService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromHours(3);

        public AppointmentStatusService(
            IServiceProvider serviceProvider,
            ILogger<AppointmentStatusService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("AppointmentStatusService started at {Time}", DateTime.Now);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessPendingAppointments();
                    _logger.LogInformation("Appointment status check completed at {Time}", DateTime.Now);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing appointment statuses at {Time}", DateTime.Now);
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }
        }

        private async Task ProcessPendingAppointments()
        {
            using var scope = _serviceProvider.CreateScope();
            var appointmentRepo = scope.ServiceProvider.GetRequiredService<IAppointmentRepository>();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var now = DateTime.Now;
            var today = DateOnly.FromDateTime(now);
            var twentyFourHoursAgo = now.AddHours(-24);

            var overdueRequested = await dbContext.Appointments
                .Where(a => a.Date == today
                         && a.Status == AppointmentStatus.Requested
                         && EF.Functions.DateDiffHour(a.CreatedAt, now) > 24) 
                .ToListAsync();

            foreach (var apt in overdueRequested)
            {
                apt.Status = AppointmentStatus.Cancelled;
                _logger.LogInformation("Auto-cancelled REQUESTED {Id}", apt.Id);
            }

            var oldApproved = await dbContext.Appointments
                .Where(a => a.Status == AppointmentStatus.Approved
                         && a.PrescriptionId == null
                         && a.CreatedAt < twentyFourHoursAgo)
                .ToListAsync();

            foreach (var apt in oldApproved)
            {
                apt.Status = AppointmentStatus.Cancelled;
                _logger.LogInformation("Auto-cancelled APPROVED {Id}", apt.Id);
            }

            if (overdueRequested.Any() || oldApproved.Any())
            {
                await dbContext.SaveChangesAsync();
                _logger.LogInformation("Saved {Requested} requested + {Approved} approved cancellations",
                    overdueRequested.Count, oldApproved.Count);
            }
        }
    }
}
