/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.data.repository.network.applications

import com.digitall.eid.data.mappers.network.applications.create.ApplicationConfirmWithBaseProfileRequestMapper
import com.digitall.eid.data.mappers.network.applications.create.ApplicationConfirmWithEIDRequestMapper
import com.digitall.eid.data.mappers.network.applications.create.ApplicationEnrollWithBaseProfileRequestMapper
import com.digitall.eid.data.mappers.network.applications.create.ApplicationEnrollWithBaseProfileResponseMapper
import com.digitall.eid.data.mappers.network.applications.create.ApplicationEnrollWithEIDRequestMapper
import com.digitall.eid.data.mappers.network.applications.create.ApplicationEnrollWithEIDResponseMapper
import com.digitall.eid.data.mappers.network.applications.create.ApplicationGenerateUserDetailsXMLResponseMapper
import com.digitall.eid.data.mappers.network.applications.create.ApplicationSendSignatureRequestMapper
import com.digitall.eid.data.mappers.network.applications.create.ApplicationSendSignatureResponseMapper
import com.digitall.eid.data.mappers.network.applications.create.ApplicationSignWithBaseProfileRequestMapper
import com.digitall.eid.data.mappers.network.applications.create.ApplicationUpdateProfileRequestMapper
import com.digitall.eid.data.mappers.network.applications.create.ApplicationUpdateProfileResponseMapper
import com.digitall.eid.data.mappers.network.applications.create.ApplicationUserDetailsResponseMapper
import com.digitall.eid.data.mappers.network.applications.create.ApplicationUserDetailsXMLRequestMapper
import com.digitall.eid.data.network.applications.ApplicationCreateApi
import com.digitall.eid.data.repository.network.base.BaseRepository
import com.digitall.eid.domain.models.applications.create.ApplicationConfirmWithBaseProfileRequestModel
import com.digitall.eid.domain.models.applications.create.ApplicationConfirmWithEIDRequestModel
import com.digitall.eid.domain.models.applications.create.ApplicationEnrollWithBaseProfileRequestModel
import com.digitall.eid.domain.models.applications.create.ApplicationEnrollWithEIDRequestModel
import com.digitall.eid.domain.models.applications.create.ApplicationSendSignatureRequestModel
import com.digitall.eid.domain.models.applications.create.ApplicationSignWithBaseProfileRequestModel
import com.digitall.eid.domain.models.applications.create.ApplicationUpdateProfileRequestModel
import com.digitall.eid.domain.models.applications.create.ApplicationDetailsXMLRequestModel
import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.base.onFailure
import com.digitall.eid.domain.models.base.onSuccess
import com.digitall.eid.domain.repository.network.applications.ApplicationCreateNetworkRepository
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.flow.flow
import kotlinx.coroutines.flow.flowOn
import org.koin.core.component.inject

