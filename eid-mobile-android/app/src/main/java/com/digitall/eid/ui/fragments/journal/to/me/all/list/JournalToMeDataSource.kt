/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.journal.to.me.all.list

import androidx.paging.PagingSource
import androidx.paging.PagingState
import com.digitall.eid.R
import com.digitall.eid.domain.models.assets.localization.logs.LogLocalizationModel
import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.journal.all.JournalRequestModel
import com.digitall.eid.domain.models.journal.filter.JournalFilterModel
import com.digitall.eid.domain.usecase.journal.GetJournalToMeUseCase
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.mappers.journal.JournalToMeUiMapper
import com.digitall.eid.models.common.EmptyDataException
import com.digitall.eid.models.common.PagingError
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.journal.common.all.JournalAdapterMarker
import kotlinx.coroutines.flow.first
import kotlinx.coroutines.flow.transformWhile

class JournalToMeDataSource(
    private val filterModel: JournalFilterModel,
    private val logsLocalizations: List<LogLocalizationModel>,
    private val journalToMeUiMapper: JournalToMeUiMapper,
    private val getJournalToMeUseCase: GetJournalToMeUseCase,
) : PagingSource<List<String>, JournalAdapterMarker>() {

    companion object {
        private const val TAG = "JournalFromMeDataSourceTag"
    }

    override suspend fun load(params: LoadParams<List<String>>): LoadResult<List<String>, JournalAdapterMarker> {
        val cursorSearchAfter = params.key
        val currentLoadSize = params.loadSize

        logDebug(
            "load: page=$cursorSearchAfter, loadSize=${params.loadSize}",
            TAG
        )

        try {
            val resultFlow = getJournalToMeUseCase.invoke(
                data = JournalRequestModel(
                    startDate = filterModel.startDate,
                    endDate = filterModel.endDate,
                    eventTypes = filterModel.eventTypes.ifEmpty { null },
                    cursorSize = currentLoadSize,
                    cursorSearchAfter = cursorSearchAfter
                )
            )

            val finalResult = resultFlow
                .transformWhile { emittedResult ->
                    if (emittedResult.status == ResultEmittedData.Status.LOADING) {
                        logDebug(
                            "load: received intermediate LOADING state for page $cursorSearchAfter, continuing to collect...",
                            TAG
                        )
                        emit(emittedResult)
                        true
                    } else {
                        emit(emittedResult)
                        false
                    }
                }
                .first { it.status != ResultEmittedData.Status.LOADING }

            return when (finalResult.status) {
                ResultEmittedData.Status.SUCCESS -> {
                    finalResult.model?.let { responseModel ->
                        val logsApiModels = responseModel.data ?: emptyList()
                        val logsUi =
                            journalToMeUiMapper.mapList(logsApiModels, logsLocalizations)

                        val isLastPage =
                            (logsUi.isEmpty() && (responseModel.data?.isEmpty() ?: true))

                        val nextKey = responseModel.searchAfter

                        logDebug(
                            "load SUCCESS: dataSize=${logsUi.size}, nextKey=$nextKey, isLastPage=$isLastPage",
                            TAG
                        )
                        LoadResult.Page(
                            data = logsUi,
                            prevKey = null,
                            nextKey = nextKey
                        )
                    } ?: run {
                        logError(
                            "Data from server is empty or malformed for page $cursorSearchAfter",
                            null,
                            TAG
                        )
                        LoadResult.Error(EmptyDataException("Server returned null content or missing page info."))
                    }
                }

                ResultEmittedData.Status.ERROR -> {
                    logError(
                        "load ERROR for page $cursorSearchAfter: ${finalResult.message}",
                        TAG
                    )
                    val displayMessage = finalResult.message?.let {
                        StringSource(
                            "$it (%s)",
                            formatArgs = listOf((finalResult.responseCode ?: 0).toString())
                        )
                    } ?: StringSource(
                        R.string.error_api_general,
                        formatArgs = listOf((finalResult.responseCode ?: 0).toString())
                    )

                    LoadResult.Error(
                        PagingError(
                            title = StringSource(R.string.information),
                            displayMessage = displayMessage,
                            originalException = finalResult.error as? Throwable,
                            errorType = finalResult.errorType
                        )
                    )
                }

                ResultEmittedData.Status.LOADING -> {
                    logError(
                        "load received UNEXPECTED LOADING state for page $cursorSearchAfter",
                        null,
                        TAG
                    )
                    LoadResult.Error(IllegalStateException("Received LOADING state after requesting a single page result."))
                }
            }
        } catch (exception: Exception) {
            logError(
                "load exception for page $cursorSearchAfter", exception,
                TAG
            )
            return LoadResult.Error(exception)
        }
    }

    override fun getRefreshKey(state: PagingState<List<String>, JournalAdapterMarker>): List<String>? {
        return null
    }
}