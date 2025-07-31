package com.digitall.eid.ui.fragments.journal.from.me.flow

import com.digitall.eid.R
import com.digitall.eid.models.common.StartDestination
import com.digitall.eid.ui.fragments.base.flow.BaseFlowViewModel

class JournalFromMeFlowViewModel : BaseFlowViewModel() {

    companion object {
        private const val TAG = "JournalFromMeFlowViewModelTag"
    }

    fun getStartDestination(): StartDestination {
        return StartDestination(R.id.journalFromMeFragment)
    }

}