/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.main.tabs.requests

import androidx.lifecycle.MutableLiveData
import androidx.lifecycle.viewModelScope
import com.digitall.eid.R
import com.digitall.eid.domain.APPLICATION_LANGUAGE
import com.digitall.eid.domain.DELAY_500
import com.digitall.eid.domain.models.base.ErrorType
import com.digitall.eid.domain.models.base.onFailure
import com.digitall.eid.domain.models.base.onLoading
import com.digitall.eid.domain.models.base.onSuccess
import com.digitall.eid.domain.models.challenge.request.SignedChallengeRequestModel
import com.digitall.eid.domain.usecase.requests.GetAllRequestsUseCase
import com.digitall.eid.domain.usecase.requests.SetOutcomeRequestUseCase
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.extensions.launchInScope
import com.digitall.eid.extensions.launchWithDispatcher
import com.digitall.eid.extensions.readOnly
import com.digitall.eid.extensions.setValueOnMainThread
import com.digitall.eid.mappers.requests.RequestUiMapper
import com.digitall.eid.models.common.BannerMessage
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.main.MainTabsEnum
import com.digitall.eid.models.requests.RequestUi
import com.digitall.eid.ui.fragments.main.base.BaseMainTabViewModel
import kotlinx.coroutines.Job
import kotlinx.coroutines.delay
import kotlinx.coroutines.flow.onEach
import kotlinx.coroutines.isActive
import org.koin.core.component.inject

class MainTabRequestsViewModel : BaseMainTabViewModel() {

    companion object {
        private const val TAG = "MainTabThreeViewModelTag"
        private const val SUCCEED = "SUCCEED"
        private const val CANCELLED = "CANCELLED"
        private const val PENDING_REQUESTS_FETCH_DELAY = 60000L
    }

    override var mainTabsEnum: MainTabsEnum? = MainTabsEnum.TAB_REQUESTS

    private val _adapterListLiveData = MutableLiveData<List<RequestUi>>()
    val adapterListLiveData = _adapterListLiveData.readOnly()

    private val getAllRequestsUseCase: GetAllRequestsUseCase by inject()
    private val setOutcomeRequestUseCase: SetOutcomeRequestUseCase by inject()
    private val requestUiMapper: RequestUiMapper by inject()
    private var fetchPendingRequestsJob: Job? = null

    override fun onPaused() {
        super.onPaused()
        cancelFetchPendingRequests()
    }

    override fun onHiddenChanged(hidden: Boolean) {
        when (hidden) {
            true -> cancelFetchPendingRequests()
            else -> refreshData()
        }
    }

    fun refreshData() {
        logDebug("refreshData", TAG)
        cancelFetchPendingRequests()
        fetchPendingRequestsJob = viewModelScope.launchWithDispatcher {
            while (isActive) {
                fetchPendingRequests()
                delay(PENDING_REQUESTS_FETCH_DELAY)
            }
        }
    }

    fun acceptRequest(requestId: String?, signedChallenge: SignedChallengeRequestModel) {
        logDebug("acceptRequest", TAG)
        setOutcomeRequestUseCase.invoke(
            requestId = requestId,
            status = SUCCEED,
            signedChallengeModel = signedChallenge
        ).onEach { result ->
            result.onLoading {
                logDebug("acceptRequest onLoading", TAG)
                showLoader()
            }.onSuccess { _, _, _ ->
                logDebug("acceptRequest onSuccess", TAG)
                refreshData()
            }.onFailure { _, _, message, responseCode, errorType ->
                logDebug("acceptRequest onFailure", TAG)
                delay(DELAY_500)
                hideLoader()
                when (errorType) {
                    ErrorType.NO_INTERNET_CONNECTION -> showErrorState(
                        title = StringSource(R.string.error_network_not_available),
                        description = StringSource(R.string.error_network_not_available_description),
                    )

                    else -> {
                        val bannerMessage = message?.let {
                            StringSource(
                                "$it (%s)",
                                formatArgs = listOf(responseCode.toString())
                            )
                        } ?: run {
                            StringSource(
                                R.string.error_api_general,
                                formatArgs = listOf(responseCode.toString())
                            )
                        }

                        showMessage(BannerMessage.error(bannerMessage))
                    }
                }
            }
        }.launchInScope(viewModelScope)
    }

    fun declineRequest(requestId: String?) {
        logDebug("declineRequest", TAG)
        setOutcomeRequestUseCase.invoke(requestId = requestId, status = CANCELLED)
            .onEach { result ->
                result.onLoading {
                    logDebug("declineRequest onLoading", TAG)
                    showLoader()
                }.onSuccess { _, _, _ ->
                    logDebug("declineRequest onSuccess", TAG)
                    refreshData()
                }.onFailure { _, _, message, responseCode, errorType ->
                    logDebug("declineRequest onFailure", TAG)
                    delay(DELAY_500)
                    hideLoader()
                    when (errorType) {
                        ErrorType.NO_INTERNET_CONNECTION -> showErrorState(
                            title = StringSource(R.string.error_network_not_available),
                            description = StringSource(R.string.error_network_not_available_description),
                        )

                        else -> {
                            val bannerMessage = message?.let {
                                StringSource(
                                    "$it (%s)",
                                    formatArgs = listOf(responseCode.toString())
                                )
                            } ?: run {
                                StringSource(
                                    R.string.error_api_general,
                                    formatArgs = listOf(responseCode.toString())
                                )
                            }

                            showMessage(BannerMessage.error(bannerMessage))
                        }
                    }
                }
            }.launchInScope(viewModelScope)
    }

    private fun fetchPendingRequests() {
        getAllRequestsUseCase.invoke().onEach { result ->
            result.onLoading {
                logDebug("getAllRequests onLoading", TAG)
                showLoader()
            }.onSuccess { model, _, _ ->
                logDebug("getAllRequests onSuccess", TAG)
                _adapterListLiveData.setValueOnMainThread(requestUiMapper.map(model, APPLICATION_LANGUAGE))
                delay(DELAY_500)
                hideLoader()
                hideErrorState()
            }.onFailure { _, title, message, responseCode, errorType ->
                logError("getAllRequests onFailure", message, TAG)
                hideLoader()
                when (errorType) {
                    ErrorType.AUTHORIZATION -> toLoginFragment()

                    else -> showErrorState(
                        title = StringSource(R.string.information),
                        description = message?.let {
                            StringSource(
                                "$it (%s)",
                                formatArgs = listOf((responseCode ?: 0).toString())
                            )
                        } ?: StringSource(
                            R.string.error_api_general,
                            formatArgs = listOf((responseCode ?: 0).toString())
                        ),
                    )
                }
            }
        }.launchInScope(viewModelScope)
    }

    private fun cancelFetchPendingRequests() = fetchPendingRequestsJob?.cancel()

}