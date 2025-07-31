/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.notifications.channels

import androidx.lifecycle.lifecycleScope
import com.digitall.eid.databinding.FragmentNotificationChannelsBinding
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.extensions.enableChangeAnimations
import com.digitall.eid.extensions.launchInScope
import com.digitall.eid.ui.fragments.base.BaseFragment
import com.digitall.eid.ui.fragments.notifications.channels.list.NotificationChannelsAdapter
import kotlinx.coroutines.flow.onEach
import org.koin.android.ext.android.inject
import org.koin.androidx.viewmodel.ext.android.viewModel

class NotificationChannelsFragment :
    BaseFragment<FragmentNotificationChannelsBinding, NotificationChannelsViewModel>(),
    NotificationChannelsAdapter.ClickListener {

    companion object {
        private const val TAG = "NotificationChannelsFragmentTag"
        fun newInstance() = NotificationChannelsFragment()
    }

    override fun getViewBinding() = FragmentNotificationChannelsBinding.inflate(layoutInflater)

    override val viewModel: NotificationChannelsViewModel by viewModel()
    private val adapter: NotificationChannelsAdapter by inject()

    override fun setupView() {
        binding.recyclerView.adapter = adapter
        binding.recyclerView.enableChangeAnimations(false)
    }

    override fun setupControls() {
        adapter.clickListener = this
        binding.refreshLayout.setOnRefreshListener {
            binding.refreshLayout.isRefreshing = false
            viewModel.refreshScreen()
        }
        binding.errorView.actionOneClickListener = {
            viewModel.refreshScreen()
        }
        binding.errorView.actionTwoClickListener = {
            viewModel.refreshScreen()
        }
        binding.emptyStateView.reloadClickListener = {
            viewModel.refreshScreen()
        }
    }

    override fun subscribeToLiveData() {
        viewModel.adapterList.onEach {
            adapter.items = it
        }.launchInScope(lifecycleScope)
    }

    override fun onNotificationSelected(id: String) {
        logDebug("onNotificationSelected id: $id", TAG)
        viewModel.onNotificationChannelSelected(id)
    }

}