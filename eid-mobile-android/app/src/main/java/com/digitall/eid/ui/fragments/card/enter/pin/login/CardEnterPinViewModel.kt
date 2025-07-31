package com.digitall.eid.ui.fragments.card.enter.pin.login

import androidx.annotation.IdRes
import androidx.lifecycle.viewModelScope
import com.digitall.eid.NavActivityDirections
import com.digitall.eid.R
import com.digitall.eid.domain.DELAY_500
import com.digitall.eid.domain.extensions.readOnly
import com.digitall.eid.domain.models.base.ErrorType
import com.digitall.eid.domain.models.base.onFailure
import com.digitall.eid.domain.models.base.onLoading
import com.digitall.eid.domain.models.base.onSuccess
import com.digitall.eid.domain.models.common.LevelOfAssurance
import com.digitall.eid.domain.usecase.authentication.AuthenticationGenerateChallengeUseCase
import com.digitall.eid.domain.usecase.authentication.AuthenticationWithCertificateUseCase
import com.digitall.eid.domain.usecase.verify.login.VerifyLoginUseCase
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.extensions.launchInScope
import com.digitall.eid.extensions.launchWithDispatcher
import com.digitall.eid.extensions.readOnly
import com.digitall.eid.extensions.setValueOnMainThread
import com.digitall.eid.models.common.BannerMessage
import com.digitall.eid.models.common.CardScanBottomSheetContent
import com.digitall.eid.models.common.CardScanContentType
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.ui.BaseViewModel
import com.digitall.eid.utils.SingleLiveEvent
import kotlinx.coroutines.delay
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.onEach
import kotlinx.coroutines.launch
import org.koin.core.component.inject

class CardEnterPinViewModel : BaseViewModel() {

    companion object {
        private const val TAG = "CardEnterPinViewModelTag"
        private const val PIN_CODE_LENGTH = 6

        enum class CardEnterPinType {
            PIN,
            PINCAN
        }
    }

    private val _enableLoginStateLiveData = MutableStateFlow(false)
    val enableLoginStateLiveData = _enableLoginStateLiveData.readOnly()

    private val _cardScanningLiveData = SingleLiveEvent<CardScanBottomSheetContent>()
    val cardScanningLiveData = _cardScanningLiveData.readOnly()

    private val authenticationGenerateChallengeUseCase: AuthenticationGenerateChallengeUseCase by inject()
    private val authenticationWithCertificateUseCase: AuthenticationWithCertificateUseCase by inject()
    private val verifyLoginUseCase: VerifyLoginUseCase by inject()

    var state = CardEnterPinType.PIN
        set(value) {
            if (value == CardEnterPinType.PIN) {
                can = null
            }
            field = value
        }

    private lateinit var pin: String
    private var can: String? = null

    override fun onBackPressed() {
        logDebug("onBackPressed", TAG)
        popBackStackToFragment(R.id.authStartFragment)
    }

    fun onPinEntered(pin: String) {
        logDebug("onPinEntered pin: $pin", TAG)
        this.pin = pin

        viewModelScope.launchWithDispatcher {
            when {
                pin.length == PIN_CODE_LENGTH && state == CardEnterPinType.PIN -> _enableLoginStateLiveData.emit(
                    true
                )

                else -> _enableLoginStateLiveData.emit(false)
            }
        }
    }

    fun onCanEntered(can: String) {
        logDebug("onCanEntered can: $can", TAG)
        this.can = can

        viewModelScope.launchWithDispatcher {
            when {
                can.length == PIN_CODE_LENGTH -> _enableLoginStateLiveData.emit(true)
                else -> _enableLoginStateLiveData.emit(false)
            }
        }
    }

    fun onPinCleared() {
        logDebug("onPinCleared", TAG)
        viewModelScope.launchWithDispatcher {
            _enableLoginStateLiveData.emit(false)
        }
    }

    fun onLoginButtonClicked() {
        logDebug("onLoginButtonClicked", TAG)
        generateAuthenticationChallenge()
    }

    private fun generateAuthenticationChallenge() {
        authenticationGenerateChallengeUseCase.invoke(levelOfAssurance = LevelOfAssurance.HIGH)
            .onEach { result ->
                result.onLoading {
                    logDebug("generateChallenge onLoading", TAG)
                    showLoader()
                }.onSuccess { model, _, _ ->
                    logDebug("generateChallenge onSuccess", TAG)
                    delay(DELAY_500)
                    hideErrorState()
                    hideLoader()
                    _cardScanningLiveData.setValueOnMainThread(
                        CardScanBottomSheetContent(
                            type = CardScanContentType.SignChallenge(
                                cardCurrentPin = pin,
                                challenge = model.challenge,
                                cardCan = if (state == CardEnterPinType.PINCAN) can else null
                            )
                        )
                    )
                }.onFailure { _, _, message, responseCode, errorType ->
                    logError("generateChallenge onFailure", message, TAG)
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

    fun authenticateWithCertificate(
        signature: String?,
        challenge: String?,
        certificate: String?,
        certificateChain: List<String>?,
    ) {
        authenticationWithCertificateUseCase.invoke(
            signature = signature,
            challenge = challenge,
            certificate = certificate,
            certificateChain = certificateChain,
        ).onEach { result ->
            result.onLoading {
                logDebug("generateChallenge onLoading", TAG)
                showLoader()
            }.onSuccess { _, _, _ ->
                logDebug("generateChallenge onSuccess", TAG)
                verifyLogin()
            }.onFailure { _, _, message, responseCode, errorType ->
                logError("generateChallenge onFailure", message, TAG)
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
        verifyLoginUseCase.invoke().onEach { result ->
            result.onLoading {
                logDebug("onNextClicked onLoading", TAG)
            }.onSuccess { model, _, _ ->
                logDebug("onNextClicked onSuccess", TAG)
                val tabId = when (model.tabToOpen) {
                    0 -> R.id.nav_main_tab_home
                    else -> R.id.nav_main_tab_requests
                }
                delay(DELAY_500)
                hideErrorState()
                hideLoader()
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