using Microsoft.Extensions.DependencyInjection;
using VIAPadelClub.Core.Application.CommandDispatching;
using VIAPadelClub.Core.Application.CommandDispatching.Commands.DailySchedule;
using VIAPadelClub.Core.Application.CommandDispatching.Commands.Player;
using VIAPadelClub.Core.Application.Decorator;
using VIAPadelClub.Core.Application.Dispatcher;
using VIAPadelClub.Core.Application.Features.Booking;
using VIAPadelClub.Core.Application.Features.Daily_Schedule;
using VIAPadelClub.Core.Domain.Common;

namespace VIAPadelClub.Core.Application.Extensions;

public static class ApplicationExtensions
{
    public static void RegisterHandlers(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<ICommandHandler<CreateDailyScheduleCommand>, CreateDailyScheduleHandler>();
        serviceCollection.AddSingleton<ICommandHandler<ActivateDailyScheduleCommand>, ActivateDailyScheduleHandler>();
        serviceCollection.AddSingleton<ICommandHandler<AddAvailableCourtCommand>, AddAvailableCourtHandler>();
        serviceCollection.AddSingleton<ICommandHandler<AddVipTimeSlotCommand>, AddVipTimeSlotsHandler>();
        serviceCollection.AddSingleton<ICommandHandler<CreateBookingCommand>, CreateBookingHandler>();
        serviceCollection.AddSingleton<ICommandHandler<PlayerCancelsBookingCommand>, PlayerCancelsBookingHandler>();
        serviceCollection.AddSingleton<ICommandHandler<RemoveAvailableCourtCommand>, RemoveAvailableCourtHandler>();
        serviceCollection.AddSingleton<ICommandHandler<BlacklistsPlayerCommand>, BlacklistsPlayerHandler>();
        serviceCollection.AddSingleton<ICommandHandler<LiftsBlacklistsPlayerCommand>, LiftsBlacklistsPlayerHandler>();
        serviceCollection.AddSingleton<ICommandHandler<CreatePlayerCommand>, CreatePlayerHandler>();
        serviceCollection.AddSingleton<ICommandHandler<DeleteDailyScheduleCommand>, DeleteDailyScheduleCommandHandler>();
        serviceCollection.AddSingleton<ICommandHandler<UpdateDailyScheduleTimeCommand>, UpdateDailyScheduleTimeHandler>();
        serviceCollection.AddSingleton<ICommandHandler<QuarantinesPlayerCommand>, QuarantinesPlayerCommandHandler>();
    }

    public static void RegisterDispatcher(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<ICommandDispatcher>(provider =>
        {
            var dispatcher = new CommandDispatcher(provider);
            var transactionDecorator = new TransactionDecorator(dispatcher, provider.GetRequiredService<IUnitOfWork>());

            return transactionDecorator;
        });
    }
}