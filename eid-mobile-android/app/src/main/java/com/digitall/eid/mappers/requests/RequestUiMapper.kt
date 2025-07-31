package com.digitall.eid.mappers.requests

import com.digitall.eid.data.mappers.base.BaseMapperWithData
import com.digitall.eid.domain.LOCALIZATIONS
import com.digitall.eid.domain.models.common.ApplicationLanguage
import com.digitall.eid.domain.models.requests.response.RequestResponseModel
import com.digitall.eid.models.requests.RequestFromUi
import com.digitall.eid.models.requests.RequestUi

class RequestUiMapper :
    BaseMapperWithData<List<RequestResponseModel>, ApplicationLanguage, List<RequestUi>>() {

    private val localizations
        get() = LOCALIZATIONS.approvalRequestTypes

    override fun map(
        from: List<RequestResponseModel>,
        data: ApplicationLanguage?
    ): List<RequestUi> {
        return buildList {
            from.forEach { requestModel ->
                add(
                    RequestUi(
                        id = requestModel.id,
                        username = requestModel.username,
                        levelOfAssurance = requestModel.levelOfAssurance,
                        requestFrom = RequestFromUi(
                            type = localizations.firstOrNull { element -> element.type == requestModel.requestFrom?.type }?.description
                                ?: requestModel.requestFrom?.type,
                            system = requestModel.requestFrom?.system?.get(data?.type?.uppercase())
                        ),
                        createDate = requestModel.createDate,
                        maxTtl = requestModel.maxTtl,
                        expiresIn = requestModel.expiresIn
                    )
                )
            }
        }
    }
}