/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.auth.password.enter

import android.content.res.ColorStateList
import androidx.core.view.isVisible
import com.digitall.eid.BuildConfig
import com.digitall.eid.R
import com.digitall.eid.databinding.FragmentAuthEnterPasswordBinding
import com.digitall.eid.domain.DEBUG_ACCOUNT_EMAIL
import com.digitall.eid.domain.DEBUG_ACCOUNT_PASSWORD
import com.digitall.eid.extensions.onClickThrottle
import com.digitall.eid.extensions.setTextChangeListener
import com.digitall.eid.extensions.setTextSource
import com.digitall.eid.models.common.AlertDialogResult
import com.digitall.eid.ui.fragments.base.BaseFragment
import com.google.android.material.textfield.TextInputLayout.END_ICON_PASSWORD_TOGGLE
import org.koin.androidx.viewmodel.ext.android.viewModel
import kotlin.system.exitProcess

class AuthEnterEmailPasswordFragment :
    BaseFragment<FragmentAuthEnterPasswordBinding, AuthEnterEmailPasswordViewModel>() {

    companion object {
        private const val TAG = "AuthEnterEmailPasswordFragmentTag"
    }

    override fun getViewBinding() = FragmentAuthEnterPasswordBinding.inflate(layoutInflater)

    override val viewModel: AuthEnterEmailPasswordViewModel by viewModel()

    override fun setupControls() {
        binding.etEmail.setTextChangeListener {
            if (isVisible && isResumed) {
                viewModel.onEmailChanged(it.trim())
            }
        }
        binding.etPassword.setTextChangeListener {
            if (isVisible && isResumed) {
                viewModel.onPasswordChanged(it.trim())
            }
        }
        binding.toolbar.navigationClickListener = {
            viewModel.onBackPressed()
        }
        binding.errorView.actionOneClickListener = {
            viewModel.authenticate()
        }
        binding.errorView.actionTwoClickListener = {
            viewModel.authenticate()
        }
        binding.btnEnter.onClickThrottle {
            viewModel.authenticate()
        }
        binding.btnForgottenPassword.onClickThrottle {
            viewModel.toForgottenPassword()
        }
    }

    override fun setupView() {
        if (BuildConfig.DEBUG && DEBUG_ACCOUNT_EMAIL.isNotEmpty()) {
            binding.etEmail.setText(DEBUG_ACCOUNT_EMAIL)
        }
        if (BuildConfig.DEBUG && DEBUG_ACCOUNT_PASSWORD.isNotEmpty()) {
            binding.etPassword.setText(DEBUG_ACCOUNT_PASSWORD)
        }
        binding.valueInputLayoutPassword.setEndIconStyle(
            mode = END_ICON_PASSWORD_TOGGLE,
            tintList = ColorStateList.valueOf(binding.root.context.getColor(R.color.color_0C53B2)),
            drawable = R.drawable.toggle_password
        )
        binding.btnEnter.isEnabled = true
    }

    override fun subscribeToLiveData() {
        viewModel.emailErrorLiveEvent.observe(viewLifecycleOwner) { error ->
            error?.let {
                binding.tvErrorEmail.setTextSource(it)
            }
            binding.tvErrorEmail.isVisible = error != null
        }
        viewModel.passwordErrorLiveEvent.observe(viewLifecycleOwner) { error ->
            error?.let {
                binding.tvErrorPassword.setTextSource(it)
            }
            binding.tvErrorPassword.isVisible = error != null
        }
    }

    override fun onAlertDialogResult(result: AlertDialogResult) {
        if (result.messageId == DIALOG_EXIT && result.isPositive) {
            exitProcess(0)
        }
    }

}