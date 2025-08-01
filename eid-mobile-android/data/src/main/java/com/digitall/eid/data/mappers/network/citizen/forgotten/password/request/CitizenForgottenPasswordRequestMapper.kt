package com.digitall.eid.data.mappers.network.citizen.forgotten.password.request

import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.data.models.network.citizen.forgotten.password.CitizenForgottenPasswordRequest
import com.digitall.eid.data.utils.StrictMapperConfig
import com.digitall.eid.domain.models.citizen.forgotten.password.CitizenForgottenPasswordRequestModel
import org.mapstruct.Mapper
import org.mapstruct.factory.Mappers

class CitizenForgottenPasswordRequestMapper :
    BaseMapper<CitizenForgottenPasswordRequestModel, CitizenForgottenPasswordRequest>() {

    @Mapper(config = StrictMapperConfig::class)
    fun interface ModelMapper {
        fun map(from: CitizenForgottenPasswordRequestModel): CitizenForgottenPasswordRequest
    }

    override fun map(from: CitizenForgottenPasswordRequestModel): CitizenForgottenPasswordRequest {
        return Mappers.getMapper(ModelMapper::class.java).map(from)
    }
}