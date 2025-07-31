package com.digitall.eid.models.certificates.stop

import com.digitall.eid.domain.models.applications.create.ApplicationUserDetailsModel
import com.digitall.eid.domain.models.nomenclatures.reasons.NomenclatureReasonModel

data class CertificateStopUiModel(
    val userModel: ApplicationUserDetailsModel? = null,
    val stopReasons: List<NomenclatureReasonModel>? = null,
) {
    val isValid: Boolean
        get() = userModel != null && stopReasons.isNullOrEmpty().not()
}
