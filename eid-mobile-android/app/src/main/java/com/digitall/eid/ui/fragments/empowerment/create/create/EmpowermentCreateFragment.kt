/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.empowerment.create.create

import android.view.View
import androidx.lifecycle.lifecycleScope
import androidx.navigation.fragment.navArgs
import com.digitall.eid.R
import com.digitall.eid.databinding.FragmentWithListBinding
import com.digitall.eid.domain.DELAY_1000
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.extensions.enableChangeAnimations
import com.digitall.eid.extensions.launchWhenResumed
import com.digitall.eid.extensions.launchWithDispatcher
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.empowerment.create.create.EmpowermentCreateAdapterMarker
import com.digitall.eid.models.list.CommonButtonTransparentUi
import com.digitall.eid.models.list.CommonButtonUi
import com.digitall.eid.models.list.CommonDatePickerUi
import com.digitall.eid.models.list.CommonDialogWithSearchMultiselectUi
import com.digitall.eid.models.list.CommonDialogWithSearchUi
import com.digitall.eid.models.list.CommonEditTextUi
import com.digitall.eid.models.list.CommonSpinnerUi
import com.digitall.eid.ui.fragments.base.BaseFragment
import com.digitall.eid.ui.fragments.empowerment.create.create.indefinite.EmpowermentCreateIndefiniteBottomSheetFragment
import com.digitall.eid.ui.fragments.empowerment.create.create.list.EmpowermentCreateAdapter
import com.digitall.eid.ui.fragments.information.InformationBottomSheetFragment
import kotlinx.coroutines.delay
import org.koin.android.ext.android.inject
import org.koin.androidx.viewmodel.ext.android.viewModel

class EmpowermentCreateFragment :
    BaseFragment<FragmentWithListBinding, EmpowermentCreateViewModel>(),
    EmpowermentCreateAdapter.ClickListener,
    EmpowermentCreateIndefiniteBottomSheetFragment.Listener {

    companion object {
        private const val TAG = "CreateEmpowermentFragmentTag"
    }

    override fun getViewBinding() = FragmentWithListBinding.inflate(layoutInflater)

    override val viewModel: EmpowermentCreateViewModel by viewModel()
    private val adapter: EmpowermentCreateAdapter by inject()
    private val args: EmpowermentCreateFragmentArgs by navArgs()

    private var empowermentCreateIndefiniteBottomSheetFragment: EmpowermentCreateIndefiniteBottomSheetFragment? =
        null

    override fun parseArguments() {
        try {
            args.model?.let { model ->
                viewModel.setupModel(model)
            }
        } catch (e: IllegalStateException) {
            logError("parseArguments Exception: ${e.message}", e, TAG)
        }
    }

    override fun setupView() {
        binding.recyclerView.adapter = adapter
        binding.recyclerView.enableChangeAnimations(false)
        binding.toolbar.setTitleText(StringSource(R.string.empowerment_create_screen_title))
        binding.toolbar.setSettingsIcon(R.drawable.ic_information) {
            showInformationBottomSheet(content = viewModel.getInformationContent())
        }
    }

    override fun setupControls() {
        binding.refreshLayout.isGestureAllowed = false
        adapter.clickListener = this
        adapter.recyclerViewProvider = { binding.recyclerView }
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
        viewModel.adapterListLiveData.observe(viewLifecycleOwner) {
            setAdapterData(it)
        }
        viewModel.scrollToErrorPositionLiveData.observe(viewLifecycleOwner) { position ->
            binding.recyclerView.post {
                launchWhenResumed {
                    try {
                        binding.recyclerView.smoothScrollToPosition(position)
                    } catch (e: Exception) {
                        logError("scrollToPosition Exception", e, TAG)
                    }
                }
            }
        }
        viewModel.showIndefiniteEmpowermentInformationLiveData.observe(viewLifecycleOwner) {
            empowermentCreateIndefiniteBottomSheetFragment =
                EmpowermentCreateIndefiniteBottomSheetFragment.newInstance(listener = this)
                    .also { bottomSheet ->
                        bottomSheet.show(
                            parentFragmentManager,
                            "EmpowermentCreateIndefiniteBottomSheetFragmentTag"
                        )
                    }
        }
    }

    override fun onDetached() {
        super.onDetached()
        setAdapterData(data = emptyList())
    }

    private fun setAdapterData(data: List<EmpowermentCreateAdapterMarker>) {
        logDebug("setAdapterData size: ${data.size}", TAG)
        adapter.items = data
    }

    override fun onEraseClicked(model: EmpowermentCreateAdapterMarker) {
        logDebug("onEraseClicked", TAG)
        viewModel.onEraseClicked(model)
    }

    override fun onDialogWithSearchClicked(model: CommonDialogWithSearchUi) {
        logDebug("onDialogWithSearchClicked", TAG)
        viewModel.showDialogWithSearch(model)
    }

    override fun onDialogWithSearchMultiselectClicked(model: CommonDialogWithSearchMultiselectUi) {
        logDebug("onDialogWithSearchMultiselectClicked", TAG)
        viewModel.showDialogWithSearchMultiselect(model)
    }

    override fun onButtonTransparentClicked(model: CommonButtonTransparentUi) {
        logDebug("onButtonTransparentClicked", TAG)
        viewModel.onButtonTransparentClicked(model)
    }

    override fun onSpinnerClicked(model: CommonSpinnerUi, anchor: View) {
        logDebug("onSpinnerClicked", TAG)
        showSpinner(
            anchor = anchor,
            model = model,
        )
    }

    override fun onButtonClicked(model: CommonButtonUi) {
        logDebug("onButtonClicked", TAG)
        viewModel.onButtonClicked(model)
    }

    override fun onFocusChanged(model: CommonEditTextUi) {
        logDebug("onFocusChanged hasFocus: ${model.hasFocus}", TAG)
        viewModel.onFocusChanged(model)
    }

    override fun onEnterTextDone(model: CommonEditTextUi) {
        logDebug("onEnterTextDone text: ${model.selectedValue}", TAG)
        viewModel.onEnterTextDone(model)
    }

    override fun onEditTextChanged(model: CommonEditTextUi) {
        logDebug("onEditTextChanged text: ${model.selectedValue}", TAG)
        viewModel.onEditTextChanged(model)
    }

    override fun onCharacterFilter(model: CommonEditTextUi, char: Char): Boolean {
        logDebug("onCharacterFilter text: ${model.selectedValue}", TAG)
        return viewModel.onCharacterFilter(model = model, char = char)
    }

    override fun onDatePickerClicked(model: CommonDatePickerUi) {
        logDebug("onDatePickerClicked $model", TAG)
        showDatePicker(model)
    }

    override fun operationCompleted() {
        lifecycleScope.launchWithDispatcher {
            delay(DELAY_1000)
            empowermentCreateIndefiniteBottomSheetFragment?.dismiss().also {
                viewModel.toPreview()
            }
        }
    }

    private fun showInformationBottomSheet(content: StringSource) {
        InformationBottomSheetFragment.newInstance(content = content)
            .also { bottomSheet ->
                bottomSheet.show(parentFragmentManager, null)
            }
    }
}