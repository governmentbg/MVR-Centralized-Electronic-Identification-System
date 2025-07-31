package com.digitall.eid.ui.fragments.contacts.flow

import com.digitall.eid.R
import com.digitall.eid.databinding.FragmentFlowContainerBinding
import com.digitall.eid.ui.fragments.base.flow.BaseFlowFragment
import org.koin.androidx.viewmodel.ext.android.viewModel

class ContactsFlowFragment:
    BaseFlowFragment<FragmentFlowContainerBinding, ContactsFlowViewModel>() {

    companion object {
        private const val TAG = "ContactsFlowFragmentTag"
    }

    override fun getViewBinding() = FragmentFlowContainerBinding.inflate(layoutInflater)

    override val viewModel: ContactsFlowViewModel by viewModel()

    override fun getFlowGraph() = R.navigation.nav_contacts

    override fun getStartDestination() = viewModel.getStartDestination()

}