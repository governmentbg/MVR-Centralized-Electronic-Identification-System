package com.digitall.eid.data.mappers.network.citizen.registration.request

import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.data.models.network.citizen.registration.CitizenRegisterNewUserRequest
import com.digitall.eid.data.utils.StrictMapperConfig
import com.digitall.eid.domain.models.citizen.register.CitizenRegisterNewUserRequestModel
import org.mapstruct.Mapper
import org.mapstruct.factory.Mappers

class RegisterNewCitizenRequestMapper :
    BaseMapper<CitizenRegisterNewUserRequestModel, CitizenRegisterNewUserRequest>() {

    @Mapper(config = StrictMapperConfig::class)
    fun interface ModelMapper {
        fun map(from: CitizenRegisterNewUserRequestModel): CitizenRegisterNewUserRequest
    }

    override fun map(from: CitizenRegisterNewUserRequestModel): CitizenRegisterNewUserRequest {
        return Mappers.getMapper(ModelMapper::class.java).map(from)
    }
}