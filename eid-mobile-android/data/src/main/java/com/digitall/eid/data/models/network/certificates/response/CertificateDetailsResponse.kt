/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.data.models.network.certificates.response

import com.google.gson.annotations.SerializedName

data class CertificateDetailsResponse(
    @SerializedName("id") val id: String?,
    @SerializedName("status") val status: String?,
    @SerializedName("eidAdministratorId") val eidAdministratorId: String?,
    @SerializedName("eidAdministratorOfficeId") val eidAdministratorOfficeId: String?,
    @SerializedName("eidAdministratorName") val eidAdministratorName: String?,
    @SerializedName("eidentityId") val eidentityId: String?,
    @SerializedName("commonName") val commonName: String?,
    @SerializedName("validityFrom") val validityFrom: String?,
    @SerializedName("validityUntil") val validityUntil: String?,
    @SerializedName("createDate") val createDate: String?,
    @SerializedName("serialNumber") val serialNumber: String?,
    @SerializedName("deviceId") val deviceId: String?,
    @SerializedName("levelOfAssurance") val levelOfAssurance: String?,
    @SerializedName("reasonId") val reasonId: String?,
    @SerializedName("reasonText") val reasonText: String?,
    @SerializedName("isExpiring") val expiring: Boolean?,
    @SerializedName("alias") val alias: String?
)
