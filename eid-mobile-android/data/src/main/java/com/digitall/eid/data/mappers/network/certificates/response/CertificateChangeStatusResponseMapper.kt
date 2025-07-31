package com.digitall.eid.data.mappers.network.certificates.response

import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.data.models.network.certificates.response.CertificateStatusChangeResponse
import com.digitall.eid.domain.models.certificates.CertificateStatusChangeModel

class CertificateChangeStatusResponseMapper :
    BaseMapper<CertificateStatusChangeResponse, CertificateStatusChangeModel>() {

    override fun map(from: CertificateStatusChangeResponse): CertificateStatusChangeModel {
        return with(from) {
            CertificateStatusChangeModel(
                id = id,
                status = status,
                eidAdministratorName = eidAdministratorName,
                fee = listOf(fee, secondaryFee),
                feeCurrency = listOf(feeCurrency, secondaryFeeCurrency),
                paymentAccessCode = paymentAccessCode,
            )
        }
    }
}