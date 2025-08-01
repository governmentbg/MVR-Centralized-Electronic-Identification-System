package com.digitall.eid.domain.models.certificates

import com.digitall.eid.domain.models.common.OriginalModel
import com.digitall.eid.domain.models.nomenclatures.reasons.NomenclatureReasonModel
import kotlinx.parcelize.Parcelize

@Parcelize
data class CertificateHistoryElementModel(
    val id: String?,
    val createdDateTime: String?,
    val validityUntil: String?,
    val validityFrom: String?,
    val status: String?,
    val applicationId: String?,
    val applicationNumber: String?,
    val modifiedDateTime: String?,
    val reasonId: String?,
    val reasonText: String?
): OriginalModel {

    val hasApplicationNumber: Boolean
        get() {
            return applicationNumber.isNullOrEmpty().not()
        }

    fun getReason(reasons: List<NomenclatureReasonModel>): String? {
        return when {
            reasonText.isNullOrEmpty().not() -> reasonText
            reasonId.isNullOrEmpty().not() && reasons.isNotEmpty() -> reasons.firstOrNull { reason -> reason.id == reasonId }?.description
            else -> null
        }
    }
}
