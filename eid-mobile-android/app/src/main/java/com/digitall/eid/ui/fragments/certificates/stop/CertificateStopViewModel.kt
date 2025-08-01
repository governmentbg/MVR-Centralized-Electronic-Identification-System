package com.digitall.eid.ui.fragments.certificates.stop

import androidx.lifecycle.MutableLiveData
import androidx.lifecycle.viewModelScope
import com.digitall.eid.R
import com.digitall.eid.domain.APPLICATION_LANGUAGE
import com.digitall.eid.domain.DELAY_500
import com.digitall.eid.domain.ToServerDateFormats
import com.digitall.eid.domain.extensions.getEnumValue
import com.digitall.eid.domain.extensions.toServerDate
import com.digitall.eid.domain.models.applications.all.ApplicationCompletionStatusEnum
import com.digitall.eid.domain.models.base.ErrorType
import com.digitall.eid.domain.models.base.onFailure
import com.digitall.eid.domain.models.base.onLoading
import com.digitall.eid.domain.models.base.onSuccess
import com.digitall.eid.domain.models.certificates.CertificateDetailsModel
import com.digitall.eid.domain.models.certificates.CertificateStatusChangeRequestModel
import com.digitall.eid.domain.models.certificates.PersonalIdentityDocumentRequestModel
import com.digitall.eid.domain.models.nomenclatures.reasons.NomenclaturePermittedUserEnum
import com.digitall.eid.domain.models.nomenclatures.reasons.NomenclatureReasonModel
import com.digitall.eid.domain.usecase.applications.all.CompleteApplicationUseCase
import com.digitall.eid.domain.usecase.certificates.CertificateChangeStatusInformationUseCase
import com.digitall.eid.domain.usecase.certificates.CertificateChangeStatusUseCase
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.extensions.launchInScope
import com.digitall.eid.extensions.launchWithDispatcher
import com.digitall.eid.extensions.readOnly
import com.digitall.eid.extensions.setValueOnMainThread
import com.digitall.eid.mappers.certificates.stop.CertificateStopUiMapper
import com.digitall.eid.models.applications.all.ApplicationStatusEnum
import com.digitall.eid.models.applications.payment.ApplicationPaymentModel
import com.digitall.eid.models.certificates.stop.CertificateStopAdapterMarker
import com.digitall.eid.models.certificates.stop.CertificateStopElementsEnumUi
import com.digitall.eid.models.certificates.stop.CertificateStopUiModel
import com.digitall.eid.models.common.AlertDialogResult
import com.digitall.eid.models.common.DialogMessage
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonButtonUi
import com.digitall.eid.models.list.CommonDatePickerUi
import com.digitall.eid.models.list.CommonDialogWithSearchUi
import com.digitall.eid.models.list.CommonEditTextUi
import com.digitall.eid.models.list.CommonSpinnerUi
import com.digitall.eid.models.list.CommonTextFieldUi
import com.digitall.eid.models.list.CommonValidationFieldUi
import com.digitall.eid.models.main.MainTabsEnum
import com.digitall.eid.ui.BaseViewModel
import com.digitall.eid.utils.SingleLiveEvent
import kotlinx.coroutines.delay
import kotlinx.coroutines.flow.onEach
import org.koin.core.component.inject

class CertificateStopViewModel : BaseViewModel() {

    companion object {
        private const val TAG = "CertificateStopViewModelTag"
        private const val STOPPED_REASON = "STOP_REASON_TYPE"
        private const val STOPPED_STATUS = "STOP_EID"
    }

    override var mainTabsEnum: MainTabsEnum? = MainTabsEnum.TAB_EIM

    private val certificateChangeStatusUseCase: CertificateChangeStatusUseCase by inject()
    private val certificateChangeStatusGetInformationUseCaseUseCase: CertificateChangeStatusInformationUseCase by inject()
    private val completeApplicationUseCase: CompleteApplicationUseCase by inject()
    private val certificateStopUiMapper: CertificateStopUiMapper by inject()

    private val _certificateStatusChangeLiveData = MutableLiveData(false)
    val certificateStatusChangeLiveData = _certificateStatusChangeLiveData.readOnly()

    @Volatile
    private var isCertificateStatusChanged = false
        set(value) {
            field = value
            _certificateStatusChangeLiveData.setValueOnMainThread(value)
        }

    private val currentList = mutableListOf<CertificateStopAdapterMarker>()

    private val _adapterListLiveData =
        MutableLiveData<List<CertificateStopAdapterMarker>>(emptyList())
    val adapterListLiveData = _adapterListLiveData.readOnly()

