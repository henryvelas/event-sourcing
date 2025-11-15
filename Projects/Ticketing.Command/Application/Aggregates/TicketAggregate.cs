
using Common.Core.Events;
using Ticketing.Command.Domain.Abstracts;
using static Ticketing.Command.Features.Tickets.TicketCreate;

namespace Ticketing.Command.Application.Aggregates;

public class TicketAggregate : AggregateRoot
{
    public bool Active {get;set;}

    public TicketAggregate(){}
     
    public TicketAggregate(TicketCreateCommand command)
    {
        var ticketCreatedEvent = new TicketCreatedEvent
        {
            Id = command.Id, 
            Username = command.ticketCreateRequest.Username, 
            TypeError = command.ticketCreateRequest.TypeError, 
            DetailError = command.ticketCreateRequest.DetailError,
        };

        RaiseEvent(ticketCreatedEvent);
    }

    public void Apply(TicketCreatedEvent @event)
    {
        _id = @event.Id;
        Active = true;
    }

    public void EditTicket(int ticketType, string description, string username)
    {
        if(!Active)
        {
            throw new InvalidOperationException(
                "No puede editar un ticket que no esta activo"
            );
        }
        
        RaiseEvent(new TicketUpdatedEvent{
            Id = Id,
            TicketType = ticketType,
            Description = description,
            Username = username,
        });
        
    }

    public void Apply(TicketUpdatedEvent @event)
    {
        _id=@event.Id;
    }

    

}