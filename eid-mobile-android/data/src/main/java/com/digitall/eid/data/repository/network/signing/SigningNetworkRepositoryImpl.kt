/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.data.repository.network.signing

import com.digitall.eid.data.mappers.network.signing.SigningCheckUserStatusRequestMapper
import com.digitall.eid.data.mappers.network.signing.borica.SigningBoricaDownloadResponseMapper
import com.digitall.eid.data.mappers.network.signing.borica.SigningBoricaSignRequestMapper
import com.digitall.eid.data.mappers.network.signing.borica.SigningBoricaSignResponseMapper
import com.digitall.eid.data.mappers.network.signing.borica.SigningBoricaStatusResponseMapper
import com.digitall.eid.data.mappers.network.signing.borica.SigningBoricaUserStatusResponseMapper
import com.digitall.eid.data.mappers.network.signing.evrotrust.SigningEvrotrustDownloadResponseMapper
import com.digitall.eid.data.mappers.network.signing.evrotrust.SigningEvrotrustSignRequestMapper
import com.digitall.eid.data.mappers.network.signing.evrotrust.SigningEvrotrustSignResponseMapper
import com.digitall.eid.data.mappers.network.signing.evrotrust.SigningEvrotrustStatusResponseMapper
import com.digitall.eid.data.mappers.network.signing.evrotrust.SigningEvrotrustUserStatusResponseMapper
import com.digitall.eid.data.network.signing.SigningApi
import com.digitall.eid.data.repository.network.base.BaseRepository
import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.base.onFailure
import com.digitall.eid.domain.models.base.onSuccess
import com.digitall.eid.domain.models.signing.SigningCheckUserStatusRequestModel
import com.digitall.eid.domain.models.signing.borica.SigningBoricaSignRequestModel
import com.digitall.eid.domain.models.signing.evrotrust.SigningEvrotrustSignRequestModel
import com.digitall.eid.domain.repository.network.signing.SigningNetworkRepository
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.flow.flow
import kotlinx.coroutines.flow.flowOn
import org.koin.core.component.inject

