package com.digitall.eid.data.mappers.network.nomenclatures.reasons

import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.data.models.network.nomenclatures.reasons.NomenclaturesReasonsResponse
import com.digitall.eid.data.utils.StrictMapperConfig
import com.digitall.eid.domain.models.nomenclatures.reasons.NomenclaturePermittedUserEnum
import com.digitall.eid.domain.models.nomenclatures.reasons.NomenclaturesReasonsModel
import org.mapstruct.Mapper
import org.mapstruct.MappingConstants
import org.mapstruct.ValueMapping
import org.mapstruct.factory.Mappers

class NomenclaturesGetReasonsResponseMapper :
    BaseMapper<List<NomenclaturesReasonsResponse>, List<NomenclaturesReasonsModel>>() {

    @Mapper(config = StrictMapperConfig::class)
    interface ModelMapper {
        fun map(from: List<NomenclaturesReasonsResponse>): List<NomenclaturesReasonsModel>

        @ValueMapping(target = "PRIVATE", source = "PRIVATE")
        @ValueMapping(target = "ADMIN", source = "ADMIN")
        @ValueMapping(target = "PUBLIC", source = "PUBLIC")
        @ValueMapping(target = "PRIVATE", source = MappingConstants.ANY_REMAINING)
        fun mapPermittedUser(permittedUser: String): NomenclaturePermittedUserEnum
    }

    override fun map(from: List<NomenclaturesReasonsResponse>): List<NomenclaturesReasonsModel> {
        return Mappers.getMapper(ModelMapper::class.java).map(from)
    }
}