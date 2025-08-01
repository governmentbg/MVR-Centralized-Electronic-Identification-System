package com.digitall.eid.domain.models.certificates

import com.digitall.eid.domain.models.applications.create.ApplicationUserDetailsModel
import com.digitall.eid.domain.models.nomenclatures.reasons.NomenclaturesReasonsModel

data class CertificateChangeStatusInformationModel(
    val userDetails: ApplicationUserDetailsModel? = null,
    val certificateDetails: CertificateDetailsModel? = null,
    val reasons: List<NomenclaturesReasonsModel>? = null,
) {
    val isValid: Boolean
        get() = userDetails != null && certificateDetails != null && reasons != null
}
