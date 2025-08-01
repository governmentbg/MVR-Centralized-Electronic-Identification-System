package com.digitall.eid.data.mappers.network.events.request

import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.data.models.network.events.request.EventRequest
import com.digitall.eid.data.utils.StrictMapperConfig
import com.digitall.eid.domain.models.events.request.EventRequestModel
import org.mapstruct.Mapper
import org.mapstruct.factory.Mappers

class EventsRequestMapper: BaseMapper<EventRequestModel, EventRequest>() {

    @Mapper(config = StrictMapperConfig::class)
    fun interface ModelMapper {
        fun map(from: EventRequestModel): EventRequest
    }

    override fun map(from: EventRequestModel): EventRequest {
        return Mappers.getMapper(ModelMapper::class.java).map(from)
    }
}