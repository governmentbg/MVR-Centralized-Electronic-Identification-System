/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.data.mappers.network.signing.evrotrust

import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.data.models.network.signing.evrotrust.SigningEvrotrustSignResponse
import com.digitall.eid.data.utils.StrictMapperConfig
import com.digitall.eid.domain.models.signing.evrotrust.SigningEvrotrustSignResponseModel
import org.mapstruct.Mapper
import org.mapstruct.factory.Mappers

class SigningEvrotrustSignResponseMapper :
    BaseMapper<SigningEvrotrustSignResponse, SigningEvrotrustSignResponseModel>() {

    @Mapper(config = StrictMapperConfig::class)
    fun interface ModelMapper {
        fun map(from: SigningEvrotrustSignResponse): SigningEvrotrustSignResponseModel
    }

    override fun map(from: SigningEvrotrustSignResponse): SigningEvrotrustSignResponseModel {
        return Mappers.getMapper(ModelMapper::class.java).map(from)
    }

}