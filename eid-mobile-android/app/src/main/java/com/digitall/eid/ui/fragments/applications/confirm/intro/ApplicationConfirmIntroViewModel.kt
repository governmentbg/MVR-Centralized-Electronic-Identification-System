/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.applications.confirm.intro

import androidx.lifecycle.viewModelScope
import com.digitall.eid.R
import com.digitall.eid.domain.EID_MOBILE_CERTIFICATE
import com.digitall.eid.domain.EID_MOBILE_CERTIFICATE_KEYS
import com.digitall.eid.domain.extensions.toBase64
import com.digitall.eid.domain.models.base.ErrorType
import com.digitall.eid.domain.models.base.onFailure
import com.digitall.eid.domain.models.base.onLoading
import com.digitall.eid.domain.models.base.onSuccess
import com.digitall.eid.domain.models.csr.algorithm.CsrAlgorithm
import com.digitall.eid.domain.models.csr.principal.CsrPrincipalModel
import com.digitall.eid.domain.usecase.applications.create.ApplicationCreateConfirmWithBaseProfileUseCase
import com.digitall.eid.domain.usecase.applications.create.ApplicationCreateEnrollWithBaseProfileUseCase
import com.digitall.eid.domain.usecase.applications.create.ApplicationCreateSignWithBaseProfileUseCase
import com.digitall.eid.domain.utils.CryptographyHelper
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.extensions.biLet
import com.digitall.eid.extensions.launchInScope
import com.digitall.eid.extensions.readOnly
import com.digitall.eid.models.common.AlertDialogResult
import com.digitall.eid.models.common.DialogMessage
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.main.MainTabsEnum
import com.digitall.eid.ui.BaseViewModel
import com.digitall.eid.ui.fragments.base.BaseFragment.Companion.DIALOG_EXIT
import com.digitall.eid.utils.SingleLiveEvent
import kotlinx.coroutines.flow.onEach
import org.koin.core.component.inject
import java.security.PrivateKey

class ApplicationConfirmIntroViewModel : BaseViewModel() {

    companion object {
        private const val TAG = "ApplicationConfirmIntroViewModelTag"
    }

    override var mainTabsEnum: MainTabsEnum? = MainTabsEnum.TAB_EIM

    private val applicationCreateConfirmWithBaseProfileUseCase: ApplicationCreateConfirmWithBaseProfileUseCase by inject()
    private val applicationCreateSignWithBaseProfileUseCase: ApplicationCreateSignWithBaseProfileUseCase by inject()
    private val applicationCreateEnrollWithBaseProfileUseCase: ApplicationCreateEnrollWithBaseProfileUseCase by inject()

    private val cryptographyHelper: CryptographyHelper by inject()

    private var qrCode: String? = null
    private var certificatePrivateKey: PrivateKey? = null
    private var certificate: String? = null
    private var certificateChain: List<String>? = null

    private val _showCreatePinLiveData = SingleLiveEvent<Unit>()
    val showCreatePinLiveData = _showCreatePinLiveData.readOnly()

    fun setupModel(qrCode: String) {
        showLoader()
        this.qrCode = qrCode
        refreshScreen()
    }

    fun refreshScreen() {
        signApplicationWithBaseProfile()
    }

    fun saveCertificate(pin: String?) {
        if (pin.isNullOrEmpty()) return
        val applicationInfo = preferences.readApplicationInfo()
        applicationInfo?.let {
            preferences.saveApplicationInfo(
                it.copy(
                    certificatePin = pin,
                )
            )
        }
        cryptographyHelper.saveCertificateWithChainToKeyStore(
            alias = EID_MOBILE_CERTIFICATE,
            certificate = certificate ?: return,
            certificateChain = certificateChain ?: return
        )
        confirmApplicationWithBaseProfile(otpCode = qrCode ?: return)
    }