class SigningNetworkRepositoryImpl : SigningNetworkRepository,
    BaseRepository() {

    companion object {
        private const val TAG = "SigningNetworkRepositoryImplTag"
    }

    private val signingApi: SigningApi by inject()
    private val signingBoricaSignRequestMapper: SigningBoricaSignRequestMapper by inject()
    private val signingBoricaSignResponseMapper: SigningBoricaSignResponseMapper by inject()
    private val signingCheckUserStatusRequestMapper: SigningCheckUserStatusRequestMapper by inject()
    private val signingEvrotrustSignRequestMapper: SigningEvrotrustSignRequestMapper by inject()
    private val signingBoricaStatusResponseMapper: SigningBoricaStatusResponseMapper by inject()
    private val signingEvrotrustSignResponseMapper: SigningEvrotrustSignResponseMapper by inject()
    private val signingBoricaDownloadResponseMapper: SigningBoricaDownloadResponseMapper by inject()
    private val signingEvrotrustStatusResponseMapper: SigningEvrotrustStatusResponseMapper by inject()
    private val signingBoricaUserStatusResponseMapper: SigningBoricaUserStatusResponseMapper by inject()
    private val signingEvrotrustDownloadResponseMapper: SigningEvrotrustDownloadResponseMapper by inject()
    private val signingEvrotrustUserStatusResponseMapper: SigningEvrotrustUserStatusResponseMapper by inject()

    override fun getBoricaStatus(transactionId: String) = flow {
        logDebug("getBoricaStatus", TAG)
        emit(ResultEmittedData.loading(model = null))
        getResult {
            signingApi.getBoricaStatus(
                transactionId = transactionId,
            )
        }.onSuccess { model, message, responseCode ->
            logDebug("getBoricaStatus onSuccess", TAG)
            emit(
                ResultEmittedData.success(
                    model = signingBoricaStatusResponseMapper.map(model),
                    message = message,
                    responseCode = responseCode,
                )
            )
        }.onFailure { error, title, message, responseCode, errorType ->
            logError("getBoricaStatus onFailure", message, TAG)
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

    override fun getBoricaDownload(transactionId: String) = flow {
        logDebug("getBoricaDownload", TAG)
        emit(ResultEmittedData.loading(model = null))
        getResult {
            signingApi.getBoricaDownload(
                transactionId = transactionId,
            )
        }.onSuccess { model, message, responseCode ->
            logDebug("getBoricaDownload onSuccess", TAG)
            emit(
                ResultEmittedData.success(
                    model = signingBoricaDownloadResponseMapper.map(model),
                    message = message,
                    responseCode = responseCode,
                )
            )
        }.onFailure { error, title, message, responseCode, errorType ->
            logError("getBoricaDownload onFailure", message, TAG)
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

    override fun signWithBorica(
        request: SigningBoricaSignRequestModel
    ) = flow {
        logDebug("signWithBorica", TAG)
        emit(ResultEmittedData.loading(model = null))
        getResult {
            signingApi.signWithBorica(
                request = signingBoricaSignRequestMapper.map(request),
            )
        }.onSuccess { model, message, responseCode ->
            logDebug("signWithBorica onSuccess", TAG)
            emit(
                ResultEmittedData.success(
                    model = signingBoricaSignResponseMapper.map(model),
                    message = message,
                    responseCode = responseCode,
                )
            )
        }.onFailure { error, title, message, responseCode, errorType ->
            logError("signWithBorica onFailure", message, TAG)
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

    override fun checkBoricaUserStatus(
        data: SigningCheckUserStatusRequestModel
    ) = flow {
        logDebug("checkBoricaUserStatus", TAG)
        emit(ResultEmittedData.loading(model = null))
        getResult {
            signingApi.checkBoricaUserStatus(
                request = signingCheckUserStatusRequestMapper.map(data)
            )
        }.onSuccess { model, message, responseCode ->
            logDebug("checkBoricaUserStatus onSuccess", TAG)
            emit(
                ResultEmittedData.success(
                    model = signingBoricaUserStatusResponseMapper.map(model),
                    message = message,
                    responseCode = responseCode,
                )
            )
        }.onFailure { error, title, message, responseCode, errorType ->
            logError("checkBoricaUserStatus onFailure", message, TAG)
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

    override fun getEvrotrustStatus(
        transactionId: String
    ) = flow {
        logDebug("getEvrotrustStatus", TAG)
        emit(ResultEmittedData.loading(model = null))
        getResult {
            signingApi.getEvrotrustStatus(
                transactionId = transactionId,
            )
        }.onSuccess { model, message, responseCode ->
            logDebug("getEvrotrustStatus onSuccess", TAG)
            emit(
                ResultEmittedData.success(
                    model = signingEvrotrustStatusResponseMapper.map(model),
                    message = message,
                    responseCode = responseCode,
                )
            )
        }.onFailure { error, title, message, responseCode, errorType ->
            logError("getEvrotrustStatus onFailure", message, TAG)
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

    override fun getEvrotrustDownload(
        transactionId: String
    ) = flow {
        logDebug("getEvrotrustDownload", TAG)
        emit(ResultEmittedData.loading(model = null))
        getResult {
            signingApi.getEvrotrustDownload(
                transactionId = transactionId,
            )
        }.onSuccess { model, message, responseCode ->
            logDebug("getEvrotrustDownload onSuccess", TAG)
            emit(
                ResultEmittedData.success(
                    model = signingEvrotrustDownloadResponseMapper.mapList(model),
                    message = message,
                    responseCode = responseCode,
                )
            )
        }.onFailure { error, title, message, responseCode, errorType ->
            logError("getEvrotrustDownload onFailure", message, TAG)
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

    override fun signWithEvrotrust(
        request: SigningEvrotrustSignRequestModel
    ) = flow {
        logDebug("signWithEvrotrust", TAG)
        emit(ResultEmittedData.loading(model = null))
        getResult {
            signingApi.signWithEvrotrust(
                request = signingEvrotrustSignRequestMapper.map(request),
            )
        }.onSuccess { model, message, responseCode ->
            logDebug("signWithEvrotrust onSuccess", TAG)
            emit(
                ResultEmittedData.success(
                    model = signingEvrotrustSignResponseMapper.map(model),
                    message = message,
                    responseCode = responseCode,
                )
            )
        }.onFailure { error, title, message, responseCode, errorType ->
            logError("signWithEvrotrust onFailure", message, TAG)
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

    override fun checkEvrotrustUserStatus(
        data: SigningCheckUserStatusRequestModel
    ) = flow {
        logDebug("checkEvrotrustUserStatus", TAG)
        emit(ResultEmittedData.loading(model = null))
        getResult {
            signingApi.checkEvrotrustUserStatus(
                request = signingCheckUserStatusRequestMapper.map(data)
            )
        }.onSuccess { model, message, responseCode ->
            logDebug("checkEvrotrustUserStatus onSuccess", TAG)
            emit(
                ResultEmittedData.success(
                    model = signingEvrotrustUserStatusResponseMapper.map(model),
                    message = message,
                    responseCode = responseCode,
                )
            )
        }.onFailure { error, title, message, responseCode, errorType ->
            logError("checkEvrotrustUserStatus onFailure", message, TAG)
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