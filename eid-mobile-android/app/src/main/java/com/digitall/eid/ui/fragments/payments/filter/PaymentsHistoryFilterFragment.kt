package com.digitall.eid.ui.fragments.payments.filter

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
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonDatePickerUi
import com.digitall.eid.models.list.CommonEditTextUi
import com.digitall.eid.models.list.CommonSpinnerUi
import com.digitall.eid.models.payments.history.filter.PaymentsHistoryFilterAdapterMarker
import com.digitall.eid.ui.fragments.base.BaseFragment
import com.digitall.eid.ui.fragments.payments.filter.list.PaymentsHistoryFilterAdapter
import kotlinx.coroutines.flow.onEach
import org.koin.android.ext.android.inject
import org.koin.androidx.viewmodel.ext.android.viewModel

class PaymentsHistoryFilterFragment :
    BaseFragment<FragmentWithTopCloseButtonBinding, PaymentsHistoryFilterViewModel>(),
    PaymentsHistoryFilterAdapter.ClickListener {

    companion object {
        private const val TAG = "CertificateFilterFragmentTag"
    }

    override fun getViewBinding() = FragmentWithTopCloseButtonBinding.inflate(layoutInflater)
    override val viewModel: PaymentsHistoryFilterViewModel by viewModel()
    private val adapter: PaymentsHistoryFilterAdapter by inject()
    private val args: PaymentsHistoryFilterFragmentArgs by navArgs()

    override fun parseArguments() {
        try {
            viewModel.setFilterModel(args.model)
        } catch (exception: Exception) {
            logError(
                "parseArguments Exception: ${exception.message}",
                exception,
                TAG
            )
        }
    }

    override fun setupView() {
        binding.toolbar.setTitleText(StringSource(R.string.payments_history_screen_title))
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
        binding.applyFilters.onClickThrottle {
            viewModel.tryApplyFilterData()
        }
        binding.clearFilters.onClickThrottle {
            viewModel.clearFilterData()
        }
        adapter.clickListener = this
    }

    override fun subscribeToLiveData() {
        viewModel.adapterListLiveData.onEach {
            setAdapterData(it)
        }.launchInScope(lifecycleScope)
        viewModel.applyFilterDataLiveData.observe(viewLifecycleOwner) { filter ->
            val bundle = Bundle().apply {
                putParcelable(FILTER_MODEL_KEY, filter)
            }
            setFragmentResult(FILTER_MODEL_REQUEST_KEY, bundle)
        }
    }

    private fun setAdapterData(data: List<PaymentsHistoryFilterAdapterMarker>) {
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

    override fun onSpinnerClicked(model: CommonSpinnerUi, anchor: View) {
        logDebug("onSpinnerClicked", TAG)
        showSpinner(
            anchor = anchor,
            model = model,
        )
    }

}