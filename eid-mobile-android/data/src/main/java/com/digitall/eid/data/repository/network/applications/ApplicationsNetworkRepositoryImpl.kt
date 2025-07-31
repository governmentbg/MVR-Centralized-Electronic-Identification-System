/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.data.repository.network.applications

import com.digitall.eid.data.mappers.network.applications.all.ApplicationCompletionStatusResponseMapper
import com.digitall.eid.data.mappers.network.applications.all.ApplicationDetailsResponseMapper
import com.digitall.eid.data.mappers.network.applications.all.ApplicationsResponseMapper
import com.digitall.eid.data.network.applications.ApplicationsApi
import com.digitall.eid.data.repository.network.base.BaseRepository
import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.base.onFailure
import com.digitall.eid.domain.models.base.onSuccess
import com.digitall.eid.domain.repository.network.applications.ApplicationsNetworkRepository
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.flow.flow
import kotlinx.coroutines.flow.flowOn
import org.koin.core.component.inject

class ApplicationsNetworkRepositoryImpl :
    ApplicationsNetworkRepository,
    BaseRepository() {

    companion object {
        private const val TAG = "ApplicationsNetworkRepositoryTag"
    }

    private val applicationsApi: ApplicationsApi by inject()
    private val applicationsResponseMapper: ApplicationsResponseMapper by inject()
    private val applicationDetailsResponseMapper: ApplicationDetailsResponseMapper by inject()
    private val applicationCompletionStatusResponseMapper: ApplicationCompletionStatusResponseMapper by inject()

    override fun getApplications(
        page: Int,
        size: Int,
        id: String?,
        sort: String?,
        applicationNumber: String?,
        status: String?,
        createDate: String?,
        deviceIds: List<String>?,
        eidAdministratorId: String?,
        applicationType: List<String>?,
    ) = flow {
        logDebug("getApplications", TAG)
        emit(ResultEmittedData.loading(model = null))
        getResult {
            applicationsApi.getApplications(
                id = id,
                page = page,
                sort = sort,
                size = size,
                status = status,
                createDate = createDate,
                deviceId = deviceIds,
                applicationType = applicationType,
                eidAdministratorId = eidAdministratorId,
                applicationNumber = applicationNumber,
            )
        }.onSuccess { model, message, responseCode ->
            logDebug("getApplications onSuccess", TAG)
            emit(
                ResultEmittedData.success(
                    model = applicationsResponseMapper.map(model),
                    message = message,
                    responseCode = responseCode,
                )
            )
        }.onFailure { error, title, message, responseCode, errorType ->
            logError("getApplications onFailure", message, TAG)
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

    override fun getApplicationDetails(id: String) = flow {
        logDebug("getApplicationDetails", TAG)
        emit(ResultEmittedData.loading(model = null))
        getResult {
            applicationsApi.getApplicationDetails(
                id = id,
            )
        }.onSuccess { model, message, responseCode ->
            logDebug("getApplicationDetails onSuccess", TAG)
            emit(
                ResultEmittedData.success(
                    model = applicationDetailsResponseMapper.map(model),
                    message = message,
                    responseCode = responseCode,
                )
            )
        }.onFailure { error, title, message, responseCode, errorType ->
            logError("getApplicationDetails onFailure", message, TAG)
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

    override fun completeApplication(id: String)= flow {
        logDebug("completeApplication", TAG)
        emit(ResultEmittedData.loading(model = null))
        getResult {
            applicationsApi.completeApplication(id = id)
        }.onSuccess { model, message, responseCode ->
            logDebug("completeApplication onSuccess", TAG)
            emit(
                ResultEmittedData.success(
                    model = applicationCompletionStatusResponseMapper.map(model),
                    message = message,
                    responseCode = responseCode,
                )
            )
        }.onFailure { error, title, message, responseCode, errorType ->
            logError("completeApplication onFailure", message, TAG)
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