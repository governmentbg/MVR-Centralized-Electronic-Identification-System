package com.digitall.eid.data.mappers.network.citizen.update.infromation.request

import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.data.models.network.citizen.update.information.CitizenUpdateInformationRequest
import com.digitall.eid.data.utils.StrictMapperConfig
import com.digitall.eid.domain.models.citizen.update.information.CitizenUpdateInformationRequestModel
import org.mapstruct.Mapper
import org.mapstruct.factory.Mappers

class CitizenUpdateInformationRequestMapper : BaseMapper<CitizenUpdateInformationRequestModel, CitizenUpdateInformationRequest>() {

    @Mapper(config = StrictMapperConfig::class)
    fun interface ModelMapper {
        fun map(from: CitizenUpdateInformationRequestModel): CitizenUpdateInformationRequest
    }

    override fun map(from: CitizenUpdateInformationRequestModel): CitizenUpdateInformationRequest {
        return Mappers.getMapper(ModelMapper::class.java).map(from)
    }
}