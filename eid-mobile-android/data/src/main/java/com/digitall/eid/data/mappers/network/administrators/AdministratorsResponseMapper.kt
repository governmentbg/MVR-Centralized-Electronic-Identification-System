/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.data.mappers.network.administrators

import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.data.models.network.administrators.AdministratorResponse
import com.digitall.eid.data.utils.StrictMapperConfig
import com.digitall.eid.domain.models.administrators.AdministratorModel
import org.mapstruct.Mapper
import org.mapstruct.factory.Mappers

class AdministratorsResponseMapper :
    BaseMapper<List<AdministratorResponse>, List<AdministratorModel>>() {

    @Mapper(config = StrictMapperConfig::class)
    fun interface ModelMapper {
        fun map(from: List<AdministratorResponse>): List<AdministratorModel>
    }

    override fun map(from: List<AdministratorResponse>): List<AdministratorModel> {
        return Mappers.getMapper(ModelMapper::class.java).map(from)
    }

}