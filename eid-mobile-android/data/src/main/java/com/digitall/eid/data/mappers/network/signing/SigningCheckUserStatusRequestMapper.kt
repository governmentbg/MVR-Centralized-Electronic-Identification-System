package com.digitall.eid.data.mappers.network.signing

import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.data.models.network.signing.SigningCheckUserStatusRequest
import com.digitall.eid.data.utils.StrictMapperConfig
import com.digitall.eid.domain.models.signing.SigningCheckUserStatusRequestModel
import org.mapstruct.Mapper
import org.mapstruct.Mapping
import org.mapstruct.factory.Mappers

class SigningCheckUserStatusRequestMapper: BaseMapper<SigningCheckUserStatusRequestModel, SigningCheckUserStatusRequest>() {

    @Mapper(config = StrictMapperConfig::class)
    fun interface ModelMapper {
        @Mapping(target = "copy", ignore = true)
        fun map(from: SigningCheckUserStatusRequestModel): SigningCheckUserStatusRequest
    }

    override fun map(from: SigningCheckUserStatusRequestModel): SigningCheckUserStatusRequest {
        return Mappers.getMapper(ModelMapper::class.java).map(from)
    }
}