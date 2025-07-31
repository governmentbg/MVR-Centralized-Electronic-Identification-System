/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.data.repository.network.certificates

import com.digitall.eid.data.mappers.network.certificates.request.CertificateAliasChangeRequestMapper
import com.digitall.eid.data.mappers.network.certificates.request.CertificateChangeStatusRequestMapper
import com.digitall.eid.data.mappers.network.certificates.response.CertificateChangeStatusResponseMapper
import com.digitall.eid.data.mappers.network.certificates.response.CertificateDetailsResponseMapper
import com.digitall.eid.data.mappers.network.certificates.response.CertificateHistoryResponseMapper
import com.digitall.eid.data.mappers.network.certificates.response.CertificatesResponseMapper
import com.digitall.eid.data.network.certificates.CertificatesApi
import com.digitall.eid.data.repository.network.base.BaseRepository
import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.base.onFailure
import com.digitall.eid.domain.models.base.onSuccess
import com.digitall.eid.domain.models.certificates.CertificateAliasChangeRequestModel
import com.digitall.eid.domain.models.certificates.CertificateStatusChangeRequestModel
import com.digitall.eid.domain.repository.network.certificates.CertificatesNetworkRepository
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.flow.flow
import kotlinx.coroutines.flow.flowOn
import org.koin.core.component.inject

class CertificatesNetworkRepositoryImpl :
    CertificatesNetworkRepository,
    BaseRepository() {

    companion object {
        private const val TAG = "CertificatesNetworkRepositoryTag"
    }

    private val certificatesApi: CertificatesApi by inject()
    private val certificatesResponseMapper: CertificatesResponseMapper by inject()
    private val certificateDetailsResponseMapper: CertificateDetailsResponseMapper by inject()
    private val certificateChangeStatusRequestMapper: CertificateChangeStatusRequestMapper by inject()
    private val certificateChangeStatusResponseMapper: CertificateChangeStatusResponseMapper by inject()
    private val certificateHistoryResponseMapper: CertificateHistoryResponseMapper by inject()
    private val certificateAliasChangeRequestMapper: CertificateAliasChangeRequestMapper by inject()

    override fun getCertificates(
        page: Int,
        size: Int,
        id: String?,
        sort: String?,
        alias: String?,
        status: String?,
        serialNumber: String?,
        validityFrom: String?,
        validityUntil: String?,
        deviceType: List<String>?,
        eidAdministratorId: String?,
    ) = flow {
        logDebug("getCertificates", TAG)
        emit(ResultEmittedData.loading(model = null))
        getResult {
            certificatesApi.getCertificates(
                id = id,
                page = page,
                sort = sort,
                size = size,
                alias = alias,
                status = status,
                deviceId = deviceType,
                serialNumber = serialNumber,
                validityFrom = validityFrom,
                validityUntil = validityUntil,
                eidAdministratorId = eidAdministratorId,
            )
        }.onSuccess { model, message, responseCode ->
            logDebug("getCertificates onSuccess", TAG)
            emit(
                ResultEmittedData.success(
                    model = certificatesResponseMapper.map(model),
                    message = message,
                    responseCode = responseCode,
                )
            )
        }.onFailure { error, title, message, responseCode, errorType ->
            logError("getCertificates onFailure", message, TAG)
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

    override fun getCertificateDetails(
        id: String,
    ) = flow {
        logDebug("getCertificateDetails", TAG)
        emit(ResultEmittedData.loading(model = null))
        getResult {
            certificatesApi.getCertificateDetails(
                id = id,
            )
        }.onSuccess { model, message, responseCode ->
            logDebug("getCertificateDetails onSuccess", TAG)
            emit(
                ResultEmittedData.success(
                    model = certificateDetailsResponseMapper.map(model),
                    message = message,
                    responseCode = responseCode,
                )
            )
        }.onFailure { error, title, message, responseCode, errorType ->
            logError("getCertificateDetails onFailure", message, TAG)
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

    override fun getCertificateHistory(id: String) = flow {
        logDebug("getCertificateDetails", TAG)
        emit(ResultEmittedData.loading(model = null))
        getResult {
            certificatesApi.getCertificateHistory(
                id = id,
            )
        }.onSuccess { model, message, responseCode ->
            logDebug("getCertificateDetails onSuccess", TAG)
            emit(
                ResultEmittedData.success(
                    model = certificateHistoryResponseMapper.map(model),
                    message = message,
                    responseCode = responseCode,
                )
            )
        }.onFailure { error, title, message, responseCode, errorType ->
            logError("getCertificateDetails onFailure", message, TAG)
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

    override fun certificateChangeStatus(
        changeRequestModel: CertificateStatusChangeRequestModel
    ) = flow {
        logDebug("certificateChangeStatus", TAG)
        emit(ResultEmittedData.loading(model = null))
        getResult {
            certificatesApi.changeCertificateStatus(
                requestBody = certificateChangeStatusRequestMapper.map(
                    changeRequestModel
                )
            )
        }.onSuccess { model, message, responseCode ->
            logDebug("getCertificateDetails onSuccess", TAG)
            emit(
                ResultEmittedData.success(
                    model = certificateChangeStatusResponseMapper.map(model),
                    message = message,
                    responseCode = responseCode,
                )
            )
        }.onFailure { error, title, message, responseCode, errorType ->
            logError("getCertificateDetails onFailure", message, TAG)
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

    override fun setCertificateAlias(
        id: String,
        aliasRequestModel: CertificateAliasChangeRequestModel
    ) = flow {
        logDebug("certificateChangeStatus", TAG)
        emit(ResultEmittedData.loading(model = null))
        getResult {
            certificatesApi.setCertificateAlias(
                id = id,
                requestBody = certificateAliasChangeRequestMapper.map(aliasRequestModel)
            )
        }.onSuccess { _, message, responseCode ->
            logDebug("getCertificateDetails onSuccess", TAG)
            emit(
                ResultEmittedData.success(
                    model = Unit,
                    message = message,
                    responseCode = responseCode,
                )
            )
        }.onFailure { error, title, message, responseCode, errorType ->
            logError("getCertificateDetails onFailure", message, TAG)
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