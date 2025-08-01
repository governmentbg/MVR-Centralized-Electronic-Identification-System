/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.empowerment.to.me.flow

import com.digitall.eid.R
import com.digitall.eid.models.common.StartDestination
import com.digitall.eid.ui.fragments.base.flow.BaseFlowViewModel

class EmpowermentToMeFlowViewModel : BaseFlowViewModel() {

    companion object {
        private const val TAG = "EmpowermentFromMeFlowViewModelTag"
    }

    fun getStartDestination(): StartDestination {
        return StartDestination(R.id.empowermentToMeFragment)
    }

}