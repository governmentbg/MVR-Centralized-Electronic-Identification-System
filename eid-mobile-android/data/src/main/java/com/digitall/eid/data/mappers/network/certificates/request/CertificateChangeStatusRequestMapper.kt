package com.digitall.eid.data.mappers.network.certificates.request

import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.data.models.network.certificates.request.CertificateStatusChangeRequest
import com.digitall.eid.data.utils.StrictMapperConfig
import com.digitall.eid.domain.models.certificates.CertificateStatusChangeRequestModel
import org.mapstruct.Mapper
import org.mapstruct.factory.Mappers

class CertificateChangeStatusRequestMapper :
    BaseMapper<CertificateStatusChangeRequestModel, CertificateStatusChangeRequest>() {

    @Mapper(config = StrictMapperConfig::class)
    fun interface ModelMapper {
        fun map(from: CertificateStatusChangeRequestModel): CertificateStatusChangeRequest
    }

    override fun map(from: CertificateStatusChangeRequestModel): CertificateStatusChangeRequest {
        return Mappers.getMapper(ModelMapper::class.java).map(from)
    }

}