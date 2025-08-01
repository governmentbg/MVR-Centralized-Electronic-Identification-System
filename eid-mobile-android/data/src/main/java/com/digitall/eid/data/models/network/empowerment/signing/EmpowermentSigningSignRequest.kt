/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.data.models.network.empowerment.signing

import com.google.gson.annotations.SerializedName

data class EmpowermentSigningSignRequest(
    @SerializedName("signatureProvider") val signatureProvider: String?,
    @SerializedName("detachedSignature") val detachedSignature: String?,
)
