/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.main.tabs.more

import androidx.lifecycle.lifecycleScope
import com.digitall.eid.databinding.FragmentMainTabMoreBinding
import com.digitall.eid.extensions.launchInScope
import com.digitall.eid.extensions.openUrlInBrowser
import com.digitall.eid.models.common.AlertDialogResult
import com.digitall.eid.models.main.more.TabMoreItems
import com.digitall.eid.ui.fragments.main.base.BaseMainTabFragment
import com.digitall.eid.ui.fragments.main.tabs.more.list.TabMoreAdapter
import kotlinx.coroutines.flow.onEach
import org.koin.android.ext.android.inject
import org.koin.androidx.viewmodel.ext.android.viewModel
import kotlin.system.exitProcess

class MainTabMoreFragment :
    BaseMainTabFragment<FragmentMainTabMoreBinding, MainTabMoreViewModel>(),
    TabMoreAdapter.ClickListener {

    companion object {
        private const val TAG = "MainTabFourFragmentTag"
    }

    private val adapter: TabMoreAdapter by inject()

    override fun getViewBinding() = FragmentMainTabMoreBinding.inflate(layoutInflater)
    override val viewModel: MainTabMoreViewModel by viewModel()

    override fun setupView() {
        binding.recyclerView.adapter = adapter
        adapter.clickListener = this
    }

    override fun setupControls() {
        binding.toolbar.navigationClickListener = {
            viewModel.onBackPressed()
        }
    }

    override fun subscribeToLiveData() {
        viewModel.adapterList.onEach {
            adapter.items = it
        }.launchInScope(lifecycleScope)
        viewModel.openUrlInBrowserEvent.observe(viewLifecycleOwner) { url ->
            context?.openUrlInBrowser(url = url)
        }
    }

    override fun onElementClicked(type: TabMoreItems) {
        viewModel.onElementClicked(type)
    }

    override fun onAlertDialogResult(result: AlertDialogResult) {
        if (result.messageId == DIALOG_EXIT && result.isPositive) {
            exitProcess(0)
        }
    }

}