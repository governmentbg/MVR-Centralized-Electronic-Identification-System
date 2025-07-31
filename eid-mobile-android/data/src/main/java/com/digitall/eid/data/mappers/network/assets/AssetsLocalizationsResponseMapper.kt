package com.digitall.eid.data.mappers.network.assets

import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.data.models.network.assets.localization.LocalizationsResponse
import com.digitall.eid.domain.models.assets.localization.LocalizationsModel
import com.digitall.eid.domain.models.assets.localization.logs.LogLocalizationModel
import com.digitall.eid.domain.models.assets.localization.approval.ApprovalRequestsLocalizationModel
import com.digitall.eid.domain.models.assets.localization.errors.ErrorLocalizationModel

class AssetsLocalizationsResponseMapper :
    BaseMapper<LocalizationsResponse, LocalizationsModel>() {


    override fun map(from: LocalizationsResponse): LocalizationsModel {

        return with(from) {
            LocalizationsModel(
                logs = logs.map { element ->
                    LogLocalizationModel(
                        type = element.key,
                        description = element.value
                    )
                },
                approvalRequestTypes = approvalRequestTypes.map { element ->
                    ApprovalRequestsLocalizationModel(
                        type = element.key,
                        description = element.value,
                    )
                },
                errors = errors.map { element ->
                    ErrorLocalizationModel(
                        type = element.key,
                        description = element.value,
                    )
                }
            )
        }
    }

}