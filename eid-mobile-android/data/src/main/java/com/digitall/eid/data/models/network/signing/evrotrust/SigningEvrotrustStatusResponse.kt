/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.data.models.network.signing.evrotrust

import com.google.gson.annotations.SerializedName

data class SigningEvrotrustStatusResponse(
    @SerializedName("status") val status: Int?,
    @SerializedName("isProcessing") val processing: Boolean?,
)
