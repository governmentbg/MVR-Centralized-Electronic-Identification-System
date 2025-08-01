/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.scanner.flow

import com.digitall.eid.R
import com.digitall.eid.models.common.StartDestination
import com.digitall.eid.ui.fragments.base.flow.BaseFlowViewModel

class ScanCodeFlowViewModel : BaseFlowViewModel() {

    companion object {
        private const val TAG = "ScanCodeFlowViewModelTag"
    }

    fun getStartDestination(): StartDestination {
        return StartDestination(R.id.scanCodeFragment)
    }

}