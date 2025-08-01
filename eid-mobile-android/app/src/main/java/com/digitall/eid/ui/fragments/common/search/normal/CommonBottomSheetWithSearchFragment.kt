/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.common.search.normal

import androidx.core.os.bundleOf
import androidx.lifecycle.lifecycleScope
import androidx.navigation.fragment.navArgs
import com.digitall.eid.databinding.BottomSheetWithSearchBinding
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.extensions.enableChangeAnimations
import com.digitall.eid.extensions.findParentFragmentResultListenerFragmentManager
import com.digitall.eid.extensions.launchInScope
import com.digitall.eid.extensions.launchWhenResumed
import com.digitall.eid.extensions.setTextChangeListener
import com.digitall.eid.models.list.CommonDialogWithSearchItemUi
import com.digitall.eid.ui.fragments.base.BaseBottomSheetFragment
import com.digitall.eid.ui.fragments.common.search.normal.list.CommonBottomSheetWithSearchAdapter
import kotlinx.coroutines.delay
import kotlinx.coroutines.flow.onEach
import org.koin.android.ext.android.inject
import org.koin.androidx.viewmodel.ext.android.viewModel

class CommonBottomSheetWithSearchFragment :
    BaseBottomSheetFragment<BottomSheetWithSearchBinding, CommonBottomSheetWithSearchViewModel>(),
    CommonBottomSheetWithSearchAdapter.ClickListener {

    companion object {
        private const val TAG = "CommonBottomSheetWithSearchFragmentTag"
        const val COMMON_BOTTOM_SHEET_WITH_SEARCH_FRAGMENT_RESULT_BUNDLE_KEY =
            "COMMON_BOTTOM_SHEET_WITH_SEARCH_FRAGMENT_RESULT_BUNDLE_KEY"
        const val COMMON_BOTTOM_SHEET_WITH_SEARCH_FRAGMENT_RESULT_DATA_KEY =
            "COMMON_BOTTOM_SHEET_WITH_SEARCH_FRAGMENT_RESULT_DATA_KEY"
    }

    override fun getViewBinding() = BottomSheetWithSearchBinding.inflate(layoutInflater)

    override val viewModel: CommonBottomSheetWithSearchViewModel by viewModel()
    private val args: CommonBottomSheetWithSearchFragmentArgs by navArgs()
    private val adapter: CommonBottomSheetWithSearchAdapter by inject()

    override fun onDismissed() {
        setAdapterData(emptyList())
    }

    override fun setupView() {
        binding.recyclerView.adapter = adapter
        binding.recyclerView.enableChangeAnimations(false)
    }

    override fun setupControls() {
        parseArguments()
        adapter.clickListener = this
        binding.etSearch.setTextChangeListener {
            viewModel.onSearch(it.trim())
        }
    }

    private fun parseArguments() {
        try {
            viewModel.setModel(args.model)
        } catch (e: IllegalStateException) {
            logError("get args Exception: ${e.message}", e, TAG)
        }
    }

    override fun subscribeToLiveData() {
        viewModel.adapterList.onEach {
            setAdapterData(it)
        }.launchInScope(lifecycleScope)
        viewModel.resultModel.observe(viewLifecycleOwner) {
            findParentFragmentResultListenerFragmentManager()?.setFragmentResult(
                COMMON_BOTTOM_SHEET_WITH_SEARCH_FRAGMENT_RESULT_BUNDLE_KEY, bundleOf(
                    COMMON_BOTTOM_SHEET_WITH_SEARCH_FRAGMENT_RESULT_DATA_KEY to it
                )
            )
            dismiss()
        }
    }

    private fun setAdapterData(data: List<CommonDialogWithSearchItemUi>) {
        logDebug("setAdapterData size: ${data.size}", TAG)
        adapter.items = data
        binding.recyclerView.post {
            launchWhenResumed {
                delay(100)
                try {
                    logDebug("scrollToPosition 0", TAG)
                    binding.recyclerView.scrollToPosition(0)
                } catch (e: Exception) {
                    logError("scrollToPosition Exception", e, TAG)
                }
            }
        }
    }

    override fun onClicked(selected: CommonDialogWithSearchItemUi) {
        logDebug("onClicked selected: ${selected.text}", TAG)
        viewModel.onClicked(selected)
    }

}