/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.models.certificates

data class CertificateDetailsModel(
    val id: String?,
    val status: String?,
    val eidAdministratorId: String?,
    val eidAdministratorOfficeId: String?,
    val eidAdministratorName: String?,
    val eidentityId: String?,
    val commonName: String?,
    val validityFrom: String?,
    val validityUntil: String?,
    val createDate: String?,
    val serialNumber: String?,
    val deviceId: String?,
    val levelOfAssurance: String?,
    val reasonId: String?,
    val reasonText: String?,
    val expiring: Boolean?,
    val alias: String?
)
