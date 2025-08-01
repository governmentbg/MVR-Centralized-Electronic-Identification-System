/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.models.applications.create

data class ApplicationSendSignatureResponseModel(
    val id: String?,
    val status: String?,
    val fee: List<Double?>,
    val feeCurrency: List<String?>,
    val eidAdministratorName: String?,
    val paymentAccessCode: String?,
)