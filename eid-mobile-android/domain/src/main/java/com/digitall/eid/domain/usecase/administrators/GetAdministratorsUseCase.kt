/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.usecase.administrators

import com.digitall.eid.domain.repository.network.administrators.AdministratorsNetworkRepository
import com.digitall.eid.domain.usecase.base.BaseUseCase
import org.koin.core.component.inject

class GetAdministratorsUseCase : BaseUseCase {

    companion object {
        private const val TAG = "GetAdministratorsUseCaseTag"
    }

    private val administratorsNetworkRepository: AdministratorsNetworkRepository by inject()

    fun invoke() = administratorsNetworkRepository.getAdministrators()

}