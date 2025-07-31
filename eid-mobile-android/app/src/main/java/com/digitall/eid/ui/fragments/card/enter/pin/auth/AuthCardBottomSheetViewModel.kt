package com.digitall.eid.ui.fragments.card.enter.pin.auth

import androidx.lifecycle.viewModelScope
import com.digitall.eid.R
import com.digitall.eid.domain.DELAY_500
import com.digitall.eid.domain.extensions.readOnly
import com.digitall.eid.domain.models.base.ErrorType
import com.digitall.eid.domain.models.base.onFailure
import com.digitall.eid.domain.models.base.onLoading
import com.digitall.eid.domain.models.base.onSuccess
import com.digitall.eid.domain.models.common.LevelOfAssurance
import com.digitall.eid.domain.usecase.authentication.AuthenticationGenerateChallengeUseCase
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.extensions.launchInScope
import com.digitall.eid.extensions.launchWithDispatcher
import com.digitall.eid.extensions.readOnly
import com.digitall.eid.extensions.setValueOnMainThread
import com.digitall.eid.models.common.CardScanBottomSheetContent
import com.digitall.eid.models.common.CardScanContentType
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.ui.BaseViewModel
import com.digitall.eid.utils.SingleLiveEvent
import kotlinx.coroutines.delay
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.onEach
import org.koin.core.component.inject

class AuthCardBottomSheetViewModel : BaseViewModel() {

    companion object {
        private const val TAG = "AuthCardBottomSheetViewModelTag"
        private const val PIN_CODE_LENGTH = 6

        enum class AuthCardType {
            PIN,
            PINCAN
        }
    }

    private val authenticationGenerateChallengeUseCase: AuthenticationGenerateChallengeUseCase by inject()

    private val _enableAuthenticateStateLiveData = MutableStateFlow(false)
    val enableAuthenticateStateLiveData = _enableAuthenticateStateLiveData.readOnly()

    private val _cardScanningLiveData = SingleLiveEvent<CardScanBottomSheetContent>()
    val cardScanningLiveData = _cardScanningLiveData.readOnly()

    var state = AuthCardType.PIN
        set(value) {
            if (value == AuthCardType.PIN) {
                can = null
            }
            field = value
        }

    private var pin: String? = null
    private var can: String? = null

    override fun onBackPressed() {
        logDebug("onBackPressed", TAG)
        popBackStack()
    }

    fun onPinEntered(pin: String) {
        logDebug("onPinEntered pin: $pin", TAG)
        this.pin = pin

        viewModelScope.launchWithDispatcher {
            when {
                pin.length == PIN_CODE_LENGTH && state == AuthCardType.PIN -> _enableAuthenticateStateLiveData.emit(
                    true
                )

                else -> _enableAuthenticateStateLiveData.emit(false)
            }
        }
    }

    fun onCanEntered(can: String) {
        logDebug("onCanEntered can: $can", TAG)
        this.can = can

        viewModelScope.launchWithDispatcher {
            when {
                can.length == PIN_CODE_LENGTH -> _enableAuthenticateStateLiveData.emit(true)
                else -> _enableAuthenticateStateLiveData.emit(false)
            }
        }
    }

    fun onPinCleared() {
        logDebug("onPinCleared", TAG)
        viewModelScope.launchWithDispatcher {
            _enableAuthenticateStateLiveData.emit(false)
        }
    }

    fun onAuthenticateButtonClicked() {
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
                                cardCan = if (state == AuthCardType.PINCAN) can else null
                            )
                        )
                    )
                }.onFailure { _, title, message, responseCode, errorType ->
                    logError("generateChallenge onFailure", message, TAG)
                    hideLoader()
                    when (errorType) {
                        ErrorType.NO_INTERNET_CONNECTION -> showErrorState(
                            title = StringSource(R.string.error_network_not_available),
                            description = StringSource(R.string.error_network_not_available_description),
                        )

                        else -> showErrorState(
                            title = StringSource(R.string.information),
                            description = message?.let {
                                StringSource(
                                    "$it (%s)",
                                    formatArgs = listOf((responseCode ?: 0).toString())
                                )
                            } ?: StringSource(
                                R.string.error_api_general,
                                formatArgs = listOf((responseCode ?: 0).toString())
                            ),
                        )
                    }
                }
            }.launchInScope(viewModelScope)
    }
}