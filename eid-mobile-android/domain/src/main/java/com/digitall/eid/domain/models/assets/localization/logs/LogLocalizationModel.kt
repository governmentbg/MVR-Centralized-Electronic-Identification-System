package com.digitall.eid.domain.models.assets.localization.logs

import com.digitall.eid.domain.models.common.OriginalModel
import kotlinx.parcelize.Parcelize

@Parcelize
data class LogLocalizationModel(
    val type: String?,
    val description: String?
): OriginalModel
