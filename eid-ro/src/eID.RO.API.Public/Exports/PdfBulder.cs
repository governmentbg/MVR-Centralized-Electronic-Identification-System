using eID.RO.Contracts.Enums;
using eID.RO.Contracts.Results;
using iText.IO.Font;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using SR = eID.RO.API.Public.Exports.StringResources;

namespace eID.RO.API.Public.Exports;

public static class PdfBulder
{
    private const float WidthLabelColumn = 35;
    private const float WidthValueColumn = 65;
    private static readonly UnitValue[] _tableColumnWidths = UnitValue.CreatePercentArray(new float[] { WidthLabelColumn, WidthValueColumn });

    public static byte[] CreateEmpowermentDocument(LanguageType lt, EmpowermentStatementWithSignaturesResult empowerment)
    {
        if (empowerment is null)
        {
            throw new ArgumentNullException(nameof(empowerment));
        }

        var fontsDirectory = Path.Combine(AppContext.BaseDirectory, "Fonts");
        var arialPath = Path.Combine(fontsDirectory, "arial.ttf");
        var arialBoldPath = Path.Combine(fontsDirectory, "arialbd.ttf");
        if (!File.Exists(arialPath))
        {
            throw new InvalidOperationException($"Font Arial ({arialPath}) does not exist in the system.");
        }
        if (!File.Exists(arialBoldPath))
        {
            throw new InvalidOperationException($"Font Arial Bold ({arialBoldPath}) does not exist in the system.");
        }

        using (var ms = new MemoryStream())
        {
            using var writer = new PdfWriter(ms);
            using var pdf = new PdfDocument(writer);
            using var document = new iText.Layout.Document(pdf, iText.Kernel.Geom.PageSize.A4);

            // Load fonts
            var arialFont = PdfFontFactory.CreateFont(arialPath, PdfEncodings.IDENTITY_H);
            var arialBoldFont = PdfFontFactory.CreateFont(arialBoldPath, PdfEncodings.IDENTITY_H);

            var title = new Paragraph(SR.Get(lt, ResourceType.DocumentName))
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFont(arialBoldFont)
                    .SetFontSize(28);
            document.Add(title);

            // Add free space
            document.Add(new Paragraph("\n"));

            var table1 = new Table(_tableColumnWidths)
                .SetWidth(UnitValue.CreatePercentValue(100))
                .SetFixedLayout();

            AddRowToTable(table1, SR.Get(lt, ResourceType.Number), empowerment.Number, arialFont);

            // Effective on
            var isEffectiveOnInTheFuture = empowerment.StatusHistory
               .Any(sh =>
                    sh.Status == EmpowermentStatementStatus.Active &&
                    sh.DateTime < empowerment.StartDate &&
                    DateTime.UtcNow < empowerment.StartDate);
            var empowermentStatus = GetStatus(lt, empowerment.CalculatedStatusOn);
            if (empowerment.CalculatedStatusOn == CalculatedEmpowermentStatus.Active && isEffectiveOnInTheFuture)
            {
                empowermentStatus = SR.Get(lt, ResourceType.Signed);
            }

            AddRowToTable(table1, SR.Get(lt, ResourceType.Status), empowermentStatus, arialFont);
            // Disagreement reason
            if (empowerment.CalculatedStatusOn == CalculatedEmpowermentStatus.DisagreementDeclared)
            {
                var disagreementReason = empowerment.EmpowermentDisagreements
                    .OrderByDescending(ed => ed.ActiveDateTime)
                    .FirstOrDefault()?.Reason ?? "-";

                AddRowToTable(table1, SR.Get(lt, ResourceType.Reason), disagreementReason, arialFont);
            }
            // Withdrawn reason
            if (empowerment.CalculatedStatusOn == CalculatedEmpowermentStatus.Withdrawn)
            {
                var withdrawnReason = empowerment.EmpowermentWithdrawals
                    .OrderByDescending(ed => ed.ActiveDateTime)
                    .FirstOrDefault()?.Reason ?? "-";

                AddRowToTable(table1, SR.Get(lt, ResourceType.Reason), withdrawnReason, arialFont);
            }

            if (empowerment.CalculatedStatusOn == CalculatedEmpowermentStatus.Denied)
            {
                var denialReason = GetDenialReason(lt, empowerment.DenialReason);

                AddRowToTable(table1, SR.Get(lt, ResourceType.DenialReason), denialReason, arialFont);
            }

            if (empowerment.CalculatedStatusOn == CalculatedEmpowermentStatus.Active)
            {
                if (isEffectiveOnInTheFuture)
                {
                    AddRowToTable(table1, SR.Get(lt, ResourceType.EffectiveOn), FormatDate(empowerment.StartDate), arialFont);
                }
                else
                {
                    if (empowerment.ExpiryDate.HasValue)
                    {
                        AddRowToTable(table1, string.Empty, FormatEndDate(lt, empowerment.ExpiryDate), arialFont);
                    }
                }
            }

            AddRowToTable(table1, SR.Get(lt, ResourceType.StartDate), FormatDate(empowerment.StartDate), arialFont);
            AddRowToTable(table1, SR.Get(lt, ResourceType.EndDate), FormatEndDate(lt, empowerment.ExpiryDate), arialFont);
            AddRowToTable(table1, SR.Get(lt, ResourceType.OnBehalfOf), GetOnBehalfOf(lt, empowerment.OnBehalfOf), arialFont);

            // Individual
            if (empowerment.OnBehalfOf == OnBehalfOf.Individual)
            {
                AddRowToTable(table1, SR.Get(lt, ResourceType.NumberOfTheApplicant), empowerment.Uid, arialFont);
                AddRowToTable(table1, SR.Get(lt, ResourceType.Name), empowerment.Name, arialFont);

                var signedDate = empowerment.EmpowermentSignatures
                    .OrderByDescending(es => es.DateTime)
                    .FirstOrDefault()?.DateTime;
                AddRowToTable(table1, SR.Get(lt, ResourceType.SignedOn), FormatDate(signedDate), arialFont);
            }

            // LegalEntity
            if (empowerment.OnBehalfOf == OnBehalfOf.LegalEntity)
            {
                AddRowToTable(table1, SR.Get(lt, ResourceType.NumberOfTheLegalEntity), empowerment.Uid, arialFont);
                AddRowToTable(table1, SR.Get(lt, ResourceType.Name), empowerment.Name, arialFont);

                if (empowerment.EmpoweredUids.Count() > 1)
                {
                    AddRowToTable(table1, SR.Get(lt, ResourceType.TypeOfEmpowerment),
                        $"{SR.Get(lt, ResourceType.Together)}\n{SR.Get(lt, ResourceType.TogetherAdditionalText)}", arialFont);
                }

                AddRowToTable(table1, SR.Get(lt, ResourceType.LegalRepresentatives),
                    PrepareLegalRepresentatives(lt, empowerment.AuthorizerUids, empowerment.EmpowermentSignatures), arialFont);
            }

            AddRowToTable(table1, SR.Get(lt, ResourceType.EmpoweredPeople), PrepareEmpoweredPeople(lt, empowerment.EmpoweredUids), arialFont);
            AddRowToTable(table1, SR.Get(lt, ResourceType.Provider), empowerment.ProviderName, arialFont);
            AddRowToTable(table1, SR.Get(lt, ResourceType.Service), $"{empowerment.ServiceId} - {empowerment.ServiceName}", arialFont);
            AddRowToTable(table1, SR.Get(lt, ResourceType.ExtentOfRepresentativeAuthority),
                string.Join("\n", empowerment.VolumeOfRepresentation.Select(eu => eu.Name)), arialFont);

            document.Add(table1);

            document.Add(new Paragraph($"\n{SR.Get(lt, ResourceType.EmpowermentHistory)}")
                .SetFont(arialBoldFont)
                .SetMarginTop(10));

            // Status history
            var table2 = new Table(_tableColumnWidths)
                .SetWidth(UnitValue.CreatePercentValue(100))
                .SetFixedLayout();

            var table2Entries = new List<Table2Entry>();
            foreach (var statusHistory in empowerment.StatusHistory.OrderByDescending(sh => sh.DateTime).Where(sh => sh.Status != EmpowermentStatementStatus.CollectingAuthorizerSignatures))
            {
                table2Entries.Add(new()
                {
                    DateTime = statusHistory.DateTime,
                    ResourceType = GetStatusHistoryResourceType(statusHistory.Status, isEffectiveOnInTheFuture),
                    Column2Content = FormatDate(statusHistory.DateTime)
                });
            }

            // Signature entities do not possess information about signers' name
            var authorizerNameByUid = empowerment.AuthorizerUids
                                    .GroupBy(a => a.Uid)
                                    .ToDictionary(g => g.Key, g => g.First().Name ?? string.Empty);
            foreach (var signature in empowerment.EmpowermentSignatures)
            {
                authorizerNameByUid.TryGetValue(signature.SignerUid, out var signerName);
                if (!string.IsNullOrWhiteSpace(signerName))
                {
                    signerName = $"\n{signerName}";
                }
                table2Entries.Add(new()
                {
                    DateTime = signature.DateTime,
                    ResourceType = ResourceType.SignedOn,
                    Column2Content = $"{FormatDate(signature.DateTime)}{signerName}"
                });
            }

            foreach (var entry in table2Entries.OrderByDescending(e => e.DateTime))
            {
                AddRowToTable(table2, SR.Get(lt, entry.ResourceType), entry.Column2Content, arialFont);
            }
            document.Add(table2);
            document.Close();

            var result = ms.ToArray();
            return result;
        }
    }

