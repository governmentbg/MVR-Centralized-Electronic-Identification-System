package com.digitall.eid.data.repository.network.assets

import com.digitall.eid.data.mappers.network.assets.AssetsLocalizationsResponseMapper
import com.digitall.eid.data.network.assets.AssetsApi
import com.digitall.eid.data.repository.network.base.BaseRepository
import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.base.onFailure
import com.digitall.eid.domain.models.base.onSuccess
import com.digitall.eid.domain.repository.network.assets.AssetsNetworkRepository
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.flow.flow
import kotlinx.coroutines.flow.flowOn
import org.koin.core.component.inject

class AssetsNetworkRepositoryImpl :
    AssetsNetworkRepository,
    BaseRepository() {

    companion object {
        private const val TAG = "AssetsNetworkRepositoryImplTag"
    }

    private val assetsApi: AssetsApi by inject()
    private val assetsLocalizationsResponseMapper: AssetsLocalizationsResponseMapper by inject()

    override fun getLocalizations(language: String) = flow {
        logDebug("getLocalizations", TAG)
        emit(ResultEmittedData.loading(model = null))
        getResult {
            assetsApi.getLocalizations(language = language)
        }.onSuccess { model, message, responseCode ->
            logDebug("getLocalizations onSuccess", TAG)
            emit(
                ResultEmittedData.success(
                    model = assetsLocalizationsResponseMapper.map(model),
                    message = message,
                    responseCode = responseCode,
                )
            )
        }.onFailure { error, title, message, responseCode, errorType ->
            logError("getLocalizations onFailure", message, TAG)
            emit(
                ResultEmittedData.error(
                    model = null,
                    error = error,
                    title = title,
                    message = message,
                    errorType = errorType,
                    responseCode = responseCode,
                )
            )
        }
    }.flowOn(Dispatchers.IO)

}