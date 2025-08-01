/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.models.empowerment.create

import com.digitall.eid.domain.models.common.OriginalModel
import kotlinx.parcelize.Parcelize

@Parcelize
data class EmpowermentServiceScopeModel(
    val id: String?,
    val code: String?,
    val name: String?,
) : OriginalModel