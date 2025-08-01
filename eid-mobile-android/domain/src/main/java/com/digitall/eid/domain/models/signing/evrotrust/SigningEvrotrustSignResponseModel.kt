/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.models.signing.evrotrust

data class SigningEvrotrustSignResponseModel(
    val threadID: String?,
    val groupSigning: String?,
    val transactions: List<SigningEvrotrustSignTransactionResponseModel>?,
)

data class SigningEvrotrustSignTransactionResponseModel(
    val transactionID: String?,
    val identificationNumber: String?,
)