/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.certificates.details

import androidx.lifecycle.lifecycleScope
import androidx.navigation.fragment.navArgs
import com.digitall.eid.R
import com.digitall.eid.databinding.FragmentWithListBinding
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.extensions.enableChangeAnimations
import com.digitall.eid.extensions.launchInScope
import com.digitall.eid.models.certificates.details.CertificateDetailsAdapterMarker
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonButtonUi
import com.digitall.eid.models.list.CommonSimpleTextInFieldUi
import com.digitall.eid.models.list.CommonSimpleTextUi
import com.digitall.eid.ui.fragments.base.BaseFragment
import com.digitall.eid.ui.fragments.certificates.details.list.CertificateDetailsAdapter
import kotlinx.coroutines.flow.onEach
import org.koin.android.ext.android.inject
import org.koin.androidx.viewmodel.ext.android.viewModel

class CertificateDetailsFragment :
    BaseFragment<FragmentWithListBinding, CertificateDetailsViewModel>(),
    CertificateDetailsAdapter.ClickListener {

    companion object {
        private const val TAG = "CertificateDetailsFragmentTag"
    }

    override fun getViewBinding() = FragmentWithListBinding.inflate(layoutInflater)
    override val viewModel: CertificateDetailsViewModel by viewModel()
    private val adapter: CertificateDetailsAdapter by inject()
    private val args: CertificateDetailsFragmentArgs by navArgs()

    override fun setupView() {
        binding.toolbar.setTitleText(StringSource(R.string.certificate_details_screen_title))
        binding.recyclerView.adapter = adapter
        binding.recyclerView.setItemViewCacheSize(48)
        binding.recyclerView.enableChangeAnimations(false)
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
    }

    override fun parseArguments() {
        try {
            viewModel.setupModel(
                certificateId = args.certificateId,
                applicationId = args.applicationId,
                detailsType = args.detailsType
            )
        } catch (e: IllegalStateException) {
            logError("parseArguments Exception: ${e.message}", e, TAG)
        }
    }

    override fun subscribeToLiveData() {
        viewModel.adapterListLiveData.onEach {
            setAdapterData(it)
        }.launchInScope(lifecycleScope)
    }

    private fun setAdapterData(data: List<CertificateDetailsAdapterMarker>) {
        logDebug("setAdapterData size: ${data.size}", TAG)
        adapter.items = data
    }

    override fun onButtonClicked(model: CommonButtonUi) {
        logDebug("onButtonClicked", TAG)
        viewModel.onButtonClicked(model)
    }

    override fun onInFieldTextClicked(model: CommonSimpleTextInFieldUi) {
        logDebug("onInFieldTextClicked", TAG)
        viewModel.onInFieldTextClicked(model)
    }

    override fun onFieldTextAction(model: CommonSimpleTextUi) {
        logDebug("onFieldTextEdit", TAG)
        viewModel.onFieldTextAction(model)
    }

    override fun onDestroyed() {
        logDebug("onDestroyFragment", TAG)
        adapter.items = emptyList()
    }

}