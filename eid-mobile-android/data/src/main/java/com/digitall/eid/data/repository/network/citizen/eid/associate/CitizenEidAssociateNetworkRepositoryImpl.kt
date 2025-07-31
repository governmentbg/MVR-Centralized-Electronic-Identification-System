package com.digitall.eid.data.repository.network.citizen.eid.associate

import com.digitall.eid.data.BuildConfig.CLIENT_ID
import com.digitall.eid.data.mappers.network.citizen.eid.associate.request.CitizenEidAssociateRequestMapper
import com.digitall.eid.data.network.citizen.eid.associate.CitizenEidAssociateApi
import com.digitall.eid.data.repository.network.base.BaseRepository
import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.base.onFailure
import com.digitall.eid.domain.models.base.onSuccess
import com.digitall.eid.domain.models.challenge.request.SignedChallengeRequestModel
import com.digitall.eid.domain.repository.network.citizen.eid.associate.CitizenEidAssociateNetworkRepository
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.flow.flow
import kotlinx.coroutines.flow.flowOn
import org.koin.core.component.inject

class CitizenEidAssociateNetworkRepositoryImpl : CitizenEidAssociateNetworkRepository,
    BaseRepository() {

    companion object {
        private const val TAG = "CitizenEidAssociateNetworkRepositoryTag"
    }

    private val citizenEidAssociateApi: CitizenEidAssociateApi by inject()
    private val citizenEidAssociateRequestMapper: CitizenEidAssociateRequestMapper by inject()

    override fun associateEid(data: SignedChallengeRequestModel) = flow {
        logDebug("associateEid", TAG)
        emit(ResultEmittedData.loading(model = null))
        getResult {
            citizenEidAssociateApi.associateEid(
                clientId = CLIENT_ID,
                requestBody = citizenEidAssociateRequestMapper.map(data)
            )
        }.onSuccess { _, message, responseCode ->
            logDebug("associateEid onSuccess", TAG)
            emit(
                ResultEmittedData.success(
                    model = Unit,
                    message = message,
                    responseCode = responseCode,
                )
            )
        }.onFailure { error, title, message, responseCode, errorType ->
            logError(
                "associateEid onFailure",
                message,
                TAG
            )
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