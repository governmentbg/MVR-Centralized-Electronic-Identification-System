package com.digitall.eid.data.mappers.network.verify.login.request

import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.data.models.network.verify.login.request.VerifyLoginRequest
import com.digitall.eid.data.utils.StrictMapperConfig
import com.digitall.eid.domain.models.verify.login.request.VerifyLoginRequestModel
import org.mapstruct.Mapper
import org.mapstruct.factory.Mappers

class VerifyLoginRequestMapper :
    BaseMapper<VerifyLoginRequestModel, VerifyLoginRequest>() {

    @Mapper(config = StrictMapperConfig::class)
    fun interface ModelMapper {
        fun map(from: VerifyLoginRequestModel): VerifyLoginRequest
    }

    override fun map(from: VerifyLoginRequestModel): VerifyLoginRequest {
        return Mappers.getMapper(ModelMapper::class.java).map(from)
    }
}