/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.data.repository.network.authentication

import com.digitall.eid.data.mappers.network.authentication.request.AuthenticationChallengeRequestMapper
import com.digitall.eid.data.mappers.network.authentication.request.AuthenticationWithBasicProfileRequestMapper
import com.digitall.eid.data.mappers.network.authentication.request.AuthenticationWithCertificateRequestMapper
import com.digitall.eid.data.mappers.network.authentication.response.AuthenticationChallengeResponseMapper
import com.digitall.eid.data.mappers.network.authentication.response.AuthenticationResponseMapper
import com.digitall.eid.data.mappers.network.verify.login.request.VerifyLoginRequestMapper
import com.digitall.eid.data.mappers.network.verify.login.response.VerifyLoginResponseMapper
import com.digitall.eid.data.network.authentication.AuthenticationApi
import com.digitall.eid.data.network.verify.login.VerifyLoginApi
import com.digitall.eid.data.repository.network.base.BaseRepository
import com.digitall.eid.domain.models.authentication.request.AuthenticationCertificateRequestModel
import com.digitall.eid.domain.models.authentication.request.AuthenticationChallengeRequestModel
import com.digitall.eid.domain.models.authentication.request.BasicProfileAuthenticationRequestModel
import com.digitall.eid.domain.models.authentication.response.AuthenticationChallengeResponseModel
import com.digitall.eid.domain.models.authentication.response.AuthenticationResponseModel
import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.base.onFailure
import com.digitall.eid.domain.models.base.onSuccess
import com.digitall.eid.domain.models.verify.login.request.VerifyLoginRequestModel
import com.digitall.eid.domain.repository.network.authentication.AuthenticationNetworkRepository
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.flow.Flow
import kotlinx.coroutines.flow.flow
import kotlinx.coroutines.flow.flowOn
import org.koin.core.component.inject

class AuthenticationNetworkRepositoryImpl :
    AuthenticationNetworkRepository,
    BaseRepository() {

    companion object {
        private const val TAG = "AuthorizationNetworkRepositoryTag"
    }

    private val authenticationApi: AuthenticationApi by inject()
    private val verifyLoginApi: VerifyLoginApi by inject()
    private val authenticationResponseMapper: AuthenticationResponseMapper by inject()
    private val authenticationWithBasicProfileRequestMapper: AuthenticationWithBasicProfileRequestMapper by inject()
    private val authenticationChallengeRequestMapper: AuthenticationChallengeRequestMapper by inject()
    private val authenticationChallengeResponseMapper: AuthenticationChallengeResponseMapper by inject()
    private val authenticationWithCertificateRequestMapper: AuthenticationWithCertificateRequestMapper by inject()
    private val verifyLoginRequestMapper: VerifyLoginRequestMapper by inject()
    private val verifyLoginResponseMapper: VerifyLoginResponseMapper by inject()

    override fun authenticateWithBasicProfile(
        data: BasicProfileAuthenticationRequestModel
    ): Flow<ResultEmittedData<AuthenticationResponseModel>> = flow {
        logDebug("authenticateWithBasicProfile", TAG)
        emit(ResultEmittedData.loading(model = null))
        getResult {
            authenticationApi.authenticateWithBasicProfile(
                requestBody = authenticationWithBasicProfileRequestMapper.map(data)
            )
        }.onSuccess { model, message, responseCode ->
            logDebug("authenticateWithBasicProfile onSuccess", TAG)
            emit(
                ResultEmittedData.success(
                    model = authenticationResponseMapper.map(model),
                    message = message,
                    responseCode = responseCode,
                )
            )
        }.onFailure { error, title, message, responseCode, errorType ->
            logError("authenticateWithBasicProfile onFailure", message, TAG)
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

    override fun generateAuthenticationChallenge(data: AuthenticationChallengeRequestModel)
            : Flow<ResultEmittedData<AuthenticationChallengeResponseModel>> = flow {
        logDebug("generateAuthenticationChallenge", TAG)
        emit(ResultEmittedData.loading(model = null))
        getResult {
            authenticationApi.generateAuthenticationChallenge(
                requestBody = authenticationChallengeRequestMapper.map(data)
            )
        }.onSuccess { model, message, responseCode ->
            logDebug("generateAuthenticationChallenge onSuccess", TAG)
            emit(
                ResultEmittedData.success(
                    model = authenticationChallengeResponseMapper.map(model),
                    message = message,
                    responseCode = responseCode,
                )
            )
        }.onFailure { error, title, message, responseCode, errorType ->
            logError("generateAuthenticationChallenge onFailure", message, TAG)
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

    override fun authenticateWithCertificate(data: AuthenticationCertificateRequestModel):
            Flow<ResultEmittedData<AuthenticationResponseModel>> = flow {
        logDebug("authenticateWithCertificate", TAG)
        emit(ResultEmittedData.loading(model = null))
        getResult {
            authenticationApi.authenticateWithCertificate(
                requestBody = authenticationWithCertificateRequestMapper.map(data)
            )
        }.onSuccess { model, message, responseCode ->
            logDebug("authenticateWithCertificate onSuccess", TAG)
            emit(
                ResultEmittedData.success(
                    model = authenticationResponseMapper.map(model),
                    message = message,
                    responseCode = responseCode,
                )
            )
        }.onFailure { error, title, message, responseCode, errorType ->
            logError("authenticateWithCertificate onFailure", message, TAG)
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

    override fun verifyLogin(data: VerifyLoginRequestModel) = flow {
        logDebug("verifyLogin", TAG)
        emit(ResultEmittedData.loading(model = null))
        getResult {
            verifyLoginApi.verifyLogin(
                requestBody = verifyLoginRequestMapper.map(data)
            )
        }.onSuccess { model, message, responseCode ->
            logDebug("verifyLogin onSuccess", TAG)
            emit(
                ResultEmittedData.success(
                    model = verifyLoginResponseMapper.map(model),
                    message = message,
                    responseCode = responseCode,
                )
            )
        }.onFailure { error, title, message, responseCode, errorType ->
            logError("verifyLogin onFailure", message, TAG)
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