    private record struct Table2Entry
    {
        public DateTime DateTime { get; set; }
        public ResourceType ResourceType { get; set; }
        public string Column2Content { get; set; }

    }

    private static void AddRowToTable(Table table, string column1, string column2, PdfFont arialFont)
    {
        var cell1 = new Cell().Add(new Paragraph(column1)
            .SetFont(arialFont));
        var cell2 = new Cell().Add(new Paragraph(column2)
            .SetFont(arialFont));

        // Remove frame border
        cell1.SetBorder(Border.NO_BORDER);
        cell2.SetBorder(Border.NO_BORDER);

        table.AddCell(cell1);
        table.AddCell(cell2);
    }

    private static string GetStatus(LanguageType lt, CalculatedEmpowermentStatus status)
        => status switch
        {
            CalculatedEmpowermentStatus.Active => SR.Get(lt, ResourceType.Active),
            CalculatedEmpowermentStatus.DisagreementDeclared => SR.Get(lt, ResourceType.DisagreementDeclared),
            CalculatedEmpowermentStatus.Withdrawn => SR.Get(lt, ResourceType.Withdrawn),
            CalculatedEmpowermentStatus.Expired => SR.Get(lt, ResourceType.Expired),
            CalculatedEmpowermentStatus.Unconfirmed => SR.Get(lt, ResourceType.Unconfirmed),
            CalculatedEmpowermentStatus.CollectingAuthorizerSignatures => SR.Get(lt, ResourceType.CollectingAuthorizerSignatures),
            CalculatedEmpowermentStatus.Denied => SR.Get(lt, ResourceType.Denied),
            _ => SR.Get(lt, ResourceType.Unknown)
        };

