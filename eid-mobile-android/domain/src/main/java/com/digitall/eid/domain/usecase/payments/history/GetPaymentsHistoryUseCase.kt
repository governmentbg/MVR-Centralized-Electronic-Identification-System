package com.digitall.eid.domain.usecase.payments.history

import com.digitall.eid.domain.repository.network.payments.PaymentsNetworkRepository
import com.digitall.eid.domain.usecase.base.BaseUseCase
import org.koin.core.component.inject

class GetPaymentsHistoryUseCase: BaseUseCase {

    companion object {
        private const val TAG = "GetPaymentsHistoryUseCaseTag"
    }

    private val paymentsNetworkRepository: PaymentsNetworkRepository by inject()

    fun invoke() = paymentsNetworkRepository.getHistory()
}