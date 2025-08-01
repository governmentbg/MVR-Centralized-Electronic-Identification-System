package com.digitall.eid.ui.fragments.certificates.resume

import android.os.Bundle
import android.view.View
import androidx.fragment.app.setFragmentResult
import androidx.fragment.app.viewModels
import androidx.navigation.fragment.navArgs
import com.digitall.eid.R
import com.digitall.eid.databinding.FragmentWithListBinding
import com.digitall.eid.domain.REFRESH_CERTIFICATES_KEY
import com.digitall.eid.domain.REFRESH_CERTIFICATES_REQUEST_KEY
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.extensions.enableChangeAnimations
import com.digitall.eid.extensions.launchWhenResumed
import com.digitall.eid.models.certificates.resume.CertificateResumeAdapterMarker
import com.digitall.eid.models.certificates.resume.CertificateResumeDeviceTypeEnum
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonButtonUi
import com.digitall.eid.models.list.CommonDatePickerUi
import com.digitall.eid.models.list.CommonDialogWithSearchUi
import com.digitall.eid.models.list.CommonEditTextUi
import com.digitall.eid.models.list.CommonSpinnerUi
import com.digitall.eid.ui.fragments.base.BaseFragment
import com.digitall.eid.ui.fragments.certificates.resume.list.CertificateResumeAdapter
import org.koin.android.ext.android.inject

class CertificateResumeFragment :
    BaseFragment<FragmentWithListBinding, CertificateResumeViewModel>(),
    CertificateResumeAdapter.ClickListener {

    companion object {
        private const val TAG = "CertificateResumeFragmentTag"
    }

    override val viewModel: CertificateResumeViewModel by viewModels()

    private val adapter: CertificateResumeAdapter by inject()

    private val args: CertificateResumeFragmentArgs by navArgs()

    override fun getViewBinding() = FragmentWithListBinding.inflate(layoutInflater)

    override fun parseArguments() {
        try {
            val identityTypes = buildMap {
                CertificateResumeDeviceTypeEnum.entries.map {
                    it.type to it.title.getString(requireContext())
                }.forEach { pair ->
                    put(pair.first, pair.second)
                }
            }
            viewModel.setupModel(id = args.id, identityTypes = identityTypes)
        } catch (exception: IllegalStateException) {
            logError(
                "parseArguments Exception: ${exception.message}",
                exception,
                TAG
            )
        }
    }

    override fun setupView() {
        binding.toolbar.setTitleText(StringSource(R.string.certificate_resume_screen_title))
        binding.recyclerView.adapter = adapter
        binding.recyclerView.enableChangeAnimations(false)
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
    }

    override fun subscribeToLiveData() {
        viewModel.adapterListLiveData.observe(viewLifecycleOwner) {
            setAdapterData(it)
        }
        viewModel.certificateStatusChangeLiveData.observe(viewLifecycleOwner) { flag ->
            val bundle = Bundle().apply {
                putBoolean(REFRESH_CERTIFICATES_KEY, flag)
            }
            setFragmentResult(REFRESH_CERTIFICATES_REQUEST_KEY, bundle)
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
    }

    override fun onDetached() {
        super.onDetached()
        setAdapterData(data = emptyList())
    }

    private fun setAdapterData(data: List<CertificateResumeAdapterMarker>) {
        logDebug("setAdapterData size: ${data.size}", TAG)
        adapter.items = data
    }

    override fun onButtonClicked(model: CommonButtonUi) {
        logDebug("onButtonClicked", TAG)
        viewModel.onButtonClicked(model)
    }

    override fun onEditTextFocusChanged(model: CommonEditTextUi) {
        logDebug("onEditTextFocusChanged", TAG)
        viewModel.onEditTextFocusChanged(model)
    }

    override fun onEditTextDone(model: CommonEditTextUi) {
        logDebug("onEditTextDone", TAG)
        viewModel.onEditTextDone(model)
    }

    override fun onEditTextChanged(model: CommonEditTextUi) {
        logDebug("onEditTextChanged", TAG)
        viewModel.onEditTextChanged(model)
    }

    override fun onDatePickerClicked(model: CommonDatePickerUi) {
        logDebug("onDatePickerClicked", TAG)
        showDatePicker(model)
    }

    override fun onSpinnerClicked(model: CommonSpinnerUi, anchor: View) {
        logDebug("onSpinnerClicked", TAG)
        if (model.list.isEmpty()) return
        showSpinner(
            anchor = anchor,
            model = model,
        )
    }

    override fun onDialogWithSearchClicked(model: CommonDialogWithSearchUi) {
        logDebug("onDialogWithSearchClicked", TAG)
        if (model.list.isEmpty()) return
        viewModel.showDialogWithSearch(model)
    }
}