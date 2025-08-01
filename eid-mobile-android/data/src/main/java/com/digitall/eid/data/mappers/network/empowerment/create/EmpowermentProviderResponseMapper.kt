/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.data.mappers.network.empowerment.create

import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.data.models.network.empowerment.create.providers.EmpowermentProvidersResponse
import com.digitall.eid.data.utils.StrictMapperConfig
import com.digitall.eid.domain.models.empowerment.create.EmpowermentProvidersModel
import org.mapstruct.Mapper
import org.mapstruct.factory.Mappers

class EmpowermentProviderResponseMapper :
    BaseMapper<EmpowermentProvidersResponse, EmpowermentProvidersModel>() {

    @Mapper(config = StrictMapperConfig::class)
    fun interface ModelMapper {
        fun map(from: EmpowermentProvidersResponse): EmpowermentProvidersModel
    }

    override fun map(from: EmpowermentProvidersResponse): EmpowermentProvidersModel {
        return Mappers.getMapper(ModelMapper::class.java).map(from)
    }

}