using Microsoft.Extensions.DependencyInjection;
using VIAPadelClub.Core.Application.CommandDispatching;
using VIAPadelClub.Core.Application.CommandDispatching.Commands.DailySchedule;
using VIAPadelClub.Core.Application.CommandDispatching.Commands.Player;
using VIAPadelClub.Core.Application.Decorator;
using VIAPadelClub.Core.Application.Dispatcher;
using VIAPadelClub.Core.Application.Features.Booking;
using VIAPadelClub.Core.Application.Features.Daily_Schedule;
using VIAPadelClub.Core.Domain.Common;
using VIAPadelClub.Core.Domain.Common.Repositories;

namespace VIAPadelClub.Core.Application.Extensions;

public static class ApplicationExtensions
{
    public static void RegisterCommandHandlers(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<ICommandHandler<CreateDailyScheduleCommand>, CreateDailyScheduleHandler>();
        serviceCollection.AddScoped<ICommandHandler<ActivateDailyScheduleCommand>, ActivateDailyScheduleHandler>();
        serviceCollection.AddScoped<ICommandHandler<AddAvailableCourtCommand>, AddAvailableCourtHandler>();
        serviceCollection.AddScoped<ICommandHandler<AddVipTimeSlotCommand>, AddVipTimeSlotsHandler>();
        serviceCollection.AddScoped<ICommandHandler<CreateBookingCommand>, CreateBookingHandler>();
        serviceCollection.AddScoped<ICommandHandler<PlayerCancelsBookingCommand>, PlayerCancelsBookingHandler>();
        serviceCollection.AddScoped<ICommandHandler<RemoveAvailableCourtCommand>, RemoveAvailableCourtHandler>();
        serviceCollection.AddScoped<ICommandHandler<BlacklistsPlayerCommand>, BlacklistsPlayerHandler>();
        serviceCollection.AddScoped<ICommandHandler<LiftsBlacklistsPlayerCommand>, LiftsBlacklistsPlayerHandler>();
        serviceCollection.AddScoped<ICommandHandler<CreatePlayerCommand>, CreatePlayerHandler>();
        serviceCollection.AddScoped<ICommandHandler<DeleteDailyScheduleCommand>, DeleteDailyScheduleCommandHandler>();
        serviceCollection.AddScoped<ICommandHandler<UpdateDailyScheduleTimeCommand>, UpdateDailyScheduleTimeHandler>();
        serviceCollection.AddScoped<ICommandHandler<QuarantinesPlayerCommand>, QuarantinesPlayerCommandHandler>();
    }

    public static void RegisterCommandDispatcher(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<ICommandDispatcher>(provider =>
        {
            var dispatcher = new CommandDispatcher(provider);
            var transactionDecorator = new TransactionDecorator(dispatcher, provider.GetRequiredService<IUnitOfWork>());

            return transactionDecorator;
        });
    }
}