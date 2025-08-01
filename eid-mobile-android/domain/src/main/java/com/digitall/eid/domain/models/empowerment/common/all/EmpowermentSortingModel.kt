/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.models.empowerment.common.all

import android.os.Parcelable
import com.digitall.eid.domain.models.base.TypeEnum
import kotlinx.parcelize.Parcelize

@Parcelize
data class EmpowermentSortingModel(
    var sortBy: EmpowermentSortingByEnum,
    var sortDirection: EmpowermentSortingDirectionEnum,
) : Parcelable

enum class EmpowermentSortingByEnum(override val type: String) : TypeEnum {
    ID("id"),
    NAME("name"),
    STATUS("status"),
    CREATED_ON("createdOn"),
    SERVICE_NAME("servicename"),
    PROVIDER_NAME("providername"),
    AUTHORIZER("authorizer"),
    DEFAULT("none")
}

enum class EmpowermentSortingDirectionEnum(override val type: String) : TypeEnum {
    ASC("Asc"),
    DESC("Desc"),
}