package com.digitall.eid.data.mappers.network.devices.response

import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.data.models.network.devices.DeviceResponse
import com.digitall.eid.data.utils.StrictMapperConfig
import com.digitall.eid.domain.models.devices.DeviceModel
import org.mapstruct.Mapper
import org.mapstruct.factory.Mappers

class DevicesResponseMapper : BaseMapper<List<DeviceResponse>, List<DeviceModel>>() {

    @Mapper(config = StrictMapperConfig::class)
    fun interface ModelMapper {
        fun map(from: List<DeviceResponse>): List<DeviceModel>
    }

    override fun map(from: List<DeviceResponse>): List<DeviceModel> {
        return Mappers.getMapper(ModelMapper::class.java).map(from)
    }
}