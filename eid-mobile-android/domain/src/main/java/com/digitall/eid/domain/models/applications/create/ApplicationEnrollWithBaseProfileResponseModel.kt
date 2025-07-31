/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.models.applications.create

data class ApplicationEnrollWithBaseProfileResponseModel(
    val id: String?,
    val certificate: String?,
    val serialNumber: String?,
    val certificateChain: List<String>?,
    val replacedExistingCertificate: Boolean?,
)