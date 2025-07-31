package com.digitall.eid.ui.fragments.auth.mfa

import android.os.CountDownTimer
import androidx.annotation.IdRes
import androidx.lifecycle.MutableLiveData
import androidx.lifecycle.viewModelScope
import com.digitall.eid.NavActivityDirections
import com.digitall.eid.R
import com.digitall.eid.domain.DELAY_500
import com.digitall.eid.domain.OTP_CODE_LENGTH
import com.digitall.eid.domain.models.authentication.AuthenticationType
import com.digitall.eid.domain.models.base.ErrorType
import com.digitall.eid.domain.models.base.onFailure
import com.digitall.eid.domain.models.base.onLoading
import com.digitall.eid.domain.models.base.onSuccess
import com.digitall.eid.domain.usecase.mfa.MfaVerifyOtpCodeUseCase
import com.digitall.eid.domain.usecase.verify.login.VerifyLoginUseCase
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.extensions.isFragmentInBackStack
import com.digitall.eid.extensions.launchInScope
import com.digitall.eid.extensions.readOnly
import com.digitall.eid.extensions.setValueOnMainThread
import com.digitall.eid.models.common.BannerMessage
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.ui.BaseViewModel
import com.digitall.eid.utils.SingleLiveEvent
import kotlinx.coroutines.delay
import kotlinx.coroutines.flow.onEach
import kotlinx.coroutines.launch
import org.koin.core.component.inject

class AuthMfaViewModel : BaseViewModel() {

    companion object {
        private const val TAG = "AuthMfaViewModelTag"
        private const val SECOND = 1000L

        private enum class AuthMfaViewModelState {
            IDLE, VERIFY_OTP, VERIFY_LOGIN
        }
    }

    override val isAuthorizationActive: Boolean = false

    private val verifyOtpCodeUseCase: MfaVerifyOtpCodeUseCase by inject()
    private val verifyLoginUseCase: VerifyLoginUseCase by inject()

    private val _enableAuthenticationButtonLiveData = MutableLiveData(false)
    val enableAuthenticationButtonLiveData = _enableAuthenticationButtonLiveData.readOnly()

    private val _countDownTimeLeftLiveData = SingleLiveEvent<Long>()
    val countDownTimeLeftLiveData = _countDownTimeLeftLiveData.readOnly()

    private var otpCode = ""

    private lateinit var sessionId: String
    private lateinit var email: String
    private lateinit var password: String
    private var ttl = 0

    private var isTimerStarted = false

    private var state = AuthMfaViewModelState.IDLE

    private var countDownTimer: CountDownTimer? = null

    override fun onBackPressed() {
        logDebug("onBackPressed", TAG)

        when {
            findFlowNavController().isFragmentInBackStack(R.id.authEnterEmailPasswordFragment) -> popBackStackToFragment(
                R.id.authEnterEmailPasswordFragment
            )

            else -> popBackStackToFragment(R.id.authStartFragment)
        }
    }

    override fun onDestroyed() {
        countDownTimer?.cancel()
        isTimerStarted = false
    }

    fun onOtpCodeChanged(text: String) {
        otpCode = text
        validateInput()
    }

    fun onAuthenticateClicked() {
        logDebug("onAuthenticateClicked otp code: $otpCode", TAG)
        verifyOtpCode()
    }

    fun restartLastAction() {
        when (state) {
            AuthMfaViewModelState.VERIFY_OTP -> verifyOtpCode()
            AuthMfaViewModelState.VERIFY_LOGIN -> verifyLogin()
            else -> {}
        }
    }

    fun setupModel(sessionId: String, email: String, password: String, ttl: Int) {
        this.sessionId = sessionId
        this.email = email
        this.password = password
        this.ttl = ttl
    }

