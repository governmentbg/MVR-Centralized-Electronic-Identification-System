/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.data.mappers.network.journal

import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.data.models.network.journal.JournalResponse
import com.digitall.eid.data.utils.StrictMapperConfig
import com.digitall.eid.domain.models.journal.all.JournalModel
import org.mapstruct.Mapper
import org.mapstruct.factory.Mappers

class JournalResponseMapper :
    BaseMapper<JournalResponse, JournalModel>() {

    @Mapper(config = StrictMapperConfig::class)
    fun interface ModelMapper {
        fun map(from: JournalResponse): JournalModel
    }

    override fun map(from: JournalResponse): JournalModel {
        return Mappers.getMapper(ModelMapper::class.java).map(from)
    }

}