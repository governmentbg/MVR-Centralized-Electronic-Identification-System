package com.digitall.eid.ui.fragments.citizen.information

import androidx.fragment.app.clearFragmentResultListener
import androidx.fragment.app.setFragmentResultListener
import androidx.lifecycle.lifecycleScope
import com.digitall.eid.R
import com.digitall.eid.databinding.FragmentWithListBinding
import com.digitall.eid.domain.REFRESH_CITIZEN_INFORMATION_KEY
import com.digitall.eid.domain.REFRESH_CITIZEN_INFORMATION_REQUEST_KEY
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.extensions.enableChangeAnimations
import com.digitall.eid.extensions.launchInScope
import com.digitall.eid.models.citizen.information.CitizenInformationAdapterMarker
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonButtonUi
import com.digitall.eid.models.list.CommonTitleCheckboxUi
import com.digitall.eid.models.list.CommonTitleDescriptionUi
import com.digitall.eid.ui.fragments.base.BaseFragment
import com.digitall.eid.ui.fragments.citizen.information.list.CitizenInformationAdapter
import kotlinx.coroutines.flow.onEach
import org.koin.android.ext.android.inject
import org.koin.androidx.viewmodel.ext.android.viewModel

class CitizenInformationFragment :
    BaseFragment<FragmentWithListBinding, CitizenInformationViewModel>(),
    CitizenInformationAdapter.ClickListener {

    companion object {
        const val TAG = "ChangeCitizenPasswordFragmentTag"
    }

    override fun getViewBinding() = FragmentWithListBinding.inflate(layoutInflater)

    override val viewModel: CitizenInformationViewModel by viewModel()

    private val adapter: CitizenInformationAdapter by inject()

    override fun setupView() {
        super.setupView()
        binding.toolbar.setTitleText(StringSource(R.string.citizen_information_screen_title))
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
        setFragmentResultListener(REFRESH_CITIZEN_INFORMATION_REQUEST_KEY) { _, bundle ->
            val shouldRefresh = bundle.getBoolean(REFRESH_CITIZEN_INFORMATION_KEY, false)
            if (shouldRefresh) {
                viewModel.refreshScreen()
            }
        }
    }

    override fun subscribeToLiveData() {
        viewModel.adapterListLiveData.onEach { items ->
            setAdapterData(data = items)
        }.launchInScope(lifecycleScope)
    }

    override fun onFieldTextAction(model: CommonTitleDescriptionUi) {
        logDebug("onFieldTextAction", TAG)
        viewModel.onFieldTextAction(model)
    }

    override fun onButtonClicked(model: CommonButtonUi) {
        logDebug("onButtonClicked", TAG)
        viewModel.onButtonClicked(model)
    }

    override fun onCheckBoxChangeState(model: CommonTitleCheckboxUi) {
        logDebug("onCheckBoxChangeState", TAG)
        viewModel.onCheckBoxChangeState(model)
    }

    private fun setAdapterData(data: List<CitizenInformationAdapterMarker>) {
        logDebug("setAdapterData size: ${data.size}", TAG)
        adapter.items = data
    }

    override fun onDestroyed() {
        super.onDestroyed()
        clearFragmentResultListener(REFRESH_CITIZEN_INFORMATION_REQUEST_KEY)
    }

}