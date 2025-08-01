/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.models.empowerment.create

import com.digitall.eid.domain.models.common.OriginalModel
import kotlinx.parcelize.Parcelize

data class EmpowermentServicesModel(
    val pageIndex: Int?,
    val totalItems: Int?,
    val data: List<EmpowermentServiceModel>?,
)

@Parcelize
data class EmpowermentServiceModel(
    val id: String,
    val name: String?,
    val providerDetailsId: String?,
    val providerSectionId: String?,
    val deleted: Boolean?,
    val external: Boolean?,
    val description: String?,
    val empowerment: Boolean?,
    val serviceNumber: String?,
    val paymentInfoNormalCost: Double?,
) : OriginalModel