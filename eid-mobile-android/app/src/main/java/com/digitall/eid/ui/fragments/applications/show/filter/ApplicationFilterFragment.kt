/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.applications.show.filter

import android.os.Bundle
import android.view.View
import androidx.fragment.app.setFragmentResult
import androidx.lifecycle.lifecycleScope
import androidx.navigation.fragment.navArgs
import com.digitall.eid.R
import com.digitall.eid.databinding.FragmentWithTopCloseButtonBinding
import com.digitall.eid.domain.FILTER_MODEL_KEY
import com.digitall.eid.domain.FILTER_MODEL_REQUEST_KEY
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.extensions.enableChangeAnimations
import com.digitall.eid.extensions.hideKeyboard
import com.digitall.eid.extensions.launchInScope
import com.digitall.eid.extensions.onClickThrottle
import com.digitall.eid.models.applications.filter.ApplicationsFilterAdapterMarker
import com.digitall.eid.models.common.BannerMessage
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonDatePickerUi
import com.digitall.eid.models.list.CommonDialogWithSearchUi
import com.digitall.eid.models.list.CommonEditTextUi
import com.digitall.eid.models.list.CommonSpinnerUi
import com.digitall.eid.ui.fragments.applications.show.filter.list.ApplicationFilterAdapter
import com.digitall.eid.ui.fragments.base.BaseFragment
import kotlinx.coroutines.flow.onEach
import org.koin.android.ext.android.inject
import org.koin.androidx.viewmodel.ext.android.viewModel

class ApplicationFilterFragment :
    BaseFragment<FragmentWithTopCloseButtonBinding, ApplicationFilterViewModel>(),
    ApplicationFilterAdapter.ClickListener {

    companion object {
        private const val TAG = "ApplicationFilterFragmentTag"
    }

    override fun getViewBinding() = FragmentWithTopCloseButtonBinding.inflate(layoutInflater)
    override val viewModel: ApplicationFilterViewModel by viewModel()
    private val adapter: ApplicationFilterAdapter by inject()
    private val args: ApplicationFilterFragmentArgs by navArgs()

    override fun parseArguments() {
        try {
            viewModel.setFilteringModel(args.model)
        } catch (exception: Exception) {
            logError(
                "parseArguments Exception: ${exception.message}",
                exception,
                TAG
            )
        }
    }

    override fun setupView() {
        binding.toolbar.setTitleText(StringSource(R.string.applications_title))
        binding.recyclerView.adapter = adapter
        binding.recyclerView.enableChangeAnimations(false)
    }

    override fun setupControls() {
        binding.toolbar.navigationClickListener = {
            viewModel.onBackPressed()
        }
        binding.icClose.onClickThrottle {
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
        binding.applyFilters.onClickThrottle {
            viewModel.tryApplyFilterData()
        }
        binding.clearFilters.onClickThrottle {
            viewModel.clearFilterData()
        }
        adapter.clickListener = this
        setupDialogWithSearchResultListener()
    }

    override fun subscribeToLiveData() {
        viewModel.adapterListLiveData.onEach {
            setAdapterData(it)
        }.launchInScope(lifecycleScope)
        viewModel.scrollToPositionLiveData.observe(viewLifecycleOwner) {
            binding.recyclerView.scrollToPosition(it)
        }
        viewModel.applyFilterDataLiveData.observe(viewLifecycleOwner) { filter ->
            val bundle = Bundle().apply {
                putParcelable(FILTER_MODEL_KEY, filter)
            }
            setFragmentResult(FILTER_MODEL_REQUEST_KEY, bundle)
        }
    }

    private fun setAdapterData(data: List<ApplicationsFilterAdapterMarker>) {
        logDebug("setAdapterData size: ${data.size}", TAG)
        adapter.items = data
        hideKeyboard()
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

    override fun onDatePickerClicked(model: CommonDatePickerUi) {
        logDebug("onDatePickerClicked", TAG)
        showDatePicker(model)
    }

    override fun onDialogWithSearchClicked(model: CommonDialogWithSearchUi) {
        logDebug("onDialogWithSearchClicked", TAG)
        if (model.list.isEmpty()) {
            logError("onDialogWithSearchClicked list is empty", TAG)
            showMessage(BannerMessage.error(StringSource("Empty")))
            return
        }
        viewModel.showDialogWithSearch(model)
    }

    override fun onSpinnerClicked(model: CommonSpinnerUi, anchor: View) {
        logDebug("onSpinnerClicked", TAG)
        if (model.list.isEmpty()) {
            logError("onSpinnerClicked list is empty", TAG)
            showMessage(BannerMessage.error(StringSource("Empty")))
            return
        }
        showSpinner(
            anchor = anchor,
            model = model,
        )
    }

}