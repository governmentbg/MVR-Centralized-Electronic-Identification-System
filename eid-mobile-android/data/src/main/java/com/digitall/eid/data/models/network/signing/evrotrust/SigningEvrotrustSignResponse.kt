/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.data.models.network.signing.evrotrust

import com.google.gson.annotations.SerializedName

data class SigningEvrotrustSignResponse(
    @SerializedName("threadID") val threadID: String?,
    @SerializedName("groupSigning") val groupSigning: String?,
    @SerializedName("transactions") val transactions: List<SigningEvrotrustSignTransactionResponse>?,
)

data class SigningEvrotrustSignTransactionResponse(
    @SerializedName("transactionID") val transactionID: String?,
    @SerializedName("identificationNumber") val identificationNumber: String?,
)
