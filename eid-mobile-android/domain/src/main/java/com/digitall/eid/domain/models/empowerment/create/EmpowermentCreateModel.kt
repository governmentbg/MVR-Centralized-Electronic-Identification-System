/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.models.empowerment.create

data class EmpowermentCreateModel(
    val uid: String?,
    val name: String?,
    val uidType: String?,
    val serviceId: String?,
    val startDate: String?,
    val expiryDate: String?,
    val onBehalfOf: String?,
    val providerId: String?,
    val serviceName: String?,
    val providerName: String?,
    val issuerPosition: String?,
    val typeOfEmpowerment: String?,
    val empoweredUids: List<EmpowermentCreateEmpoweredUIDModel>?,
    val authorizerUids: List<EmpowermentCreateAuthorizerUIDModel>?,
    val volumeOfRepresentation: List<EmpowermentCreateVolumeOfRepresentationModel>?,
)

data class EmpowermentCreateEmpoweredUIDModel(
    val uid: String?,
    val uidType: String?,
    val name: String?,
)

data class EmpowermentCreateAuthorizerUIDModel(
    val uid: String?,
    val uidType: String?,
    val name: String?,
    val issuer: Boolean?,
)

data class EmpowermentCreateVolumeOfRepresentationModel(
    val code: String?,
    val name: String?,
)