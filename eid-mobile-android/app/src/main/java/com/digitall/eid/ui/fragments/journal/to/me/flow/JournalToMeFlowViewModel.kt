package com.digitall.eid.ui.fragments.journal.to.me.flow

import com.digitall.eid.R
import com.digitall.eid.models.common.StartDestination
import com.digitall.eid.ui.fragments.base.flow.BaseFlowViewModel

class JournalToMeFlowViewModel : BaseFlowViewModel() {

    companion object {
        private const val TAG = "JournalToMeFlowViewModelTag"
    }

    fun getStartDestination(): StartDestination {
        return StartDestination(R.id.journalToMeFragment)
    }

}