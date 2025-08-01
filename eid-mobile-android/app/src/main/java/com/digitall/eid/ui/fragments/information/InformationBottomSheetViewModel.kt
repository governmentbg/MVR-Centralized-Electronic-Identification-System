package com.digitall.eid.ui.fragments.information

import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.ui.BaseViewModel

class InformationBottomSheetViewModel: BaseViewModel() {

    companion object {
        private const val TAG = "InformationBottomSheetViewModelTag"
    }

    override fun onBackPressed() {
        logDebug("onBackPressed", TAG)
        popBackStack()
    }
}