    private fun signApplicationWithBaseProfile() {
        val applicationInfo = preferences.readApplicationInfo()
        val firebaseToken = preferences.readFirebaseToken()
        val applicationId = applicationInfo?.mobileApplicationInstanceId
        val firebaseId = firebaseToken?.token
        applicationCreateSignWithBaseProfileUseCase.invoke(
            otpCode = qrCode ?: return,
            firebaseId = firebaseId ?: return,
            mobileApplicationInstanceId = applicationId ?: return,
        ).onEach { result ->
            result.onLoading {
                logDebug("applicationCreateSignWithBaseProfileUseCase onLoading", TAG)
                hideErrorState()
                showLoader()
            }.onSuccess { model, _, _ ->
                when {
                    model != "SIGNED" -> showErrorState(
                        title = StringSource("Server error"),
                        description = StringSource("Application not signed"),
                    )

                    else -> {
                        val userModel = preferences.readApplicationInfo()?.userModel
                        val principal = CsrPrincipalModel(
                            name = userModel?.nameCyrillic ?: "",
                            givenName = userModel?.givenName ?: "",
                            surname = userModel?.familyName ?: "",
                            country = "BG",
                            serialNumber = "PI:BG-${userModel?.eidEntityId}"
                        )
                        val (key, csrRequest) = cryptographyHelper.generateCSR(
                            keyAlias = EID_MOBILE_CERTIFICATE_KEYS,
                            algorithm = CsrAlgorithm.RSA_3072,
                            principal = principal
                        )

                        (key to csrRequest).biLet { privateKey, csr ->
                            certificatePrivateKey = privateKey
                            enrollApplicationWithBaseProfile(
                                otpCode = qrCode ?: return@biLet,
                                csr = csr.toBase64()
                            )
                        }
                    }
                }
            }.onFailure { _, title, message, responseCode, errorType ->
                logError("applicationCreateSignWithBaseProfileUseCase onFailure", message, TAG)
                hideLoader()
                when (errorType) {
                    ErrorType.AUTHORIZATION -> toLoginFragment()

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

    private fun enrollApplicationWithBaseProfile(otpCode: String, csr: String) {
        applicationCreateEnrollWithBaseProfileUseCase.invoke(
            otpCode = otpCode,
            certificateSigningRequest = csr
        ).onEach { result ->
            result.onLoading {
                logDebug("applicationCreateEnrollWithBaseProfileUseCase onLoading", TAG)
            }.onSuccess { model, _, _ ->
                certificate = model.certificate
                certificateChain = model.certificateChain
                _showCreatePinLiveData.callOnMainThread()
            }.onFailure { _, title, message, responseCode, errorType ->
                logError("applicationCreateEnrollWithBaseProfileUseCase onFailure", message, TAG)
                hideLoader()
                when (errorType) {
                    ErrorType.AUTHORIZATION -> toLoginFragment()

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

    private fun confirmApplicationWithBaseProfile(otpCode: String) {
        applicationCreateConfirmWithBaseProfileUseCase.invoke(otpCode = otpCode).onEach { result ->
            result.onLoading {
                logDebug("confirmApplicationWithBaseProfile onLoading", TAG)
            }.onSuccess { _, _, _ ->
                logDebug("confirmApplicationWithBaseProfile onSuccess", TAG)
                hideLoader()
                showMessage(
                    DialogMessage(
                        message = StringSource(R.string.create_application_success_confirm_message),
                        title = StringSource(R.string.information),
                        positiveButtonText = StringSource(R.string.ok),
                    )
                )
            }.onFailure { _, _, message, responseCode, errorType ->
                logError("confirmApplicationWithBaseProfile onFailure", message, TAG)
                hideLoader()
                when (errorType) {
                    ErrorType.AUTHORIZATION -> toLoginFragment()

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

    override fun onAlertDialogResult(result: AlertDialogResult) {
        logDebug("onAlertDialogResult", TAG)
        when {
            result.messageId == DIALOG_EXIT -> {
                when {
                    result.isPositive -> backToTab()
                    else -> _showCreatePinLiveData.callOnMainThread()
                }
            }

            else -> backToTab()
        }
    }

    override fun onBackPressed() {
        logDebug("onBackPressed", TAG)
        backToTab()
    }
}