/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.models.empowerment.common

data class EmpowermentReasonModel(
    val id: String?,
    val translations: List<EmpowermentReasonTranslationModel>?,
)

data class EmpowermentReasonTranslationModel(
    val language: String?,
    val name: String?,
)