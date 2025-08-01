/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.models.empowerment.common.all

import com.digitall.eid.domain.models.empowerment.common.EmpowermentUidModel

data class EmpowermentRequestModel(
    val pageSize: Int,
    val pageIndex: Int,
    val number: String?,
    val status: String?,
    val sortBy: String?,
    val authorizer: String?,
    val eik: String?,
    val onBehalfOf: String?,
    val validToDate: String?,
    val serviceName: String?,
    val providerName: String?,
    val sortDirection: String?,
    val showOnlyNoExpiryDate: Boolean?,
    val empoweredUids: List<EmpowermentUidModel>?,
)