    private static string GetOnBehalfOf(LanguageType lt, OnBehalfOf behalfOf)
        => behalfOf switch
        {
            OnBehalfOf.Individual => SR.Get(lt, ResourceType.Individual),
            OnBehalfOf.LegalEntity => SR.Get(lt, ResourceType.LegalEntity),
            _ => SR.Get(lt, ResourceType.Unknown)
        };

    private static string FormatDate(DateTime? dateTime)
    {
        if (!dateTime.HasValue)
        {
            return "-";
        }

        return dateTime.Value.ToUniversalTime().ToString("yyyy-MM-dd, HH:mm:ss (UTC)");
    }

    private static string FormatEndDate(LanguageType lt, DateTime? dateTime)
        => !dateTime.HasValue ? SR.Get(lt, ResourceType.Indefinitely) : FormatDate(dateTime);

    private static string GetUidType(LanguageType lt, IdentifierType identifierType)
        => identifierType switch
        {
            IdentifierType.EGN => SR.Get(lt, ResourceType.EGN),
            IdentifierType.LNCh => SR.Get(lt, ResourceType.LNCH),
            _ => "-"
        };

    private static string PrepareEmpoweredPeople(LanguageType lt, IEnumerable<UidResult> empoweredUids)
        => string.Join("\n", empoweredUids.Select(eu => $"{GetUidType(lt, eu.UidType)}: {eu.Uid} - {eu.Name}"));

