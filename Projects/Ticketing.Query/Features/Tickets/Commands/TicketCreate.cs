using MediatR;
using Ticketing.Query.Domain.Abstractions;
using Ticketing.Query.Domain.Employees;
using Ticketing.Query.Domain.Tickets;
using Ticketing.Query.Domain.TicketTypes;

namespace Ticketing.Query.Features.Tickets.Commands;

public sealed class TicketCreate
{
    public record TicketCreateCommand(
        string Id,
        string Username, 
        int TicketTipe,
        string DetailError
    ): IRequest<string>;

    public class TicketCreateCommandHandler
    : IRequestHandler<TicketCreateCommand, string>
    {
        private readonly IUnitOfWork _unitOfWork;

        public TicketCreateCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<string> Handle(
            TicketCreateCommand request, 
            CancellationToken cancellationToken
        )
        {
           //1. inserte data de employee
           var employee = await _unitOfWork
                .EmployeeRepository.GetByUsernameAsync(request.Username);

            if (employee is null)
            {
                employee = Employee.Create(
                   string.Empty, string.Empty, null!, request.Username 
                );
                _unitOfWork.EmployeeRepository.AddEntity(employee);
            }

           //2. inserte data del ticket
           var ticket = Ticket.Create(
             new Guid(request.Id),
             TicketType.Create(request.TicketTipe),
             request.DetailError
           );
            
            _unitOfWork.RepositoyGeneric<Ticket>().AddEntity(ticket);
            
           //3. inserte data del ticketEmployee
             var ticketEmployee = TicketEmployee.Create(ticket, employee);
             _unitOfWork.RepositoyGeneric<TicketEmployee>()
                                    .AddEntity(ticketEmployee);

             await _unitOfWork.Complete(); 

             return Convert.ToString(ticket.Id)!;                      
        }
    }
}
