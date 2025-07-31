package com.digitall.eid.domain.models.nomenclatures.reasons

import com.digitall.eid.domain.models.common.OriginalModel
import kotlinx.parcelize.Parcelize

data class NomenclaturesReasonsModel(
    val id: String?,
    val name: String?,
    val nomenclatures: List<NomenclatureReasonModel>?
)

@Parcelize
data class NomenclatureReasonModel(
    val id: String?,
    val name: String?,
    val description: String?,
    val language: String?,
    val textRequired: Boolean?,
    val permittedUser: NomenclaturePermittedUserEnum?
): OriginalModel