    private val _scrollToErrorPositionLiveData = SingleLiveEvent<Int>()
    val scrollToErrorPositionLiveData = _scrollToErrorPositionLiveData.readOnly()

    private var errorPosition: Int? = null
        set(value) {
            field = value
            value?.let { position ->
                _scrollToErrorPositionLiveData.setValueOnMainThread(position)
            }
        }


    private var id: String? = null

    private var certificateStopUiModel = CertificateStopUiModel()

    private var certificateDetailsModel: CertificateDetailsModel? = null

    private var identityTypes = emptyMap<String, String>()

    fun setupModel(id: String, identityTypes: Map<String, String>) {
        logDebug("setupModel id: $id", TAG)
        this.id = id
        this.identityTypes = identityTypes
        viewModelScope.launchWithDispatcher {
            refreshScreen()
        }
    }

    override fun onBackPressed() {
        logDebug("onBackPressed", TAG)
        popBackStack()
    }

    override fun onAlertDialogResult(result: AlertDialogResult) {
        if (result.isPositive && isCertificateStatusChanged) {
            navigateToCertificates()
        }
    }

    fun refreshScreen() {
        logDebug("refreshScreen", TAG)
        when {
            certificateStopUiModel.isValid && certificateDetailsModel != null -> changeStatus()
            else -> fetchInformation()
        }
    }

    fun onEditTextFocusChanged(model: CommonEditTextUi) {
        logDebug("onEditTextFocusChanged", TAG)
        if (model.hasFocus.not()) {
            changeValidationField(model)
        }
    }

    fun onEditTextDone(model: CommonEditTextUi) {
        logDebug("onEditTextDone", TAG)
        changeValidationField(model)
    }

    fun onEditTextChanged(model: CommonEditTextUi) {
        logDebug("onEditTextChanged", TAG)
        changeValidationField(model)
    }

    fun onButtonClicked(model: CommonButtonUi) {
        logDebug("onButtonClicked", TAG)
        when (model.elementEnum) {
            CertificateStopElementsEnumUi.BUTTON_BACK -> onBackPressed()
            CertificateStopElementsEnumUi.BUTTON_CONFIRM -> {
                if (canSubmitForm()) {
                    changeStatus()
                }
            }
        }
    }

    private fun navigateToCertificates() {
        logDebug("navigateToCertificates", TAG)
        popBackStackToFragment(R.id.certificatesFragment)
    }

    private fun changeValidationField(model: CommonValidationFieldUi<*>) {
        _adapterListLiveData.value = _adapterListLiveData.value?.map { item ->
            if (item.elementEnum == model.elementEnum) {
                when (model) {
                    is CommonEditTextUi -> model.copy(validationError = model.validationError)
                        .apply {
                            triggerValidation()
                        }

                    is CommonSpinnerUi -> model.copy(validationError = model.validationError)
                        .apply {
                            triggerValidation()
                        }

                    is CommonDatePickerUi -> model.copy(validationError = model.validationError)
                        .apply {
                            triggerValidation()
                        }

                    is CommonDialogWithSearchUi -> model.copy(validationError = model.validationError)
                        .apply {
                            triggerValidation()
                        }

                    else -> item
                }
            } else {
                item
            }
        }
    }

    private fun canSubmitForm(): Boolean {
        var allFieldsValid = true
        errorPosition = null
        _adapterListLiveData.value = _adapterListLiveData.value?.mapIndexed { index, item ->
            when (item) {
                is CommonEditTextUi -> {
                    item.copy(validationError = null).apply {
                        val itemIsValid = triggerValidation()
                        if (itemIsValid.not()) {
                            allFieldsValid = false
                            if (errorPosition == null) {
                                errorPosition = index
                            }
                        }
                    }
                }

                is CommonDatePickerUi -> {
                    item.copy(validationError = null).apply {
                        val itemIsValid = triggerValidation()
                        if (itemIsValid.not()) {
                            allFieldsValid = false
                            if (errorPosition == null) {
                                errorPosition = index
                            }
                        }
                    }
                }

                is CommonDialogWithSearchUi -> {
                    item.copy(validationError = null).apply {
                        val itemIsValid = triggerValidation()
                        if (itemIsValid.not()) {
                            allFieldsValid = false
                            if (errorPosition == null) {
                                errorPosition = index
                            }
                        }
                    }
                }

                is CommonSpinnerUi -> {
                    item.copy(validationError = null).apply {
                        val itemIsValid = triggerValidation()
                        if (itemIsValid.not()) {
                            allFieldsValid = false
                            if (errorPosition == null) {
                                errorPosition = index
                            }
                        }
                    }
                }

                else -> item
            }
        }

        return allFieldsValid
    }

