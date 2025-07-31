/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.main.tabs.home

import androidx.lifecycle.viewModelScope
import com.digitall.eid.R
import com.digitall.eid.domain.DELAY_500
import com.digitall.eid.domain.models.base.ErrorType
import com.digitall.eid.domain.models.base.onFailure
import com.digitall.eid.domain.models.base.onLoading
import com.digitall.eid.domain.models.base.onSuccess
import com.digitall.eid.domain.usecase.citizen.eid.associate.CitizenEidAssociateUseCase
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.extensions.launchInScope
import com.digitall.eid.extensions.launchWithDispatcher
import com.digitall.eid.extensions.readOnly
import com.digitall.eid.extensions.setValueOnMainThread
import com.digitall.eid.models.common.AlertDialogResult
import com.digitall.eid.models.common.DialogMessage
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.main.MainTabsEnum
import com.digitall.eid.ui.fragments.applications.confirm.pin.ApplicationConfirmPinBottomSheetViewModel.Companion.DIALOG_EXIT_PIN_CREATION
import com.digitall.eid.ui.fragments.citizen.information.CitizenInformationViewModel
import com.digitall.eid.ui.fragments.main.base.BaseMainTabViewModel
import com.digitall.eid.utils.AuthenticationManager
import com.digitall.eid.utils.SingleLiveEvent
import kotlinx.coroutines.delay
import kotlinx.coroutines.flow.onEach
import org.koin.core.component.inject
import java.util.UUID

class MainTabHomeViewModel : BaseMainTabViewModel() {

    companion object {
        private const val TAG = "MainTabsFlowViewModelTag"
        private const val DIALOG_PIN_CREATE = "DIALOG_PIN_CREATE"
    }

    override var mainTabsEnum: MainTabsEnum? = MainTabsEnum.TAB_HOME

    private val _showCertificateAuthenticationLiveData = SingleLiveEvent<Unit>()
    val showCertificateAuthenticationLiveData = _showCertificateAuthenticationLiveData.readOnly()

    private val _showAssociateEIDActionLiveData = SingleLiveEvent<Boolean>()
    val showAssociateEIDActionLiveData = _showAssociateEIDActionLiveData.readOnly()

    private val citizenEidAssociateUseCase: CitizenEidAssociateUseCase by inject()
    private val authenticationManager: AuthenticationManager by inject()

    private val _showCreatePinBottomSheetEvent = SingleLiveEvent<Unit>()
    val showCreatePinBottomSheetEvent = _showCreatePinBottomSheetEvent.readOnly()

    override fun onAlertDialogResult(result: AlertDialogResult) {
        when (result.messageId) {
            DIALOG_PIN_CREATE -> {
                authenticationManager.setUserBeenPromptForPinCreation()
                if (result.isPositive) {
                    showCreatePinBottomSheet()
                }
            }
            DIALOG_EXIT_PIN_CREATION -> {
                if (result.isPositive.not()) {
                    _showCreatePinBottomSheetEvent.setValueOnMainThread(Unit)
                }
            }
            else -> {
                if (result.isPositive && isCitizenEidAssociatedSuccessful) {
                    val userModel = preferences.readApplicationInfo()?.userModel
                    preferences.readApplicationInfo()?.let { applicationInformation ->
                        userModel?.copy(eidEntityId = UUID.randomUUID().toString())
                            ?.let {
                                preferences.saveApplicationInfo(
                                    applicationInformation.copy(userModel = it)
                                )
                            }
                    }
                }
            }
        }
    }

    @Volatile
    private var isCitizenEidAssociatedSuccessful = false
        set(value) {
            field = value
            _showAssociateEIDActionLiveData.setValueOnMainThread(value.not())
        }

    override fun citizenAssociateEID() {
        logDebug("associateEIM", TAG)
        _showCertificateAuthenticationLiveData.setValueOnMainThread(Unit)
    }

    fun showActionButton() {
        val userHasAssociatedEid = checkIfUserHasAssociatedEID()
        _showAssociateEIDActionLiveData.setValueOnMainThread(userHasAssociatedEid.not())
    }

    fun promptUserToSecureCredentials() {
        if (authenticationManager.hasBeenUserPromptForPinCreation().not()) {
            showMessage(
                DialogMessage(
                    messageID = DIALOG_PIN_CREATE,
                    title = StringSource(R.string.information),
                    message = StringSource(R.string.pin_creation_message),
                    positiveButtonText = StringSource(R.string.yes),
                    negativeButtonText = StringSource(R.string.no),
                ),
            )

        }
    }

    fun setupApplicationPin(pin: String?) {
        viewModelScope.launchWithDispatcher {
            authenticationManager.setupApplicationPin(pin = pin ?: return@launchWithDispatcher)
        }
    }

    fun associateEid(
        signature: String?,
        challenge: String?,
        certificate: String?,
        certificateChain: List<String>?
    ) {
        logDebug("associateEid", TAG)
        isCitizenEidAssociatedSuccessful = false
        citizenEidAssociateUseCase.invoke(
            signature = signature,
            challenge = challenge,
            certificate = certificate,
            certificateChain = certificateChain
        ).onEach { result ->
            result.onLoading {
                logDebug(
                    "associateEid onLoading",
                    TAG
                )
                showLoader()
            }.onSuccess { _, _, _ ->
                logDebug(
                    "associateEid onSuccess",
                    CitizenInformationViewModel.TAG
                )
                isCitizenEidAssociatedSuccessful = true
                delay(DELAY_500)
                hideLoader()
                hideErrorState()
                showMessage(
                    DialogMessage(
                        message = StringSource(R.string.citizen_information_change_associate_eid_successful_message),
                        title = StringSource(R.string.information),
                        positiveButtonText = StringSource(R.string.ok),
                    )
                )
            }.onFailure { _, _, message, responseCode, errorType ->
                logError("associateEid onFailure", message, TAG)
                hideLoader()
                when (errorType) {
                    ErrorType.AUTHORIZATION -> toLoginFragment()

                    else -> showMessage(
                        DialogMessage(
                            title = StringSource(R.string.information),
                            message = message?.let {
                                StringSource(
                                    "$it (%s)",
                                    formatArgs = listOf((responseCode ?: 0).toString())
                                )
                            } ?: StringSource(
                                R.string.error_api_general,
                                formatArgs = listOf((responseCode ?: 0).toString())
                            ),
                            positiveButtonText = StringSource(R.string.ok)
                        )
                    )
                }
            }
        }.launchInScope(viewModelScope)
    }

    private fun showCreatePinBottomSheet() {
        _showCreatePinBottomSheetEvent.setValueOnMainThread(Unit)
    }

}