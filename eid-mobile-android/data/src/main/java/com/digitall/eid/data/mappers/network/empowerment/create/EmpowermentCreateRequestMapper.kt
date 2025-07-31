/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.data.mappers.network.empowerment.create

import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.data.models.network.empowerment.create.create.EmpowermentCreateRequest
import com.digitall.eid.data.utils.StrictMapperConfig
import com.digitall.eid.domain.models.empowerment.create.EmpowermentCreateModel
import org.mapstruct.Mapper
import org.mapstruct.factory.Mappers

class EmpowermentCreateRequestMapper :
    BaseMapper<EmpowermentCreateModel, EmpowermentCreateRequest>() {

    @Mapper(config = StrictMapperConfig::class)
    fun interface ModelMapper {
        fun map(from: EmpowermentCreateModel): EmpowermentCreateRequest
    }

    override fun map(from: EmpowermentCreateModel): EmpowermentCreateRequest {
        return Mappers.getMapper(ModelMapper::class.java).map(from)
    }

}