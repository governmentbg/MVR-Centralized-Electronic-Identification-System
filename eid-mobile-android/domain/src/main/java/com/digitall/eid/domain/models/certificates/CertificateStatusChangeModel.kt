package com.digitall.eid.domain.models.certificates

data class CertificateStatusChangeModel(
    val id: String?,
    val status: String?,
    val eidAdministratorName: String?,
    val fee: List<Double?>,
    val feeCurrency: List<String?>,
    val paymentAccessCode: String?,
)
