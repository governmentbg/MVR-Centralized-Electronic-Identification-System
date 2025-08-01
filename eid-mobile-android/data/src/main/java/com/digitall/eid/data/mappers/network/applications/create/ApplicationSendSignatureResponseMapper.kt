/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.data.mappers.network.applications.create

import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.data.models.network.applications.create.ApplicationSendSignatureResponse
import com.digitall.eid.domain.models.applications.create.ApplicationSendSignatureResponseModel

class ApplicationSendSignatureResponseMapper :
    BaseMapper<ApplicationSendSignatureResponse, ApplicationSendSignatureResponseModel>() {

    override fun map(from: ApplicationSendSignatureResponse): ApplicationSendSignatureResponseModel {
        return with(from) {
            ApplicationSendSignatureResponseModel(
                id = id,
                status = status,
                fee = listOf(fee, secondaryFee),
                feeCurrency = listOf(feeCurrency, secondaryFeeCurrency),
                eidAdministratorName = eidAdministratorName,
                paymentAccessCode = paymentAccessCode,
            )
        }
    }

}