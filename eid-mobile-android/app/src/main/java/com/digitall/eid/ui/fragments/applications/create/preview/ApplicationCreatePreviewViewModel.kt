/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.applications.create.preview

import androidx.lifecycle.viewModelScope
import com.digitall.eid.R
import com.digitall.eid.domain.EID_MOBILE_CERTIFICATE_KEYS
import com.digitall.eid.domain.SIGNING_REQUEST_TIMEOUT
import com.digitall.eid.domain.extensions.getEnumValue
import com.digitall.eid.domain.extensions.readOnly
import com.digitall.eid.domain.models.administrators.AdministratorFrontOfficeModel
import com.digitall.eid.domain.models.applications.create.ApplicationDetailsDocumentXMLRequestModel
import com.digitall.eid.domain.models.applications.create.ApplicationDetailsXMLRequestModel
import com.digitall.eid.domain.models.applications.create.ApplicationSendSignatureResponseModel
import com.digitall.eid.domain.models.base.onFailure
import com.digitall.eid.domain.models.base.onLoading
import com.digitall.eid.domain.models.base.onSuccess
import com.digitall.eid.domain.models.user.UserAcrEnum
import com.digitall.eid.domain.usecase.applications.create.ApplicationCreateEnrollWithEIDUseCase
import com.digitall.eid.domain.usecase.applications.create.ApplicationSignWithBoricaUseCase
import com.digitall.eid.domain.usecase.applications.create.ApplicationSignWithEvrotrustUseCase
import com.digitall.eid.domain.usecase.applications.create.ApplicationSignWithoutProviderUseCase
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.extensions.launchInScope
import com.digitall.eid.extensions.launchWithDispatcher
import com.digitall.eid.models.applications.all.ApplicationStatusEnum
import com.digitall.eid.models.applications.create.ApplicationCreateIntroElementsEnumUi
import com.digitall.eid.models.applications.create.ApplicationCreateIntroSigningMethodsEnumUi
import com.digitall.eid.models.applications.create.ApplicationCreatePinModel
import com.digitall.eid.models.applications.create.ApplicationCreatePreviewAdapterMarker
import com.digitall.eid.models.applications.create.ApplicationCreatePreviewUi
import com.digitall.eid.models.applications.details.ONLINE_OFFICE
import com.digitall.eid.models.applications.payment.ApplicationPaymentModel
import com.digitall.eid.models.common.DialogMessage
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonButtonUi
import com.digitall.eid.models.list.CommonTextFieldUi
import com.digitall.eid.models.main.MainTabsEnum
import com.digitall.eid.ui.BaseViewModel
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.onEach
import org.koin.core.component.inject

class ApplicationCreatePreviewViewModel : BaseViewModel() {

    companion object {
        private const val TAG = "ApplicationCreatePreviewViewModelTag"
        private val LOADING_MESSAGES = mapOf(
            "PLEASE_WAIT" to R.string.wait,
            "OPEN_EVROTRUST_APPLICATION" to R.string.evrotrust_open_application_message,
            "OPEN_BORICA_APPLICATION" to R.string.borica_open_application_message,
        )
    }

    private val applicationSignWithBoricaUseCase: ApplicationSignWithBoricaUseCase by inject()
    private val applicationSignWithEvrotrustUseCase: ApplicationSignWithEvrotrustUseCase by inject()
    private val applicationSignWithoutProviderUseCase: ApplicationSignWithoutProviderUseCase by inject()
    private val applicationCreateEnrollWithEIDUseCase: ApplicationCreateEnrollWithEIDUseCase by inject()

    private var applicationCreatePreviewUi: ApplicationCreatePreviewUi? = null

    private val _adapterListLiveData =
        MutableStateFlow<List<ApplicationCreatePreviewAdapterMarker>>(emptyList())
    val adapterListLiveData = _adapterListLiveData.readOnly()

    override var mainTabsEnum: MainTabsEnum? = MainTabsEnum.TAB_EIM

    fun refreshScreen() {
        logDebug("refreshScreen", TAG)
        generateXML()
    }

