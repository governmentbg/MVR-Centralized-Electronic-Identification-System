package com.digitall.eid.ui.fragments.certificates.enter.pin

import android.util.Base64
import androidx.annotation.IdRes
import androidx.lifecycle.viewModelScope
import com.digitall.eid.NavActivityDirections
import com.digitall.eid.R
import com.digitall.eid.domain.DELAY_500
import com.digitall.eid.domain.EID_MOBILE_CERTIFICATE
import com.digitall.eid.domain.EID_MOBILE_CERTIFICATE_KEYS
import com.digitall.eid.domain.extensions.readOnly
import com.digitall.eid.domain.models.base.ErrorType
import com.digitall.eid.domain.models.base.onFailure
import com.digitall.eid.domain.models.base.onLoading
import com.digitall.eid.domain.models.base.onSuccess
import com.digitall.eid.domain.models.common.LevelOfAssurance
import com.digitall.eid.domain.usecase.authentication.AuthenticationGenerateChallengeUseCase
import com.digitall.eid.domain.usecase.authentication.AuthenticationWithCertificateUseCase
import com.digitall.eid.domain.usecase.verify.login.VerifyLoginUseCase
import com.digitall.eid.domain.utils.CryptographyHelper
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.extensions.launchInScope
import com.digitall.eid.extensions.launchWithDispatcher
import com.digitall.eid.models.common.BannerMessage
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.ui.BaseViewModel
import kotlinx.coroutines.delay
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.onEach
import kotlinx.coroutines.launch
import org.bouncycastle.asn1.ASN1Integer
import org.bouncycastle.asn1.ASN1Sequence
import org.bouncycastle.crypto.signers.PlainDSAEncoding
import org.koin.core.component.inject
import java.math.BigInteger
import java.security.Signature
import java.security.interfaces.ECKey
import java.security.interfaces.ECPublicKey
import java.security.interfaces.RSAKey
import java.security.interfaces.RSAPublicKey
import java.security.spec.MGF1ParameterSpec
import java.security.spec.PSSParameterSpec


class CertificateEnterPinViewModel : BaseViewModel() {

    companion object {
        private const val TAG = "CertificateEnterPinViewModelTag"
        private const val PIN_CODE_LENGTH = 6
    }

    private val _enableLoginStateLiveData = MutableStateFlow(false)
    val enableLoginStateLiveData = _enableLoginStateLiveData.readOnly()

    private val authenticationGenerateChallengeUseCase: AuthenticationGenerateChallengeUseCase by inject()
    private val authenticationWithCertificateUseCase: AuthenticationWithCertificateUseCase by inject()
    private val verifyLoginUseCase: VerifyLoginUseCase by inject()

    private val cryptographyHelper: CryptographyHelper by inject()

    private lateinit var pin: String

    override fun onBackPressed() {
        logDebug("onBackPressed", TAG)
        popBackStackToFragment(R.id.authStartFragment)
    }

    fun onPinEntered(pin: String) {
        logDebug("onPinEntered pin: $pin", TAG)
        this.pin = pin
        val certificatePin = preferences.readApplicationInfo()?.certificatePin

        viewModelScope.launchWithDispatcher {
            when {
                pin.length == PIN_CODE_LENGTH -> _enableLoginStateLiveData.emit(true)
                pin != certificatePin -> showMessage(BannerMessage.error(StringSource(R.string.certificate_enter_pin_pin_mismatch)))
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
        authenticationGenerateChallengeUseCase.invoke(levelOfAssurance = LevelOfAssurance.SUBSTANTIAL)
            .onEach { result ->
                result.onLoading {
                    logDebug("generateChallenge onLoading", TAG)
                    showLoader()
                    hideErrorState()
                }.onSuccess { model, _, _ ->
                    logDebug("generateChallenge onSuccess", TAG)
                    val signedChallenge =
                        signChallenge(challenge = model.challenge ?: return@onSuccess)

                    signedChallenge?.let { challenge ->
                        val certificate =
                            cryptographyHelper.getCertificate(alias = EID_MOBILE_CERTIFICATE)
                        val certificateChain =
                            cryptographyHelper.getCertificateChain(
                                alias = EID_MOBILE_CERTIFICATE,
                                chainCount = 2
                            )

                        val signature = when (certificate?.publicKey) {
                            is ECPublicKey ->
                                toP1363(
                                    encodedSignature = challenge,
                                    order = (certificate.publicKey as ECPublicKey).params.order
                                )

                            is RSAPublicKey -> challenge

                            else -> {
                                showMessage(BannerMessage.error(StringSource("Unsupported key of type \"${certificate?.publicKey?.javaClass}\"")))
                                null
                            }
                        }

                        authenticateWithCertificate(
                            signature = Base64.encodeToString(signature, Base64.NO_WRAP),
                            challenge = model.challenge,
                            certificate = Base64.encodeToString(
                                certificate?.encoded,
                                Base64.NO_WRAP
                            ),
                            certificateChain = certificateChain.map {
                                Base64.encodeToString(
                                    it.encoded,
                                    Base64.NO_WRAP
                                )
                            }
                        )
                    } ?: run {
                        showMessage(BannerMessage.error(StringSource("Couldn't generate a signature")))
                    }


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

    private fun authenticateWithCertificate(
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

    private fun signChallenge(challenge: String): ByteArray? {
        val privateKey = cryptographyHelper.getPrivateKey(alias = EID_MOBILE_CERTIFICATE_KEYS)
        return privateKey?.let { key ->
            val signer = when (key) {
                is ECKey -> Signature.getInstance("SHA256WithECDSA")
                is RSAKey -> Signature.getInstance("SHA256withRSA/PSS").apply {
                    setParameter(
                        PSSParameterSpec("SHA-256", "MGF1", MGF1ParameterSpec.SHA256, 32, 1)
                    )
                }

                else -> throw UnsupportedOperationException("Key not supported")
            }

            signer.initSign(key)
            signer.update(challenge.toByteArray())
            signer.sign()
        }
    }

    private fun toP1363(encodedSignature: ByteArray, order: BigInteger): ByteArray {
        val seq = ASN1Sequence.getInstance(encodedSignature)
        val r = (seq.getObjectAt(0) as ASN1Integer).value
        val s = (seq.getObjectAt(1) as ASN1Integer).value
        return PlainDSAEncoding.INSTANCE.encode(order, r, s)
    }
}