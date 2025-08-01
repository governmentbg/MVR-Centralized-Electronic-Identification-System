package com.digitall.eid.ui.fragments.citizen.profile.security

import androidx.lifecycle.MutableLiveData
import androidx.lifecycle.viewModelScope
import com.digitall.eid.R
import com.digitall.eid.domain.DELAY_500
import com.digitall.eid.domain.models.applications.create.ApplicationUserDetailsModel
import com.digitall.eid.domain.models.base.ErrorType
import com.digitall.eid.domain.models.base.onFailure
import com.digitall.eid.domain.models.base.onLoading
import com.digitall.eid.domain.models.base.onSuccess
import com.digitall.eid.domain.models.citizen.update.information.CitizenUpdateInformationRequestModel
import com.digitall.eid.domain.usecase.applications.create.GetApplicationUserDetailsUseCase
import com.digitall.eid.domain.usecase.citizen.update.information.UpdateCitizenInformationUseCase
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.extensions.launchInScope
import com.digitall.eid.extensions.launchWithDispatcher
import com.digitall.eid.extensions.readOnly
import com.digitall.eid.extensions.setValueOnMainThread
import com.digitall.eid.mappers.citizen.profile.security.CitizenProfileSecurityUiMapper
import com.digitall.eid.models.citizen.profile.security.CitizenProfileSecurityElementsEnumUi
import com.digitall.eid.models.citizen.profile.security.CitizenProfileSecurityUiModel
import com.digitall.eid.models.common.AlertDialogResult
import com.digitall.eid.models.common.BannerMessage
import com.digitall.eid.models.common.DialogMessage
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonListElementAdapterMarker
import com.digitall.eid.models.list.CommonTitleCheckboxUi
import com.digitall.eid.models.main.MainTabsEnum
import com.digitall.eid.ui.BaseViewModel
import com.digitall.eid.ui.fragments.applications.confirm.pin.ApplicationConfirmPinBottomSheetViewModel.Companion.DIALOG_EXIT_PIN_CREATION
import com.digitall.eid.utils.AuthenticationManager
import com.digitall.eid.utils.SingleLiveEvent
import kotlinx.coroutines.delay
import kotlinx.coroutines.flow.onEach
import org.koin.core.component.inject

class CitizenProfileSecurityViewModel : BaseViewModel() {

    companion object {
        const val TAG = "CitizenProfileSecurityViewModelTag"
    }

    override var mainTabsEnum: MainTabsEnum? = MainTabsEnum.TAB_MORE

    private val citizenProfileSecurityUiMapper: CitizenProfileSecurityUiMapper by inject()

    private val getApplicationUserDetailsUseCase: GetApplicationUserDetailsUseCase by inject()
    private val updateCitizenInformationUseCase: UpdateCitizenInformationUseCase by inject()
    private val authenticationManager: AuthenticationManager by inject()

    private val _adapterListLiveData =
        MutableLiveData<List<CommonListElementAdapterMarker>>(emptyList())
    val adapterListLiveData = _adapterListLiveData.readOnly()

    private val _showCreatePinBottomSheetEvent = SingleLiveEvent<Unit>()
    val showCreatePinBottomSheetEvent = _showCreatePinBottomSheetEvent.readOnly()

    val authenticationResultEvent = authenticationManager.authenticationResultEvent

    private lateinit var information: ApplicationUserDetailsModel
    private var citizenProfileSecurityUiModel = CitizenProfileSecurityUiModel()
        set(value) {
            field = value
            buildElements(information = value)
        }

    override fun onBackPressed() {
        logDebug("onBackPressed", TAG)
        backToTab()
    }

    override fun onFirstAttach() {
        refreshScreen()
    }

    override fun onAlertDialogResult(result: AlertDialogResult) {
        when (result.messageId) {
            DIALOG_EXIT_PIN_CREATION -> {
                if (result.isPositive.not()) {
                    _showCreatePinBottomSheetEvent.setValueOnMainThread(Unit)
                } else {
                    citizenProfileSecurityUiModel =
                        citizenProfileSecurityUiModel.copy(
                            isPinEnabled = authenticationManager.isApplicationPinEnabled(),
                            isBiometricAvailable = authenticationManager.isApplicationPinEnabled() && authenticationManager.isBiometricsAvailable()
                        )
                }
            }

            else -> {}
        }
    }