class ApplicationCreateNetworkRepositoryImpl :
    ApplicationCreateNetworkRepository,
    BaseRepository() {

    companion object {
        private const val TAG = "ApplicationCreateNetworkRepositoryTag"
    }

    private val applicationCreateApi: ApplicationCreateApi by inject()
    private val applicationUserDetailsResponseMapper: ApplicationUserDetailsResponseMapper by inject()
    private val applicationUpdateProfileRequestMapper: ApplicationUpdateProfileRequestMapper by inject()
    private val applicationUpdateProfileResponseMapper: ApplicationUpdateProfileResponseMapper by inject()
    private val applicationEnrollWithEIDRequestMapper: ApplicationEnrollWithEIDRequestMapper by inject()
    private val applicationSendSignatureRequestMapper: ApplicationSendSignatureRequestMapper by inject()
    private val applicationConfirmWithEIDRequestMapper: ApplicationConfirmWithEIDRequestMapper by inject()
    private val applicationEnrollWithEIDResponseMapper: ApplicationEnrollWithEIDResponseMapper by inject()
    private val applicationSendSignatureResponseMapper: ApplicationSendSignatureResponseMapper by inject()
    private val applicationUserDetailsXMLRequestMapper: ApplicationUserDetailsXMLRequestMapper by inject()
    private val applicationSignWithBaseProfileRequestMapper: ApplicationSignWithBaseProfileRequestMapper by inject()
    private val applicationEnrollWithBaseProfileRequestMapper: ApplicationEnrollWithBaseProfileRequestMapper by inject()
    private val applicationConfirmWithBaseProfileRequestMapper: ApplicationConfirmWithBaseProfileRequestMapper by inject()
    private val applicationEnrollWithBaseProfileResponseMapper: ApplicationEnrollWithBaseProfileResponseMapper by inject()
    private val applicationGenerateUserDetailsXMLResponseMapper: ApplicationGenerateUserDetailsXMLResponseMapper by inject()

    override fun getUserDetails() = flow {
        logDebug("getUserDetails", TAG)
        emit(ResultEmittedData.loading(model = null))
        getResult {
            applicationCreateApi.getUserDetails()
        }.onSuccess { model, message, responseCode ->
            logDebug("getUserDetails onSuccess", TAG)
            emit(
                ResultEmittedData.success(
                    message = message,
                    responseCode = responseCode,
                    model = applicationUserDetailsResponseMapper.map(model),
                )
            )
        }.onFailure { error, title, message, responseCode, errorType ->
            logError("getUserDetails onFailure", message, TAG)
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

    override fun generateUserDetailsXML(
        data: ApplicationDetailsXMLRequestModel,
    ) = flow {
        logDebug("generateUserDetailsXML", TAG)
        emit(ResultEmittedData.loading(model = null))
        val request = applicationUserDetailsXMLRequestMapper.map(data)
        getResult {
            applicationCreateApi.generateUserDetailsXML(
                request = request,
            )
        }.onSuccess { model, message, responseCode ->
            logDebug("generateUserDetailsXML onSuccess", TAG)
            emit(
                ResultEmittedData.success(
                    message = message,
                    responseCode = responseCode,
                    model = applicationGenerateUserDetailsXMLResponseMapper.map(model),
                )
            )
        }.onFailure { error, title, message, responseCode, errorType ->
            logError("generateUserDetailsXML onFailure", message, TAG)
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

    override fun sendSignature(
        data: ApplicationSendSignatureRequestModel,
    ) = flow {
        logDebug("sendSignature", TAG)
        emit(ResultEmittedData.loading(model = null))
        getResult {
            applicationCreateApi.sendSignature(
                request = applicationSendSignatureRequestMapper.map(data),
            )
        }.onSuccess { model, message, responseCode ->
            logDebug("sendSignature onSuccess", TAG)
            emit(
                ResultEmittedData.success(
                    message = message,
                    responseCode = responseCode,
                    model = applicationSendSignatureResponseMapper.map(model),
                )
            )
        }.onFailure { error, title, message, responseCode, errorType ->
            logError("sendSignature onFailure", message, TAG)
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

    override fun signWithBaseProfile(
        data: ApplicationSignWithBaseProfileRequestModel
    ) = flow {
        logDebug("signWithBaseProfile", TAG)
        emit(ResultEmittedData.loading(model = null))
        getResult {
            val request = applicationSignWithBaseProfileRequestMapper.map(data)
            applicationCreateApi.signWithBaseProfile(
                request = request,
            )
        }.onSuccess { model, message, responseCode ->
            logDebug("signWithBaseProfile onSuccess", TAG)
            emit(
                ResultEmittedData.success(
                    model = model,
                    message = message,
                    responseCode = responseCode,
                )
            )
        }.onFailure { error, title, message, responseCode, errorType ->
            logError("signWithBaseProfile onFailure", message, TAG)
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

    override fun enrollWithBaseProfile(
        data: ApplicationEnrollWithBaseProfileRequestModel
    ) = flow {
        logDebug("enrollWithBaseProfile", TAG)
        emit(ResultEmittedData.loading(model = null))
        getResult {
            val request = applicationEnrollWithBaseProfileRequestMapper.map(data)
            applicationCreateApi.enrollWithBaseProfile(
                request = request,
            )
        }.onSuccess { model, message, responseCode ->
            logDebug("enrollWithBaseProfile onSuccess", TAG)
            emit(
                ResultEmittedData.success(
                    message = message,
                    responseCode = responseCode,
                    model = applicationEnrollWithBaseProfileResponseMapper.map(model),
                )
            )
        }.onFailure { error, title, message, responseCode, errorType ->
            logError("enrollWithBaseProfile onFailure", message, TAG)
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

    override fun confirmWithBaseProfile(
        data: ApplicationConfirmWithBaseProfileRequestModel
    ) = flow {
        logDebug("confirmWithBaseProfile", TAG)
        emit(ResultEmittedData.loading(model = null))
        getResult {
            val request = applicationConfirmWithBaseProfileRequestMapper.map(data)
            applicationCreateApi.confirmWithBaseProfile(
                request = request,
            )
        }.onSuccess { model, message, responseCode ->
            logDebug("confirmWithBaseProfile onSuccess", TAG)
            emit(
                ResultEmittedData.success(
                    model = model,
                    message = message,
                    responseCode = responseCode,
                )
            )
        }.onFailure { error, title, message, responseCode, errorType ->
            logError("confirmWithBaseProfile onFailure", message, TAG)
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

    override fun enrollWithEID(
        data: ApplicationEnrollWithEIDRequestModel,
    ) = flow {
        logDebug("enrollWithEID", TAG)
        emit(ResultEmittedData.loading(model = null))
        val request = applicationEnrollWithEIDRequestMapper.map(data)
        getResult {
            applicationCreateApi.enrollWithEID(
                request = request,
            )
        }.onSuccess { model, message, responseCode ->
            logDebug("enrollWithEID onSuccess", TAG)
            emit(
                ResultEmittedData.success(
                    message = message,
                    responseCode = responseCode,
                    model = applicationEnrollWithEIDResponseMapper.map(model),
                )
            )
        }.onFailure { error, title, message, responseCode, errorType ->
            logError("enrollWithEID onFailure", message, TAG)
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

    override fun confirmWithEID(
        data: ApplicationConfirmWithEIDRequestModel,
    ) = flow {
        logDebug("confirmWithEID", TAG)
        emit(ResultEmittedData.loading(model = null))
        val request = applicationConfirmWithEIDRequestMapper.map(data)
        getResult {
            applicationCreateApi.confirmWithEID(
                request = request,
            )
        }.onSuccess { model, message, responseCode ->
            logDebug("confirmWithEID onSuccess", TAG)
            emit(
                ResultEmittedData.success(
                    model = model,
                    message = message,
                    responseCode = responseCode,
                )
            )
        }.onFailure { error, title, message, responseCode, errorType ->
            logError("confirmWithEID onFailure", message, TAG)
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

    override fun updateProfile(
        data: ApplicationUpdateProfileRequestModel,
    ) = flow {
        logDebug("updateProfile", TAG)
        emit(ResultEmittedData.loading(model = null))
        val request = applicationUpdateProfileRequestMapper.map(data)
        getResult {
            applicationCreateApi.updateProfile(
                request = request,
            )
        }.onSuccess { model, message, responseCode ->
            logDebug("updateProfile onSuccess", TAG)
            emit(
                ResultEmittedData.success(
                    message = message,
                    responseCode = responseCode,
                    model = applicationUpdateProfileResponseMapper.map(model),
                )
            )
        }.onFailure { error, title, message, responseCode, errorType ->
            logError("updateProfile onFailure", message, TAG)
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

    override fun sendCertificateApplication(data: ApplicationSendSignatureRequestModel) = flow {
        logDebug("sendCertificateApplication", TAG)
        emit(ResultEmittedData.loading(model = null))
        getResult {
            applicationCreateApi.sendCertificateApplication(
                request = applicationSendSignatureRequestMapper.map(data),
            )
        }.onSuccess { model, message, responseCode ->
            logDebug("sendCertificateApplication onSuccess", TAG)
            emit(
                ResultEmittedData.success(
                    message = message,
                    responseCode = responseCode,
                    model = applicationSendSignatureResponseMapper.map(model),
                )
            )
        }.onFailure { error, title, message, responseCode, errorType ->
            logError("sendCertificateApplication onFailure", message, TAG)
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