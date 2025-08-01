package com.digitall.eid.domain.models.assets.localization.approval

import com.digitall.eid.domain.models.common.OriginalModel
import kotlinx.parcelize.Parcelize

@Parcelize
data class ApprovalRequestsLocalizationModel(
    val type: String?,
    val description: String?
): OriginalModel