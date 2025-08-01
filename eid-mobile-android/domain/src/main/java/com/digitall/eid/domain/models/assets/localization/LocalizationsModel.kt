package com.digitall.eid.domain.models.assets.localization

import com.digitall.eid.domain.models.assets.localization.logs.LogLocalizationModel
import com.digitall.eid.domain.models.assets.localization.approval.ApprovalRequestsLocalizationModel
import com.digitall.eid.domain.models.assets.localization.errors.ErrorLocalizationModel

data class LocalizationsModel(
    val logs: List<LogLocalizationModel>,
    val approvalRequestTypes: List<ApprovalRequestsLocalizationModel>,
    val errors: List<ErrorLocalizationModel>,
) {
    val isInitialized: Boolean
        get() = logs.isNotEmpty() && approvalRequestTypes.isNotEmpty() && errors.isNotEmpty()
}
