/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.data.mappers.network.empowerment.signing

import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.data.models.network.empowerment.common.EmpowermentReasonResponse
import com.digitall.eid.data.utils.StrictMapperConfig
import com.digitall.eid.domain.models.empowerment.common.EmpowermentReasonModel
import org.mapstruct.Mapper
import org.mapstruct.factory.Mappers

class EmpowermentReasonResponseMapper :
    BaseMapper<EmpowermentReasonResponse, EmpowermentReasonModel>() {

    @Mapper(config = StrictMapperConfig::class)
    fun interface ModelMapper {
        fun map(from: EmpowermentReasonResponse): EmpowermentReasonModel
    }

    override fun map(from: EmpowermentReasonResponse): EmpowermentReasonModel {
        return Mappers.getMapper(ModelMapper::class.java).map(from)
    }
}