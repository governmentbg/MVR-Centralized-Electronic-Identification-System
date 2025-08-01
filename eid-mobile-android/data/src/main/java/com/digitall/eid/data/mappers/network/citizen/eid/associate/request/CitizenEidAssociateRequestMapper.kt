package com.digitall.eid.data.mappers.network.citizen.eid.associate.request

import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.data.models.network.challenge.request.SignedChallengeRequest
import com.digitall.eid.data.utils.StrictMapperConfig
import com.digitall.eid.domain.models.challenge.request.SignedChallengeRequestModel
import org.mapstruct.Mapper
import org.mapstruct.factory.Mappers

class CitizenEidAssociateRequestMapper :
    BaseMapper<SignedChallengeRequestModel, SignedChallengeRequest>() {

    @Mapper(config = StrictMapperConfig::class)
    fun interface ModelMapper {
        fun map(from: SignedChallengeRequestModel): SignedChallengeRequest
    }

    override fun map(from: SignedChallengeRequestModel): SignedChallengeRequest {
        return Mappers.getMapper(ModelMapper::class.java).map(from)
    }
}