package com.digitall.eid.data.mappers.network.mfa.request

import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.data.models.network.mfa.request.VerifyOtpCodeRequest
import com.digitall.eid.data.utils.StrictMapperConfig
import com.digitall.eid.domain.models.mfa.request.VerifyOtpCodeRequestModel
import org.mapstruct.Mapper
import org.mapstruct.factory.Mappers

class MfaVerifyOtpCodeRequestMapper :
    BaseMapper<VerifyOtpCodeRequestModel, VerifyOtpCodeRequest>() {

    @Mapper(config = StrictMapperConfig::class)
    fun interface ModelMapper {
        fun map(from: VerifyOtpCodeRequestModel): VerifyOtpCodeRequest
    }

    override fun map(from: VerifyOtpCodeRequestModel): VerifyOtpCodeRequest {
        return Mappers.getMapper(ModelMapper::class.java).map(from)
    }
}
