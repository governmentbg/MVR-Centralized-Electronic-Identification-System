package com.digitall.eid.ui.fragments.contacts.flow

import com.digitall.eid.R
import com.digitall.eid.models.common.StartDestination
import com.digitall.eid.ui.fragments.base.flow.BaseFlowViewModel

class ContactsFlowViewModel : BaseFlowViewModel() {

    companion object {
        private const val TAG = "ContactsFlowViewModelTag"
    }

    fun getStartDestination() = StartDestination(R.id.contactsFragment)
}