using Common.Core.Events;
using MediatR;
using Ticketing.Query.Domain.Abstractions;
using static Ticketing.Query.Features.Tickets.Commands.TicketCreate;
using static Ticketing.Query.Features.Tickets.Commands.TicketUpdate;


namespace Ticketing.Query.Infrastructure.Handlers;

public class EventHandler : IEventHandler
{
    private readonly IMediator _mediator;

    public EventHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task On(TicketCreatedEvent @event)
    {
        var command = new TicketCreateCommand(
         @event.Id,
         @event.Username,
         @event.TypeError,
         @event.DetailError
        );

        await _mediator.Send(command);
    }

    public async Task On(TicketUpdatedEvent @event)
    {
        var ticketUpdatedCommand = new TicketUpdateCommand(
            @event.Id,
            @event.TicketType,
            @event.Description!,
            @event.Username!
        );

        await _mediator.Send(ticketUpdatedCommand);
    }
}
