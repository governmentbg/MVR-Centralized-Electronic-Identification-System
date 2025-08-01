/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.data.mappers.network.signing.borica

import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.data.models.network.signing.borica.SigningBoricaDownloadResponse
import com.digitall.eid.data.utils.StrictMapperConfig
import com.digitall.eid.domain.models.signing.borica.SigningBoricaDownloadResponseModel
import org.mapstruct.Mapper
import org.mapstruct.factory.Mappers

class SigningBoricaDownloadResponseMapper :
    BaseMapper<SigningBoricaDownloadResponse, SigningBoricaDownloadResponseModel>() {

    @Mapper(config = StrictMapperConfig::class)
    fun interface ModelMapper {
        fun map(from: SigningBoricaDownloadResponse): SigningBoricaDownloadResponseModel
    }

    override fun map(from: SigningBoricaDownloadResponse): SigningBoricaDownloadResponseModel {
        return Mappers.getMapper(ModelMapper::class.java).map(from)
    }

}