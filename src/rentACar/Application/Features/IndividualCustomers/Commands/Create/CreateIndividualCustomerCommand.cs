using Application.Features.IndividualCustomers.Constants;
using Application.Features.IndividualCustomers.Rules;
using Application.Services.FindeksCreditRateService;
using Application.Services.Repositories;
using AutoMapper;
using Core.Application.Pipelines.Authorization;
using Domain.Entities;
using MediatR;
using static Application.Features.IndividualCustomers.Constants.IndividualCustomersOperationClaims;
using static Domain.Constants.OperationClaims;

namespace Application.Features.IndividualCustomers.Commands.Create;

public class CreateIndividualCustomerCommand : IRequest<CreatedIndividualCustomerResponse>, ISecuredRequest
{
    public int CustomerId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string NationalIdentity { get; set; }

    public string[] Roles => new[] { Domain.Constants.OperationClaims.Admin, IndividualCustomersOperationClaims.Admin, Write, Add };

    public class
        CreateIndividualCustomerCommandHandler : IRequestHandler<CreateIndividualCustomerCommand,
            CreatedIndividualCustomerResponse>
    {
        private readonly IIndividualCustomerRepository _individualCustomerRepository;
        private readonly IMapper _mapper;
        private readonly IndividualCustomerBusinessRules _individualCustomerBusinessRules;
        private readonly IFindeksCreditRateService _findeksCreditRateService;

        public CreateIndividualCustomerCommandHandler(IIndividualCustomerRepository individualCustomerRepository,
                                                      IMapper mapper,
                                                      IndividualCustomerBusinessRules individualCustomerBusinessRules,
                                                      IFindeksCreditRateService findeksCreditRateService)
        {
            _individualCustomerRepository = individualCustomerRepository;
            _mapper = mapper;
            _individualCustomerBusinessRules = individualCustomerBusinessRules;
            _findeksCreditRateService = findeksCreditRateService;
        }

        public async Task<CreatedIndividualCustomerResponse> Handle(CreateIndividualCustomerCommand request,
                                                               CancellationToken cancellationToken)
        {
            await _individualCustomerBusinessRules.IndividualCustomerNationalIdentityCanNotBeDuplicatedWhenInserted(
                request.NationalIdentity);

            IndividualCustomer mappedIndividualCustomer = _mapper.Map<IndividualCustomer>(request);
            IndividualCustomer createdIndividualCustomer =
                await _individualCustomerRepository.AddAsync(mappedIndividualCustomer);

            await _findeksCreditRateService.Add(new FindeksCreditRate
            { CustomerId = createdIndividualCustomer.CustomerId });


            CreatedIndividualCustomerResponse createdIndividualCustomerDto =
                _mapper.Map<CreatedIndividualCustomerResponse>(createdIndividualCustomer);
            return createdIndividualCustomerDto;
        }
    }
}