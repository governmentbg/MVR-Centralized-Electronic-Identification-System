/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.notifications.channels

import androidx.lifecycle.viewModelScope
import com.digitall.eid.R
import com.digitall.eid.domain.APPLICATION_LANGUAGE
import com.digitall.eid.domain.DELAY_500
import com.digitall.eid.domain.extensions.readOnly
import com.digitall.eid.domain.models.base.ErrorType
import com.digitall.eid.domain.models.base.onFailure
import com.digitall.eid.domain.models.base.onLoading
import com.digitall.eid.domain.models.base.onSuccess
import com.digitall.eid.domain.usecase.notifications.channels.GetNotificationChannelsUseCase
import com.digitall.eid.domain.usecase.notifications.channels.SetSelectedNotificationChannelsUseCase
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.extensions.launchInScope
import com.digitall.eid.mappers.notifications.channels.NotificationChannelUiMapper
import com.digitall.eid.models.common.BannerMessage
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.main.MainTabsEnum
import com.digitall.eid.models.notifications.channels.NotificationChannelUi
import com.digitall.eid.ui.BaseViewModel
import kotlinx.coroutines.delay
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.onEach
import org.koin.core.component.inject
import kotlin.concurrent.Volatile

class NotificationChannelsViewModel : BaseViewModel() {

    companion object {
        private const val TAG = "NotificationChannelsViewModelTag"
    }

    override var mainTabsEnum: MainTabsEnum? = MainTabsEnum.TAB_MORE

    private val notificationChannelUiMapper: NotificationChannelUiMapper by inject()
    private val getNotificationChannelsUseCase: GetNotificationChannelsUseCase by inject()
    private val setSelectedNotificationChannelsUseCase: SetSelectedNotificationChannelsUseCase by inject()

    @Volatile
    private var inProgress = false

    private val _adapterList = MutableStateFlow<List<NotificationChannelUi>>(emptyList())
    val adapterList = _adapterList.readOnly()

    @Volatile
    private var enabledNotificationsChannels = mutableListOf<String>()

    override fun onFirstAttach() {
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
        getNotificationChannelsUseCase.invoke().onEach { result ->
            result.onLoading {
                logDebug("getNotificationChannelsUseCase onLoading", TAG)
                if (adapterList.value.isEmpty()) {
                    showLoader()
                }
            }.onSuccess { model, _, _ ->
                logDebug("getNotificationChannelsUseCase onSuccess", TAG)
                inProgress = false
                enabledNotificationsChannels = (model.channels?.map { element -> element.id }
                    ?.filter { id -> model.enabledChannels?.contains(id) == true }
                    ?: emptyList()).toMutableList()
                _adapterList.emit(
                    notificationChannelUiMapper.map(
                        model,
                        APPLICATION_LANGUAGE
                    )
                )
                delay(DELAY_500)
                hideErrorState()
                hideLoader()
            }.onFailure { _, title, message, responseCode, errorType ->
                logError("getNotificationChannelsUseCase onFailure", message, TAG)
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

    fun onNotificationChannelSelected(id: String) {
        logDebug("onNotificationChannelSelected id: $id", TAG)

        when {
            enabledNotificationsChannels.contains(id) -> enabledNotificationsChannels.remove(id)
            else -> enabledNotificationsChannels.add(id)
        }

        setSelectedNotificationChannelsUseCase.invoke(
            ids = enabledNotificationsChannels,
        ).onEach { result ->
            result.onLoading {
                logDebug("setSelectedNotificationChannelsUseCase onLoading", TAG)
                showLoader()
            }.onSuccess { _, _, _ ->
                logDebug("setSelectedNotificationChannelsUseCase onSuccess", TAG)
                refreshScreen()
            }.onFailure { _, title, message, responseCode, errorType ->
                logError("setSelectedNotificationChannelsUseCase onFailure", message, TAG)
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

    override fun onBackPressed() {
        logDebug("onBackPressed", TAG)
        backToTab()
    }

}