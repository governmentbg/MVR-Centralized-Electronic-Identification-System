/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.base.flow

import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.ui.BaseViewModel

abstract class BaseFlowViewModel : BaseViewModel() {

    companion object {
        private const val TAG = "BaseFlowViewModelTag"
    }

    final override fun onBackPressed() {
        logError("onBackPressed", TAG)
        // not need
    }

}