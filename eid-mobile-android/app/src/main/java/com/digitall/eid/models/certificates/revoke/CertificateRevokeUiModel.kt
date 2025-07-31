package com.digitall.eid.models.certificates.revoke

import com.digitall.eid.domain.models.applications.create.ApplicationUserDetailsModel
import com.digitall.eid.domain.models.nomenclatures.reasons.NomenclatureReasonModel

data class CertificateRevokeUiModel(
    val userModel: ApplicationUserDetailsModel? = null,
    val revokeReasons: List<NomenclatureReasonModel>? = null,
) {
    val isValid: Boolean
        get() = userModel != null && revokeReasons.isNullOrEmpty().not()
}
