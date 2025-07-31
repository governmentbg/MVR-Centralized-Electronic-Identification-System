/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.empowerment.from.me.signing

import android.os.Bundle
import android.view.View
import androidx.fragment.app.setFragmentResult
import androidx.lifecycle.lifecycleScope
import androidx.navigation.fragment.navArgs
import com.digitall.eid.R
import com.digitall.eid.databinding.FragmentWithListBinding
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.extensions.enableChangeAnimations
import com.digitall.eid.extensions.launchInScope
import com.digitall.eid.models.common.BannerMessage
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.empowerment.signing.EmpowermentFromMeSigningAdapterMarker
import com.digitall.eid.models.list.CommonButtonUi
import com.digitall.eid.models.list.CommonSpinnerUi
import com.digitall.eid.ui.fragments.base.BaseFragment
import com.digitall.eid.ui.fragments.empowerment.from.me.signing.list.EmpowermentFromMeSigningAdapter
import kotlinx.coroutines.flow.onEach
import org.koin.android.ext.android.inject
import org.koin.androidx.viewmodel.ext.android.viewModel

class EmpowermentFromMeSigningFragment :
    BaseFragment<FragmentWithListBinding, EmpowermentFromMeSigningViewModel>(),
    EmpowermentFromMeSigningAdapter.ClickListener {

    companion object {
        private const val TAG = "EmpowermentFromMeSigningFragmentTag"
        private const val REQUEST_REFRESH_KEY = "REQUEST_REFRESH_KEY"
        private const val REFRESH_FLAG_KEY = "REFRESH_FLAG_KEY"
    }

    override fun getViewBinding() = FragmentWithListBinding.inflate(layoutInflater)

    override val viewModel: EmpowermentFromMeSigningViewModel by viewModel()
    private val adapter: EmpowermentFromMeSigningAdapter by inject()
    private val args: EmpowermentFromMeSigningFragmentArgs by navArgs()

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
            viewModel.onBackPressed()
        }
        binding.errorView.actionTwoClickListener = {
            viewModel.onBackPressed()
        }
        binding.emptyStateView.reloadClickListener = {
            viewModel.onBackPressed()
        }
    }

    override fun subscribeToLiveData() {
        viewModel.adapterListLiveData.onEach {
            setAdapterData(it)
        }.launchInScope(lifecycleScope)
        viewModel.requestRefreshLiveData.observe(viewLifecycleOwner) {
            val bundle = Bundle().apply {
                putBoolean(REFRESH_FLAG_KEY, true)
            }
            setFragmentResult(REQUEST_REFRESH_KEY, bundle)
        }
    }

    private fun setAdapterData(data: List<EmpowermentFromMeSigningAdapterMarker>) {
        logDebug("setAdapterData size: ${data.size}", TAG)
        adapter.items = data
    }

    override fun onButtonClicked(model: CommonButtonUi) {
        logDebug("onButtonClicked", TAG)
        viewModel.onButtonClicked(model)
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

    override fun onDestroyed() {
        logDebug("onDestroyFragment", TAG)
        adapter.items = emptyList()
    }

}