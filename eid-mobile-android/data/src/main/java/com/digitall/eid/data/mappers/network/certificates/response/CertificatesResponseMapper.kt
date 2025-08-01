/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.data.mappers.network.certificates.response

import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.data.models.network.certificates.response.CertificatesResponse
import com.digitall.eid.data.utils.StrictMapperConfig
import com.digitall.eid.domain.models.certificates.CertificatesModel
import org.mapstruct.Mapper
import org.mapstruct.factory.Mappers

class CertificatesResponseMapper :
    BaseMapper<CertificatesResponse, CertificatesModel>() {

    @Mapper(config = StrictMapperConfig::class)
    fun interface ModelMapper {
        fun map(from: CertificatesResponse): CertificatesModel
    }

    override fun map(from: CertificatesResponse): CertificatesModel {
        return Mappers.getMapper(ModelMapper::class.java).map(from)
    }

}