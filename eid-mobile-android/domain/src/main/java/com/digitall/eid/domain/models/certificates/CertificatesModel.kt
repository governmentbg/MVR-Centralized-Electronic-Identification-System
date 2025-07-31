/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.models.certificates

import com.digitall.eid.domain.models.common.OriginalModel
import kotlinx.parcelize.Parcelize

data class CertificatesModel(
    val size: Int?,
    val number: Int?,
    val last: Boolean?,
    val first: Boolean?,
    val totalPages: Int?,
    val totalElements: Int?,
    val numberOfElements: Int?,
    val content: List<CertificateItem>?,
)

@Parcelize
data class CertificateItem(
    val id: String?,
    val status: String?,
    val deviceId: String?,
    val eidentityId: String?,
    val validityFrom: String?,
    val serialNumber: String?,
    val validityUntil: String?,
    val expiring: Boolean?,
    val alias: String?
): OriginalModel