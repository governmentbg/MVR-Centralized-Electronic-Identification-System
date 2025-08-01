package com.digitall.eid.domain.usecase.events

import com.digitall.eid.domain.models.events.request.EventRequestModel
import com.digitall.eid.domain.repository.network.events.EventsNetworkRepository
import com.digitall.eid.domain.usecase.base.BaseUseCase
import org.koin.core.component.inject

class LogEventUseCase: BaseUseCase {

    companion object {
        private const val TAG = "LogEventUseCaseTag"
    }

    private val eventsNetworkRepository: EventsNetworkRepository by inject()

    fun invoke(data: EventRequestModel) = eventsNetworkRepository.logEvent(data = data)
}