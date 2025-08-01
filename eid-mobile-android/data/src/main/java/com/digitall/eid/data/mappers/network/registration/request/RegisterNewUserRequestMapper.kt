package com.digitall.eid.data.mappers.network.registration.request

import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.data.models.network.registration.RegisterNewUserRequest
import com.digitall.eid.data.utils.StrictMapperConfig
import com.digitall.eid.domain.models.user.RegisterNewUserRequestModel
import org.mapstruct.Mapper
import org.mapstruct.factory.Mappers

class RegisterNewUserRequestMapper :
    BaseMapper<RegisterNewUserRequestModel, RegisterNewUserRequest>() {

    @Mapper(config = StrictMapperConfig::class)
    fun interface ModelMapper {
        fun map(from: RegisterNewUserRequestModel): RegisterNewUserRequest
    }

    override fun map(from: RegisterNewUserRequestModel): RegisterNewUserRequest {
        return Mappers.getMapper(ModelMapper::class.java).map(from)
    }
}