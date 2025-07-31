/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.data.mappers.network.applications.all

import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.data.models.network.applications.all.ApplicationDetailsResponse
import com.digitall.eid.data.models.network.applications.all.ApplicationDetailsXml
import com.digitall.eid.data.utils.StrictMapperConfig
import com.digitall.eid.domain.models.applications.all.ApplicationDetailsFromJSONModel
import com.digitall.eid.domain.models.applications.all.ApplicationDetailsFromXMLModel
import com.digitall.eid.domain.models.applications.all.ApplicationDetailsModel
import com.digitall.eid.domain.utils.LogUtil.logError
import org.koin.core.component.KoinComponent
import org.koin.core.component.inject
import org.mapstruct.Mapper
import org.mapstruct.factory.Mappers
import org.simpleframework.xml.core.Persister

class ApplicationDetailsResponseMapper :
    BaseMapper<ApplicationDetailsResponse, ApplicationDetailsModel>(),
    KoinComponent {

    companion object {
        private const val TAG = "ApplicationDetailsResponseMapperTag"
    }

    private val serializer: Persister by inject()

    @Mapper(config = StrictMapperConfig::class)
    interface ModelMapper {
        fun mapJSON(from: ApplicationDetailsResponse): ApplicationDetailsFromJSONModel
        fun mapXML(from: ApplicationDetailsXml): ApplicationDetailsFromXMLModel
    }

    override fun map(from: ApplicationDetailsResponse): ApplicationDetailsModel {
        val data = Mappers.getMapper(ModelMapper::class.java).mapJSON(from)
        return try {
            val xml = serializer.read(ApplicationDetailsXml::class.java, from.xml!!)!!
            ApplicationDetailsModel(
                fromJSON = data,
                fromXML = Mappers.getMapper(ModelMapper::class.java).mapXML(xml),
            )
        } catch (e: Exception) {
            logError("parse xml exception: ${e.message}", e, TAG)
            ApplicationDetailsModel(
                fromJSON = data,
                fromXML = null,
            )
        }
    }

}