    override fun onSpinnerSelected(model: CommonSpinnerUi) {
        logDebug("onSpinnerSelected", TAG)
        changeValidationField(model)
    }

    override fun onDatePickerChanged(model: CommonDatePickerUi) {
        logDebug("onDatePickerChanged date: ${model.selectedValue}", TAG)
        changeValidationField(model)
    }

    override fun onDialogElementSelected(model: CommonDialogWithSearchUi) {
        logDebug("onDialogElementSelected", TAG)
        changeValidationField(model)
    }

    private fun changeStatus() {
        var reasonText: String? = null
        var forname: String? = null
        var middlename: String? = null
        var surname: String? = null
        var fornameLatin: String? = null
        var middlenameLatin: String? = null
        var surnameLatin: String? = null
        var dateOfBirth: String? = null
        var citizenIdentifierNumber: String? = null
        var documentNumber: String? = null
        var issuedFrom: String? = null
        var citizenship: String? = null
        var reasonId: String? = null
        var documentType: String? = null
        var citizenIdentifierType: String? = null
        var issuedOnDate: String? = null
        var validUntilDate: String? = null
        currentList.forEach { element ->
            when (element) {
                is CommonEditTextUi -> {
                    when (element.elementEnum) {
                        CertificateStopElementsEnumUi.EDIT_TEXT_REASON -> reasonText =
                            element.selectedValue?.trim()

                        CertificateStopElementsEnumUi.EDIT_TEXT_FORNAME -> forname =
                            element.selectedValue?.trim()

                        CertificateStopElementsEnumUi.EDIT_TEXT_MIDDLENAME -> middlename =
                            element.selectedValue?.trim()

                        CertificateStopElementsEnumUi.EDIT_TEXT_SURNAME -> surname =
                            element.selectedValue?.trim()

                        CertificateStopElementsEnumUi.EDIT_TEXT_FORNAME_LATIN -> fornameLatin =
                            element.selectedValue?.trim()

                        CertificateStopElementsEnumUi.EDIT_TEXT_MIDDLENAME_LATIN -> middlenameLatin =
                            element.selectedValue?.trim()

                        CertificateStopElementsEnumUi.EDIT_TEXT_SURNAME_LATIN -> surnameLatin =
                            element.selectedValue?.trim()

                        CertificateStopElementsEnumUi.EDIT_TEXT_IDENTIFIER -> citizenIdentifierNumber =
                            element.selectedValue?.trim()

                        CertificateStopElementsEnumUi.EDIT_TEXT_DOCUMENT_NUMBER -> documentNumber =
                            element.selectedValue?.trim()

                        CertificateStopElementsEnumUi.EDIT_TEXT_ISSUED_FROM -> issuedFrom =
                            element.selectedValue?.trim()

                        CertificateStopElementsEnumUi.EDIT_TEXT_CITIZENSHIP -> citizenship =
                            element.selectedValue?.trim()
                    }
                }

                is CommonSpinnerUi -> {
                    when (element.elementEnum) {
                        CertificateStopElementsEnumUi.SPINNER_IDENTIFIER_TYPE -> citizenIdentifierType =
                            element.selectedValue?.serverValue

                        CertificateStopElementsEnumUi.SPINNER_DOCUMENT_TYPE -> documentType =
                            identityTypes[element.selectedValue?.serverValue]
                    }
                }

                is CommonDatePickerUi -> {
                    when (element.elementEnum) {
                        CertificateStopElementsEnumUi.DATE_PICKER_ISSUED_ON -> issuedOnDate =
                            element.selectedValue?.toServerDate(dateFormat = ToServerDateFormats.ONLY_DATE)

                        CertificateStopElementsEnumUi.DATE_PICKER_VALID_UNTIL -> validUntilDate =
                            element.selectedValue?.toServerDate(dateFormat = ToServerDateFormats.ONLY_DATE)

                        CertificateStopElementsEnumUi.DATE_PICKER_DATE_OF_BIRTH -> dateOfBirth =
                            element.selectedValue?.toServerDate(dateFormat = ToServerDateFormats.ONLY_DATE)
                    }
                }

                is CommonTextFieldUi -> {
                    when (element.elementEnum) {
                        CertificateStopElementsEnumUi.TEXT_VIEW_REASON -> {
                            val reason = element.originalModel as? NomenclatureReasonModel
                            reasonId = reason?.id
//                            reasonText = reason?.description
                        }
                    }
                }
            }
        }
        certificateChangeStatusUseCase.invoke(
            changeRequestModel = CertificateStatusChangeRequestModel(
                forname = forname,
                middlename = middlename,
                surname = surname,
                fornameLatin = fornameLatin,
                middlenameLatin = middlenameLatin,
                surnameLatin = surnameLatin,
                dateOfBirth = dateOfBirth,
                eidAdministratorId = certificateDetailsModel?.eidAdministratorId,
                eidAdministratorOfficeId = certificateDetailsModel?.eidAdministratorId,
                applicationType = STOPPED_STATUS,
                citizenship = citizenship?.uppercase(),
                citizenIdentifierType = citizenIdentifierType,
                citizenIdentifierNumber = citizenIdentifierNumber,
                identityDocument = PersonalIdentityDocumentRequestModel(
                    number = documentNumber,
                    type = documentType,
                    issueDate = issuedOnDate,
                    issuer = issuedFrom,
                    validUntilDate = validUntilDate
                ),
                reasonId = reasonId,
                reasonText = reasonText,
                certificateId = certificateDetailsModel?.id
            )
        ).onEach { result ->
            result.onLoading {
                logDebug("changeStatus onLoading", TAG)
                showLoader()
            }.onSuccess { model, _, _ ->
                logDebug("changeStatus onSuccess", TAG)
                delay(DELAY_500)
                hideLoader()
                hideErrorState()
                model.status?.let {
                    val status = getEnumValue<ApplicationStatusEnum>(it)
                        ?: ApplicationStatusEnum.UNKNOWN

                    when (status) {
                        ApplicationStatusEnum.PENDING_PAYMENT -> toPayment(
                            model = ApplicationPaymentModel(
                                fee = model.fee,
                                carrierPrice = 0,
                                currency = model.feeCurrency,
                                paymentAccessCode = model.paymentAccessCode,
                            )
                        )

                        ApplicationStatusEnum.PAID -> completeApplication(id = model.id)

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
            }.onFailure { _, _, message, responseCode, errorType ->
                logError(
                    "changeStatus onFailure",
                    message,
                    TAG
                )
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
                            positiveButtonText = StringSource(R.string.ok),
                        )
                    )
                }
            }
        }.launchInScope(viewModelScope)
    }

    private fun fetchInformation() {
        certificateChangeStatusGetInformationUseCaseUseCase.invoke(
            id = id ?: return,
        ).onEach { result ->
            result.onLoading {
                logDebug("fetchInformation onLoading", TAG)
                showLoader()
            }.onSuccess { model, _, _ ->
                logDebug("fetchInformation onSuccess", TAG)
                certificateDetailsModel = model.certificateDetails
                val language = APPLICATION_LANGUAGE
                certificateStopUiModel = certificateStopUiModel.copy(
                    userModel = model.userDetails,
                    stopReasons = model.reasons?.firstOrNull { reason -> reason.name == STOPPED_REASON }?.nomenclatures?.filter { nomenclature ->
                        nomenclature.language == language.type && nomenclature.permittedUser == NomenclaturePermittedUserEnum.PUBLIC
                    }
                )
                setStartElements(certificateStopUiModel = certificateStopUiModel)
                delay(DELAY_500)
                hideErrorState()
                hideLoader()
            }.onFailure { _, _, message, responseCode, errorType ->
                logError(
                    "nomenclaturesGetReasonsUseCase onFailure",
                    message,
                    TAG
                )
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

    private fun completeApplication(id: String?) {
        logDebug("completeApplication", TAG)
        isCertificateStatusChanged = false
        completeApplicationUseCase.invoke(
            id = id ?: return
        ).onEach { result ->
            result.onLoading {
                logDebug("completeApplicationUseCase onLoading", TAG)
                showLoader()
            }.onSuccess { model, _, _ ->
                logDebug("completeApplicationUseCase onSuccess", TAG)
                isCertificateStatusChanged = true
                hideLoader()
                when (model) {
                    ApplicationCompletionStatusEnum.COMPLETED -> {
                        showMessage(
                            DialogMessage(
                                message = StringSource(R.string.certificate_stop_success_message),
                                title = StringSource(R.string.information),
                                positiveButtonText = StringSource(R.string.ok),
                            )
                        )
                    }

                    else -> {}
                }
            }.onFailure { _, _, message, responseCode, errorType ->
                logError("completeApplicationUseCase onFailure", message, TAG)
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

    private fun setStartElements(certificateStopUiModel: CertificateStopUiModel) {
        _adapterListLiveData.postValue(certificateStopUiMapper.map(from = certificateStopUiModel))
    }

    private fun toPayment(model: ApplicationPaymentModel) {
        logDebug("toPayment", TAG)
        navigateInFlow(
            CertificateStopFragmentDirections.toApplicationPaymentFragment(
                model = model
            )
        )
    }
}