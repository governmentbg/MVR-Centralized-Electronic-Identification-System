/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.data.mappers.network.applications.create

import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.data.models.network.applications.create.ApplicationConfirmWithEIDRequest
import com.digitall.eid.data.utils.StrictMapperConfig
import com.digitall.eid.domain.models.applications.create.ApplicationConfirmWithEIDRequestModel
import org.mapstruct.Mapper
import org.mapstruct.factory.Mappers

class ApplicationConfirmWithEIDRequestMapper :
    BaseMapper<ApplicationConfirmWithEIDRequestModel, ApplicationConfirmWithEIDRequest>() {

    @Mapper(config = StrictMapperConfig::class)
    fun interface ModelMapper {
        fun map(from: ApplicationConfirmWithEIDRequestModel): ApplicationConfirmWithEIDRequest
    }

    override fun map(from: ApplicationConfirmWithEIDRequestModel): ApplicationConfirmWithEIDRequest {
        return Mappers.getMapper(ModelMapper::class.java).map(from)
    }

}