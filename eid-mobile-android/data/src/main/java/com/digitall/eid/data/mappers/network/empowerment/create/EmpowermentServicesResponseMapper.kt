/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.data.mappers.network.empowerment.create

import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.data.models.network.empowerment.create.services.EmpowermentServicesGetResponse
import com.digitall.eid.data.utils.StrictMapperConfig
import com.digitall.eid.domain.models.empowerment.create.EmpowermentServicesModel
import org.mapstruct.Mapper
import org.mapstruct.factory.Mappers

class EmpowermentServicesResponseMapper :
    BaseMapper<EmpowermentServicesGetResponse, EmpowermentServicesModel>() {

    @Mapper(config = StrictMapperConfig::class)
    fun interface ModelMapper {
        fun map(from: EmpowermentServicesGetResponse): EmpowermentServicesModel
    }

    override fun map(from: EmpowermentServicesGetResponse): EmpowermentServicesModel {
        return Mappers.getMapper(ModelMapper::class.java).map(from)
    }

}