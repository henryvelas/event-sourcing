using MediatR;
using Ticketing.Query.Domain.Abstractions;
using Ticketing.Query.Domain.Employees;
using Ticketing.Query.Domain.Tickets;
using Ticketing.Query.Domain.TicketTypes;

namespace Ticketing.Query.Features.Tickets.Commands;

public class TicketUpdate
{

    public record TicketUpdateCommand(
        string Id,
        int TicketType,
        string Description,
        string Username
    ) : IRequest<string>;


    public class TicketUpdateCommandHandler
    : IRequestHandler<TicketUpdateCommand, string>
    {
        private readonly IUnitOfWork _unitOfWork;

        public TicketUpdateCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<string> Handle(
            TicketUpdateCommand request, 
            CancellationToken cancellationToken
        )
        {
           var ticket = await _unitOfWork
                        .RepositoyGeneric<Ticket>()
                        .GetByIdAsync(new Guid(request.Id));

           if(ticket is null)
           {
             throw new Exception("No se encontro el ticket en la base de datos");
           }
           
           
           var employee = await _unitOfWork
                                    .EmployeeRepository
                                    .GetByUsernameAsync(request.Username);

          if(employee is null)
          {
            employee = Employee.Create(
                string.Empty, string.Empty, null!, request.Username
            );

            _unitOfWork.EmployeeRepository.AddEntity(employee);
          }
    
           ticket.Description = request.Description;
           ticket.TicketType =  TicketType.Create(request.TicketType);      
        
           await _unitOfWork.Complete();

           return Convert.ToString(ticket.Id)!;
        }
    }
}
