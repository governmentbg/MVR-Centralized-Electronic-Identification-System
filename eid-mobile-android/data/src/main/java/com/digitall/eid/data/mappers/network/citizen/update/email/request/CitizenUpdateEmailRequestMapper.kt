package com.digitall.eid.data.mappers.network.citizen.update.email.request

import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.data.models.network.citizen.update.email.CitizenUpdateEmailRequest
import com.digitall.eid.domain.models.citizen.update.email.CitizenUpdateEmailRequestModel

class CitizenUpdateEmailRequestMapper: BaseMapper<CitizenUpdateEmailRequestModel, CitizenUpdateEmailRequest>() {

    override fun map(from: CitizenUpdateEmailRequestModel): CitizenUpdateEmailRequest {
        return with(from) {
            CitizenUpdateEmailRequest(
                email = email,
            )
        }
    }
}