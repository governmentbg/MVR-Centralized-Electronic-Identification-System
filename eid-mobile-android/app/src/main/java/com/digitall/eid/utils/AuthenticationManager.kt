package com.digitall.eid.utils

import android.app.Application
import android.content.Context
import android.content.SharedPreferences
import android.security.keystore.UserNotAuthenticatedException
import androidx.biometric.BiometricManager
import androidx.biometric.BiometricPrompt
import androidx.core.content.ContextCompat
import androidx.fragment.app.FragmentActivity
import com.digitall.eid.data.BuildConfig.PROPERTY_KEY_BIOMETRICS_ENABLED_FOR_UNLOCK
import com.digitall.eid.domain.models.common.ApplicationPin
import com.digitall.eid.domain.repository.common.PreferencesRepository
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.extensions.readOnly
import com.digitall.eid.extensions.toByteArray
import com.digitall.eid.extensions.toHex
import com.digitall.eid.models.common.AuthenticationResult
import com.digitall.eid.models.common.StringSource
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.withContext
import org.koin.core.component.KoinComponent
import org.koin.core.component.inject
import java.security.SecureRandom
import java.util.concurrent.Executor
import javax.crypto.SecretKeyFactory
import javax.crypto.spec.PBEKeySpec
import androidx.core.content.edit
import com.digitall.eid.R
import com.digitall.eid.data.BuildConfig.PROPERTY_KEY_APP_PIN_PROMPT
import com.digitall.eid.domain.models.common.ApplicationCredentials
import com.digitall.eid.domain.utils.CryptographyHelper
import com.digitall.eid.extensions.setValueOnMainThread
import com.digitall.eid.models.common.AuthenticationType
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.launch

class AuthenticationManager(private val application: Application) : KoinComponent {

    companion object {
        private const val TAG = "AuthenticationManagerTag"
        private const val SP_NAME = "__eid_encrypted_data"

        private const val MAX_PIN_ATTEMPTS = 3

        private const val GCM_IV_LENGTH_BYTES = 12

        private const val PIN_HASH_ITERATIONS = 100000
        private const val PIN_HASH_KEY_LENGTH = 256
        private const val PIN_SALT_LENGTH = 16
    }

    private val preferences: PreferencesRepository by inject()
    private val currentContext: CurrentContext by inject()
    private val cryptographyHelper: CryptographyHelper by inject()

    private val _authenticationResultEvent =
        LatestObserverSingleLiveEvent<AuthenticationResult?>(null)
    val authenticationResultEvent = _authenticationResultEvent.readOnly()

    private val _authenticationTypeEvent =
        SingleLiveEvent<AuthenticationType?>(null)
    val authenticationTypeEvent = _authenticationTypeEvent.readOnly()

    private lateinit var mainThreadExecutor: Executor
    private lateinit var biometricPrompt: BiometricPrompt
    private var biometricPromptInfo: BiometricPrompt.PromptInfo? = null

    private var currentPinAttempts = 0

    private val sharedPreferences: SharedPreferences by lazy {
        application.getSharedPreferences(SP_NAME, Context.MODE_PRIVATE)
    }

    fun initialize(activity: FragmentActivity) {
        mainThreadExecutor = ContextCompat.getMainExecutor(activity)
        biometricPromptInfo = BiometricPrompt.PromptInfo.Builder()
            .setTitle(currentContext.getString(R.string.biometrics_prompt_title))
            .setNegativeButtonText(currentContext.getString(R.string.biometrics_prompt_negative_button_title))
            .setConfirmationRequired(false)
            .build()

        biometricPrompt = BiometricPrompt(
            activity, mainThreadExecutor,
            object : BiometricPrompt.AuthenticationCallback() {
                override fun onAuthenticationError(errorCode: Int, errString: CharSequence) {
                    super.onAuthenticationError(errorCode, errString)
                    when (errorCode) {
                        BiometricPrompt.ERROR_USER_CANCELED -> _authenticationResultEvent.setValueOnMainThread(
                            AuthenticationResult.Cancelled
                        )

                        BiometricPrompt.ERROR_NEGATIVE_BUTTON -> {
                            resetPinAttempts()
                            _authenticationTypeEvent.setValueOnMainThread(AuthenticationType.PIN)
                            _authenticationResultEvent.setValueOnMainThread(AuthenticationResult.FallbackToPin)
                        }

                        else -> {
                            resetPinAttempts()
                            _authenticationTypeEvent.setValueOnMainThread(AuthenticationType.PIN)
                            _authenticationResultEvent.setValueOnMainThread(AuthenticationResult.FallbackToPin)
                        }
                    }
                }

                override fun onAuthenticationSucceeded(result: BiometricPrompt.AuthenticationResult) {
                    super.onAuthenticationSucceeded(result)
                    logDebug("Biometric authentication succeeded.", TAG)
                    CoroutineScope(Dispatchers.IO).launch {
                        preferences.readCredentials()?.let {
                            _authenticationResultEvent.setValueOnMainThread(
                                AuthenticationResult.Success(
                                    credentials = it
                                )
                            )
                        }
                    }
                }

                override fun onAuthenticationFailed() {
                    super.onAuthenticationFailed()
                    logDebug(
                        "Biometric authentication failed (unrecognized). Prompt may allow retry.",
                        TAG
                    )
                }
            })
    }

