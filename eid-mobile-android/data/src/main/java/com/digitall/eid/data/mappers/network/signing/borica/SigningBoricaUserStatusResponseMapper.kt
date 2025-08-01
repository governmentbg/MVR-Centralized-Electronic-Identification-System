/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.data.mappers.network.signing.borica

import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.data.models.network.signing.borica.SigningBoricaUserStatusResponse
import com.digitall.eid.domain.models.signing.borica.SigningBoricaUserStatusResponseModel

class SigningBoricaUserStatusResponseMapper :
    BaseMapper<SigningBoricaUserStatusResponse, SigningBoricaUserStatusResponseModel>() {

    override fun map(from: SigningBoricaUserStatusResponse): SigningBoricaUserStatusResponseModel {
        return with(from) {
            SigningBoricaUserStatusResponseModel(
                responseCode = responseCode,
                code = code,
                message = message,
                devices = data?.devices,
                certReqId = data?.certReqId,
                encodedCert = data?.encodedCert,
            )
        }
    }

}