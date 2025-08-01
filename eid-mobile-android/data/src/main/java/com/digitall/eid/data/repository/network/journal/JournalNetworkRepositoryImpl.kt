/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.data.repository.network.journal

import com.digitall.eid.data.mappers.network.journal.JournalRequestMapper
import com.digitall.eid.data.mappers.network.journal.JournalResponseMapper
import com.digitall.eid.data.network.journal.JournalApi
import com.digitall.eid.data.repository.network.base.BaseRepository
import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.base.onFailure
import com.digitall.eid.domain.models.base.onSuccess
import com.digitall.eid.domain.models.journal.all.JournalRequestModel
import com.digitall.eid.domain.repository.network.journal.JournalNetworkRepository
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.flow.flow
import kotlinx.coroutines.flow.flowOn
import org.koin.core.component.inject

class JournalNetworkRepositoryImpl :
    JournalNetworkRepository,
    BaseRepository() {

    companion object {
        private const val TAG = "JournalNetworkRepositoryTag"
    }

    private val journalApi: JournalApi by inject()
    private val journalResponseMapper: JournalResponseMapper by inject()
    private val journalRequestMapper: JournalRequestMapper by inject()

    override fun getJournalFromMe(
        data: JournalRequestModel,
    ) = flow {
        logDebug("getJournalFromMe", TAG)
        emit(ResultEmittedData.loading(model = null))
        getResult {
            journalApi.getJournalFromMe(
                request = journalRequestMapper.map(data),
            )
        }.onSuccess { model, message, responseCode ->
            logDebug("getJournalFromMe onSuccess", TAG)
            emit(
                ResultEmittedData.success(
                    model = journalResponseMapper.map(model),
                    message = message,
                    responseCode = responseCode,
                )
            )
        }.onFailure { error, title, message, responseCode, errorType ->
            logError("getJournalFromMe onFailure", message, TAG)
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

    override fun getJournalToMe(
        data: JournalRequestModel,
    ) = flow {
        logDebug("getJournalToMe", TAG)
        emit(ResultEmittedData.loading(model = null))
        getResult {
            journalApi.getJournalToMe(
                request = journalRequestMapper.map(data),
            )
        }.onSuccess { model, message, responseCode ->
            logDebug("getJournalToMe onSuccess", TAG)
            emit(
                ResultEmittedData.success(
                    model = journalResponseMapper.map(model),
                    message = message,
                    responseCode = responseCode,
                )
            )
        }.onFailure { error, title, message, responseCode, errorType ->
            logError("getJournalToMe onFailure", message, TAG)
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