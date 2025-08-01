/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.data.mappers.network.empowerment.signing

import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.data.models.network.empowerment.signing.EmpowermentSigningSignRequest
import com.digitall.eid.data.utils.StrictMapperConfig
import com.digitall.eid.domain.models.empowerment.signing.EmpowermentSigningSignRequestModel
import org.mapstruct.Mapper
import org.mapstruct.factory.Mappers

class EmpowermentSigningSignRequestMapper :
    BaseMapper<EmpowermentSigningSignRequestModel, EmpowermentSigningSignRequest>() {

    @Mapper(config = StrictMapperConfig::class)
    fun interface ModelMapper {
        fun map(from: EmpowermentSigningSignRequestModel): EmpowermentSigningSignRequest
    }

    override fun map(from: EmpowermentSigningSignRequestModel): EmpowermentSigningSignRequest {
        return Mappers.getMapper(ModelMapper::class.java).map(from)
    }
}