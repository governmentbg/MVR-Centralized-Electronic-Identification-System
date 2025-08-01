package com.digitall.eid.ui.fragments.common.search.multiselect

import androidx.core.os.bundleOf
import androidx.lifecycle.lifecycleScope
import androidx.navigation.fragment.navArgs
import com.digitall.eid.databinding.BottomSheetWithSearchMultiselectBinding
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.extensions.enableChangeAnimations
import com.digitall.eid.extensions.findParentFragmentResultListenerFragmentManager
import com.digitall.eid.extensions.launchInScope
import com.digitall.eid.extensions.onClickThrottle
import com.digitall.eid.extensions.setTextChangeListener
import com.digitall.eid.models.list.CommonDialogWithSearchAdapterMarker
import com.digitall.eid.models.list.CommonDialogWithSearchMultiselectItemUi
import com.digitall.eid.ui.fragments.base.BaseBottomSheetFragment
import com.digitall.eid.ui.fragments.common.search.multiselect.list.CommonBottomSheetWithSearchMultiselectAdapter
import kotlinx.coroutines.flow.onEach
import org.koin.android.ext.android.inject
import org.koin.androidx.viewmodel.ext.android.viewModel

class CommonBottomSheetWithSearchMultiselectFragment :
    BaseBottomSheetFragment<BottomSheetWithSearchMultiselectBinding, CommonBottomSheetWithSearchMultiselectViewModel>(),
    CommonBottomSheetWithSearchMultiselectAdapter.ClickListener {

    companion object {
        private const val TAG = "CommonBottomSheetWithSearchFragmentTag"
        const val COMMON_BOTTOM_SHEET_WITH_SEARCH_MULTISELECT_FRAGMENT_RESULT_BUNDLE_KEY =
            "COMMON_BOTTOM_SHEET_WITH_SEARCH_MULTISELECT_FRAGMENT_RESULT_BUNDLE_KEY"
        const val COMMON_BOTTOM_SHEET_WITH_SEARCH_MULTISELECT_FRAGMENT_RESULT_DATA_KEY =
            "COMMON_BOTTOM_SHEET_WITH_SEARCH_MULTISELECT_FRAGMENT_RESULT_DATA_KEY"
    }

    override fun getViewBinding() = BottomSheetWithSearchMultiselectBinding.inflate(layoutInflater)

    override val viewModel: CommonBottomSheetWithSearchMultiselectViewModel by viewModel()
    private val args: CommonBottomSheetWithSearchMultiselectFragmentArgs by navArgs()
    private val adapter: CommonBottomSheetWithSearchMultiselectAdapter by inject()

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
        binding.btnDone.onClickThrottle {
            viewModel.onDone()
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
                COMMON_BOTTOM_SHEET_WITH_SEARCH_MULTISELECT_FRAGMENT_RESULT_BUNDLE_KEY, bundleOf(
                    COMMON_BOTTOM_SHEET_WITH_SEARCH_MULTISELECT_FRAGMENT_RESULT_DATA_KEY to it
                )
            )
            dismiss()
        }
    }

    private fun setAdapterData(data: List<CommonDialogWithSearchAdapterMarker>) {
        logDebug("setAdapterData size: ${data.size}", TAG)
        adapter.items = data
    }

    override fun onChecked(selected: CommonDialogWithSearchMultiselectItemUi) {
        logDebug("onChecked selected: ${selected.text}", TAG)
        viewModel.onChecked(selected)
    }

}