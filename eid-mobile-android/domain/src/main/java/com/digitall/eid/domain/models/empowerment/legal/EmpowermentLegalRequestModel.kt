package com.digitall.eid.domain.models.empowerment.legal

import com.digitall.eid.domain.models.empowerment.common.EmpowermentUidModel

data class EmpowermentLegalRequestModel(
    val pageSize: Int,
    val pageIndex: Int,
    val legalNumber: String?,
    val status: String?,
    val sortBy: String?,
    val validToDate: String?,
    val serviceName: String?,
    val providerName: String?,
    val sortDirection: String?,
    val showOnlyNoExpiryDate: Boolean?,
    val authorizerUids: List<EmpowermentUidModel>?,
)