package com.digitall.eid.data.mappers.network.mfa.request

import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.data.models.network.mfa.request.GenerateNewOtpCodeRequest
import com.digitall.eid.data.utils.StrictMapperConfig
import com.digitall.eid.domain.models.mfa.request.GenerateNewOtpCodeRequestModel
import org.mapstruct.Mapper
import org.mapstruct.Mapping
import org.mapstruct.factory.Mappers

class MfaGenerateNewOtpCodeRequestMapper :
    BaseMapper<GenerateNewOtpCodeRequestModel, GenerateNewOtpCodeRequest>() {

    @Mapper(config = StrictMapperConfig::class)
    fun interface ModelMapper {
        @Mapping(target = "copy", ignore = true)
        fun map(from: GenerateNewOtpCodeRequestModel): GenerateNewOtpCodeRequest
    }

    override fun map(from: GenerateNewOtpCodeRequestModel): GenerateNewOtpCodeRequest {
        return Mappers.getMapper(ModelMapper::class.java).map(from)
    }
}