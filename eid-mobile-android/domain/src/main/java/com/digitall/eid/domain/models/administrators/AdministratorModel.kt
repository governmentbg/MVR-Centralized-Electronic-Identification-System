/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.models.administrators

import com.digitall.eid.domain.models.common.OriginalModel
import kotlinx.parcelize.Parcelize

@Parcelize
data class AdministratorModel(
    val id: String?,
    val name: String?,
    val nameLatin: String?,
    val eikNumber: String?,
    val active: Boolean?,
    val contact: String?,
    val eidManagerFrontOfficeIds: List<String>?,
    val deviceIds: List<String>?,
) : OriginalModel
