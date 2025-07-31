package com.digitall.eid.ui.fragments.empowerment.legal.flow

import com.digitall.eid.R
import com.digitall.eid.models.common.StartDestination
import com.digitall.eid.ui.fragments.base.flow.BaseFlowViewModel

class EmpowermentLegalFlowViewModel : BaseFlowViewModel() {

    companion object {
        private const val TAG = "EmpowermentLegalFlowViewModelTag"
    }

    fun getStartDestination(): StartDestination {
        return StartDestination(R.id.empowermentsLegalEntitySearchFragment)
    }

}