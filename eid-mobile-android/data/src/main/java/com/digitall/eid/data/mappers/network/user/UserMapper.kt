/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.data.mappers.network.user

import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.data.models.network.user.UserJsonData
import com.digitall.eid.data.utils.StrictMapperConfig
import com.digitall.eid.domain.models.user.UserAcrEnum
import com.digitall.eid.domain.models.user.UserModel
import org.mapstruct.Mapper
import org.mapstruct.MappingConstants
import org.mapstruct.ValueMapping
import org.mapstruct.factory.Mappers

class UserMapper : BaseMapper<UserJsonData, UserModel>() {

    @Mapper(config = StrictMapperConfig::class)
    interface ModelMapper {

        fun map(from: UserJsonData): UserModel

        @ValueMapping(target = "LOW", source = "eid_low")
        @ValueMapping(target = "SUBSTANTIAL", source = "eid_substantial")
        @ValueMapping(target = "HIGH", source = "eid_high")
        @ValueMapping(target = "LOW", source = MappingConstants.ANY_REMAINING)
        fun mapArc(arc: String): UserAcrEnum
    }

    override fun map(from: UserJsonData): UserModel {
        return Mappers.getMapper(ModelMapper::class.java).map(from)
    }

}