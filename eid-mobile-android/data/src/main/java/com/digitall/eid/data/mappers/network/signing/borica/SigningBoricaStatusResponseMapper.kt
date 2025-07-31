/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.data.mappers.network.signing.borica

import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.data.models.network.signing.borica.SigningBoricaStatusResponse
import com.digitall.eid.domain.models.signing.borica.SigningBoricaStatusResponseModel

class SigningBoricaStatusResponseMapper :
    BaseMapper<SigningBoricaStatusResponse, SigningBoricaStatusResponseModel>() {

    override fun map(from: SigningBoricaStatusResponse): SigningBoricaStatusResponseModel {
        return with(from) {
            val signature = data?.signatures?.first()
            SigningBoricaStatusResponseModel(
                responseCode = responseCode,
                code = code,
                message = message,
                cert = data?.cert,
                status = signature?.status,
                signature = signature?.signature,
                signatureType = signature?.signatureType,
            )
        }
    }

}