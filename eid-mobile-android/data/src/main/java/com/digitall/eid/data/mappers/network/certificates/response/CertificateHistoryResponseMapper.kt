package com.digitall.eid.data.mappers.network.certificates.response

import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.data.models.network.certificates.response.CertificateHistoryElementResponse
import com.digitall.eid.data.utils.StrictMapperConfig
import com.digitall.eid.domain.models.certificates.CertificateHistoryElementModel
import org.mapstruct.Mapper
import org.mapstruct.factory.Mappers

class CertificateHistoryResponseMapper: BaseMapper<List<CertificateHistoryElementResponse>, List<CertificateHistoryElementModel>>() {

    @Mapper(config = StrictMapperConfig::class)
    fun interface ModelMapper {
        fun map(from: List<CertificateHistoryElementResponse>): List<CertificateHistoryElementModel>
    }

    override fun map(from: List<CertificateHistoryElementResponse>): List<CertificateHistoryElementModel> {
        return Mappers.getMapper(ModelMapper::class.java).map(from)
    }
}