/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.notifications.notifications

import androidx.lifecycle.lifecycleScope
import com.digitall.eid.databinding.FragmentNotificationsBinding
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.extensions.enableChangeAnimations
import com.digitall.eid.extensions.launchInScope
import com.digitall.eid.ui.fragments.base.BaseFragment
import com.digitall.eid.ui.fragments.notifications.notifications.list.NotificationsAdapter
import kotlinx.coroutines.flow.onEach
import org.koin.android.ext.android.inject
import org.koin.androidx.viewmodel.ext.android.viewModel

class NotificationsFragment :
    BaseFragment<FragmentNotificationsBinding, NotificationsViewModel>(),
    NotificationsAdapter.ClickListener {

    companion object {
        private const val TAG = "NotificationsFragmentTag"
        fun newInstance() = NotificationsFragment()
    }

    override fun getViewBinding() = FragmentNotificationsBinding.inflate(layoutInflater)

    override val viewModel: NotificationsViewModel by viewModel()
    private val adapter: NotificationsAdapter by inject()


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

    override fun onParentElementClicked(id: String) {
        logDebug("onParentElementClicked id: $id", TAG)
        viewModel.onParentElementClicked(id)
    }

    override fun onChildElementClicked(id: String) {
        viewModel.onChildElementClicked(id)
    }

    override fun onParentElementCheckBoxClicked(id: String) {
        viewModel.onParentElementCheckBoxClicked(id)
    }

}