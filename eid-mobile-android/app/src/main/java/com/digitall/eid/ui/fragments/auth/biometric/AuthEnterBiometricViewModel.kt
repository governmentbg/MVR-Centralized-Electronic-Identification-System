/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.auth.biometric

import androidx.lifecycle.viewModelScope
import com.digitall.eid.NavActivityDirections
import com.digitall.eid.R
import com.digitall.eid.domain.DELAY_500
import com.digitall.eid.domain.utils.CryptographyHelper
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.extensions.readOnly
import com.digitall.eid.extensions.setValueOnMainThread
import com.digitall.eid.models.common.BannerMessage
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.ui.BaseViewModel
import com.digitall.eid.utils.SingleLiveEvent
import kotlinx.coroutines.delay
import kotlinx.coroutines.launch
import org.koin.core.component.inject
import javax.crypto.Cipher

class AuthEnterBiometricViewModel : BaseViewModel() {

    companion object {
        private const val TAG = "EnterBiometricViewModelTag"
    }

    override val isAuthorizationActive: Boolean = false

    private val cryptographyHelper: CryptographyHelper by inject()

    private val _startBiometricAuthLiveData = SingleLiveEvent<Cipher>()
    val startBiometricAuthLiveData = _startBiometricAuthLiveData.readOnly()

    override fun onBackPressed() {
        logDebug("onBackPressed", TAG)
        popBackStack()
    }

    override fun onFirstAttach() {
        logDebug("onFirstAttach", TAG)
    }

    fun onLanguageCLicked() {
        logDebug("onLanguageCLicked", TAG)

    }

    fun startBiometricAuth() {
        logDebug("startBiometricAuth", TAG)
        val pinCode = preferences.readApplicationInfo()
        if (pinCode == null) {
            logError("startBiometricAuth pinCode == null", TAG)
            showMessage(BannerMessage.error(StringSource(R.string.error_pin_code_not_setup)))
            logout()
            return
        }
        val cipher = getBiometricCipherForDecryption()
        if (cipher == null) {
            logError("startBiometricAuth cipher == null", TAG)
            showMessage(BannerMessage.error(StringSource("Error use biometric")))
            toEnterPinFragment(true)
            return
        }
        _startBiometricAuthLiveData.setValueOnMainThread(cipher)
    }

    fun onBiometricSuccess(cipher: Cipher?) {
        logDebug("onBiometricSuccess", TAG)
        val pinCode = preferences.readApplicationInfo()
        if (pinCode == null) {
            logError("onBiometricSuccess pinCode == null", TAG)
            showMessage(BannerMessage.error(StringSource(R.string.error_pin_code_not_setup)))
            logout()
            return
        }
        if (cipher == null) {
            logError("onBiometricSuccess cipher == null", TAG)
            showMessage(BannerMessage.error(StringSource("Error use biometric")))
            toEnterPinFragment(true)
            return
        }
        try {
            val decryptedPin = cryptographyHelper.decrypt(pinCode.applicationPin!!, cipher)
            if (decryptedPin.isEmpty()) {
                logError("onBiometricSuccess decryptedPin.isEmpty()", TAG)
                showMessage(BannerMessage.error(StringSource("Error use biometric")))
                toEnterPinFragment(true)
                return
            }
            if (decryptedPin != pinCode.applicationPin) {
                logError("onBiometricSuccess decryptedPin != pinCode.decryptedPin", TAG)
                showMessage(BannerMessage.error(StringSource("Error use biometric")))
                toEnterPinFragment(true)
                return
            }
            onCodeLocalCheckSuccess()
        } catch (e: Exception) {
            logError(e, TAG)
            showMessage(BannerMessage.error(StringSource("Error enable biometric")))
            toEnterPinFragment(true)
        }
    }

    private fun onCodeLocalCheckSuccess() {
        logDebug("onCodeLocalCheckSuccess", TAG)
//        authorizationEnterToAccountUseCase.enterToAccount(
//            email = email,
//            personalIdentificationNumber = personalIdentificationNumber,
//        ).onEach { result ->
//            result.onLoading {
//                logDebug("onCodeLocalCheckSuccess onLoading", TAG)
//                showLoader()
//            }.onSuccess {
//                logDebug("onCodeLocalCheckSuccess onSuccess", TAG)
//                navigateNext()
//            }.onFailure {
//                logError("onCodeLocalCheckSuccess onFailure", it, TAG)
//                showBannerMessage(BannerMessage.error(R.string.auth_enter_pin_error_check_remote))
//                toEnterPinFragment(true)
//            }
//        }.launchInScope(viewModelScope)
    }

    fun onBiometricTooManyAttempts() {
        logError("onBiometricTooManyAttempts", TAG)
        showMessage(BannerMessage.error(StringSource(R.string.auth_biometric_scanner_many_attempts)))
        toEnterPinFragment(true)
    }

    fun onBiometricError() {
        logError("onBiometricError", TAG)
        showMessage(BannerMessage.error(StringSource(R.string.auth_biometric_scanner_failed)))
        toEnterPinFragment(true)
    }

    fun onBiometricException() {
        logError("onBiometricException", TAG)
        showMessage(BannerMessage.error(StringSource("Error use biometric")))
        toEnterPinFragment(true)
    }

    private fun navigateNext() {
        logDebug("toMainTabs", TAG)
        navigateNewRootInActivity(
            NavActivityDirections.toMainTabsFlowFragment(tabId = R.id.nav_main_tab_home)
        )
        viewModelScope.launch {
            delay(DELAY_500)
            hideLoader()
        }
    }

    fun toEnterPinFragment(forceDisableBiometric: Boolean) {
        logDebug("toEnterPinFragment forceDisableBiometric: $forceDisableBiometric", TAG)
        navigateInFlow(AuthEnterBiometricFragmentDirections.toAuthEnterEmailPasswordFragment())
        viewModelScope.launch {
            delay(DELAY_500)
            hideLoader()
        }
    }
}