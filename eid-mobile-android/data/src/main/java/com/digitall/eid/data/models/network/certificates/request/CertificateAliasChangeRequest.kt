package com.digitall.eid.data.models.network.certificates.request

import com.google.gson.annotations.SerializedName

data class CertificateAliasChangeRequest(
    @SerializedName("alias") val alias: String?
)
