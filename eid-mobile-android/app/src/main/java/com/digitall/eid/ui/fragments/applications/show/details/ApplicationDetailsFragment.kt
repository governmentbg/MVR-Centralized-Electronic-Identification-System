/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.applications.show.details

import android.os.Bundle
import androidx.fragment.app.setFragmentResult
import androidx.lifecycle.lifecycleScope
import androidx.navigation.fragment.navArgs
import com.digitall.eid.R
import com.digitall.eid.databinding.FragmentWithListBinding
import com.digitall.eid.domain.ENVIRONMENT
import com.digitall.eid.domain.REFRESH_APPLICATIONS_KEY
import com.digitall.eid.domain.REFRESH_APPLICATIONS_REQUEST_KEY
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.extensions.enableChangeAnimations
import com.digitall.eid.extensions.findParentFragmentResultListenerFragmentManager
import com.digitall.eid.extensions.launchInScope
import com.digitall.eid.extensions.openUrlInBrowser
import com.digitall.eid.models.applications.details.ApplicationDetailsAdapterMarker
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonButtonUi
import com.digitall.eid.models.list.CommonSimpleTextUi
import com.digitall.eid.ui.fragments.applications.show.details.list.ApplicationDetailsAdapter
import com.digitall.eid.ui.fragments.base.BaseFragment
import kotlinx.coroutines.flow.onEach
import org.koin.android.ext.android.inject
import org.koin.androidx.viewmodel.ext.android.viewModel

class ApplicationDetailsFragment :
    BaseFragment<FragmentWithListBinding, ApplicationDetailsViewModel>(),
    ApplicationDetailsAdapter.ClickListener {

    companion object {
        private const val TAG = "ApplicationDetailsFragmentTag"
        private const val NAVIGATE_TO_APPLICATIONS_REQUEST_KEY =
            "NAVIGATE_TO_APPLICATIONS_REQUEST_KEY"
        private const val NAVIGATE_TO_APPLICATIONS_KEY = "NAVIGATE_TO_APPLICATIONS_KEY"
    }

    override fun getViewBinding() = FragmentWithListBinding.inflate(layoutInflater)
    override val viewModel: ApplicationDetailsViewModel by viewModel()
    private val adapter: ApplicationDetailsAdapter by inject()
    private val args: ApplicationDetailsFragmentArgs by navArgs()

    override fun setupView() {
        binding.toolbar.setTitleText(StringSource(R.string.application_details_screen_title))
        binding.recyclerView.adapter = adapter
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
        findParentFragmentResultListenerFragmentManager()?.setFragmentResultListener(
            NAVIGATE_TO_APPLICATIONS_REQUEST_KEY,
            viewLifecycleOwner
        ) { _, bundle ->
            if (bundle.getBoolean(NAVIGATE_TO_APPLICATIONS_KEY, false)) {
                viewModel.onBackPressed()
            }
        }
    }

    override fun parseArguments() {
        try {
            viewModel.setupModel(
                applicationId = args.applicationId,
                certificateId = args.certificateId,
            )
        } catch (e: IllegalStateException) {
            logError("parseArguments Exception: ${e.message}", e, TAG)
            viewModel.setupModel(
                applicationId = null,
                certificateId = null,
            )
        }
    }

    override fun subscribeToLiveData() {
        viewModel.adapterListLiveData.onEach {
            setAdapterData(it)
        }.launchInScope(lifecycleScope)
        viewModel.openPaymentEvent.observe(viewLifecycleOwner) { paymentAccessCode ->
            val basePaymentUrl = ENVIRONMENT.urlPayment
            val paymentUrl =
                paymentAccessCode?.let { "$basePaymentUrl?code=$it" } ?: run { basePaymentUrl }
            context?.openUrlInBrowser(url = paymentUrl)
        }
        viewModel.certificateStatusChangeLiveData.observe(viewLifecycleOwner) { flag ->
            val bundle = Bundle().apply {
                putBoolean(REFRESH_APPLICATIONS_KEY, flag)
            }
            setFragmentResult(REFRESH_APPLICATIONS_REQUEST_KEY, bundle)
        }
    }

    private fun setAdapterData(data: List<ApplicationDetailsAdapterMarker>) {
        logDebug("setAdapterData size: ${data.size}", TAG)
        adapter.items = data
    }

    override fun onTextClicked(model: CommonSimpleTextUi) {
        logDebug("onTextClicked", TAG)
        viewModel.onTextClicked(model)
    }

    override fun onButtonClicked(model: CommonButtonUi) {
        logDebug("onButtonClicked", TAG)
        viewModel.onButtonClicked(model)
    }

    override fun onDestroyed() {
        logDebug("onDestroyFragment", TAG)
        adapter.items = emptyList()
    }

}