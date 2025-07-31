package com.digitall.eid.domain.models.applications.all

import com.digitall.eid.domain.models.nomenclatures.reasons.NomenclaturesReasonsModel

data class ApplicationFullDetailsModel(
    val information: ApplicationDetailsModel? = null,
    val nomenclatures: List<NomenclaturesReasonsModel>? = null
) {
    val isValid: Boolean
        get() = information != null && nomenclatures != null
}
