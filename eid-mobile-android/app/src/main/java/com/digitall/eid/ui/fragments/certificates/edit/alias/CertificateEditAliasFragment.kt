package com.digitall.eid.ui.fragments.certificates.edit.alias

import android.os.Bundle
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
import com.digitall.eid.models.certificates.edit.alias.CertificateEditAliasAdapterMarker
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonButtonUi
import com.digitall.eid.models.list.CommonEditTextUi
import com.digitall.eid.ui.fragments.base.BaseFragment
import com.digitall.eid.ui.fragments.certificates.edit.alias.list.CertificateEditAliasAdapter
import org.koin.android.ext.android.inject

class CertificateEditAliasFragment :
    BaseFragment<FragmentWithListBinding, CertificateEditAliasViewModel>(),
    CertificateEditAliasAdapter.ClickListener {

    companion object {
        private const val TAG = "CertificateEditNameFragmentTag"
    }

    override val viewModel: CertificateEditAliasViewModel by viewModels()

    override fun getViewBinding() = FragmentWithListBinding.inflate(layoutInflater)

    private val adapter: CertificateEditAliasAdapter by inject()

    private val args: CertificateEditAliasFragmentArgs by navArgs()

    override fun setupView() {
        binding.toolbar.setTitleText(StringSource(R.string.certificate_edit_alias_screen_title))
        binding.recyclerView.adapter = adapter
        binding.recyclerView.enableChangeAnimations(false)
    }

    override fun parseArguments() {
        try {
            viewModel.setupModel(certificateId = args.id, certificateAlias = args.alias)
        } catch (exception: IllegalStateException) {
            logError(
                "parseArguments Exception: ${exception.message}",
                exception, TAG
            )
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

        viewModel.certificateAliasChangeLiveData.observe(viewLifecycleOwner) {
            val bundle = Bundle().apply {
                putBoolean(REFRESH_CERTIFICATES_KEY, it)
            }
            setFragmentResult(REFRESH_CERTIFICATES_REQUEST_KEY, bundle)
        }
    }

    override fun subscribeToLiveData() {
        viewModel.adapterListLiveData.observe(viewLifecycleOwner) { items ->
            setAdapterData(items)
        }
    }

    override fun onDetached() {
        super.onDetached()
        setAdapterData(data = emptyList())
    }

    private fun setAdapterData(data: List<CertificateEditAliasAdapterMarker>) {
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
}