    private static string PrepareLegalRepresentatives(LanguageType lt, IEnumerable<UidResult> authorizerUids, IEnumerable<EmpowermentSignatureResult> signatures)
        => string.Join("\n", authorizerUids.Select(eu =>
            $"{GetUidType(lt, eu.UidType)}: {eu.Uid} - {eu.Name}\n{SR.Get(lt, ResourceType.SignedOn)} " +
            $"{FormatDate(signatures.FirstOrDefault(sgn => sgn.SignerUid == eu.Uid)?.DateTime)}"));

    private static ResourceType GetStatusHistoryResourceType(EmpowermentStatementStatus status, bool isEffectiveOnInTheFuture)
        => status switch
        {
            EmpowermentStatementStatus.Active => isEffectiveOnInTheFuture ? ResourceType.SignedOn : ResourceType.EffectiveOn,
            EmpowermentStatementStatus.Created => ResourceType.SubmittedTo,
            EmpowermentStatementStatus.Denied => ResourceType.DeniedOn,
            EmpowermentStatementStatus.CollectingAuthorizerSignatures => ResourceType.CollectingAuthorizerSignaturesOn,
            EmpowermentStatementStatus.Withdrawn => ResourceType.WithdrawnOn,
            EmpowermentStatementStatus.DisagreementDeclared => ResourceType.DisagreementDeclaredOn,
            EmpowermentStatementStatus.Unconfirmed => ResourceType.UnconfirmedOn,
            _ => ResourceType.None
        };

