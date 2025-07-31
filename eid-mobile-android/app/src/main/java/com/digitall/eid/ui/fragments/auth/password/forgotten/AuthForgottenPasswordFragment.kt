package com.digitall.eid.ui.fragments.auth.password.forgotten

import com.digitall.eid.R
import com.digitall.eid.databinding.FragmentAuthForgottenPasswordBinding
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.extensions.enableChangeAnimations
import com.digitall.eid.extensions.launchWhenResumed
import com.digitall.eid.extensions.setTextSource
import com.digitall.eid.models.auth.password.forgotten.AuthForgottenPasswordAdapterMarker
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonButtonUi
import com.digitall.eid.models.list.CommonEditTextUi
import com.digitall.eid.ui.fragments.auth.password.forgotten.list.AuthForgottenPasswordAdapter
import com.digitall.eid.ui.fragments.base.BaseFragment
import org.koin.android.ext.android.inject
import org.koin.androidx.viewmodel.ext.android.viewModel

class AuthForgottenPasswordFragment :
    BaseFragment<FragmentAuthForgottenPasswordBinding, AuthForgottenPasswordViewModel>(),
    AuthForgottenPasswordAdapter.ClickListener {

    companion object {
        private const val TAG = "AuthForgottenPasswordFragmentTag"
    }

    override fun getViewBinding() = FragmentAuthForgottenPasswordBinding.inflate(layoutInflater)

    override val viewModel: AuthForgottenPasswordViewModel by viewModel()

    private val adapter: AuthForgottenPasswordAdapter by inject()

    override fun setupView() {
        super.setupView()
        binding.tvRegistration.setTextSource(StringSource(R.string.forgotten_password_title))
        binding.recyclerView.adapter = adapter
        binding.recyclerView.enableChangeAnimations(false)
    }

    override fun setupControls() {
        super.setupControls()
        adapter.clickListener = this
        adapter.recyclerViewProvider = { binding.recyclerView }
        binding.toolbar.navigationClickListener = {
            viewModel.onBackPressed()
        }
    }

    override fun subscribeToLiveData() {
        viewModel.adapterListLiveData.observe(viewLifecycleOwner) { items ->
            setAdapterData(data = items)
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

    private fun setAdapterData(data: List<AuthForgottenPasswordAdapterMarker>) {
        logDebug("setAdapterData size: ${data.size}", TAG)
        adapter.items = data
    }

    override fun onEditTextFocusChanged(model: CommonEditTextUi) {
        logDebug("onEditTextFocusChanged hasFocus: ${model.hasFocus}", TAG)
        viewModel.onEditTextFocusChanged(model = model)
    }

    override fun onEditTextDone(model: CommonEditTextUi) {
        logDebug("onEditTextDone text: ${model.selectedValue}", TAG)
        viewModel.onEditTextDone(model = model)
    }

    override fun onEditTextChanged(model: CommonEditTextUi) {
        logDebug("onEditTextChanged text: ${model.selectedValue}", TAG)
        viewModel.onEditTextChanged(model = model)
    }

    override fun onCharacterFilter(model: CommonEditTextUi, char: Char): Boolean {
        logDebug("onCharacterFilter text: ${model.selectedValue}", TAG)
        return viewModel.onCharacterFilter(model = model, char = char)
    }

    override fun onButtonClicked(model: CommonButtonUi) {
        logDebug("onButtonClicked type: ${model.elementEnum}", TAG)
        viewModel.onButtonClicked(model)
    }
}