using AutoMapper;
using Common.Core.Events;
using FluentValidation;
using MediatR;
using Microsoft.VisualBasic;
using MongoDB.Driver;
using Ticketing.Command.Aplication.Aggregates;
using Ticketing.Command.Domain.Abstracts;
using Ticketing.Command.Domain.EventModels;
using Ticketing.Command.Feature.Apis;

namespace Ticketing.Command.Feature.Tickets;

public sealed class TicketCreate : IMinimalApi
{
    public void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapPost("/api/ticket", async(
            TicketCreateRequest ticketCreateRequest, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var id = Guid.CreateVersion7(DateTimeOffset.UtcNow).ToString();
            var command = new TicketCreateCommand(id,ticketCreateRequest);
            var result = await mediator.Send(command, cancellationToken);

            return Results.Ok(result);

        }).WithName("CreateTicket");
    } 
    public sealed class TicketCreateRequest(string username, int typeError, string detailError)
    {
        public string Username { get; set; } = username;
        public int TypeError { get; set; } = typeError;
        public string DetailError { get; set; } = detailError;
    }

    public record TicketCreateCommand(string Id,TicketCreateRequest ticketCreateRequest) : IRequest<bool>;

    public class TicketCreateCommandValidator : AbstractValidator<TicketCreateCommand>
    {
        public TicketCreateCommandValidator()
        {
            RuleFor(x => x.ticketCreateRequest).SetValidator(new TicketCreateValidator());

            RuleFor(x =>x.Id).NotEmpty().WithMessage("Ingrese el id del evento");
        }
    }

    public class TicketCreateValidator : AbstractValidator<TicketCreateRequest>
    {
        public TicketCreateValidator()
        {
            RuleFor(x => x.Username).NotEmpty().WithMessage("Ingrese un nombre")
            .EmailAddress().WithMessage("debe ser un email");

            RuleFor(x => x.TypeError).NotEmpty().WithMessage("debe existir el tipo de error")
            .InclusiveBetween(1,5).WithMessage("el rango del error es de 1 a 5");

            RuleFor(x => x.DetailError).NotEmpty().WithMessage("Ingrese un detalle de error");
        }
    }

    public sealed class TicketCreateCommandHandler(
        IEventSourcingHandler<TicketAggregate> eventSourcingHandler) 
        : IRequestHandler<TicketCreateCommand, bool>
    {
        private readonly IEventSourcingHandler<TicketAggregate> _eventSourcingHandler = eventSourcingHandler;
        public async Task<bool> Handle(TicketCreateCommand request, CancellationToken cancellationToken)
        {
            var Aggregate = new TicketAggregate(request);

            await _eventSourcingHandler.SaveAsync(Aggregate, cancellationToken);

            return true;
        }
    }

    
}