package com.digitall.eid.domain.models.csr.principal

data class CsrPrincipalModel(
    val name: String,
    val givenName: String,
    val surname: String,
    val country: String,
    val serialNumber: String,
)