    fun getInitialAuthenticationState() {
        resetPinAttempts()
        val applicationCredentials = preferences.readCredentials()

        applicationCredentials?.let {
            when {
                (isBiometricsEnabledForUnlock() && isBiometricsAvailable()) -> {
                    cryptographyHelper.getBiometricCipherForDecryption(
                        initializationVector = ByteArray(
                            GCM_IV_LENGTH_BYTES
                        )
                    )?.let {
                        _authenticationTypeEvent.setValueOnMainThread(AuthenticationType.BIOMETRICS)
                    } ?: run {
                        if (isApplicationPinEnabled()) {
                            _authenticationTypeEvent.setValueOnMainThread(AuthenticationType.PIN)
                        } else {
                            _authenticationTypeEvent.setValueOnMainThread(AuthenticationType.PASSWORD)
                        }
                    }
                }

                isApplicationPinEnabled() -> _authenticationTypeEvent.setValueOnMainThread(
                    AuthenticationType.PIN
                )

                else -> _authenticationTypeEvent.setValueOnMainThread(AuthenticationType.NONE)
            }
        } ?: run {
            _authenticationTypeEvent.setValueOnMainThread(AuthenticationType.NONE)
        }
    }

    fun hasBeenUserPromptForPinCreation() = sharedPreferences.contains(PROPERTY_KEY_APP_PIN_PROMPT)

    fun setUserBeenPromptForPinCreation() =
        sharedPreferences.edit { putBoolean(PROPERTY_KEY_APP_PIN_PROMPT, true) }

    fun requestBiometricUnlock() {
        if (isBiometricsEnabledForUnlock().not() || isBiometricsAvailable().not()) {
            if (isApplicationPinEnabled()) {
                resetPinAttempts()
                _authenticationTypeEvent.setValueOnMainThread(AuthenticationType.PIN)
                _authenticationResultEvent.setValueOnMainThread(AuthenticationResult.FallbackToPin)
            } else {
                _authenticationTypeEvent.setValueOnMainThread(AuthenticationType.PASSWORD)
                _authenticationResultEvent.setValueOnMainThread(AuthenticationResult.FallbackToPassword)
            }
            return
        }

        if (::biometricPrompt.isInitialized.not() || biometricPromptInfo == null) {
            _authenticationResultEvent.setValueOnMainThread(
                AuthenticationResult.Failure(
                    StringSource(R.string.error_biometrics_system)
                )
            )
            return
        }

        try {
            val cipher = cryptographyHelper.getBiometricCipherForDecryption(
                initializationVector = ByteArray(GCM_IV_LENGTH_BYTES)
            )
            if (cipher == null) {
                setBiometricsEnabledForUnlock(false)
                if (isApplicationPinEnabled()) {
                    resetPinAttempts()
                    _authenticationTypeEvent.setValueOnMainThread(AuthenticationType.PIN)
                    _authenticationResultEvent.setValueOnMainThread(AuthenticationResult.FallbackToPin)
                } else {
                    _authenticationTypeEvent.setValueOnMainThread(AuthenticationType.PASSWORD)
                    _authenticationResultEvent.setValueOnMainThread(AuthenticationResult.FallbackToPassword)
                }
                return
            }

            biometricPromptInfo?.let { biometricPromptInfo ->
                biometricPrompt.authenticate(
                    biometricPromptInfo,
                    BiometricPrompt.CryptoObject(cipher)
                )
            }
        } catch (e: UserNotAuthenticatedException) {
            if (isApplicationPinEnabled()) {
                resetPinAttempts()
                _authenticationTypeEvent.setValueOnMainThread(AuthenticationType.PIN)
                _authenticationResultEvent.setValueOnMainThread(AuthenticationResult.FallbackToPin)
            } else {
                _authenticationTypeEvent.setValueOnMainThread(AuthenticationType.PASSWORD)
                _authenticationResultEvent.setValueOnMainThread(AuthenticationResult.FallbackToPassword)
            }
        } catch (e: Exception) {
            if (isApplicationPinEnabled()) {
                resetPinAttempts()
                _authenticationTypeEvent.setValueOnMainThread(AuthenticationType.PIN)
                _authenticationResultEvent.setValueOnMainThread(AuthenticationResult.FallbackToPin)
            } else {
                _authenticationTypeEvent.setValueOnMainThread(AuthenticationType.PASSWORD)
                _authenticationResultEvent.setValueOnMainThread(AuthenticationResult.FallbackToPassword)
            }
        }
    }

