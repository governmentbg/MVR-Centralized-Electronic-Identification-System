package com.digitall.eid.data.repository.network.mfa

import com.digitall.eid.data.mappers.network.authentication.response.AuthenticationResponseMapper
import com.digitall.eid.data.mappers.network.mfa.request.MfaGenerateNewOtpCodeRequestMapper
import com.digitall.eid.data.mappers.network.mfa.request.MfaVerifyOtpCodeRequestMapper
import com.digitall.eid.data.network.mfa.MfaApi
import com.digitall.eid.data.repository.network.base.BaseRepository
import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.base.onFailure
import com.digitall.eid.domain.models.base.onSuccess
import com.digitall.eid.domain.models.mfa.request.GenerateNewOtpCodeRequestModel
import com.digitall.eid.domain.models.mfa.request.VerifyOtpCodeRequestModel
import com.digitall.eid.domain.repository.network.mfa.MfaNetworkRepository
import com.digitall.eid.domain.utils.LogUtil
import com.digitall.eid.domain.utils.LogUtil.logDebug
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.flow.flow
import kotlinx.coroutines.flow.flowOn
import org.koin.core.component.inject

class MfaNetworkRepositoryImpl : BaseRepository(),
    MfaNetworkRepository {

    companion object {
        private const val TAG = "MfaNetworkRepositoryTag"
    }

    private val mfaApi: MfaApi by inject()

    private val authenticationResponseMapper: AuthenticationResponseMapper by inject()
    private val mfaVerifyOtpCodeRequestMapper: MfaVerifyOtpCodeRequestMapper by inject()
    private val mfaGenerateNewOtpCodeRequestMapper: MfaGenerateNewOtpCodeRequestMapper by inject()

    override fun verifyOtpCode(data: VerifyOtpCodeRequestModel) = flow {
        logDebug("verifyOtpCode", TAG)
        emit(ResultEmittedData.loading(model = null))
        getResult {
            mfaApi.verifyOtpCode(
                requestBody = mfaVerifyOtpCodeRequestMapper.map(data),
            )
        }.onSuccess { model, message, responseCode ->
            logDebug("verifyOtpCode onSuccess", TAG)
            emit(
                ResultEmittedData.success(
                    model = authenticationResponseMapper.map(model),
                    message = message,
                    responseCode = responseCode,
                )
            )
        }.onFailure { error, title, message, responseCode, errorType ->
            LogUtil.logError(
                "verifyOtpCode onFailure",
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

    override fun generateNewOtpCode(data: GenerateNewOtpCodeRequestModel) = flow {
        logDebug("generateNewOtpCode", TAG)
        emit(ResultEmittedData.loading(model = null))
        getResult {
            mfaApi.generateNewOtpCode(
                requestBody = mfaGenerateNewOtpCodeRequestMapper.map(data),
            )
        }.onSuccess { _, message, responseCode ->
            logDebug("generateNewOtpCode onSuccess", TAG)
            emit(
                ResultEmittedData.success(
                    model = Unit,
                    message = message,
                    responseCode = responseCode,
                )
            )
        }.onFailure { error, title, message, responseCode, errorType ->
            LogUtil.logError(
                "generateNewOtpCode onFailure",
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
    }
}