/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.notifications.notifications

import androidx.lifecycle.viewModelScope
import com.digitall.eid.R
import com.digitall.eid.domain.APPLICATION_LANGUAGE
import com.digitall.eid.domain.extensions.readOnly
import com.digitall.eid.domain.models.base.ErrorType
import com.digitall.eid.domain.models.base.onFailure
import com.digitall.eid.domain.models.base.onLoading
import com.digitall.eid.domain.models.base.onSuccess
import com.digitall.eid.domain.usecase.notifications.notifications.GetNotificationsUseCase
import com.digitall.eid.domain.usecase.notifications.notifications.ReverseNotificationOpenStateUseCase
import com.digitall.eid.domain.usecase.notifications.notifications.SetNotSelectedNotificationUseCase
import com.digitall.eid.domain.usecase.notifications.notifications.SetNotSelectedParentNotificationUseCase
import com.digitall.eid.domain.usecase.notifications.notifications.SubscribeToNotificationsUseCase
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.extensions.launchInScope
import com.digitall.eid.extensions.launchWithDispatcher
import com.digitall.eid.mappers.notifications.notifications.NotificationUiMapper
import com.digitall.eid.models.common.BannerMessage
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.main.MainTabsEnum
import com.digitall.eid.models.notifications.notifications.NotificationAdapterMarker
import com.digitall.eid.ui.BaseViewModel
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.onEach
import org.koin.core.component.inject

class NotificationsViewModel : BaseViewModel() {

    companion object {
        private const val TAG = "NotificationsViewModelTag"
    }

    override var mainTabsEnum: MainTabsEnum? = MainTabsEnum.TAB_MORE

    private val notificationUiMapper: NotificationUiMapper by inject()
    private val getNotificationsUseCase: GetNotificationsUseCase by inject()
    private val subscribeToNotificationsUseCase: SubscribeToNotificationsUseCase by inject()
    private val setNotSelectedNotificationUseCase: SetNotSelectedNotificationUseCase by inject()
    private val reverseNotificationOpenStateUseCase: ReverseNotificationOpenStateUseCase by inject()
    private val setNotSelectedParentNotificationUseCase: SetNotSelectedParentNotificationUseCase by inject()

    @Volatile
    private var inProgress = false

    private val _adapterList = MutableStateFlow<List<NotificationAdapterMarker>>(emptyList())
    val adapterList = _adapterList.readOnly()

    fun onParentElementClicked(id: String) {
        logDebug("onParentElementClicked id: $id", TAG)
        viewModelScope.launchWithDispatcher {
            reverseNotificationOpenStateUseCase.invoke(id)
        }
    }

    fun onChildElementClicked(id: String) {
        logDebug("onChildElementClicked", TAG)
        setNotSelectedNotificationUseCase.invoke(
            id = id,
        ).onEach { result ->
            result.onLoading {
                logDebug("setNotSelectedNotificationUseCase onLoading", TAG)
                showLoader()
            }.onSuccess { _, _, _ ->
                logDebug("setNotSelectedNotificationUseCase onSuccess", TAG)
                hideLoader()
            }.onFailure { _, title, message, responseCode, errorType ->
                logError("setNotSelectedNotificationUseCase onFailure", message, TAG)
                hideLoader()
                when (errorType) {
                    ErrorType.NO_INTERNET_CONNECTION -> showErrorState(
                        title = StringSource(R.string.error_network_not_available),
                        description = StringSource(R.string.error_network_not_available_description),
                    )

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

    fun onParentElementCheckBoxClicked(id: String) {
        logDebug("onParentElementCheckBoxClicked", TAG)
        setNotSelectedParentNotificationUseCase.invoke(
            id = id,
        ).onEach { result ->
            result.onLoading {
                logDebug("setNotSelectedParentNotificationUseCase onLoading", TAG)
                showLoader()
            }.onSuccess { _, _, _ ->
                logDebug("setNotSelectedParentNotificationUseCase onSuccess", TAG)
                hideLoader()
            }.onFailure { _, title, message, responseCode, errorType ->
                logError("setNotSelectedParentNotificationUseCase onFailure", message, TAG)
                hideLoader()
                when (errorType) {
                    ErrorType.NO_INTERNET_CONNECTION -> showErrorState(
                        title = StringSource(R.string.error_network_not_available),
                        description = StringSource(R.string.error_network_not_available_description),
                    )

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

    override fun onFirstAttach() {
        subscribeToNotifications()
        refreshScreen()
    }


    fun refreshScreen() {
        logDebug("refreshScreen", TAG)
        if (inProgress) {
            logError("refreshScreen inProgress return", TAG)
            hideLoader()
            showMessage(BannerMessage.error(StringSource("Update in progress, please wait")))
            return
        }
        inProgress = true
        getNotificationsUseCase.invoke().onEach { result ->
            result.onLoading {
                logDebug("getNotificationsUseCase onLoading", TAG)
            }.onSuccess { _, _, _ ->
                logDebug("getNotificationsUseCase onSuccess", TAG)
                inProgress = false
                hideErrorState()
                if (adapterList.value.isEmpty()) {
                    showEmptyState()
                } else {
                    showReadyState()
                }
            }.onFailure { _, title, message, responseCode, errorType ->
                logError("getNotificationsUseCase onFailure", message, TAG)
                inProgress = false
                hideLoader()
                when (errorType) {
                    ErrorType.NO_INTERNET_CONNECTION -> showErrorState(
                        title = StringSource(R.string.error_network_not_available),
                        description = StringSource(R.string.error_network_not_available_description),
                    )

                    ErrorType.AUTHORIZATION -> toLoginFragment()

                    else -> if (adapterList.value.isEmpty()) {
                        showErrorState(
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
                    } else {
                        showMessage(BannerMessage.error(StringSource(R.string.error_server_error)))
                    }
                }
            }
        }.launchInScope(viewModelScope)
    }

    private fun subscribeToNotifications() {
        subscribeToNotificationsUseCase.invoke().onEach {
            logDebug("subscribeToNotificationsUseCase size: ${it.size}", TAG)
            val language = APPLICATION_LANGUAGE
            _adapterList.emit(notificationUiMapper.map(it, language))
            if (it.isEmpty()) {
                showEmptyState()
            } else {
                showReadyState()
            }
        }.launchInScope(viewModelScope)
    }

    override fun onBackPressed() {
        logDebug("onBackPressed", TAG)
        backToTab()
    }

}