package com.digitall.eid.domain.usecase.assets.localization

import com.digitall.eid.domain.repository.network.assets.AssetsNetworkRepository
import com.digitall.eid.domain.usecase.base.BaseUseCase
import org.koin.core.component.inject

class GetAssetsLocalizationsUseCase : BaseUseCase {

    companion object {
        private const val TAG = "AssetsGetLocalizationsTag"
    }

    private val assetsNetworkRepository: AssetsNetworkRepository by inject()

    fun invoke(language: String) = assetsNetworkRepository.getLocalizations(language = language)

}