/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.data.mappers.network.certificates.response

import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.data.models.network.certificates.response.CertificateDetailsResponse
import com.digitall.eid.data.utils.StrictMapperConfig
import com.digitall.eid.domain.models.certificates.CertificateDetailsModel
import org.mapstruct.Mapper
import org.mapstruct.factory.Mappers

class CertificateDetailsResponseMapper :
    BaseMapper<CertificateDetailsResponse, CertificateDetailsModel>() {

    @Mapper(config = StrictMapperConfig::class)
    fun interface ModelMapper {
        fun map(from: CertificateDetailsResponse): CertificateDetailsModel
    }

    override fun map(from: CertificateDetailsResponse): CertificateDetailsModel {
        return Mappers.getMapper(ModelMapper::class.java).map(from)
    }

}