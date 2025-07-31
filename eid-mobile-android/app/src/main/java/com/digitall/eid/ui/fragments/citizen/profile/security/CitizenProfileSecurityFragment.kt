package com.digitall.eid.ui.fragments.citizen.profile.security

import com.digitall.eid.R
import com.digitall.eid.databinding.FragmentWithListBinding
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.extensions.enableChangeAnimations
import com.digitall.eid.models.citizen.profile.security.CitizenProfileSecurityAdapterMarker
import com.digitall.eid.models.common.AuthenticationResult
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonTitleCheckboxUi
import com.digitall.eid.ui.fragments.base.BaseFragment
import com.digitall.eid.ui.fragments.citizen.profile.security.list.CitizenProfileSecurityAdapter
import com.digitall.eid.ui.fragments.pin.citizen.profile.create.CreatePinCitizenProfileBottomSheetFragment
import org.koin.android.ext.android.inject
import org.koin.androidx.viewmodel.ext.android.viewModel

class CitizenProfileSecurityFragment:
    BaseFragment<FragmentWithListBinding, CitizenProfileSecurityViewModel>(),
    CitizenProfileSecurityAdapter.ClickListener,
    CreatePinCitizenProfileBottomSheetFragment.Listener {

    companion object {
        const val TAG = "ChangeCitizenPasswordFragmentTag"
    }

    override fun getViewBinding() = FragmentWithListBinding.inflate(layoutInflater)

    override val viewModel: CitizenProfileSecurityViewModel by viewModel()

    private val adapter: CitizenProfileSecurityAdapter by inject()

    private var createPinCitizenProfileBottomSheetFragment: CreatePinCitizenProfileBottomSheetFragment? = null

    override fun setupView() {
        super.setupView()
        binding.toolbar.setTitleText(StringSource(R.string.citizen_profile_security_screen_title))
        binding.recyclerView.adapter = adapter
        binding.recyclerView.enableChangeAnimations(false)
        binding.refreshLayout.isGestureAllowed = false
    }

    override fun setupControls() {
        super.setupControls()
        binding.toolbar.navigationClickListener = {
            viewModel.onBackPressed()
        }
        binding.errorView.actionOneClickListener = {
            viewModel.refreshScreen()
        }
        adapter.clickListener = this
    }

    override fun subscribeToLiveData() {
        viewModel.adapterListLiveData.observe(viewLifecycleOwner) { items ->
            setAdapterData(data = items)
        }

        viewModel.showCreatePinBottomSheetEvent.observe(viewLifecycleOwner) {
            showCreatePinBottomSheet()
        }

        viewModel.authenticationResultEvent.observe(viewLifecycleOwner) { result ->
            if (result is AuthenticationResult.Failure) {
                viewModel.showBannerErrorMessage(result.message)
            }
        }
    }

    override fun onDetached() {
        super.onDetached()
        setAdapterData(data = emptyList())
    }

    override fun onCheckBoxChangeState(model: CommonTitleCheckboxUi) {
        logDebug("onCheckBoxChangeState", TAG)
        viewModel.onCheckBoxChangeState(model)
    }

    private fun setAdapterData(data: List<CitizenProfileSecurityAdapterMarker>) {
        logDebug("setAdapterData size: ${data.size}", TAG)
        adapter.items = data
    }

    override fun operationCompleted(pin: String?) {
        createPinCitizenProfileBottomSheetFragment?.dismiss().also {
            createPinCitizenProfileBottomSheetFragment = null
        }
        viewModel.setupApplicationPin(pin = pin)
    }

    private fun showCreatePinBottomSheet() {
        createPinCitizenProfileBottomSheetFragment = CreatePinCitizenProfileBottomSheetFragment.newInstance(listener = this)
            .also { bottomSheet ->
                bottomSheet.show(
                    parentFragmentManager,
                    "ApplicationConfirmPinBottomSheetFragmentTag"
                )
            }

    }

}