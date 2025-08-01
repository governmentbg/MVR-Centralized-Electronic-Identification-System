/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.data.mappers.network.signing.borica

import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.data.models.network.signing.borica.SigningBoricaSignResponse
import com.digitall.eid.domain.models.signing.borica.SigningBoricaSignResponseModel

class SigningBoricaSignResponseMapper :
    BaseMapper<SigningBoricaSignResponse, SigningBoricaSignResponseModel>() {

    override fun map(from: SigningBoricaSignResponse): SigningBoricaSignResponseModel {
        return with(from) {
            SigningBoricaSignResponseModel(
                code = code,
                message = message,
                validity = data?.validity,
                responseCode = responseCode,
                callbackId = data?.callbackId,
            )
        }
    }

}