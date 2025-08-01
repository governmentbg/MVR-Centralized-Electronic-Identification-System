/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.models.applications.create

data class ApplicationEnrollWithEIDResponseModel(
    val id: String?,
    val issuerDN: String?,
    val certificate: String?,
    val serialNumber: String?,
    val endEntityProfile: String?,
    val certificateProfile: String?,
    val certificateChain: List<String>?,
)