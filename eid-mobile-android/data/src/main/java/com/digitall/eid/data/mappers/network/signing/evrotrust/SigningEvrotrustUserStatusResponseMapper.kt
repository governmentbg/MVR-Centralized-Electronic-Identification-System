/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.data.mappers.network.signing.evrotrust

import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.data.models.network.signing.evrotrust.SigningEvrotrustUserStatusResponse
import com.digitall.eid.data.utils.StrictMapperConfig
import com.digitall.eid.domain.models.signing.evrotrust.SigningEvrotrustUserStatusResponseModel
import org.mapstruct.Mapper
import org.mapstruct.factory.Mappers

class SigningEvrotrustUserStatusResponseMapper :
    BaseMapper<SigningEvrotrustUserStatusResponse, SigningEvrotrustUserStatusResponseModel>() {

    @Mapper(config = StrictMapperConfig::class)
    fun interface ModelMapper {
        fun map(from: SigningEvrotrustUserStatusResponse): SigningEvrotrustUserStatusResponseModel
    }

    override fun map(from: SigningEvrotrustUserStatusResponse): SigningEvrotrustUserStatusResponseModel {
        return Mappers.getMapper(ModelMapper::class.java).map(from)
    }
}