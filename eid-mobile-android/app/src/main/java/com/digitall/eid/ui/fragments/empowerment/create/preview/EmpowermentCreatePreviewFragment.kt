/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.empowerment.create.preview

import androidx.lifecycle.lifecycleScope
import androidx.navigation.fragment.navArgs
import com.digitall.eid.R
import com.digitall.eid.databinding.FragmentWithListBinding
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.extensions.enableChangeAnimations
import com.digitall.eid.extensions.hideKeyboard
import com.digitall.eid.extensions.launchInScope
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.empowerment.create.preview.EmpowermentCreatePreviewAdapterMarker
import com.digitall.eid.models.list.CommonButtonUi
import com.digitall.eid.ui.fragments.base.BaseFragment
import com.digitall.eid.ui.fragments.empowerment.create.preview.list.EmpowermentCreatePreviewAdapter
import kotlinx.coroutines.flow.onEach
import org.koin.android.ext.android.inject
import org.koin.androidx.viewmodel.ext.android.viewModel

class EmpowermentCreatePreviewFragment :
    BaseFragment<FragmentWithListBinding, EmpowermentCreatePreviewViewModel>(),
    EmpowermentCreatePreviewAdapter.ClickListener {

    companion object {
        private const val TAG = "CreateEmpowermentFragmentTag"
    }

    override fun getViewBinding() = FragmentWithListBinding.inflate(layoutInflater)

    override val viewModel: EmpowermentCreatePreviewViewModel by viewModel()
    private val adapter: EmpowermentCreatePreviewAdapter by inject()
    private val args: EmpowermentCreatePreviewFragmentArgs by navArgs()

    override fun parseArguments() {
        try {
            viewModel.setupModel(args.model)
        } catch (e: IllegalStateException) {
            logError("parseArguments Exception: ${e.message}", e, TAG)
        }
    }

    override fun setupView() {
        binding.recyclerView.adapter = adapter
        binding.recyclerView.enableChangeAnimations(false)
        binding.toolbar.setTitleText(StringSource(R.string.empowerment_from_me_title))
    }

    override fun setupControls() {
        binding.refreshLayout.isGestureAllowed = false
        adapter.clickListener = this
        binding.toolbar.navigationClickListener = {
            viewModel.onBackPressed()
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
        setupDialogWithSearchResultListener()
        setupDialogWithSearchMultiselectResultListener()
    }

    override fun subscribeToLiveData() {
        viewModel.adapterListLiveData.onEach {
            setAdapterData(it)
        }.launchInScope(lifecycleScope)
        viewModel.scrollToPositionLiveData.observe(viewLifecycleOwner) {
            binding.recyclerView.scrollToPosition(it)
        }
    }

    private fun setAdapterData(data: List<EmpowermentCreatePreviewAdapterMarker>) {
        logDebug("setAdapterData size: ${data.size}", TAG)
        adapter.items = data
        hideKeyboard()
    }

    override fun onButtonClicked(model: CommonButtonUi) {
        logDebug("onButtonClicked else", TAG)
        viewModel.onButtonClicked(model = model)
    }

}