    private static string GetDenialReason(LanguageType lt, EmpowermentsDenialReason denialReason)
        => denialReason switch
        {
            EmpowermentsDenialReason.DeceasedUid => SR.Get(lt, ResourceType.DenialReason_DeceasedUid),
            EmpowermentsDenialReason.ProhibitedUid => SR.Get(lt, ResourceType.DenialReason_ProhibitedUid),
            EmpowermentsDenialReason.NTRCheckFailed => SR.Get(lt, ResourceType.DenialReason_NTRCheckFailed),
            EmpowermentsDenialReason.TimedOut => SR.Get(lt, ResourceType.DenialReason_TimedOut),
            EmpowermentsDenialReason.BelowLawfulAge => SR.Get(lt, ResourceType.DenialReason_BelowLawfulAge),
            EmpowermentsDenialReason.NoPermit => SR.Get(lt, ResourceType.DenialReason_NoPermit),
            EmpowermentsDenialReason.LawfulAgeInfoNotAvailable => SR.Get(lt, ResourceType.DenialReason_LawfulAgeInfoNotAvailable),
            EmpowermentsDenialReason.UnsuccessfulRestrictionsCheck => SR.Get(lt, ResourceType.DenialReason_UnsuccessfulRestrictionsCheck),
            EmpowermentsDenialReason.LegalEntityNotActive => SR.Get(lt, ResourceType.DenialReason_LegalEntityNotActive),
            EmpowermentsDenialReason.LegalEntityRepresentationNotMatch => SR.Get(lt, ResourceType.DenialReason_LegalEntityRepresentationNotMatch),
            EmpowermentsDenialReason.UnsuccessfulLegalEntityCheck => SR.Get(lt, ResourceType.DenialReason_UnsuccessfulLegalEntityCheck),
            EmpowermentsDenialReason.EmpowermentStatementNotFound => SR.Get(lt, ResourceType.DenialReason_EmpowermentStatementNotFound),
            EmpowermentsDenialReason.BulstatCheckFailed => SR.Get(lt, ResourceType.DenialReason_BulstatCheckFailed),
            EmpowermentsDenialReason.ReregisteredInNTR => SR.Get(lt, ResourceType.DenialReason_ReregisteredInNTR),
            EmpowermentsDenialReason.ArchivedInBulstat => SR.Get(lt, ResourceType.DenialReason_ArchivedInBulstat),
            EmpowermentsDenialReason.InInsolvencyProceedingsInBulstat => SR.Get(lt, ResourceType.DenialReason_InInsolvencyProceedingsInBulstat),
            EmpowermentsDenialReason.InsolventInBulstat => SR.Get(lt, ResourceType.DenialReason_InsolventInBulstat),
            EmpowermentsDenialReason.InLiquidationInBulstat => SR.Get(lt, ResourceType.DenialReason_InLiquidationInBulstat),
            EmpowermentsDenialReason.InactiveInBulstat => SR.Get(lt, ResourceType.DenialReason_InactiveInBulstat),
            EmpowermentsDenialReason.ClosedInBulstat => SR.Get(lt, ResourceType.DenialReason_ClosedInBulstat),
            EmpowermentsDenialReason.TerminatedThroughMergerInBulstat => SR.Get(lt, ResourceType.DenialReason_TerminatedThroughMergerInBulstat),
            EmpowermentsDenialReason.TerminatedThroughIncorporationInBulstat => SR.Get(lt, ResourceType.DenialReason_TerminatedThroughIncorporationInBulstat),
            EmpowermentsDenialReason.TerminatedThroughDivisionInBulstat => SR.Get(lt, ResourceType.DenialReason_TerminatedThroughDivisionInBulstat),
            EmpowermentsDenialReason.DeletedFromJudicialRegister => SR.Get(lt, ResourceType.DenialReason_DeletedFromJudicialRegister),
            EmpowermentsDenialReason.DeletedFromBTPPRegister => SR.Get(lt, ResourceType.DenialReason_DeletedFromBTPPRegister),
            EmpowermentsDenialReason.RegistrationAnnulledInBulstat => SR.Get(lt, ResourceType.DenialReason_RegistrationAnnulledInBulstat),
            EmpowermentsDenialReason.TerminatedDueToEnterpriseTransactionInBulstat => SR.Get(lt, ResourceType.DenialReason_TerminatedDueToEnterpriseTransactionInBulstat),
            EmpowermentsDenialReason.DeregisteredInBulstat => SR.Get(lt, ResourceType.DenialReason_DeregisteredInBulstat),
            EmpowermentsDenialReason.SignatureCollectionTimeOut => SR.Get(lt, ResourceType.DenialReason_SignatureCollectionTimeOut),
            EmpowermentsDenialReason.UnsuccessfulTimestamping => SR.Get(lt, ResourceType.DenialReason_UnsuccessfulTimestamping),
            EmpowermentsDenialReason.DeniedByDeauAdministrator => SR.Get(lt, ResourceType.DenialReason_DeniedByDeauAdministrator),
            EmpowermentsDenialReason.InvalidUidRegistrationStatusDetected => SR.Get(lt, ResourceType.DenialReason_InvalidUidRegistrationStatusDetected),
            EmpowermentsDenialReason.UidsRegistrationStatusInfoNotAvailable => SR.Get(lt, ResourceType.DenialReason_UidsRegistrationStatusInfoNotAvailable),
            EmpowermentsDenialReason.RegistrationStatusUnavailable => SR.Get(lt, ResourceType.DenialReason_RegistrationStatusUnavailable),
            EmpowermentsDenialReason.InactiveProfile => SR.Get(lt, ResourceType.DenialReason_InactiveProfile),
            EmpowermentsDenialReason.NoBaseProfile => SR.Get(lt, ResourceType.DenialReason_NoBaseProfile),
            EmpowermentsDenialReason.NameMismatch => SR.Get(lt, ResourceType.DenialReason_NameMismatch),
            EmpowermentsDenialReason.NoRegistration => SR.Get(lt, ResourceType.DenialReason_NoRegistration),
            _ => "-"
        };
}
