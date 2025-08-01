/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.data.mappers.network.empowerment.common.all

import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.data.models.network.empowerment.common.all.EmpowermentRequest
import com.digitall.eid.data.utils.StrictMapperConfig
import com.digitall.eid.domain.models.empowerment.common.all.EmpowermentRequestModel
import org.mapstruct.Mapper
import org.mapstruct.factory.Mappers

class EmpowermentRequestMapper :
    BaseMapper<EmpowermentRequestModel, EmpowermentRequest>() {

    @Mapper(config = StrictMapperConfig::class)
    fun interface ModelMapper {
        fun map(from: EmpowermentRequestModel): EmpowermentRequest
    }

    override fun map(from: EmpowermentRequestModel): EmpowermentRequest {
        return Mappers.getMapper(ModelMapper::class.java).map(from)
    }

}