/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.applications.create.intro

import android.view.View
import com.digitall.eid.R
import com.digitall.eid.databinding.FragmentWithListBinding
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.extensions.enableChangeAnimations
import com.digitall.eid.extensions.launchWhenResumed
import com.digitall.eid.models.applications.create.ApplicationCreateIntroAdapterMarker
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonButtonUi
import com.digitall.eid.models.list.CommonDatePickerUi
import com.digitall.eid.models.list.CommonDialogWithSearchUi
import com.digitall.eid.models.list.CommonEditTextUi
import com.digitall.eid.models.list.CommonSpinnerUi
import com.digitall.eid.ui.fragments.applications.create.intro.adapter.ApplicationCreateIntroAdapter
import com.digitall.eid.ui.fragments.base.BaseFragment
import org.koin.android.ext.android.inject
import org.koin.androidx.viewmodel.ext.android.viewModel

class ApplicationCreateIntroFragment :
    BaseFragment<FragmentWithListBinding, ApplicationCreateIntroViewModel>(),
    ApplicationCreateIntroAdapter.ClickListener {

    companion object {
        private const val TAG = "ApplicationCreateIntroFragmentTag"
    }

    override fun getViewBinding() = FragmentWithListBinding.inflate(layoutInflater)

    override val viewModel: ApplicationCreateIntroViewModel by viewModel()
    private val adapter: ApplicationCreateIntroAdapter by inject()

    override fun setupView() {
        binding.recyclerView.adapter = adapter
        binding.recyclerView.enableChangeAnimations(false)
        binding.toolbar.setTitleText(StringSource(R.string.create_application_screen_title))
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

    private fun setAdapterData(data: List<ApplicationCreateIntroAdapterMarker>) {
        logDebug("setAdapterData size: ${data.size}", TAG)
        adapter.items = data
    }

    override fun onDialogWithSearchClicked(model: CommonDialogWithSearchUi) {
        logDebug("onDialogWithSearchClicked", TAG)
        viewModel.showDialogWithSearch(model)
    }

    override fun onSpinnerClicked(model: CommonSpinnerUi, anchor: View) {
        logDebug("onSpinnerClicked", TAG)
        showSpinner(
            anchor = anchor,
            model = model,
        )
    }

    override fun onButtonClicked(model: CommonButtonUi) {
        logDebug("onButtonClicked", TAG)
        viewModel.onButtonClicked(model)
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

    override fun onCharacterFilter(model: CommonEditTextUi, char: Char): Boolean {
        logDebug("onCharacterFilter", TAG)
        return viewModel.onCharacterFilter(model, char)
    }

}