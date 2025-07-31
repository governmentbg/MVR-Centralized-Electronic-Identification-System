package com.digitall.eid.domain.models.assets.localization.errors

import com.digitall.eid.domain.models.common.OriginalModel
import kotlinx.parcelize.Parcelize

@Parcelize
data class ErrorLocalizationModel(
    val type: String?,
    val description: String?
): OriginalModel