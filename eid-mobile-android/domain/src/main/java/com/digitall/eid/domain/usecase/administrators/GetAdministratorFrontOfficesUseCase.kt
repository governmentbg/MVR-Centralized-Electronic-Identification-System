package com.digitall.eid.domain.usecase.administrators

import com.digitall.eid.domain.repository.network.administrators.AdministratorsNetworkRepository
import com.digitall.eid.domain.usecase.base.BaseUseCase
import org.koin.core.component.inject

class GetAdministratorFrontOfficesUseCase : BaseUseCase {

    companion object {
        private const val TAG = "GetAdministratorFrontOfficesUseCaseTag"
    }

    private val administratorsNetworkRepository: AdministratorsNetworkRepository by inject()

    fun invoke(eidAdministratorId: String) = administratorsNetworkRepository.getAdministratorFrontOffices(eidAdministratorId = eidAdministratorId)
}