    fun setupModel(model: ApplicationCreatePreviewUi?) {
        logDebug("setupModel", TAG)
        if (model == null) {
            showErrorState(
                title = StringSource(R.string.error_internal_error_short),
                description = StringSource("Required element is empty")
            )
            return
        }
        applicationCreatePreviewUi = model
        viewModelScope.launchWithDispatcher {
            _adapterListLiveData.emit(model.previewList)
        }
    }

    fun onButtonClicked(model: CommonButtonUi) {
        logDebug("onButtonClicked", TAG)
        when (model.elementEnum) {
            ApplicationCreateIntroElementsEnumUi.BUTTON_SEND, ApplicationCreateIntroElementsEnumUi.BUTTON_SIGN  -> generateXML()
            ApplicationCreateIntroElementsEnumUi.BUTTON_EDIT -> onBackPressed()
        }
    }

    private fun generateXML() {
        logDebug("generateXML", TAG)
        viewModelScope.launchWithDispatcher {
            var deviceId: String? = null
            var dateOfBirth: String? = null
            var citizenship: String? = null
            var lastNameLatin: String? = null
            var firstNameLatin: String? = null
            var secondNameLatin: String? = null
            var eidAdministratorId: String? = null
            var citizenIdentifierType: String? = null
            var citizenIdentifierNumber: String? = null
            var eidAdministratorOfficeId: String? = null
            var identityType: String? = null
            var identityIssuer: String? = null
            var identityNumber: String? = null
            var identityIssueDate: String? = null
            var identityValidityToDate: String? = null
            val applicationType = "ISSUE_EID"
            applicationCreatePreviewUi?.previewList?.filterIsInstance<CommonTextFieldUi>()
                ?.forEach {
                    when (it.elementEnum) {
                        ApplicationCreateIntroElementsEnumUi.EDIT_TEXT_FIRST_LATIN_NAME -> {
                            firstNameLatin = it.serverValue?.trim()
                        }

                        ApplicationCreateIntroElementsEnumUi.EDIT_TEXT_SECOND_LATIN_NAME -> {
                            secondNameLatin = it.serverValue?.trim()
                        }

                        ApplicationCreateIntroElementsEnumUi.EDIT_TEXT_LAST_LATIN_NAME -> {
                            lastNameLatin = it.serverValue?.trim()
                        }

                        ApplicationCreateIntroElementsEnumUi.DATE_PICKER_BORN_DATE -> {
                            dateOfBirth = it.serverValue?.trim()
                        }

                        ApplicationCreateIntroElementsEnumUi.SPINNER_ID_TYPE -> {
                            citizenIdentifierType = it.serverValue?.trim()
                        }

                        ApplicationCreateIntroElementsEnumUi.EDIT_TEXT_ID_NUMBER -> {
                            citizenIdentifierNumber = it.serverValue?.trim()
                        }

                        ApplicationCreateIntroElementsEnumUi.EDIT_TEXT_CITIZENSHIP -> {
                            citizenship = it.serverValue?.trim()
                        }

                        ApplicationCreateIntroElementsEnumUi.SPINNER_DOCUMENT_TYPE -> {
                            identityType = "Лична карта"
                        }

                        ApplicationCreateIntroElementsEnumUi.EDIT_TEXT_DOCUMENT_NUMBER -> {
                            identityNumber = it.serverValue?.trim()
                        }

                        ApplicationCreateIntroElementsEnumUi.DATE_PICKER_CREATED_ON -> {
                            identityIssueDate = it.serverValue?.trim()
                        }

                        ApplicationCreateIntroElementsEnumUi.EDIT_TEXT_ISSUED_FROM -> {
                            identityIssuer = it.serverValue?.trim()
                        }

                        ApplicationCreateIntroElementsEnumUi.DATE_PICKER_VALID_TO -> {
                            identityValidityToDate = it.serverValue?.trim()
                        }

                        ApplicationCreateIntroElementsEnumUi.DIALOG_ADMINISTRATOR -> {
                            eidAdministratorId = it.serverValue?.trim()
                        }

                        ApplicationCreateIntroElementsEnumUi.DIALOG_ADMINISTRATOR_OFFICE -> {
                            eidAdministratorOfficeId = it.serverValue?.trim()
                        }

                        ApplicationCreateIntroElementsEnumUi.SPINNER_DEVICE_TYPE -> {
                            deviceId = it.serverValue?.trim()
                        }
                    }
                }
            if (deviceId.isNullOrEmpty() ||
                dateOfBirth.isNullOrEmpty() ||
                citizenship.isNullOrEmpty() ||
                lastNameLatin.isNullOrEmpty() ||
                firstNameLatin.isNullOrEmpty() ||
                eidAdministratorId.isNullOrEmpty() ||
                citizenIdentifierType.isNullOrEmpty() ||
                citizenIdentifierNumber.isNullOrEmpty() ||
                eidAdministratorOfficeId.isNullOrEmpty() ||
                identityType.isNullOrEmpty() ||
                identityNumber.isNullOrEmpty() ||
                identityIssueDate.isNullOrEmpty() ||
                identityValidityToDate.isNullOrEmpty()
            ) {
                showErrorState(
                    title = StringSource(R.string.error_internal_error_short),
                    description = StringSource("Some element is empty")
                )
                return@launchWithDispatcher
            }
            val request = ApplicationDetailsXMLRequestModel(
                lastName = applicationCreatePreviewUi?.userModel?.lastName,
                firstName = applicationCreatePreviewUi?.userModel?.firstName,
                secondName = applicationCreatePreviewUi?.userModel?.secondName,
                deviceId = deviceId,
                dateOfBirth = dateOfBirth,
                citizenship = citizenship,
                lastNameLatin = lastNameLatin,
                firstNameLatin = firstNameLatin,
                secondNameLatin = secondNameLatin,
                applicationType = applicationType,
                eidAdministratorId = eidAdministratorId,
                citizenIdentifierType = citizenIdentifierType,
                citizenIdentifierNumber = citizenIdentifierNumber,
                eidAdministratorOfficeId = eidAdministratorOfficeId,
                reasonId = null,
                reasonText = null,
                certificateId = null,
                personalIdentityDocument = ApplicationDetailsDocumentXMLRequestModel(
                    identityType = identityType,
                    identityIssuer = identityIssuer,
                    identityNumber = identityNumber,
                    identityIssueDate = identityIssueDate,
                    identityValidityToDate = identityValidityToDate,
                )
            )
            val user = preferences.readApplicationInfo()?.userModel

            if (user?.acr == UserAcrEnum.HIGH) {
                signApplicationRequestWithoutProvider(request = request)
            } else {
                signApplicationRequest(
                    request = request,
                    signMethod = applicationCreatePreviewUi?.signMethod
                        ?: ApplicationCreateIntroSigningMethodsEnumUi.EVROTRUST
                )
            }
        }
    }

