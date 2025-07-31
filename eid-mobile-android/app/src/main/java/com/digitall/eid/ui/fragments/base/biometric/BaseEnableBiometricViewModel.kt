/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.base.biometric

import com.digitall.eid.NavActivityDirections
import com.digitall.eid.R
import com.digitall.eid.domain.utils.CryptographyHelper
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.models.common.BannerMessage
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.ui.BaseViewModel
import com.digitall.eid.utils.SupportBiometricManager
import org.koin.core.component.inject
import javax.crypto.Cipher

abstract class BaseEnableBiometricViewModel : BaseViewModel(
) {

    companion object {
        private const val TAG = "BaseEnableBiometricViewModelTag"
    }

    private val cryptographyHelper: CryptographyHelper by inject()
    private val biometricManager: SupportBiometricManager by inject()

    abstract fun navigateNext()

    fun startBiometric() {
        logDebug("startBiometric", TAG)
        val cipher = cryptographyHelper.getBiometricCipherForEncryption()
        if (cipher == null) {
            logError("startBiometric cipher == null", TAG)
            navigateInActivity(NavActivityDirections.toBiometricErrorBottomSheetFragment())
            return
        }
        try {
            biometricManager.authenticate(cipher)
        } catch (e: Exception) {
            logError("handleStartBiometricAuth exception: ${e.message}", e, TAG)
            showMessage(BannerMessage.error(StringSource("Error use biometric")))
            setupLater()
        }
    }

    fun setupLater() {
        logDebug("setupLater", TAG)
        disableBiometric()
        navigateNext()
    }

    fun onBiometricTooManyAttempts() {
        logError("onBiometricTooManyAttempts", TAG)
        showMessage(BannerMessage.error(StringSource(R.string.auth_biometric_scanner_many_attempts)))
        setupLater()
    }

    fun onBiometricError() {
        logError("onBiometricError", TAG)
        showMessage(BannerMessage.error(StringSource(R.string.auth_biometric_scanner_failed)))
        setupLater()
    }

    fun onBiometricSuccess(cipher: Cipher?) {
        logDebug("confirmFingerprint", TAG)
        val pinCode = preferences.readApplicationInfo()
        if (pinCode == null) {
            logError("confirmFingerprint pinCode == null", TAG)
            showMessage(BannerMessage.error(StringSource(R.string.error_pin_code_not_setup)))
            logout()
            return
        }
        if (cipher == null) {
            logError("confirmFingerprint cipher == null", TAG)
            showMessage(BannerMessage.error(StringSource("Error enable biometric")))
            disableBiometric()
            navigateNext()
            return
        }
        try {
            val encryptedPin = cryptographyHelper.encrypt(pinCode.applicationPin!!, cipher)
            logDebug(
                "confirmFingerprint pinCode.applicationPin: ${pinCode.applicationPin}\n" +
                        "encryptedPin: $encryptedPin", TAG
            )
            if (encryptedPin.isEmpty()) {
                logError("confirmFingerprint encryptedPin.isEmpty()", TAG)
                showMessage(BannerMessage.error(StringSource("Error enable biometric")))
                disableBiometric()
                navigateNext()
                return
            }
            enableBiometric(encryptedPin)
            navigateNext()
        } catch (e: Exception) {
            logError(e, TAG)
            showMessage(BannerMessage.error(StringSource("Error enable biometric")))
            disableBiometric()
            navigateNext()
        }
    }

    override fun onBackPressed() {
        logDebug("onBackPressed", TAG)
        disableBiometric()
        navigateNext()
    }

}