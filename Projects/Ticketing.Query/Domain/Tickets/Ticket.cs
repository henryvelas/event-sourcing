using Ticketing.Query.Domain.Abstractions;
using Ticketing.Query.Domain.TicketTypes;

namespace Ticketing.Query.Domain.Tickets;

public class Ticket : Entity
{
    public string? Description { get; set; }
    public virtual TicketType? TicketType { get; set; }
}
