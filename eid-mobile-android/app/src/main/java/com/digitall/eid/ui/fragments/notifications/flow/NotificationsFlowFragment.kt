/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.notifications.flow

import com.digitall.eid.R
import com.digitall.eid.databinding.FragmentFlowContainerBinding
import com.digitall.eid.models.common.StartDestination
import com.digitall.eid.ui.fragments.base.flow.BaseFlowFragment
import org.koin.androidx.viewmodel.ext.android.viewModel

class NotificationsFlowFragment :
    BaseFlowFragment<FragmentFlowContainerBinding, NotificationsFlowViewModel>() {

    override fun getViewBinding() = FragmentFlowContainerBinding.inflate(layoutInflater)

    override val viewModel: NotificationsFlowViewModel by viewModel()

    override fun getFlowGraph() = R.navigation.nav_notifications

    override fun getStartDestination(): StartDestination {
        return viewModel.getStartDestination()
    }

}