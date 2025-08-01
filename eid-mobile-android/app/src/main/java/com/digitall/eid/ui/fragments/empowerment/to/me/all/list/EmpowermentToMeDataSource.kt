/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 **/
package com.digitall.eid.ui.fragments.empowerment.to.me.all.list

import androidx.paging.PagingSource
import androidx.paging.PagingState
import com.digitall.eid.R
import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.empowerment.common.all.EmpowermentSortingModel
import com.digitall.eid.domain.models.empowerment.common.filter.EmpowermentFilterModel
import com.digitall.eid.domain.usecase.empowerment.to.me.GetEmpowermentToMeUseCase
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.mappers.empowerment.to.me.all.EmpowermentToMeUiMapper
import com.digitall.eid.models.common.EmptyDataException
import com.digitall.eid.models.common.PagingError
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.empowerment.common.all.EmpowermentAdapterMarker
import kotlinx.coroutines.flow.first
import kotlinx.coroutines.flow.transformWhile


class EmpowermentToMeDataSource(
    private val sortingModel: EmpowermentSortingModel,
    private val filterModel: EmpowermentFilterModel,
    private val empowermentToMeUiMapper: EmpowermentToMeUiMapper,
    private val getEmpowermentToMeUseCase: GetEmpowermentToMeUseCase,
) : PagingSource<Int, EmpowermentAdapterMarker>() {

    companion object {
        private const val TAG = "EmpowermentToMeDataSourceTag"
        private const val STARTING_PAGE_INDEX = 1
    }

    override suspend fun load(params: LoadParams<Int>): LoadResult<Int, EmpowermentAdapterMarker> {
        val currentPage = params.key ?: STARTING_PAGE_INDEX
        logDebug(
            "load: page=$currentPage, loadSize=${params.loadSize}",
            TAG
        )

        try {
            val resultFlow = getEmpowermentToMeUseCase.invoke(
                pageSize = params.loadSize,
                pageIndex = currentPage,
                filterModel = filterModel,
                sortingModel = sortingModel,
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
                        val empowermentsApiModels = responseModel.data ?: emptyList()
                        val empowermentsUi =
                            empowermentToMeUiMapper.mapList(empowermentsApiModels)

                        val isLastPage =
                            (empowermentsUi.isEmpty() && (responseModel.data?.isEmpty() ?: true))


                        val prevKey =
                            if (currentPage == STARTING_PAGE_INDEX) null else currentPage - 1
                        val nextKey = if (isLastPage) null else currentPage + 1

                        logDebug(
                            "load SUCCESS: dataSize=${empowermentsUi.size}, prevKey=$prevKey, nextKey=$nextKey, isLastPage=$isLastPage",
                            TAG
                        )
                        LoadResult.Page(
                            data = empowermentsUi,
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
            logError(
                "load exception for page $currentPage", exception,
                TAG
            )
            return LoadResult.Error(exception)
        }
    }

    override fun getRefreshKey(state: PagingState<Int, EmpowermentAdapterMarker>): Int? {
        return state.anchorPosition?.let { anchorPosition ->
            val anchorPage = state.closestPageToPosition(anchorPosition)
            anchorPage?.prevKey?.plus(1) ?: anchorPage?.nextKey?.minus(1)
        }
    }
}