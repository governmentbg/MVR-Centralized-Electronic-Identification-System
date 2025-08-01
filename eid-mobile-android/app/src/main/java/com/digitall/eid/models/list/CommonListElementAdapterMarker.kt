/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.models.list

import com.digitall.eid.models.applications.create.ApplicationCreateIntroAdapterMarker
import com.digitall.eid.models.applications.create.ApplicationCreatePreviewAdapterMarker
import com.digitall.eid.models.applications.details.ApplicationDetailsAdapterMarker
import com.digitall.eid.models.applications.filter.ApplicationsFilterAdapterMarker
import com.digitall.eid.models.auth.password.forgotten.AuthForgottenPasswordAdapterMarker
import com.digitall.eid.models.registration.RegistrationAdapterMarker
import com.digitall.eid.models.card.change.pin.CertificateChangePinAdapterMarker
import com.digitall.eid.models.certificates.details.CertificateDetailsAdapterMarker
import com.digitall.eid.models.certificates.edit.alias.CertificateEditAliasAdapterMarker
import com.digitall.eid.models.certificates.filter.CertificateFilterAdapterMarker
import com.digitall.eid.models.certificates.resume.CertificateResumeAdapterMarker
import com.digitall.eid.models.certificates.revoke.CertificateRevokeAdapterMarker
import com.digitall.eid.models.certificates.stop.CertificateStopAdapterMarker
import com.digitall.eid.models.empowerment.common.all.EmpowermentAdapterMarker
import com.digitall.eid.models.empowerment.common.cancel.EmpowermentCancelAdapterMarker
import com.digitall.eid.models.empowerment.common.details.EmpowermentDetailsAdapterMarker
import com.digitall.eid.models.empowerment.common.filter.EmpowermentFilterAdapterMarker
import com.digitall.eid.models.empowerment.create.create.EmpowermentCreateAdapterMarker
import com.digitall.eid.models.empowerment.create.preview.EmpowermentCreatePreviewAdapterMarker
import com.digitall.eid.models.empowerment.legal.search.EmpowermentLegalSearchAdapterMarker
import com.digitall.eid.models.empowerment.signing.EmpowermentFromMeSigningAdapterMarker
import com.digitall.eid.models.citizen.change.email.ChangeCitizenEmailAdapterMarker
import com.digitall.eid.models.citizen.change.password.ChangeCitizenPasswordAdapterMarker
import com.digitall.eid.models.citizen.change.information.ChangeCitizenInformationAdapterMarker
import com.digitall.eid.models.citizen.information.CitizenInformationAdapterMarker
import com.digitall.eid.models.citizen.profile.security.CitizenProfileSecurityAdapterMarker
import com.digitall.eid.models.journal.common.all.JournalAdapterMarker
import com.digitall.eid.models.journal.common.filter.JournalFilterAdapterMarker
import com.digitall.eid.models.payments.history.filter.PaymentsHistoryFilterAdapterMarker

sealed interface CommonListElementAdapterMarker :
    EmpowermentCreateAdapterMarker,
    EmpowermentFilterAdapterMarker,
    EmpowermentDetailsAdapterMarker,
    EmpowermentCreatePreviewAdapterMarker,
    EmpowermentAdapterMarker,
    EmpowermentCancelAdapterMarker,
    EmpowermentFromMeSigningAdapterMarker,
    CertificateDetailsAdapterMarker,
    ApplicationDetailsAdapterMarker,
    ApplicationsFilterAdapterMarker,
    CertificateFilterAdapterMarker,
    ApplicationCreateIntroAdapterMarker,
    ApplicationCreatePreviewAdapterMarker,
    CertificateStopAdapterMarker,
    CertificateResumeAdapterMarker,
    CertificateRevokeAdapterMarker,
    RegistrationAdapterMarker,
    ChangeCitizenEmailAdapterMarker,
    ChangeCitizenPasswordAdapterMarker,
    AuthForgottenPasswordAdapterMarker,
    EmpowermentLegalSearchAdapterMarker,
    ChangeCitizenInformationAdapterMarker,
    CitizenInformationAdapterMarker,
    CitizenProfileSecurityAdapterMarker,
    CertificateChangePinAdapterMarker,
    JournalAdapterMarker,
    JournalFilterAdapterMarker,
    PaymentsHistoryFilterAdapterMarker,
    CertificateEditAliasAdapterMarker