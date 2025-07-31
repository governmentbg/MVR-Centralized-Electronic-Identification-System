package com.digitall.eid.ui.fragments.faq.flow

import com.digitall.eid.R
import com.digitall.eid.models.common.StartDestination
import com.digitall.eid.ui.fragments.base.flow.BaseFlowViewModel

class FaqFlowViewModel: BaseFlowViewModel() {

    companion object {
        private const val TAG = "FaqFlowViewModelTag"
    }

    fun getStartDestination() = StartDestination(R.id.faqFragment)
}