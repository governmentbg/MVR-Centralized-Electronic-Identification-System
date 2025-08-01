package com.digitall.eid.data.mappers.network.empowerment.legal

import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.data.models.network.empowerment.legal.EmpowermentLegalRequest
import com.digitall.eid.data.utils.StrictMapperConfig
import com.digitall.eid.domain.models.empowerment.legal.EmpowermentLegalRequestModel
import org.mapstruct.Mapper
import org.mapstruct.factory.Mappers

class EmpowermentLegalRequestMapper :
    BaseMapper<EmpowermentLegalRequestModel, EmpowermentLegalRequest>() {

    @Mapper(config = StrictMapperConfig::class)
    fun interface ModelMapper {
        fun map(from: EmpowermentLegalRequestModel): EmpowermentLegalRequest
    }

    override fun map(from: EmpowermentLegalRequestModel): EmpowermentLegalRequest {
        return Mappers.getMapper(ModelMapper::class.java).map(from)
    }

}