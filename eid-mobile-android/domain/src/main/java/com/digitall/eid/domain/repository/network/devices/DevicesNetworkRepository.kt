package com.digitall.eid.domain.repository.network.devices

import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.devices.DeviceModel
import kotlinx.coroutines.flow.Flow

interface DevicesNetworkRepository {

    fun getDevices(): Flow<ResultEmittedData<List<DeviceModel>>>
}