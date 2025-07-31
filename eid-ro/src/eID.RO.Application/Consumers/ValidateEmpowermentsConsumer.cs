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

        var legalEntityActualState = await _verificationService.GetLegalEntityActualStateAsync(context.Message.CorrelationId, empowermentStatement.Uid);

        //If Legal Entity Restrictions check failed, respond with Denial Reason
        //Error while getting Legal Entity actual state in TR
        if (legalEntityActualState is null || legalEntityActualState.Result is null || legalEntityActualState?.Result?.Response?.ActualStateResponseV3?.Deed is null)
        {
            await context.RespondAsync<LegalEntityNotPresentInNTR>(new
            {
                context.Message.CorrelationId,
                context.Message.EmpowermentId,
                MissingOrMalformedResponse = true
            });
            return;
        }

        var legalEntity = legalEntityActualState.Result;
        var representativesDataIsAvailable = legalEntity.ContainsRepresentativesData();

        //Confirm Authorizers in Legal Entity
        var someAuthorizersAreNotRepresenters = empowermentStatement.AuthorizerUids.Any(authorizer => !legalEntity.IsAmongRepresentatives(authorizer.Uid, authorizer.Name));
        if (representativesDataIsAvailable && someAuthorizersAreNotRepresenters)
        {
            await context.RespondAsync<LegalEntityEmpowermentValidationFailed>(new
            {
                context.Message.CorrelationId,
                context.Message.EmpowermentId,
                DenialReason = EmpowermentsDenialReason.LegalEntityRepresentationNotMatch
            });

            return;
        }

        if (legalEntity.HasValidDeedStatus())
        {
            if (legalEntity.IsToBeCheckedInBulstat())
            {
                await context.RespondAsync<LegalEntityNotPresentInNTR>(new
                {
                    context.Message.CorrelationId,
                    context.Message.EmpowermentId
                });
                return;
            }
            //Not active - Legal Entity is not active in TR and we have to deny the empowerment
            //Empowerment status: Denied with Reason: LegalEntityNotActive
            if (legalEntity.IsInactive())
            {
                await context.RespondAsync<LegalEntityEmpowermentValidationFailed>(new
                {
                    context.Message.CorrelationId,
                    context.Message.EmpowermentId,
                    DenialReason = EmpowermentsDenialReason.LegalEntityNotActive
                });
                return;
            }
        }

        //Confirm legal entity name and issuer
        var issuer = context.Message.AuthorizerUids.FirstOrDefault(a => a.IsIssuer);
        if (issuer is null)
        {
            // Since we don't store IsIssuer data in Authorizers, during EmpowermentValidation process we can't retrieve it.
            // We have it in the XML, so we fallback to getting it from there.
            var deserializedXML = XMLSerializationHelper.DeserializeEmpowermentStatementItem(empowermentStatement.XMLRepresentation);
            issuer = deserializedXML.AuthorizerUids.FirstOrDefault(a => a.IsIssuer);
            if (issuer is null)
            {
                await context.RespondAsync<LegalEntityEmpowermentValidationFailed>(new
                {
                    context.Message.CorrelationId,
                    context.Message.EmpowermentId,
                    DenialReason = EmpowermentsDenialReason.NTRCheckFailed
                });
                return;
            }
        }
        var incorrectCompanyData = !legalEntity.MatchCompanyData(empowermentStatement.Name, empowermentStatement.Uid);
        var issuerIsNotRepresenter = !legalEntity.IsAmongRepresentatives(issuer.Uid, issuer.Name);
        if (incorrectCompanyData || (representativesDataIsAvailable && issuerIsNotRepresenter))
        {
            await context.RespondAsync<LegalEntityEmpowermentValidationFailed>(new
            {
                context.Message.CorrelationId,
                context.Message.EmpowermentId,
                DenialReason = EmpowermentsDenialReason.NTRCheckFailed
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
            EmpowermentStatementStatus.CollectingAuthorizerSignatures,
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

        var legalEntityActualState = await _verificationService.GetLegalEntityActualStateAsync(context.Message.CorrelationId, empowermentStatement.Uid);
        if (legalEntityActualState is null || legalEntityActualState.Result is null || legalEntityActualState?.Result?.Response?.ActualStateResponseV3?.Deed is null)
        {
            await context.RespondAsync<LegalEntityNotPresentInNTR>(new
            {
                context.Message.CorrelationId,
                context.Message.EmpowermentId,
                MissingOrMalformedResponse = true
            });
            return;
        }

        var legalEntity = legalEntityActualState.Result;
        if (legalEntity.IsToBeCheckedInBulstat())
        {
            await context.RespondAsync<LegalEntityNotPresentInNTR>(new
            {
                context.Message.CorrelationId,
                context.Message.EmpowermentId
            });
            return;
        }

        //Confirm legal entity name and issuer
        var incorrectCompanyData = !legalEntity.MatchCompanyData(empowermentStatement.Name, empowermentStatement.Uid);
        var issuerIsNotRepresenter = !legalEntity.IsAmongRepresentatives(context.Message.IssuerUid, context.Message.IssuerName);
        if (incorrectCompanyData || (legalEntity.ContainsRepresentativesData() && issuerIsNotRepresenter))
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
