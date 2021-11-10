using System.Globalization;
using Grpc.Core;
using Northwind.IntegrationEvents.Protobuf.Audit.V1;

namespace AuditCenter.Services;

public class AuditService : Auditor.AuditorBase
{
    private readonly ILogger<AuditService> _logger;

    public AuditService(ILogger<AuditService> logger)
    {
        _logger = logger;
    }

    public override async Task<SubmitAuditResponse> SubmitAudit(SubmitAuditRequest request, ServerCallContext context)
    {
        if (request.Event == "StateMachine.Started")
        {
            _logger.Log(LogLevel.Information, "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX");
        }

        _logger.Log(LogLevel.Information,
            "[{Event}]-[{AuditedAt}] Actor is [{Actor}] with Order is [{CorrelateId}] and Status is [{Status}]",
            request.Event,
            request.AuditedAt.ToDateTime().ToString(CultureInfo.InvariantCulture),
            request.Actor,
            request.CorrelateId,
            request.Status);

        return await Task.FromResult(new SubmitAuditResponse {Done = true});
    }
}
