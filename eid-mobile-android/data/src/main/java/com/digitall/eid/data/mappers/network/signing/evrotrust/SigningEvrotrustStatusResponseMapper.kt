/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.data.mappers.network.signing.evrotrust

import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.data.models.network.signing.evrotrust.SigningEvrotrustStatusResponse
import com.digitall.eid.data.utils.StrictMapperConfig
import com.digitall.eid.domain.models.signing.evrotrust.SigningEvrotrustStatusResponseModel
import org.mapstruct.Mapper
import org.mapstruct.factory.Mappers

class SigningEvrotrustStatusResponseMapper :
    BaseMapper<SigningEvrotrustStatusResponse, SigningEvrotrustStatusResponseModel>() {


    @Mapper(config = StrictMapperConfig::class)
    fun interface ModelMapper {
        fun map(from: SigningEvrotrustStatusResponse): SigningEvrotrustStatusResponseModel

    }

    override fun map(from: SigningEvrotrustStatusResponse): SigningEvrotrustStatusResponseModel {
        return Mappers.getMapper(ModelMapper::class.java).map(from)
    }

}