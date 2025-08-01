/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.applications.show.all.list

import androidx.paging.PagingSource
import androidx.paging.PagingState
import com.digitall.eid.R
import com.digitall.eid.domain.ToServerDateFormats
import com.digitall.eid.domain.extensions.toServerDate
import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.common.ApplicationLanguage
import com.digitall.eid.domain.usecase.applications.all.GetApplicationsUseCase
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.mappers.applications.show.all.ApplicationsUiMapper
import com.digitall.eid.models.applications.all.ApplicationUi
import com.digitall.eid.models.applications.filter.ApplicationsFilterModel
import com.digitall.eid.models.common.EmptyDataException
import com.digitall.eid.models.common.PagingError
import com.digitall.eid.models.common.StringSource
import kotlinx.coroutines.flow.first
import kotlinx.coroutines.flow.transformWhile

class ApplicationsDataSource(
    private val sort: String?,
    private val filterModel: ApplicationsFilterModel,
    private val applicationsUiMapper: ApplicationsUiMapper,
    private val getApplicationsUseCase: GetApplicationsUseCase,
    private val language: ApplicationLanguage,
) : PagingSource<Int, ApplicationUi>() {

    companion object {
        private const val TAG = "ApplicationsDataSourceTag"
        private const val STARTING_PAGE_INDEX = 0
    }

    override suspend fun load(params: LoadParams<Int>): LoadResult<Int, ApplicationUi> {
        val currentPage = params.key ?: STARTING_PAGE_INDEX
        logDebug("load: page=$currentPage, loadSize=${params.loadSize}", TAG)

        try {
            val resultFlow = getApplicationsUseCase.invoke(
                size = params.loadSize,
                sort = sort,
                page = currentPage,
                id = filterModel.id,
                status = filterModel.status?.serverValue,
                createDate = filterModel.createDate?.toServerDate(
                    dateFormat = ToServerDateFormats.ONLY_DATE,
                ),
                eidAdministratorId = filterModel.administrator?.id,
                deviceIds = filterModel.deviceType?.serverValue?.let { listOf(it) },
                applicationType = filterModel.applicationType?.serverValue?.let { listOf(it) },
                applicationNumber = filterModel.applicationNumber,
            )

            val finalResult = resultFlow
                .transformWhile { emittedResult ->
                    if (emittedResult.status == ResultEmittedData.Status.LOADING) {
                        logDebug(
                            "load: received intermediate LOADING state for page $currentPage, continuing to collect...",
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
                        val applicationsApiModels = responseModel.content ?: emptyList()
                        val applicationsUi =
                            applicationsUiMapper.mapList(applicationsApiModels, language)

                        val isLastPage =
                            if (responseModel.totalPages != null && responseModel.number != null) {
                                (responseModel.number ?: 0) >= (responseModel.totalPages ?: 0) - 1
                            } else {
                                responseModel.last
                                    ?: (applicationsUi.isEmpty() && (responseModel.content?.isEmpty()
                                        ?: true))
                            }

                        val prevKey =
                            if (currentPage == STARTING_PAGE_INDEX) null else currentPage - 1
                        val nextKey = if (isLastPage) null else currentPage + 1

                        logDebug(
                            "load SUCCESS: dataSize=${applicationsUi.size}, prevKey=$prevKey, nextKey=$nextKey, isLastPage=$isLastPage",
                            TAG
                        )
                        LoadResult.Page(
                            data = applicationsUi,
                            prevKey = prevKey,
                            nextKey = nextKey
                        )
                    } ?: run {
                        logError(
                            "Data from server is empty or malformed for page $currentPage",
                            null,
                            TAG
                        )
                        LoadResult.Error(EmptyDataException("Server returned null content or missing page info."))
                    }
                }

                ResultEmittedData.Status.ERROR -> {
                    logError(
                        "load ERROR for page $currentPage: ${finalResult.message}",
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
                        "load received UNEXPECTED LOADING state for page $currentPage",
                        null,
                        TAG
                    )
                    LoadResult.Error(IllegalStateException("Received LOADING state after requesting a single page result."))
                }
            }
        } catch (exception: Exception) {
            logError("load exception for page $currentPage", exception, TAG)
            return LoadResult.Error(exception)
        }
    }

    override fun getRefreshKey(state: PagingState<Int, ApplicationUi>): Int? {
        return state.anchorPosition?.let { anchorPosition ->
            val anchorPage = state.closestPageToPosition(anchorPosition)
            anchorPage?.prevKey?.plus(1) ?: anchorPage?.nextKey?.minus(1)
        }
    }
}