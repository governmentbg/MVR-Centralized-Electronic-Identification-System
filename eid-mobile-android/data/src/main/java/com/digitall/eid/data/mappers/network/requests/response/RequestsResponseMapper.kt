package com.digitall.eid.data.mappers.network.requests.response

import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.data.models.network.requests.response.RequestResponse
import com.digitall.eid.data.utils.StrictMapperConfig
import com.digitall.eid.domain.models.requests.response.RequestResponseModel
import org.mapstruct.Mapper
import org.mapstruct.factory.Mappers

class RequestsResponseMapper: BaseMapper<List<RequestResponse>, List<RequestResponseModel>>() {

    @Mapper(config = StrictMapperConfig::class)
    fun interface ModelMapper {
        fun map(from: List<RequestResponse>): List<RequestResponseModel>
    }

    override fun map(from: List<RequestResponse>): List<RequestResponseModel> {
        return Mappers.getMapper(ModelMapper::class.java).map(from)
    }
}