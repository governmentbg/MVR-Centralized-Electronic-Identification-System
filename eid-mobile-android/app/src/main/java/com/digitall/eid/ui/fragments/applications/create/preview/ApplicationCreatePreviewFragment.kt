/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.applications.create.preview

import androidx.lifecycle.lifecycleScope
import androidx.navigation.fragment.navArgs
import com.digitall.eid.R
import com.digitall.eid.databinding.FragmentWithListBinding
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.extensions.enableChangeAnimations
import com.digitall.eid.extensions.hideKeyboard
import com.digitall.eid.extensions.launchInScope
import com.digitall.eid.models.applications.create.ApplicationCreatePreviewAdapterMarker
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonButtonUi
import com.digitall.eid.ui.fragments.applications.create.preview.adapter.ApplicationCreatePreviewAdapter
import com.digitall.eid.ui.fragments.base.BaseFragment
import kotlinx.coroutines.flow.onEach
import org.koin.android.ext.android.inject
import org.koin.androidx.viewmodel.ext.android.viewModel

class ApplicationCreatePreviewFragment :
    BaseFragment<FragmentWithListBinding, ApplicationCreatePreviewViewModel>(),
    ApplicationCreatePreviewAdapter.ClickListener {

    companion object {
        private const val TAG = "CreateEmpowermentFragmentTag"
    }

    override fun getViewBinding() = FragmentWithListBinding.inflate(layoutInflater)

    override val viewModel: ApplicationCreatePreviewViewModel by viewModel()
    private val adapter: ApplicationCreatePreviewAdapter by inject()
    private val args: ApplicationCreatePreviewFragmentArgs by navArgs()

    override fun parseArguments() {
        try {
            viewModel.setupModel(args.model)
        } catch (e: IllegalStateException) {
            logError("parseArguments Exception: ${e.message}", e, TAG)
            viewModel.setupModel(null)
        }
    }

    override fun setupView() {
        binding.recyclerView.adapter = adapter
        binding.recyclerView.enableChangeAnimations(false)
        binding.recyclerView.setItemViewCacheSize(24)
        binding.toolbar.setTitleText(StringSource(R.string.create_application_screen_title))
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
        setupDialogWithSearchResultListener()
    }

    override fun subscribeToLiveData() {
        viewModel.adapterListLiveData.onEach {
            setAdapterData(it)
        }.launchInScope(lifecycleScope)
    }

    private fun setAdapterData(data: List<ApplicationCreatePreviewAdapterMarker>) {
        logDebug("setAdapterData size: ${data.size}", TAG)
        adapter.items = data
        hideKeyboard()
    }

    override fun onButtonClicked(model: CommonButtonUi) {
        logDebug("onButtonClicked else", TAG)
        viewModel.onButtonClicked(model = model)
    }

}