    private fun signApplicationRequestWithoutProvider(
        request: ApplicationDetailsXMLRequestModel,
    ) {
        val applicationInfo = preferences.readApplicationInfo()
        val firebaseToken = preferences.readFirebaseToken()
        val applicationId = applicationInfo?.mobileApplicationInstanceId ?: ""
        val firebaseId = firebaseToken?.token ?: ""

        applicationSignWithoutProviderUseCase.invoke(
            firebaseId = firebaseId,
            mobileApplicationInstanceId = applicationId,
            data = request
        ).onEach { result ->
            result.onLoading { message ->
                logDebug("signApplicationRequestWithoutProvider onLoading", TAG)
                showLoader()
            }.onSuccess { model, _, _ ->
                logDebug("signApplicationRequestWithoutProvider onSuccess", TAG)
                hideLoader()
                val userModel = preferences.readApplicationInfo()?.userModel
                if (userModel == null) {
                    showErrorState(
                        title = StringSource(R.string.error_internal_error_short),
                        description = StringSource("User model not found")
                    )
                    return@onEach
                }
                val isBaseProfile = userModel.eidEntityId.isNullOrEmpty()
                if (isBaseProfile) {
                    showMessage(
                        DialogMessage(
                            message = StringSource(R.string.create_application_success_message),
                            title = StringSource(R.string.information),
                            positiveButtonText = StringSource(R.string.ok),
                        )
                    )
                    return@onEach
                }
                val selectedOffice =
                    applicationCreatePreviewUi?.previewList?.filterIsInstance<CommonTextFieldUi>()
                        ?.firstOrNull {
                            it.elementEnum == ApplicationCreateIntroElementsEnumUi.DIALOG_ADMINISTRATOR_OFFICE
                        }?.originalModel as? AdministratorFrontOfficeModel
                if (selectedOffice == null) {
                    showErrorState(
                        title = StringSource(R.string.error_internal_error_short),
                        description = StringSource("Office not selected")
                    )
                    return@onEach
                }

                model.status?.let {
                    val status = getEnumValue<ApplicationStatusEnum>(it)
                        ?: ApplicationStatusEnum.UNKNOWN

                    when (status) {
                        ApplicationStatusEnum.PENDING_PAYMENT -> toPayment(
                            model = ApplicationPaymentModel(
                                fee = model.fee,
                                carrierPrice = 0,
                                currency = model.feeCurrency,
                                paymentAccessCode = model.paymentAccessCode
                            )
                        )

                        ApplicationStatusEnum.SUBMITTED -> {
                            when {
                                selectedOffice.name != ONLINE_OFFICE -> showMessage(
                                    DialogMessage(
                                        message = StringSource(R.string.create_application_success_message),
                                        title = StringSource(R.string.information),
                                        positiveButtonText = StringSource(R.string.ok),
                                    )
                                )

                                model.id.isNullOrEmpty() -> showErrorState(
                                    title = StringSource(R.string.error_internal_error_short),
                                    description = StringSource("User id not found")
                                )

                                else -> confirmWithEID(model)
                            }
                        }

                        else -> {
                            showErrorState(
                                title = StringSource(R.string.error_internal_error_short),
                                description = StringSource("Status of application is wrong after submission")
                            )
                            return@onEach
                        }
                    }
                } ?: run {
                    showErrorState(
                        title = StringSource(R.string.error_internal_error_short),
                        description = StringSource("Model status not found")
                    )
                    return@onEach
                }
            }.onFailure { _, _, message, responseCode, _ ->
                logError("signApplicationRequestWithoutProvider onFailure", message, TAG)
                hideLoader()
                showErrorState(
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
        }.launchInScope(viewModelScope)
    }

    private fun signApplicationRequest(
        request: ApplicationDetailsXMLRequestModel,
        signMethod: ApplicationCreateIntroSigningMethodsEnumUi
    ) {
        val applicationInfo = preferences.readApplicationInfo()
        val firebaseToken = preferences.readFirebaseToken()
        val applicationId = applicationInfo?.mobileApplicationInstanceId ?: ""
        val firebaseId = firebaseToken?.token ?: ""
        inactivityTimer.setNewInactivityTimeout(timeoutInMilliseconds = SIGNING_REQUEST_TIMEOUT)


        (when (signMethod) {
            ApplicationCreateIntroSigningMethodsEnumUi.BORIKA -> applicationSignWithBoricaUseCase.invoke(
                data = request,
                firebaseId = firebaseId,
                mobileApplicationInstanceId = applicationId,
            )

            ApplicationCreateIntroSigningMethodsEnumUi.EVROTRUST -> applicationSignWithEvrotrustUseCase.invoke(
                data = request,
                firebaseId = firebaseId,
                mobileApplicationInstanceId = applicationId,
            )
        }).onEach { result ->
            result.onLoading { message ->
                logDebug("signApplicationRequest onLoading", TAG)
                showFullscreenLoader(
                    message = LOADING_MESSAGES[message]?.let { resId ->
                        StringSource(resId)
                    } ?: run { StringSource(message) }
                )
            }.onSuccess { model, _, _ ->
                logDebug("signApplicationRequest onSuccess", TAG)
                hideFullscreenLoader()
                val userModel = preferences.readApplicationInfo()?.userModel
                if (userModel == null) {
                    showErrorState(
                        title = StringSource(R.string.error_internal_error_short),
                        description = StringSource("User model not found")
                    )
                    return@onEach
                }
                val isBaseProfile = userModel.eidEntityId.isNullOrEmpty()
                if (isBaseProfile) {
                    showMessage(
                        DialogMessage(
                            message = StringSource(R.string.create_application_success_message),
                            title = StringSource(R.string.information),
                            positiveButtonText = StringSource(R.string.ok),
                        )
                    )
                    return@onEach
                }
                val selectedOffice =
                    applicationCreatePreviewUi?.previewList?.filterIsInstance<CommonTextFieldUi>()
                        ?.firstOrNull {
                            it.elementEnum == ApplicationCreateIntroElementsEnumUi.DIALOG_ADMINISTRATOR_OFFICE
                        }?.originalModel as? AdministratorFrontOfficeModel
                if (selectedOffice == null) {
                    showErrorState(
                        title = StringSource(R.string.error_internal_error_short),
                        description = StringSource("Office not selected")
                    )
                    return@onEach
                }

                model.status?.let {
                    val status = getEnumValue<ApplicationStatusEnum>(it)
                        ?: ApplicationStatusEnum.UNKNOWN

                    when (status) {
                        ApplicationStatusEnum.PENDING_PAYMENT -> toPayment(
                            model = ApplicationPaymentModel(
                                fee = model.fee,
                                carrierPrice = 0,
                                currency = model.feeCurrency,
                                paymentAccessCode = model.paymentAccessCode
                            )
                        )

                        ApplicationStatusEnum.SUBMITTED -> {
                            when {
                                selectedOffice.name != ONLINE_OFFICE -> showMessage(
                                    DialogMessage(
                                        message = StringSource(R.string.create_application_success_message),
                                        title = StringSource(R.string.information),
                                        positiveButtonText = StringSource(R.string.ok),
                                    )
                                )

                                model.id.isNullOrEmpty() -> showErrorState(
                                    title = StringSource(R.string.error_internal_error_short),
                                    description = StringSource("User id not found")
                                )

                                else -> confirmWithEID(model)
                            }
                        }

                        else -> {
                            showErrorState(
                                title = StringSource(R.string.error_internal_error_short),
                                description = StringSource("Status of application is wrong after submission")
                            )
                            return@onEach
                        }
                    }
                } ?: run {
                    showErrorState(
                        title = StringSource(R.string.error_internal_error_short),
                        description = StringSource("Model status not found")
                    )
                    return@onEach
                }
            }.onFailure { _, _, message, responseCode, _ ->
                logError("signApplicationRequest onFailure", message, TAG)
                hideFullscreenLoader()
                showErrorState(
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
        }.launchInScope(viewModelScope)
    }

    private fun confirmWithEID(
        response: ApplicationSendSignatureResponseModel
    ) {
        logDebug("createWithEID", TAG)
        applicationCreateEnrollWithEIDUseCase.invoke(
            applicationId = response.id ?: return,
            keyAlias = EID_MOBILE_CERTIFICATE_KEYS,
        ).onEach { result ->
            result.onSuccess { model, _, _ ->
                logDebug("createWithEID onSuccess", TAG)
                if (response.id.isNullOrEmpty() ||
                    model.certificate.isNullOrEmpty() ||
                    model.certificateChain?.firstOrNull().isNullOrEmpty()
                ) {
                    showErrorState(
                        title = StringSource(R.string.error_internal_error_short),
                        description = StringSource("Required element is empty")
                    )
                    return@onEach
                }
                hideLoader()
                toCreatePin(
                    model = ApplicationCreatePinModel(
                        applicationId = response.id ?: return@onEach,
                        certificate = model.certificate ?: return@onEach,
                        certificateChain = model.certificateChain ?: return@onEach,
                    )
                )
            }.onFailure { _, title, message, responseCode, _ ->
                logError("createWithEID onFailure", message, TAG)
                hideLoader()
                showErrorState(
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
        }.launchInScope(viewModelScope)
    }

    private fun toCreatePin(model: ApplicationCreatePinModel) {
        logDebug("toCreatePin", TAG)
        navigateInFlow(
            ApplicationCreatePreviewFragmentDirections.toApplicationCreatePinFragment(
                model = model,
            )
        )
    }

    private fun toPayment(model: ApplicationPaymentModel) {
        logDebug("toPayment", TAG)
        navigateInFlow(
            ApplicationCreatePreviewFragmentDirections.toApplicationPaymentFragment(
                model = model
            )
        )
    }

    override fun onAlertDialogResult() {
        logDebug("onAlertDialogResult", TAG)
        popBackStackToFragmentInTab(R.id.mainTabEIMFragment)
    }

    override fun onBackPressed() {
        logDebug("onBackPressed", TAG)
        popBackStackToFragment(R.id.applicationCreateIntroFragment)
    }

}