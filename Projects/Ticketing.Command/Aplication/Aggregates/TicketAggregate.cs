using System.ComponentModel.Design;
using Common.Core.Events;
using Ticketing.Command.Domain.Abstracts;
using static Ticketing.Command.Feature.Tickets.TicketCreate;

namespace Ticketing.Command.Aplication.Aggregates;

public class TicketAggregate : AggregateRoot
{
    public bool Active { get; set; }

    public TicketAggregate(){}
    public TicketAggregate(TicketCreateCommand command)
    {
        var TicketCreatedEvent = new TicketCreatedEvent
        {
            Id = command.Id,
            Username = command.ticketCreateRequest.Username,
            TypeError = command.ticketCreateRequest.TypeError,
            DetailError = command.ticketCreateRequest.DetailError,
        };

        RaiseEvent(TicketCreatedEvent);
    }

    public void Apply(TicketCreatedEvent @event)
    {
        _id = @event.Id;
        Active = true;  
    }
}