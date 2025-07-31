package com.digitall.eid.ui.fragments.certificates.change.pin

import androidx.lifecycle.MutableLiveData
import com.digitall.eid.R
import com.digitall.eid.domain.PIN_MAX_LENGTH
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.extensions.readOnly
import com.digitall.eid.extensions.setValueOnMainThread
import com.digitall.eid.mappers.certificates.change.pin.CertificateChangePinUiMapper
import com.digitall.eid.models.card.change.pin.CertificateChangePinAdapterMarker
import com.digitall.eid.models.card.change.pin.CertificateChangePinElementsEnumUi
import com.digitall.eid.models.common.AlertDialogResult
import com.digitall.eid.models.common.CardScanBottomSheetContent
import com.digitall.eid.models.common.CardScanContentType
import com.digitall.eid.models.common.DialogMessage
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.common.validator.FieldsMatchValidator
import com.digitall.eid.models.common.validator.MinLengthEditTextValidator
import com.digitall.eid.models.common.validator.NonEmptyEditTextValidator
import com.digitall.eid.models.list.CommonButtonUi
import com.digitall.eid.models.list.CommonEditTextUi
import com.digitall.eid.models.list.CommonEditTextUiType
import com.digitall.eid.models.main.MainTabsEnum
import com.digitall.eid.ui.BaseViewModel
import com.digitall.eid.ui.fragments.registration.RegistrationViewModel
import com.digitall.eid.utils.SingleLiveEvent
import org.koin.core.component.inject

class CertificateChangePinViewModel : BaseViewModel() {

    companion object {
        private const val TAG = "CardChangePinViewModelTag"

        enum class CertificateChangePinType {
            PIN,
            PIN_CAN
        }
    }

    override var mainTabsEnum: MainTabsEnum? = MainTabsEnum.TAB_EIM

    private val certificateChangePinUiMapper: CertificateChangePinUiMapper by inject()

    private val _adapterListLiveData =
        MutableLiveData<List<CertificateChangePinAdapterMarker>>(emptyList())
    val adapterListLiveData = _adapterListLiveData.readOnly()

    private val _cardScanningLiveData = SingleLiveEvent<CardScanBottomSheetContent>()
    val cardScanningLiveData = _cardScanningLiveData.readOnly()

    private val _changeLocalCertificatePinLiveData = SingleLiveEvent<Unit>()
    val changeLocalCertificatePinLiveData = _changeLocalCertificatePinLiveData.readOnly()

    var state = CertificateChangePinType.PIN
        set(value) {
            val currentItems = _adapterListLiveData.value?.toMutableList()
            when (value) {
                CertificateChangePinType.PIN -> {
                    currentItems?.apply {
                        removeIf { element -> element.elementEnum == CertificateChangePinElementsEnumUi.EDIT_TEXT_CURRENT_CARD_CAN }
                    }
                    can = null
                }

                CertificateChangePinType.PIN_CAN -> {
                    val confirmPinElementIndex =
                        currentItems?.indexOfLast { element -> element.elementEnum == CertificateChangePinElementsEnumUi.EDIT_TEXT_CONFIRM_CARD_PIN }
                    confirmPinElementIndex?.let { index ->
                        if (index != -1) {
                            currentItems.apply {
                                add(
                                    index = index, element = CommonEditTextUi(
                                        elementEnum = CertificateChangePinElementsEnumUi.EDIT_TEXT_CURRENT_CARD_CAN,
                                        required = true,
                                        question = false,
                                        title = CertificateChangePinElementsEnumUi.EDIT_TEXT_CURRENT_CARD_CAN.title,
                                        type = CommonEditTextUiType.PASSWORD_NUMBERS,
                                        selectedValue = null,
                                        maxSymbols = PIN_MAX_LENGTH,
                                        validators = listOf(
                                            NonEmptyEditTextValidator(),
                                            MinLengthEditTextValidator(
                                                minLength = PIN_MAX_LENGTH,
                                                errorMessage = StringSource(
                                                    R.string.error_pin_code_length,
                                                    formatArgs = listOf(PIN_MAX_LENGTH.toString())
                                                )
                                            ),
                                        )
                                    )
                                )
                            }
                        }
                    }
                }
            }

            _adapterListLiveData.value = currentItems

            field = value
        }

    private var currentPin: String? = null
    private var newPin: String? = null
    private var can: String? = null

    override fun onBackPressed() {
        logDebug("onBackPressed", TAG)
        popBackStackToFragment(R.id.certificatesFragment)
    }

