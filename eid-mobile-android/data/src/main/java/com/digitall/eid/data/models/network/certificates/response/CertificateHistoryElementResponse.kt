package com.digitall.eid.data.models.network.certificates.response

import com.google.gson.annotations.SerializedName

data class CertificateHistoryElementResponse(
    @SerializedName("id") val id: String?,
    @SerializedName("createdDateTime") val createdDateTime: String?,
    @SerializedName("validityUntil") val validityUntil: String?,
    @SerializedName("validityFrom") val validityFrom: String?,
    @SerializedName("status") val status: String?,
    @SerializedName("applicationId") val applicationId: String?,
    @SerializedName("applicationNumber") val applicationNumber: String?,
    @SerializedName("modifiedDateTime") val modifiedDateTime: String?,
    @SerializedName("reasonId") val reasonId: String?,
    @SerializedName("reasonText") val reasonText: String?
)
