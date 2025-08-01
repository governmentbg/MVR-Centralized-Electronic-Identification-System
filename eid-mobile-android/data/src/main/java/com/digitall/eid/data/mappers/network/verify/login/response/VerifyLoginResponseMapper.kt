package com.digitall.eid.data.mappers.network.verify.login.response

import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.data.models.network.verify.login.response.VerifyLoginResponse
import com.digitall.eid.data.utils.StrictMapperConfig
import com.digitall.eid.domain.models.verify.login.response.VerifyLoginResponseModel
import org.mapstruct.Mapper
import org.mapstruct.factory.Mappers

class VerifyLoginResponseMapper :
    BaseMapper<VerifyLoginResponse, VerifyLoginResponseModel>() {

    @Mapper(config = StrictMapperConfig::class)
    fun interface ModelMapper {
        fun map(from: VerifyLoginResponse): VerifyLoginResponseModel
    }

    override fun map(from: VerifyLoginResponse): VerifyLoginResponseModel {
        return Mappers.getMapper(ModelMapper::class.java).map(from)
    }
}