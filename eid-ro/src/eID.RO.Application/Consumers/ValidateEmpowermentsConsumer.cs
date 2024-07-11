using eID.RO.Contracts.Commands;
using eID.RO.Contracts.Enums;
using eID.RO.Contracts.Events;
using eID.RO.Service;
using eID.RO.Service.Interfaces;
using MassTransit;

namespace eID.RO.Application.Consumers;

public class ValidateEmpowermentsConsumer : BaseConsumer,
    IConsumer<ValidateLegalEntityEmpowerment>,
    IConsumer<VerifyLegalEntityWithdrawalRequester>
{
    private readonly IVerificationService _verificationService;
    private readonly EmpowermentsService _empowermentsService;

    //At this point we know those statuses:
    //N - нова
    //Е - Пререгистрирана фирма по Булстат
    //C - Нова партида затворена
    //L - Пререгистрирана фирма по Булстат затворена
    private readonly string[] _notActiveStatuses = { "C", "L" };
    private readonly string[] _checkInBulstatStatuses = { "E", "L" };

    public ValidateEmpowermentsConsumer(
        ILogger<ValidateEmpowermentsConsumer> logger,
        IVerificationService verificationService,
        EmpowermentsService empowermentsService) : base(logger)
    {
        _verificationService = verificationService ?? throw new ArgumentNullException(nameof(verificationService));
        _empowermentsService = empowermentsService ?? throw new ArgumentNullException(nameof(empowermentsService));
    }

    public async Task Consume(ConsumeContext<ValidateLegalEntityEmpowerment> context)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        var empowermentStatement = await _empowermentsService.GetEmpowermentStatementByIdAsync(context.Message.EmpowermentId);
        if (empowermentStatement is null)
        {
            await context.RespondAsync<LegalEntityEmpowermentValidationFailed>(new
            {
                context.Message.CorrelationId,
                context.Message.EmpowermentId,
                DenialReason = EmpowermentsDenialReason.EmpowermentStatementNotFound
            });

            return;
        }

        var legalEntityActualState = await _verificationService.GetLegalEntityActualStateAsync(empowermentStatement.Uid);

        //If Legal Entity Restrictions check failed, respond with Denial Reason
        if (legalEntityActualState?.Result?.Response?.ActualStateResponseV3?.Deed is null)
        {
            await context.RespondAsync<LegalEntityNotPresentInNTR>(new
            {
                context.Message.CorrelationId,
                context.Message.EmpowermentId
            });
            return;
        }

        var legalEntityState = legalEntityActualState?.Result?.Response;

        var validAuthorizers = await _empowermentsService.ConfirmAuthorizersInLegalEntityRepresentationAsync(context.Message.EmpowermentId, legalEntityActualState.Result);
        if (!validAuthorizers)
        {
            await context.RespondAsync<LegalEntityEmpowermentValidationFailed>(new
            {
                context.Message.CorrelationId,
                context.Message.EmpowermentId,
                DenialReason = EmpowermentsDenialReason.LegalEntityRepresentationNotMatch,
            });

            return;
        }

        //Not active - Legal Entity is not active in TR and we have to deny the empowerment
        //Empowerment status: Denied with Reason: LegalEntityNotActive
        var legalEntityStatus = legalEntityState?.ActualStateResponseV3?.Deed?.DeedStatus;
        if (!string.IsNullOrWhiteSpace(legalEntityStatus) && _notActiveStatuses.Contains(legalEntityStatus))
        {
            if (_checkInBulstatStatuses.Contains(legalEntityStatus))
            {
                await context.RespondAsync<LegalEntityNotPresentInNTR>(new
                {
                    context.Message.CorrelationId,
                    context.Message.EmpowermentId
                });
                return;
            }

            if (_notActiveStatuses.Contains(legalEntityStatus))
            {
                await context.RespondAsync<LegalEntityEmpowermentValidationFailed>(new
                {
                    context.Message.CorrelationId,
                    context.Message.EmpowermentId,
                    DenialReason = EmpowermentsDenialReason.LegalEntityNotActive,
                });
                return;
            }
        }

        var issuer = context.Message.AuthorizerUids.FirstOrDefault(a => a.IsIssuer);
        var verificationResult = VerificationService.CalculateVerificationResult(new CheckLegalEntityInNTRData
        {
            CorrelationId = context.Message.CorrelationId,
            Uid = empowermentStatement.Uid,                                        // Eik of the legal entity. Used to match with data in TR.
            Name = empowermentStatement.Name,                                      // Name of the legal entity. Used to match with data in TR.
            IssuerName = issuer?.Name,                                             // Name of the requester. Used to match with data in TR.
            IssuerPosition = nameof(CheckLegalEntityInNTRData.IssuerPosition),     // TODO: Not used at the moment
            IssuerUid = issuer?.Uid,                                               // Uid of the requester. Used to match with data in TR.
            IssuerUidType = issuer.UidType,                                        // UidType of the requester. Used to match with data in TR.
        }, legalEntityActualState.Result);

        if (!verificationResult.Successfull)
        {
            await context.RespondAsync<LegalEntityEmpowermentValidationFailed>(new
            {
                context.Message.CorrelationId,
                context.Message.EmpowermentId,
                DenialReason = EmpowermentsDenialReason.NTRCheckFailed,
            });

            return;
        }

        await context.RespondAsync<LegalEntityEmpowermentValidated>(new
        {
            context.Message.CorrelationId,
            context.Message.EmpowermentId
        });
    }

    public async Task Consume(ConsumeContext<VerifyLegalEntityWithdrawalRequester> context)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        var empowermentStatement = await _empowermentsService.GetEmpowermentStatementByIdAsync(context.Message.EmpowermentId);
        var activeEmpowermentStatuses = new EmpowermentStatementStatus[] {
            EmpowermentStatementStatus.Active,
            EmpowermentStatementStatus.Unconfirmed
        };
        if (empowermentStatement is null || !activeEmpowermentStatuses.Contains(empowermentStatement.Status))
        {
            await context.RespondAsync<VerifyLegalEntityWithdrawalRequesterFailed>(new
            {
                context.Message.CorrelationId,
                context.Message.EmpowermentId
            });
            return;
        }

        var legalEntityActualState = await _verificationService.GetLegalEntityActualStateAsync(empowermentStatement.Uid);
        if (legalEntityActualState?.Result?.Response?.ActualStateResponseV3?.Deed is null)
        {
            await context.RespondAsync<LegalEntityNotPresentInNTR>(new
            {
                context.Message.CorrelationId,
                context.Message.EmpowermentId
            });
            return;
        }

        var legalEntityDeedStatus = legalEntityActualState?.Result?.Response?.ActualStateResponseV3?.Deed?.DeedStatus;
        if (!string.IsNullOrWhiteSpace(legalEntityDeedStatus) && _checkInBulstatStatuses.Contains(legalEntityDeedStatus))
        {
            await context.RespondAsync<LegalEntityNotPresentInNTR>(new
            {
                context.Message.CorrelationId,
                context.Message.EmpowermentId
            });
            return;
        }

        var verificationResult = VerificationService.CalculateVerificationResult(new CheckLegalEntityInNTRData
        {
            CorrelationId = context.Message.CorrelationId,
            Uid = empowermentStatement.Uid,                                        // Eik of the legal entity. Used to match with data in TR.
            Name = empowermentStatement.Name,                                      // Name of the legal entity. Used to match with data in TR.
            IssuerName = context.Message.IssuerName,                               // Name of the requester. Used to match with data in TR.
            IssuerPosition = nameof(CheckLegalEntityInNTRData.IssuerPosition),     // TODO: Not used at the moment
            IssuerUid = context.Message.IssuerUid,                                 // Uid of the requester. Used to match with data in TR.
            IssuerUidType = context.Message.IssuerUidType,                         // UidType of the requester. Used to match with data in TR.
        }, legalEntityActualState.Result);

        if (!verificationResult.Successfull)
        {
            await context.RespondAsync<VerifyLegalEntityWithdrawalRequesterFailed>(new
            {
                context.Message.CorrelationId,
                context.Message.EmpowermentId
            });
            return;
        }

        await context.RespondAsync<VerifyLegalEntityWithdrawalRequesterSucceeded>(new
        {
            context.Message.CorrelationId,
            context.Message.EmpowermentId
        });
    }
}
