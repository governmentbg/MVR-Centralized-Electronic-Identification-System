package com.digitall.eid.ui.fragments.empowerment.from.me.cancel.withdrawal

import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.ui.BaseViewModel

class EmpowermentFromMeCancelWithdrawalBottomSheetViewModel : BaseViewModel() {

    companion object {
        private const val TAG = "EmpowermentFromMeCancelWithdrawalBottomSheetViewModel"
    }

    override fun onBackPressed() {
        logDebug("onBackPressed", TAG)
        popBackStack()
    }

}