    fun startTimer() {
        if (isTimerStarted || ttl <= 0) return

        countDownTimer?.cancel()
        countDownTimer = object : CountDownTimer(ttl * SECOND, SECOND) {
            override fun onFinish() {
                _countDownTimeLeftLiveData.setValueOnMainThread(0)
            }


            override fun onTick(millisUntilFinished: Long) {
                _countDownTimeLeftLiveData.setValueOnMainThread(millisUntilFinished / SECOND)
            }

        }
        countDownTimer?.start()
        isTimerStarted = true
    }

    private fun validateInput() {
        val isOtpCodeValid = isOtpCodeValid()
        _enableAuthenticationButtonLiveData.setValueOnMainThread(isOtpCodeValid)
    }

    private fun isOtpCodeValid(): Boolean {
        return otpCode.length == OTP_CODE_LENGTH
    }

    private fun verifyOtpCode() {
        logDebug("verifyOtpCode code: $otpCode", TAG)
        state = AuthMfaViewModelState.VERIFY_OTP
        verifyOtpCodeUseCase.invoke(
            sessionId = sessionId,
            otpCode = otpCode,
            email = email,
            password = password
        ).onEach { result ->
            result.onLoading {
                logDebug("onAuthenticateClicked onLoading", TAG)
                showLoader()
            }.onSuccess { model, _, _ ->
                logDebug("onAuthenticateClicked onSuccess", TAG)
                when (model) {
                    is AuthenticationType.Token -> verifyLogin()
                    else -> hideLoader()
                }
            }.onFailure { _, _, message, responseCode, errorType ->
                logError("onAuthenticateClicked onFailure", message, TAG)
                hideLoader()
                when (errorType) {
                    ErrorType.NO_INTERNET_CONNECTION -> showErrorState(
                        title = StringSource(R.string.error_network_not_available),
                        description = StringSource(R.string.error_network_not_available_description),
                    )

                    else -> {
                        val bannerMessage = message?.let {
                            StringSource(
                                "$it (%s)",
                                formatArgs = listOf(responseCode.toString())
                            )
                        } ?: run {
                            StringSource(
                                R.string.error_api_general,
                                formatArgs = listOf(responseCode.toString())
                            )
                        }

                        showMessage(BannerMessage.error(bannerMessage))
                    }
                }
            }
        }.launchInScope(viewModelScope)
    }

    private fun verifyLogin() {
        logDebug("verifyLogin", TAG)
        state = AuthMfaViewModelState.VERIFY_LOGIN
        verifyLoginUseCase.invoke().onEach { result ->
            result.onLoading {
                logDebug("onNextClicked onLoading", TAG)
            }.onSuccess { model, _, _ ->
                logDebug("onNextClicked onSuccess", TAG)
                val tabId = when (model.tabToOpen) {
                    0 -> R.id.nav_main_tab_home
                    else -> R.id.nav_main_tab_requests
                }

                navigateNext(tabId = tabId)
            }.onFailure { _, _, message, responseCode, errorType ->
                logError("onNextClicked onFailure", message, TAG)
                hideLoader()
                when (errorType) {
                    ErrorType.NO_INTERNET_CONNECTION -> showErrorState(
                        title = StringSource(R.string.error_network_not_available),
                        description = StringSource(R.string.error_network_not_available_description),
                    )

                    else -> {
                        val bannerMessage = message?.let {
                            StringSource(
                                "$it (%s)",
                                formatArgs = listOf(responseCode.toString())
                            )
                        } ?: run {
                            StringSource(
                                R.string.error_api_general,
                                formatArgs = listOf(responseCode.toString())
                            )
                        }

                        showMessage(BannerMessage.error(bannerMessage))
                    }
                }
            }
        }.launchInScope(viewModelScope)
    }

    private fun navigateNext(@IdRes tabId: Int) {
        logDebug("navigateNext", TAG)
        navigateNewRootInActivity(
            NavActivityDirections.toMainTabsFlowFragment(tabId = tabId)
        )
        viewModelScope.launch {
            delay(DELAY_500)
            hideLoader()
        }
    }
}