/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.scanner.scanner

import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.extensions.readOnly
import com.digitall.eid.extensions.setValueOnMainThread
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.main.MainTabsEnum
import com.digitall.eid.ui.BaseViewModel
import com.digitall.eid.ui.fragments.main.tabs.eim.MainTabEIMFragmentDirections
import com.digitall.eid.utils.SingleLiveEvent

class ScanCodeViewModel : BaseViewModel() {

    companion object {
        private const val TAG = "ScanCodeViewModelTag"
    }

    override var mainTabsEnum: MainTabsEnum? = MainTabsEnum.TAB_EIM

    private var qrCode: String? = null

    private val _dialogMessageLiveData = SingleLiveEvent<StringSource>()
    val dialogMessageLiveData = _dialogMessageLiveData.readOnly()

    private val _dialogError = SingleLiveEvent<StringSource>()
    val dialogError = _dialogError.readOnly()

    fun refreshScreen() {

    }

    fun onScanCodeSuccess(qrCode: String) {
        this.qrCode = qrCode
        if (!qrCode.startsWith("otpauth://totp/eIdentity?secret=")) {
            _dialogError.setValueOnMainThread(
                StringSource("Link $qrCode is not valid")
            )
            return
        }
        navigateInTab(
            MainTabEIMFragmentDirections.toApplicationConfirmFlowFragment(
                qrCode = qrCode
            )
        )
    }

    override fun onBackPressed() {
        logDebug("onBackPressed", TAG)
        backToTab()
    }

}