using eID.MIS.Contracts.EP.Enums;
using eID.MIS.Contracts.EP.Validators;
using eID.MIS.Contracts.Requests;
using FluentValidation;

namespace eID.MIS.Contracts.EP.External;

public class CreatePaymentPayload : IValidatableRequest
{
    public Guid CitizenProfileId { get; set; }
    public RegisterPaymentRequest Request { get; set; }

    public IValidator GetValidator() => new CreatePaymentPayloadValidator();
}


public class RegisterPaymentRequest
{
    public IEnumerable<PayerProfile> Actors { get; set; }
    public PaymentData PaymentData { get; set; }
}

public class PayerProfile
{
    public ParticipantType ParticipantType { get; } = ParticipantType.APPLICANT;
    public CommonTypeActorEnum Type { get; set; } = CommonTypeActorEnum.PERSON;
    public CommonTypeUID Uid { get; set; }
    public string Name { get; set; }
    public CommonTypeInfo Info { get; set; }
}

public class CommonTypeUID
{
    public CommonTypeUIDEnum Type { get; set; }
    public string Value { get; set; }
}

public class CommonTypeInfo
{
    public CommonTypeContacts Contacts { get; set; }
    public CommonTypeBankAccount BankAccount { get; set; }
}

public class CommonTypeContacts
{
    public string Phone { get; set; }
    public string Email { get; set; }
    public CommonTypeAddress Address { get; set; }
}

public class CommonTypeAddress
{
    public string Country { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string Zip { get; set; }
    public string Address { get; set; }
}

public class CommonTypeBankAccount
{
    public string Name { get; set; }
    public string Bank { get; set; }
    public string Bic { get; set; }
    public string Iban { get; set; }
}

public class PaymentData
{
    public string PaymentId { get; set; }
    public string Currency { get; set; }
    public decimal Amount { get; set; }
    public PaymentStatusType Status { get; set; }
    public int TypeCode { get; set; }
    public string ReferenceNumber { get; set; }
    public string ReferenceType { get; set; }
    /// <summary>
    ///  ISO 8601
    /// </summary>
    public string ReferenceDate { get; set; }
    /// <summary>
    ///  ISO 8601
    /// </summary>
    public string ExpirationDate { get; set; }
    /// <summary>
    ///  ISO 8601
    /// </summary>
    public string CreateDate { get; set; }
    public string Reason { get; set; }
    public string AdditionalInformation { get; set; }
    public string AdministrativeServiceUri { get; set; }
    public string AdministrativeServiceSupplierUri { get; set; }
    public string AdministrativeServiceNotificationURL { get; set; }
}
