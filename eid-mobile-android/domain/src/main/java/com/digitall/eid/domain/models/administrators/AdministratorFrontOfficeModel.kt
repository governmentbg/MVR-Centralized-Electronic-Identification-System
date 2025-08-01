package com.digitall.eid.domain.models.administrators

import com.digitall.eid.domain.models.common.OriginalModel
import kotlinx.parcelize.Parcelize

@Parcelize
data class AdministratorFrontOfficeModel(
    val id: String?,
    val name: String?,
    val eidManagerId: String?,
    val location: String?,
    val region: String?,
    val contact: String?,
    val active: Boolean?,
) : OriginalModel