    fun saveCredentials(credentials: ApplicationCredentials) =
        preferences.saveCredentials(credentials = credentials)

    fun isBiometricsEnabledForUnlock() = sharedPreferences.getBoolean(
        PROPERTY_KEY_BIOMETRICS_ENABLED_FOR_UNLOCK, false
    )

    fun isBiometricsAvailable() = BiometricManager.from(currentContext.get())
        .canAuthenticate(BiometricManager.Authenticators.BIOMETRIC_STRONG) == BiometricManager.BIOMETRIC_SUCCESS

    fun setBiometricsEnabledForUnlock(enabled: Boolean) {
        if (enabled) {
            if (isBiometricsAvailable().not()) {
                _authenticationResultEvent.setValueOnMainThread(
                    AuthenticationResult.Failure(
                        message = StringSource(
                            R.string.error_biometrics_not_available
                        )
                    )
                )
                return
            }

            try {
                cryptographyHelper.getBiometricCipherForEncryption()
                sharedPreferences.edit {
                    putBoolean(
                        PROPERTY_KEY_BIOMETRICS_ENABLED_FOR_UNLOCK,
                        true
                    )
                }
            } catch (exception: Exception) {
                sharedPreferences.edit {
                    putBoolean(
                        PROPERTY_KEY_BIOMETRICS_ENABLED_FOR_UNLOCK,
                        false
                    )
                }
                _authenticationResultEvent.setValueOnMainThread(
                    AuthenticationResult.Failure(
                        message = StringSource(
                            R.string.error_biometrics_enable
                        )
                    )
                )
            }
        } else {
            sharedPreferences.edit { putBoolean(PROPERTY_KEY_BIOMETRICS_ENABLED_FOR_UNLOCK, false) }
        }
    }

    fun isApplicationPinEnabled() = preferences.readApplicationPin() != null

    fun clearApplicationPin() = preferences.removeApplicationPin()

    suspend fun setupApplicationPin(pin: String): Boolean {
        return withContext(Dispatchers.IO) {
            try {
                val salt = generateSaltForPin()
                val hashPin = hashPin(pin = pin, salt = salt)
                preferences.saveApplicationPin(
                    pin = ApplicationPin(
                        hashPin.toHex(),
                        salt.toHex(),
                    )
                )
                true
            } catch (exception: Exception) {
                logError("Error setting up Application Pin", exception, TAG)
                false
            }
        }
    }

    suspend fun verifyApplicationPin(enteredPin: String) {
        if (isApplicationPinEnabled().not()) {
            _authenticationResultEvent.setValueOnMainThread(
                AuthenticationResult.Failure(
                    StringSource("App PIN not set up."),
                    requiresFullReAuth = true
                )
            )
            return
        }

        currentPinAttempts += 1
        withContext(Dispatchers.IO) {
            val storedApplicationPin =
                preferences.readApplicationPin() ?: run { return@withContext }

            try {
                val salt = storedApplicationPin.salt.toByteArray()
                val enteredHash = hashPin(pin = enteredPin, salt = salt)
                if (enteredHash.toHex() == storedApplicationPin.hash) {
                    resetPinAttempts()
                    preferences.readCredentials()?.let {
                        _authenticationResultEvent.setValueOnMainThread(
                            AuthenticationResult.Success(
                                credentials = it
                            )
                        )
                    }
                } else {
                    if (currentPinAttempts >= MAX_PIN_ATTEMPTS) {
                        _authenticationTypeEvent.setValueOnMainThread(AuthenticationType.PASSWORD)
                        _authenticationResultEvent.setValueOnMainThread(AuthenticationResult.FallbackToPassword)
                    } else {
                        _authenticationResultEvent.setValueOnMainThread(
                            AuthenticationResult.Failure(
                                StringSource(
                                    R.string.error_pin_code_incorrect, formatArgs = listOf(
                                        (MAX_PIN_ATTEMPTS - currentPinAttempts).toString()
                                    )
                                )
                            )
                        )
                    }
                }
            } catch (exception: Exception) {
                logError("Error verifying Application Pin", exception, TAG)
            }
        }
    }

    private fun resetPinAttempts() {
        currentPinAttempts = 0
    }

    private fun generateSaltForPin(): ByteArray {
        val random = SecureRandom()
        val salt = ByteArray(PIN_SALT_LENGTH)
        random.nextBytes(salt)
        return salt
    }

    private fun hashPin(pin: String, salt: ByteArray): ByteArray {
        val spec = PBEKeySpec(pin.toCharArray(), salt, PIN_HASH_ITERATIONS, PIN_HASH_KEY_LENGTH)
        // Ensure you use a strong algorithm like SHA256 or SHA512 for PBKDF2
        val factory = SecretKeyFactory.getInstance("PBKDF2WithHmacSHA256")
        return factory.generateSecret(spec).encoded
    }

}