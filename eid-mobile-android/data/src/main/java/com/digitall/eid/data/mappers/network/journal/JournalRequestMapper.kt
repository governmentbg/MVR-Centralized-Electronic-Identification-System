package com.digitall.eid.data.mappers.network.journal

import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.data.models.network.journal.JournalRequest
import com.digitall.eid.data.utils.StrictMapperConfig
import com.digitall.eid.domain.models.journal.all.JournalRequestModel
import org.mapstruct.Mapper
import org.mapstruct.factory.Mappers

class JournalRequestMapper :
    BaseMapper<JournalRequestModel, JournalRequest>() {

    @Mapper(config = StrictMapperConfig::class)
    fun interface ModelMapper {
        fun map(from: JournalRequestModel): JournalRequest
    }

    override fun map(from: JournalRequestModel): JournalRequest {
        return Mappers.getMapper(ModelMapper::class.java).map(from)
    }

}