package com.digitall.eid.data.network.devices

import com.digitall.eid.data.models.network.devices.DeviceResponse
import retrofit2.Response
import retrofit2.http.GET

interface DevicesApi {

    @GET("raeicei/external/api/v1/device/getAll")
    suspend fun getDevices(): Response<List<DeviceResponse>>
}