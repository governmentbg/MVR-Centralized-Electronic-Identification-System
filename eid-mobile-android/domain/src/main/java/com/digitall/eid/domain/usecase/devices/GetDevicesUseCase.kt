package com.digitall.eid.domain.usecase.devices

import com.digitall.eid.domain.repository.network.devices.DevicesNetworkRepository
import com.digitall.eid.domain.usecase.base.BaseUseCase
import org.koin.core.component.inject

class GetDevicesUseCase: BaseUseCase {

    companion object {
        private const val TAG = "GetDevicesUseCaseTag"
    }

    private val devicesNetworkRepository: DevicesNetworkRepository by inject()

    fun invoke() = devicesNetworkRepository.getDevices()
}