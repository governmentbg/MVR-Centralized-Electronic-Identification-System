package com.digitall.eid.domain.usecase.requests

import com.digitall.eid.domain.repository.network.requests.RequestsNetworkRepository
import com.digitall.eid.domain.usecase.base.BaseUseCase
import org.koin.core.component.inject

class GetAllRequestsUseCase: BaseUseCase {

    companion object {
        private const val TAG = "GetAllRequestsUseCaseTag"
    }

    private val requestsNetworkRepository: RequestsNetworkRepository by inject()

    fun invoke() = requestsNetworkRepository.getAll()

}