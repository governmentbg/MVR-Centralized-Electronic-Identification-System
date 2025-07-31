/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.data.mappers.network.applications.all

import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.data.models.network.applications.all.ApplicationsResponse
import com.digitall.eid.data.utils.StrictMapperConfig
import com.digitall.eid.domain.models.applications.all.ApplicationsModel
import org.mapstruct.Mapper
import org.mapstruct.factory.Mappers

class ApplicationsResponseMapper :
    BaseMapper<ApplicationsResponse, ApplicationsModel>() {

    @Mapper(config = StrictMapperConfig::class)
    fun interface ModelMapper {
        fun map(from: ApplicationsResponse): ApplicationsModel
    }

    override fun map(from: ApplicationsResponse): ApplicationsModel {
        return Mappers.getMapper(ModelMapper::class.java).map(from)
    }

}