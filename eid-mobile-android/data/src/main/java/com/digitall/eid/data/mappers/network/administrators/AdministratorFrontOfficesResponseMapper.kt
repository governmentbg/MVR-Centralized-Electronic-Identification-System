package com.digitall.eid.data.mappers.network.administrators

import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.data.models.network.administrators.AdministratorFrontOfficeResponse
import com.digitall.eid.data.utils.StrictMapperConfig
import com.digitall.eid.domain.models.administrators.AdministratorFrontOfficeModel
import org.mapstruct.Mapper
import org.mapstruct.factory.Mappers

class AdministratorFrontOfficesResponseMapper :
    BaseMapper<List<AdministratorFrontOfficeResponse>, List<AdministratorFrontOfficeModel>>() {

    @Mapper(config = StrictMapperConfig::class)
    fun interface ModelMapper {
        fun map(from: List<AdministratorFrontOfficeResponse>): List<AdministratorFrontOfficeModel>
    }

    override fun map(from: List<AdministratorFrontOfficeResponse>): List<AdministratorFrontOfficeModel> {
        return Mappers.getMapper(ModelMapper::class.java).map(from)
    }
}