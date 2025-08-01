/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.main.tabs.eim

import androidx.lifecycle.viewModelScope
import com.digitall.eid.R
import com.digitall.eid.domain.DELAY_500
import com.digitall.eid.domain.DEVICES
import com.digitall.eid.domain.models.base.onFailure
import com.digitall.eid.domain.models.base.onLoading
import com.digitall.eid.domain.models.base.onSuccess
import com.digitall.eid.domain.usecase.devices.GetDevicesUseCase
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.extensions.launchInScope
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.main.MainTabsEnum
import com.digitall.eid.ui.fragments.main.base.BaseMainTabViewModel
import kotlinx.coroutines.delay
import kotlinx.coroutines.flow.onEach
import org.koin.core.component.inject

class MainTabEIMViewModel : BaseMainTabViewModel() {

    companion object {
        private const val TAG = "MainTabTwoViewModelTag"
    }

    override var mainTabsEnum: MainTabsEnum? = MainTabsEnum.TAB_EIM

    private val getDevicesUseCase: GetDevicesUseCase by inject()

    override fun onFirstAttach() {
        logDebug("onFirstAttach", TAG)
        getDevicesUseCase.invoke().onEach { result ->
            result.onLoading {
                logDebug("getDevices onLoading", TAG)
                showLoader()
            }.onSuccess { model, _, _ ->
                logDebug("getDevices onSuccess", TAG)
                delay(DELAY_500)
                hideLoader()
                hideErrorState()
                DEVICES = model
            }.onFailure { _, _, message, responseCode, _ ->
                logDebug("getDevices onSuccess", TAG)
                delay(DELAY_500)
                hideLoader()
                showErrorState(
                    title = StringSource(R.string.information),
                    description = message?.let { StringSource(it) }
                        ?: StringSource(R.string.error_api_general, formatArgs = listOf(responseCode.toString())),
                )
            }
        }.launchInScope(viewModelScope)
    }

    fun toApplications() {
        logDebug("toApplications", TAG)
        navigateInTab(
            MainTabEIMFragmentDirections.toApplicationsFlowFragment(
                applicationId = null,
                certificateId = null,
            )
        )
    }

    fun toCertificates() {
        logDebug("toCertificates", TAG)
        navigateInTab(
            MainTabEIMFragmentDirections.toCertificatesFlowFragment(
                certificateId = null,
                applicationId = null,
            )
        )
    }

    fun toScanCode() {
        logDebug("toScanCode", TAG)
        navigateInTab(
            MainTabEIMFragmentDirections.toScanCodeFlowFragment()
        )
    }

    fun onCreateClicked() {
        logDebug("onCreateClicked", TAG)
        navigateInTab(MainTabEIMFragmentDirections.toApplicationCreateFlowFragment())
    }
}