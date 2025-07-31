/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.auth.password.enter

import androidx.annotation.IdRes
import androidx.lifecycle.viewModelScope
import com.digitall.eid.BuildConfig
import com.digitall.eid.NavActivityDirections
import com.digitall.eid.R
import com.digitall.eid.domain.DEBUG_ACCOUNT_EMAIL
import com.digitall.eid.domain.DEBUG_ACCOUNT_PASSWORD
import com.digitall.eid.domain.DELAY_500
import com.digitall.eid.domain.EMAIL_ADDRESS_PATTERN
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.extensions.readOnly
import com.digitall.eid.extensions.setValueOnMainThread
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.ui.fragments.base.auth.BaseAuthViewModel
import com.digitall.eid.utils.SingleLiveEvent
import kotlinx.coroutines.delay
import kotlinx.coroutines.launch

class AuthEnterEmailPasswordViewModel : BaseAuthViewModel() {

    companion object {
        private const val TAG = "AuthEnterEmailPasswordViewModelTag"
    }

    override val isAuthorizationActive: Boolean = false

    private val _emailErrorLiveEvent = SingleLiveEvent<StringSource?>()
    val emailErrorLiveEvent = _emailErrorLiveEvent.readOnly()

    private val _passwordErrorLiveEvent = SingleLiveEvent<StringSource?>()
    val passwordErrorLiveEvent = _passwordErrorLiveEvent.readOnly()

    private var email = ""
    private var password = ""

    override fun onBackPressed() {
        logDebug("onBackPressed", TAG)
        popBackStackToFragment(R.id.authStartFragment)
    }

    override fun onFirstAttach() {
        logDebug("onFirstAttach", TAG)
        if (BuildConfig.DEBUG && DEBUG_ACCOUNT_EMAIL.isNotEmpty()) {
            email = DEBUG_ACCOUNT_EMAIL
        }
        if (BuildConfig.DEBUG && DEBUG_ACCOUNT_PASSWORD.isNotEmpty()) {
            password = DEBUG_ACCOUNT_PASSWORD
        }
    }

    fun onEmailChanged(text: String) {
        email = text
        validateInput()
    }

    fun onPasswordChanged(text: String) {
        password = text
        validateInput()
    }

    fun toForgottenPassword() {
        logDebug("toForgottenPassword", TAG)
        navigateInFlow(AuthEnterEmailPasswordFragmentDirections.toForgottenPasswordFragment())
    }

    private fun validateInput() {
        validateEmail()
        validatePassword()
    }

    private fun validateEmail() {
        val error = when {
            email.isEmpty() -> StringSource(R.string.error_field_required)
            EMAIL_ADDRESS_PATTERN.matcher(email).matches()
                .not() -> StringSource(R.string.error_valid_email_format)

            else -> null
        }

        _emailErrorLiveEvent.setValueOnMainThread(error)
    }

    private fun validatePassword() {
        val error = when {
            password.isEmpty() -> StringSource(R.string.error_field_required)
            else -> null
        }

        _passwordErrorLiveEvent.setValueOnMainThread(error)
    }

    private fun isValidEmail(): Boolean {
        return email.isNotEmpty() && EMAIL_ADDRESS_PATTERN.matcher(email).matches()
    }

    private fun isValidPassword(): Boolean {
        return password.isNotEmpty()
    }

    fun authenticate() {
        validateInput()
        val isEmailValid = isValidEmail()
        val isPasswordValid = isValidPassword()

        if (isPasswordValid && isEmailValid) {
            login(username = email, password = password)
        }
    }



    override fun navigateNext(@IdRes tabId: Int) {
        logDebug("navigateNext", TAG)
        navigateNewRootInActivity(
            NavActivityDirections.toMainTabsFlowFragment(tabId = tabId)
        )
        viewModelScope.launch {
            delay(DELAY_500)
            hideLoader()
        }
    }

    override fun toMfaFragment(sessionId: String, ttl: Int, username: String, password: String) {
        logDebug("toMfaFragment", TAG)
        navigateInFlow(
            AuthEnterEmailPasswordFragmentDirections.toMfaFragment(
                sessionId = sessionId,
                email = username,
                password = password,
                ttl = ttl
            )
        )
    }

}