/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.models.applications.all

import com.digitall.eid.domain.models.common.OriginalModel
import kotlinx.parcelize.Parcelize

data class ApplicationsModel(
    val size: Int?,
    val number: Int?,
    val last: Boolean?,
    val first: Boolean?,
    val empty: Boolean?,
    val totalPages: Int?,
    val totalElements: Int?,
    val numberOfElements: Int?,
    val content: List<ApplicationItem>?,
)

@Parcelize
data class ApplicationItem(
    val id: String?,
    val status: String?,
    val applicationNumber: String?,
    val createDate: String?,
    val deviceId: String?,
    val eidentityId: String?,
    val applicationType: String?,
    val eidAdministratorName: String?,
    val paymentAccessCode: String?,
): OriginalModel