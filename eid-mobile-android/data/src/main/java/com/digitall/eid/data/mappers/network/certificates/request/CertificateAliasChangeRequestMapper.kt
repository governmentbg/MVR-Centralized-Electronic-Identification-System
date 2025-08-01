package com.digitall.eid.data.mappers.network.certificates.request

import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.data.models.network.certificates.request.CertificateAliasChangeRequest
import com.digitall.eid.data.utils.StrictMapperConfig
import com.digitall.eid.domain.models.certificates.CertificateAliasChangeRequestModel
import org.mapstruct.Mapper
import org.mapstruct.Mapping
import org.mapstruct.factory.Mappers

class CertificateAliasChangeRequestMapper :
    BaseMapper<CertificateAliasChangeRequestModel, CertificateAliasChangeRequest>() {

    @Mapper(config = StrictMapperConfig::class)
    fun interface ModelMapper {
        @Mapping(target = "copy", ignore = true)
        fun map(from: CertificateAliasChangeRequestModel): CertificateAliasChangeRequest
    }

    override fun map(from: CertificateAliasChangeRequestModel): CertificateAliasChangeRequest {
        return Mappers.getMapper(ModelMapper::class.java).map(from)
    }

}