    override fun onFirstAttach() {
        showLoader()
        setStartScreenElements()
        hideLoader()
    }

    fun successfulPinChange() {
        showMessage(
            DialogMessage(
                message = StringSource(R.string.change_certificate_pin_card_success_message),
                title = StringSource(R.string.information),
                positiveButtonText = StringSource(R.string.ok),
            )
        )
    }

    override fun onAlertDialogResult(result: AlertDialogResult) {
        if (result.isPositive) {
            onBackPressed()
        }
    }

    private fun setStartScreenElements() {
        _adapterListLiveData.value = certificateChangePinUiMapper.map(Unit)
    }

    fun onEditTextFocusChanged(model: CommonEditTextUi) {
        logDebug("onEditTextFocusChanged hasFocus: ${model.hasFocus}", RegistrationViewModel.TAG)
        if (model.hasFocus.not()) {
            changeValidationField(model)
        }
    }

    fun onEditTextDone(model: CommonEditTextUi) {
        logDebug("onEditTextDone text: ${model.selectedValue}", RegistrationViewModel.TAG)
        changeValidationField(model)
    }

    fun onEditTextChanged(model: CommonEditTextUi) {
        logDebug("onEditTextChanged text: ${model.selectedValue}", RegistrationViewModel.TAG)
        changeValidationField(model)
    }

    fun onCharacterFilter(model: CommonEditTextUi, char: Char): Boolean {
        return when (model.elementEnum) {
            CertificateChangePinElementsEnumUi.EDIT_TEXT_NEW_CARD_PIN,
            CertificateChangePinElementsEnumUi.EDIT_TEXT_CURRENT_CARD_PIN,
            CertificateChangePinElementsEnumUi.EDIT_TEXT_CONFIRM_CARD_PIN,
            CertificateChangePinElementsEnumUi.EDIT_TEXT_CURRENT_CARD_CAN -> char.isDigit()

            else -> true
        }
    }

    fun onButtonClicked(model: CommonButtonUi) {
        if (canSubmitForm()) {
            _adapterListLiveData.value?.filterIsInstance<CommonEditTextUi>()?.forEach { inputField ->
                when (inputField.elementEnum) {
                    CertificateChangePinElementsEnumUi.EDIT_TEXT_CURRENT_CARD_PIN -> currentPin =
                        inputField.selectedValue

                    CertificateChangePinElementsEnumUi.EDIT_TEXT_NEW_CARD_PIN -> newPin =
                        inputField.selectedValue

                    CertificateChangePinElementsEnumUi.EDIT_TEXT_CURRENT_CARD_CAN -> can =
                        inputField.selectedValue
                }
            }

            _cardScanningLiveData.setValueOnMainThread(
                CardScanBottomSheetContent(
                    type = CardScanContentType.ChangePin(
                        cardCurrentPin = currentPin,
                        cardNewPin = newPin,
                        cardCan = can
                    )
                )
            )
        }
    }

    private fun changeValidationField(model: CommonEditTextUi) {
        _adapterListLiveData.value = _adapterListLiveData.value?.map { item ->
            if (item.elementEnum == model.elementEnum) {
                model.copy(validationError = null).apply {
                    triggerValidation()
                }
            } else {
                when (item) {
                    is CommonEditTextUi -> {
                        when {
                            item.elementEnum == CertificateChangePinElementsEnumUi.EDIT_TEXT_CONFIRM_CARD_PIN &&
                                    model.elementEnum == CertificateChangePinElementsEnumUi.EDIT_TEXT_NEW_CARD_PIN -> item.copy(
                                validationError = null,
                                validators = item.validators.toMutableList().apply {
                                    removeAt(lastIndex)
                                    add(
                                        FieldsMatchValidator(
                                            originalFieldTextProvider = { model.selectedValue },
                                            errorMessage = StringSource(R.string.error_pin_code_mismatch)
                                        )
                                    )
                                }
                            ).apply { triggerValidation() }

                            else -> item
                        }

                    }

                    else -> item
                }
            }
        }
    }

    private fun canSubmitForm(): Boolean {
        var allFieldsValid = true
        _adapterListLiveData.value = _adapterListLiveData.value?.map { item ->
            when (item) {
                is CommonEditTextUi -> {
                    item.copy(validationError = null).apply {
                        val itemIsValid = triggerValidation()
                        if (itemIsValid.not()) {
                            allFieldsValid = false
                        }
                    }
                }

                else -> item
            }
        }

        return allFieldsValid
    }
}