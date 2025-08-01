package com.digitall.eid.domain.models.certificates

import com.digitall.eid.domain.models.nomenclatures.reasons.NomenclaturesReasonsModel

data class CertificateFullDetailsModel(
    val information: CertificateDetailsModel? = null,
    val history: List<CertificateHistoryElementModel>? = null,
    val nomenclatures: List<NomenclaturesReasonsModel>? = null
) {
    val isValid: Boolean
        get() = information != null && history != null && nomenclatures != null
}
