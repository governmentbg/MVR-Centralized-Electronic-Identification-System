/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.usecase.authentication

import com.digitall.eid.domain.models.authentication.AuthenticationType
import com.digitall.eid.domain.models.authentication.request.BasicProfileAuthenticationRequestModel
import com.digitall.eid.domain.models.authentication.response.MFAResponseModel
import com.digitall.eid.domain.models.authentication.response.TokenResponseModel
import com.digitall.eid.domain.models.base.ErrorType
import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.base.onFailure
import com.digitall.eid.domain.models.base.onLoading
import com.digitall.eid.domain.models.base.onSuccess
import com.digitall.eid.domain.models.common.ApplicationInfo
import com.digitall.eid.domain.models.common.ApplicationLanguage
import com.digitall.eid.domain.models.common.ApplicationStatus
import com.digitall.eid.domain.models.common.ApplicationThemeType
import com.digitall.eid.domain.models.common.BiometricStatus
import com.digitall.eid.domain.models.common.ErrorStatus
import com.digitall.eid.domain.repository.common.PreferencesRepository
import com.digitall.eid.domain.repository.network.authentication.AuthenticationNetworkRepository
import com.digitall.eid.domain.usecase.base.BaseUseCase
import com.digitall.eid.domain.utils.JWTDecoder
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import com.google.firebase.installations.FirebaseInstallations
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.flow.Flow
import kotlinx.coroutines.flow.FlowCollector
import kotlinx.coroutines.flow.collect
import kotlinx.coroutines.flow.flow
import kotlinx.coroutines.flow.flowOn
import kotlinx.coroutines.flow.onEach
import kotlinx.coroutines.tasks.await
import org.koin.core.component.inject

/** imports for flow:
 * import kotlinx.coroutines.flow.Flow
 * import kotlinx.coroutines.flow.collect
 * import kotlinx.coroutines.flow.flow
 * import kotlinx.coroutines.flow.onEach
 **/

class AuthenticationWithBasicProfileUseCase : BaseUseCase {

    companion object {
        private const val TAG = "AuthorizationEnterToAccountUseCaseTag"
    }

    private val jWTDecoder: JWTDecoder by inject()
    private val preferences: PreferencesRepository by inject()
    private val authenticationNetworkRepository: AuthenticationNetworkRepository by inject()

    fun invoke(
        email: String?,
        password: String?,
    ): Flow<ResultEmittedData<AuthenticationType>> = flow {
        logDebug("authenticateWithBasicProfile email: $email password: $password", TAG)
        authenticateWithBasicProfile(
            email = email,
            password = password,
            flow = this@flow,
        )
    }.flowOn(Dispatchers.IO)

    private suspend fun authenticateWithBasicProfile(
        email: String?,
        password: String?,
        flow: FlowCollector<ResultEmittedData<AuthenticationType>>,
    ) {
        authenticationNetworkRepository.authenticateWithBasicProfile(
            data = BasicProfileAuthenticationRequestModel(email = email, password = password),
        ).onEach { result ->
            result.onLoading {
                logDebug("authenticateWithBasicProfile onLoading", TAG)
                flow.emit(ResultEmittedData.loading(model = null))
            }.onSuccess { model, message, responseCode ->
                logDebug("authenticateWithBasicProfile onSuccess", TAG)
                when (model.data) {
                    is TokenResponseModel -> {
                        if (model.data.accessToken.isNullOrEmpty() || model.data.refreshToken.isNullOrEmpty()) {
                            logError(
                                "authenticateWithBasicProfile some token is null or empty",
                                TAG
                            )
                            flow.emit(
                                ResultEmittedData.error(
                                    model = null,
                                    error = null,
                                    title = "Server error",
                                    responseCode = responseCode,
                                    message = "Data from server is empty",
                                    errorType = ErrorType.SERVER_DATA_ERROR,
                                )
                            )
                            return@onEach
                        }
                        val user = jWTDecoder.getUser(model.data.accessToken)
                        if (user == null) {
                            logError("authenticateWithBasicProfile user == null", TAG)
                            flow.emit(
                                ResultEmittedData.error(
                                    model = null,
                                    error = null,
                                    title = "Server error",
                                    responseCode = responseCode,
                                    message = "Data from server is empty",
                                    errorType = ErrorType.SERVER_DATA_ERROR,
                                )
                            )
                            return@onEach
                        }
                        logDebug("user: $user", TAG)
                        val applicationInstanceId = FirebaseInstallations.getInstance().id.await()

                        preferences.saveApplicationInfo(
                            preferences.readApplicationInfo()?.copy(
                                userModel = user,
                                accessToken = model.data.accessToken,
                                refreshToken = model.data.refreshToken,
                                mobileApplicationInstanceId = applicationInstanceId
                            ) ?: ApplicationInfo(
                                email = email ?: "",
                                userModel = user,
                                password = password ?: "",
                                accessToken = model.data.accessToken,
                                refreshToken = model.data.refreshToken,
                                applicationLanguage = ApplicationLanguage.BG,
                                applicationStatus = ApplicationStatus.REGISTERED,
                                applicationThemeType = ApplicationThemeType.FOLLOW_SYSTEM,
                                biometricStatus = BiometricStatus.UNSPECIFIED,
                                databaseKey = "",
                                applicationPin = "",
                                certificatePin = "",
                                errorCount = 0,
                                errorStatus = ErrorStatus.NO_TIMEOUT,
                                errorTimeCode = 0,
                                serverPublicKey = 0,
                                mobileApplicationInstanceId = applicationInstanceId
                            )
                        )

                        flow.emit(
                            ResultEmittedData.success(
                                model = AuthenticationType.Token,
                                message = message,
                                responseCode = responseCode,
                            )
                        )
                    }

                    is MFAResponseModel -> flow.emit(
                        ResultEmittedData.success(
                            model = AuthenticationType.Mfa(
                                sessionId = model.data.sessionId,
                                ttl = model.data.ttl
                            ),
                            message = message,
                            responseCode = responseCode,
                        )
                    )

                }
            }.onFailure { error, title, message, responseCode, errorType ->
                logError("authenticateWithBasicProfile onFailure", message, TAG)
                flow.emit(
                    ResultEmittedData.error(
                        model = null,
                        error = error,
                        title = title,
                        message = message,
                        errorType = errorType,
                        responseCode = responseCode
                    )
                )
            }
        }.collect()
    }

}