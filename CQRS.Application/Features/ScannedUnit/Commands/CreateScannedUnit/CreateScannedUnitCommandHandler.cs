using AutoMapper;
using CQRS.Application.Contracts.Interface;
using CQRS.Application.Shared.Response;
using MediatR;
using Microsoft.Extensions.Logging;
namespace CQRS.Application.Features.ScannedUnit.Commands.CreateScannedUnit
{
    public class CreateScannedUnitCommandHandler(
        IMapper mapper,
        IScannedUnitsRepository scannedUnitsRepository,
        IMainSerialRepository mainSerialRepository,
        ILogger<CreateScannedUnitCommandHandler> logger
    ) : IRequestHandler<CreateScannedUnitCommand, CustomResultResponse>
    {
        public async Task<CustomResultResponse> Handle(
            CreateScannedUnitCommand request,
            CancellationToken cancellationToken)
        {
            // Validation is being handled by ValidationHandlingMiddleware.
            var contractNo = await scannedUnitsRepository.GetMainSerialContractNo(request.MainSerial);
            var jobOrderNo = await scannedUnitsRepository.GetJobOrderNumberAsync(contractNo.BatchSerial_ContractNo);
            var scannedUnit = mapper.Map<Domain.ScannedUnits>(request);
            scannedUnit.ContractNo = contractNo.BatchSerial_ContractNo;
            scannedUnit.JoNo = jobOrderNo;
            await scannedUnitsRepository.CreateAsync(scannedUnit);
            await mainSerialRepository.UpdateSerialJoNo(scannedUnit.MainSerial, scannedUnit.JoNo);
            logger.LogInformation(
                "Scanned unit created successfully: MainSerial={MainSerial}",
                scannedUnit.MainSerial);
            return CustomResultResponse.Success(
                "Scanned unit created successfully",
                scannedUnit.Id.ToString());
        }
    }
}