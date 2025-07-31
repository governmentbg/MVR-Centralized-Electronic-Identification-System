package com.digitall.eid.ui.fragments.empowerment.create.create.indefinite

import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.ui.BaseViewModel

class EmpowermentCreateIndefiniteBottomSheetViewModel : BaseViewModel() {

    companion object {
        private const val TAG = "EmpowermentCreateIndefiniteBottomSheetViewModelTag"
    }

    override fun onBackPressed() {
        logDebug("onBackPressed", TAG)
        popBackStack()
    }

}