/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.data.mappers.network.applications.create

import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.data.models.network.applications.create.ApplicationEnrollWithEIDRequest
import com.digitall.eid.data.utils.StrictMapperConfig
import com.digitall.eid.domain.models.applications.create.ApplicationEnrollWithEIDRequestModel
import org.mapstruct.Mapper
import org.mapstruct.factory.Mappers

class ApplicationEnrollWithEIDRequestMapper :
    BaseMapper<ApplicationEnrollWithEIDRequestModel, ApplicationEnrollWithEIDRequest>() {

    @Mapper(config = StrictMapperConfig::class)
    fun interface ModelMapper {
        fun map(from: ApplicationEnrollWithEIDRequestModel): ApplicationEnrollWithEIDRequest
    }

    override fun map(from: ApplicationEnrollWithEIDRequestModel): ApplicationEnrollWithEIDRequest {
        return Mappers.getMapper(ModelMapper::class.java).map(from)
    }

}