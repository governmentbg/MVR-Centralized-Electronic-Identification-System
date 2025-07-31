package com.digitall.eid.ui.fragments.termsandconditions.flow

import com.digitall.eid.R
import com.digitall.eid.models.common.StartDestination
import com.digitall.eid.ui.fragments.base.flow.BaseFlowViewModel

class TermsAndConditionsFlowViewModel : BaseFlowViewModel() {

    companion object {
        private const val TAG = "TermsAndConditionsFlowViewModelTag"
    }

    fun getStartDestination() = StartDestination(R.id.termsAndConditionsFragment)
}