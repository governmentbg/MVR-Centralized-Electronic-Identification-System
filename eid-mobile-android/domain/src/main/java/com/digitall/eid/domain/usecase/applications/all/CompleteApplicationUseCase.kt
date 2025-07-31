package com.digitall.eid.domain.usecase.applications.all

import com.digitall.eid.domain.repository.network.applications.ApplicationsNetworkRepository
import com.digitall.eid.domain.usecase.base.BaseUseCase
import org.koin.core.component.inject

class CompleteApplicationUseCase: BaseUseCase {

    companion object {
        private const val TAG = "CompleteApplicationUseCaseTag"
    }

    private val applicationsNetworkRepository: ApplicationsNetworkRepository by inject()

    fun invoke(id: String) = applicationsNetworkRepository.completeApplication(id = id)
}