using FluentValidation;
using MediatR;
using Ticketing.Command.Application.Aggregates;
using Ticketing.Command.Domain.Abstracts;
using Ticketing.Command.Features.Apis;

namespace Ticketing.Command.Features.Tickets;

public class TicketUpdate : IMinimalApi
{
    public void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
      endpointRouteBuilder.MapPut(
        "/api/ticket/{id}",
        async (
            string id,
            TicketUpdateRequest ticketUpdateRequest,
            IMediator mediator,
            CancellationToken cancellationToken
        ) => {
            var command = new TicketUpdateCommand(id, ticketUpdateRequest);
            var result = await mediator.Send(command, cancellationToken);
            return Results.Ok(result);
        }
      ).WithName("UpdateTicket");
    }

    public sealed class TicketUpdateRequest(
        int ticketType, 
        string description,
        string username
    )
    {
        public int TicketType {get;} = ticketType;
        public string Description {get;} = description;
        public string Username {get;} = username;
    }

    public record TicketUpdateCommand(
        string Id,
        TicketUpdateRequest TicketUpdateRequest
    ): IRequest<bool>;

    public class TicketUpdateCommandValidator 
    : AbstractValidator<TicketUpdateCommand>
    {
        public TicketUpdateCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x=> x.TicketUpdateRequest)
                .SetValidator(new TicketUpdateRequestValidator());
        }
    }

    public class TicketUpdateRequestValidator 
    : AbstractValidator<TicketUpdateRequest>
    {
        public TicketUpdateRequestValidator()
        {
            RuleFor(x => x.Description).NotEmpty()
            .WithMessage("Ingrese una descripcion");

            RuleFor(x => x.Username).NotEmpty()
            .WithMessage("Ingrese un username");

            RuleFor(x => x.TicketType).NotEmpty()
            .WithMessage("ingrese el tipo de ticket")
            .InclusiveBetween(1,5)
            .WithMessage("El tipo ticket puede estar en el rango de 1 a 5");
        }
    }


    public sealed class TicketUpdateCommandHandler(
        IEventSourcingHandler<TicketAggregate> eventSourcingHandler
    ) 
    : IRequestHandler<TicketUpdateCommand, bool>
    {
        private readonly IEventSourcingHandler<TicketAggregate> 
                _eventSourcingHandler = eventSourcingHandler;
        
        public async Task<bool> Handle(
            TicketUpdateCommand request, 
            CancellationToken cancellationToken
        )
        {
            var aggregate = await _eventSourcingHandler
                                    .GetByIdAsync(request.Id, cancellationToken);

            aggregate.EditTicket(
                request.TicketUpdateRequest.TicketType,
                request.TicketUpdateRequest.Description,
                request.TicketUpdateRequest.Username
            );
            
            await _eventSourcingHandler.SaveAsync(aggregate, cancellationToken);
            
            return true;
        }
    }

}