    fun onCheckBoxChangeState(model: CommonTitleCheckboxUi) {
        _adapterListLiveData.value = _adapterListLiveData.value?.map { item ->
            if (item.elementEnum == model.elementEnum) {
                model.copy()
            } else {
                item
            }
        }

        when (model.elementEnum) {
            CitizenProfileSecurityElementsEnumUi.MULTI_FACTOR_AUTHENTICATION_CHECKBOX -> {
                updateCitizenInformation(
                    data = CitizenUpdateInformationRequestModel(
                        firstName = information.firstName,
                        secondName = information.secondName,
                        lastName = information.lastName,
                        firstNameLatin = information.firstNameLatin,
                        secondNameLatin = information.secondNameLatin,
                        lastNameLatin = information.lastNameLatin,
                        phoneNumber = information.phoneNumber,
                        twoFaEnabled = model.isChecked
                    )
                )
            }

            CitizenProfileSecurityElementsEnumUi.PROFILE_SECURITY_PIN_CHECKBOX -> {
                if (model.isChecked) {
                    _showCreatePinBottomSheetEvent.callOnMainThread()
                } else {
                    authenticationManager.clearApplicationPin()
                    authenticationManager.setBiometricsEnabledForUnlock(enabled = false)
                    citizenProfileSecurityUiModel =
                        citizenProfileSecurityUiModel.copy(
                            isPinEnabled = authenticationManager.isApplicationPinEnabled(),
                            isBiometricEnabled = authenticationManager.isBiometricsEnabledForUnlock(),
                            isBiometricAvailable = authenticationManager.isApplicationPinEnabled() && authenticationManager.isBiometricsAvailable()
                        )
                }
            }

            CitizenProfileSecurityElementsEnumUi.PROFILE_SECURITY_BIOMETRICS_CHECKBOX -> {
                authenticationManager.setBiometricsEnabledForUnlock(enabled = model.isChecked)
                citizenProfileSecurityUiModel =
                    citizenProfileSecurityUiModel.copy(
                        isBiometricEnabled = authenticationManager.isBiometricsEnabledForUnlock(),
                        isBiometricAvailable = authenticationManager.isApplicationPinEnabled() && authenticationManager.isBiometricsAvailable()
                    )
            }

            else -> {}
        }
    }

    fun refreshScreen() {
        logDebug("refreshScreen", TAG)
        getApplicationUserDetailsUseCase.invoke().onEach { result ->
            result.onLoading {
                logDebug(
                    "getApplicationUserDetailsUseCase onLoading",
                    TAG
                )
                showLoader()
            }.onSuccess { model, _, _ ->
                logDebug(
                    "getApplicationUserDetailsUseCase onSuccess",
                    TAG
                )
                if (model.firstName.isNullOrEmpty() ||
                    model.lastName.isNullOrEmpty() ||
                    model.secondName.isNullOrEmpty()
                ) {
                    showErrorState(
                        title = StringSource(R.string.error_server_error),
                        description = StringSource("Not received usernames from the server")
                    )
                    return@onSuccess
                }
                information = model
                citizenProfileSecurityUiModel = CitizenProfileSecurityUiModel(
                    isTwoFactorEnabled = information.twoFaEnabled ?: false,
                    isPinEnabled = authenticationManager.isApplicationPinEnabled(),
                    isBiometricEnabled = authenticationManager.isBiometricsEnabledForUnlock() && authenticationManager.isBiometricsAvailable(),
                    isBiometricAvailable = authenticationManager.isApplicationPinEnabled() && authenticationManager.isBiometricsAvailable(),
                )
                delay(DELAY_500)
                hideLoader()
                hideErrorState()
            }.onFailure { _, _, message, responseCode, errorType ->
                logError("getApplicationUserDetailsUseCase onFailure", message, TAG)
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

    fun setupApplicationPin(pin: String?) {
        viewModelScope.launchWithDispatcher {
            authenticationManager.setupApplicationPin(pin = pin ?: return@launchWithDispatcher)
            citizenProfileSecurityUiModel =
                citizenProfileSecurityUiModel.copy(
                    isPinEnabled = authenticationManager.isApplicationPinEnabled(),
                    isBiometricAvailable = authenticationManager.isApplicationPinEnabled() && authenticationManager.isBiometricsAvailable()
                )
        }
    }

    fun showBannerErrorMessage(message: StringSource) {
        showMessage(BannerMessage.error(message = message))
        buildElements(information = citizenProfileSecurityUiModel)
    }

    private fun buildElements(information: CitizenProfileSecurityUiModel) {
        _adapterListLiveData.postValue(
            citizenProfileSecurityUiMapper.map(
                information,
                preferences.readApplicationInfo()?.userModel?.acr
            )
        )
    }

    private fun updateCitizenInformation(data: CitizenUpdateInformationRequestModel) {
        updateCitizenInformationUseCase.invoke(
            data = data
        ).onEach { result ->
            result.onLoading {
                logDebug(
                    "onLoading updateCitizenPhone",
                    TAG
                )
                showLoader()
            }.onSuccess { _, _, _ ->
                logDebug("onSuccess updateCitizenPhone", TAG)
                information = information.copy(twoFaEnabled = data.twoFaEnabled)
                citizenProfileSecurityUiModel = citizenProfileSecurityUiModel.copy(
                    isTwoFactorEnabled = data.twoFaEnabled ?: false
                )
                delay(DELAY_500)
                hideLoader()
            }.onFailure { _, _, message, responseCode, errorType ->
                logDebug("onFailure updateCitizenPhone", TAG)
                buildElements(
                    information = citizenProfileSecurityUiModel,
                )
                delay(DELAY_500)
                hideLoader()
                when (errorType) {
                    ErrorType.AUTHORIZATION -> toLoginFragment()

                    else -> showMessage(
                        DialogMessage(
                            title = StringSource(R.string.information),
                            message = message?.let { StringSource(message) }
                                ?: StringSource(
                                    R.string.error_api_general,
                                    formatArgs = listOf(responseCode.toString())
                                ),
                            positiveButtonText = StringSource(R.string.ok)
                        )
                    )
                }
            }
        }.launchInScope(viewModelScope)
    }
}