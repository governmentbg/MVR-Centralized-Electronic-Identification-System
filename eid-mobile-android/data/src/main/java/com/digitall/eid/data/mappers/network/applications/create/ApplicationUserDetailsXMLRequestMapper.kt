/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.data.mappers.network.applications.create

import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.data.models.network.applications.create.ApplicationDetailsXMLRequest
import com.digitall.eid.data.utils.StrictMapperConfig
import com.digitall.eid.domain.models.applications.create.ApplicationDetailsXMLRequestModel
import org.mapstruct.Mapper
import org.mapstruct.factory.Mappers

class ApplicationUserDetailsXMLRequestMapper :
    BaseMapper<ApplicationDetailsXMLRequestModel, ApplicationDetailsXMLRequest>() {

    @Mapper(config = StrictMapperConfig::class)
    fun interface ModelMapper {
        fun map(from: ApplicationDetailsXMLRequestModel): ApplicationDetailsXMLRequest
    }

    override fun map(from: ApplicationDetailsXMLRequestModel): ApplicationDetailsXMLRequest {
        return Mappers.getMapper(ModelMapper::class.java).map(from)
    }

}