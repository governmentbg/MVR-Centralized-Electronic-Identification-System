/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.error.biometric

import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.ui.BaseViewModel

class BiometricErrorBottomSheetViewModel : BaseViewModel() {

    companion object {
        private const val TAG = "BiometricErrorBottomSheetViewModelTag"
    }

    override fun onBackPressed() {
        logDebug("onBackPressed", TAG)
        popBackStack()
    }

}