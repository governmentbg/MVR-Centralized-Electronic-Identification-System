package com.digitall.eid.ui.fragments.citizen.change.information

import androidx.lifecycle.MutableLiveData
import androidx.lifecycle.viewModelScope
import com.digitall.eid.R
import com.digitall.eid.domain.BG_COUNTRY_CODE
import com.digitall.eid.domain.DELAY_500
import com.digitall.eid.domain.NAMES_MIN_LENGTH
import com.digitall.eid.domain.models.applications.create.ApplicationUserDetailsModel
import com.digitall.eid.domain.models.base.ErrorType
import com.digitall.eid.domain.models.base.onFailure
import com.digitall.eid.domain.models.base.onLoading
import com.digitall.eid.domain.models.base.onSuccess
import com.digitall.eid.domain.models.citizen.update.information.CitizenUpdateInformationRequestModel
import com.digitall.eid.domain.usecase.citizen.update.information.UpdateCitizenInformationUseCase
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.extensions.isCyrillic
import com.digitall.eid.extensions.isLatin
import com.digitall.eid.extensions.launchInScope
import com.digitall.eid.extensions.prependIfMissing
import com.digitall.eid.extensions.readOnly
import com.digitall.eid.extensions.setValueOnMainThread
import com.digitall.eid.mappers.citizen.change.information.ChangeCitizenInformationUiMapper
import com.digitall.eid.models.citizen.change.information.ChangeCitizenInformationAdapterMarker
import com.digitall.eid.models.citizen.change.information.ChangeCitizenInformationElementsEnumUi
import com.digitall.eid.models.common.AlertDialogResult
import com.digitall.eid.models.common.DialogMessage
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.common.validator.FirstUpperCasedValidator
import com.digitall.eid.models.common.validator.MinLengthEditTextValidator
import com.digitall.eid.models.common.validator.SharedNameDependencyValidator
import com.digitall.eid.models.list.CommonButtonUi
import com.digitall.eid.models.list.CommonEditTextUi
import com.digitall.eid.models.list.CommonPhoneTextUi
import com.digitall.eid.models.list.CommonValidationFieldUi
import com.digitall.eid.models.main.MainTabsEnum
import com.digitall.eid.models.registration.RegistrationElementsEnumUi
import com.digitall.eid.ui.BaseViewModel
import com.digitall.eid.utils.SingleLiveEvent
import kotlinx.coroutines.delay
import kotlinx.coroutines.flow.onEach
import org.koin.core.component.inject

class ChangeCitizenInformationViewModel : BaseViewModel() {

    companion object {
        const val TAG = "ChangeCitizenInformationViewModelTag"
    }

    override var mainTabsEnum: MainTabsEnum? = MainTabsEnum.TAB_MORE

    private val updateCitizenInformationUseCase: UpdateCitizenInformationUseCase by inject()
    private val changeCitizenInformationUiMapper: ChangeCitizenInformationUiMapper by inject()

    private val _adapterListLiveData =
        MutableLiveData<List<ChangeCitizenInformationAdapterMarker>>(emptyList())
    val adapterListLiveData = _adapterListLiveData.readOnly()

    private val _citizenInformationChangeLiveData =
        MutableLiveData(false)
    val citizenInformationChangeLiveData = _citizenInformationChangeLiveData.readOnly()

    private lateinit var information: ApplicationUserDetailsModel

    @Volatile
    private var isChangeCitizenInformationSuccessful = false

    private val _scrollToErrorPositionLiveData = SingleLiveEvent<Int>()
    val scrollToErrorPositionLiveData = _scrollToErrorPositionLiveData.readOnly()

    private var errorPosition: Int? = null
        set(value) {
            field = value
            value?.let { position ->
                _scrollToErrorPositionLiveData.setValueOnMainThread(position)
            }
        }

    override fun onBackPressed() {
        logDebug("onBackPressed", TAG)
        popBackStackToFragment(R.id.citizenInformationFragment)
    }

    fun setupModel(information: ApplicationUserDetailsModel) {
        this.information = information
        setStartScreenElements(information = information)
    }

    override fun onAlertDialogResult(result: AlertDialogResult) {
        if (result.isPositive && isChangeCitizenInformationSuccessful) {
            onBackPressed()
        }
    }

    fun onEditTextFocusChanged(model: CommonEditTextUi) {
        logDebug(
            "onEditTextFocusChanged hasFocus: ${model.hasFocus}",
            TAG
        )
        if (model.hasFocus.not()) {
            changeValidationField(model)
        }
    }

    fun onPhoneTextFocusChanged(model: CommonPhoneTextUi) {
        logDebug(
            "onPhoneTextFocusChanged hasFocus: ${model.hasFocus}",
            TAG
        )
        if (model.hasFocus.not()) {
            changeValidationField(model)
        }
    }

    fun onEditTextDone(model: CommonEditTextUi) {
        logDebug(
            "onEditTextDone text: ${model.selectedValue}",
            TAG
        )
        changeValidationField(model)
    }

    fun onPhoneTextDone(model: CommonPhoneTextUi) {
        logDebug(
            "onPhoneTextDone text: ${model.selectedValue}",
            TAG
        )
        changeValidationField(model)
    }

    fun onEditTextChanged(model: CommonEditTextUi) {
        logDebug(
            "onEditTextChanged text: ${model.selectedValue}",
            TAG
        )
        changeValidationField(model)
    }

    fun onPhoneTextChanged(model: CommonPhoneTextUi) {
        logDebug(
            "onPhoneTextChanged text: ${model.selectedValue}",
            TAG
        )
        changeValidationField(model)
    }

    fun onCharacterFilter(model: CommonEditTextUi, char: Char): Boolean {
        return when (model.elementEnum) {
            ChangeCitizenInformationElementsEnumUi.EDIT_TEXT_FORNAME,
            ChangeCitizenInformationElementsEnumUi.EDIT_TEXT_MIDDLENAME,
            ChangeCitizenInformationElementsEnumUi.EDIT_TEXT_SURNAME -> (char.isLetter() && char.isCyrillic()) ||
                    char == '\'' ||
                    char == '-' ||
                    char.isWhitespace()

            ChangeCitizenInformationElementsEnumUi.EDIT_TEXT_FORNAME_LATIN,
            ChangeCitizenInformationElementsEnumUi.EDIT_TEXT_MIDDLENAME_LATIN,
            ChangeCitizenInformationElementsEnumUi.EDIT_TEXT_SURNAME_LATIN -> (char.isLetter() && char.isLatin()) ||
                    char == '\'' ||
                    char == '-' ||
                    char.isWhitespace()

            else -> true
        }
    }

    fun onPhoneCharacterFilter(model: CommonPhoneTextUi, char: Char): Boolean {
        return when (model.elementEnum) {
            ChangeCitizenInformationElementsEnumUi.EDIT_TEXT_PHONE_NUMBER -> when {
                char == '0' -> model.selectedValue?.firstOrNull() != '0'
                else -> char.isDigit() || char == '+'
            }

            else -> true
        }
    }

    fun onButtonClicked(model: CommonButtonUi) {
        when (model.elementEnum) {
            ChangeCitizenInformationElementsEnumUi.BUTTON_CONFIRM -> {
                var forname: String? = null
                var fathersName: String? = null
                var surname: String? = null
                var fornameLatin: String? = null
                var fathersNameLatin: String? = null
                var surnameLatin: String? = null
                var phoneNumber: String? = null
                _adapterListLiveData.value?.forEach { element ->
                    when (element) {
                        is CommonEditTextUi -> {
                            when (element.elementEnum) {
                                ChangeCitizenInformationElementsEnumUi.EDIT_TEXT_FORNAME -> forname =
                                    element.selectedValue

                                ChangeCitizenInformationElementsEnumUi.EDIT_TEXT_MIDDLENAME -> fathersName =
                                    element.selectedValue

                                ChangeCitizenInformationElementsEnumUi.EDIT_TEXT_SURNAME -> surname =
                                    element.selectedValue

                                ChangeCitizenInformationElementsEnumUi.EDIT_TEXT_FORNAME_LATIN -> fornameLatin =
                                    element.selectedValue

                                ChangeCitizenInformationElementsEnumUi.EDIT_TEXT_MIDDLENAME_LATIN -> fathersNameLatin =
                                    element.selectedValue

                                ChangeCitizenInformationElementsEnumUi.EDIT_TEXT_SURNAME_LATIN -> surnameLatin =
                                    element.selectedValue
                            }
                        }

                        is CommonPhoneTextUi -> {
                            when (element.elementEnum) {
                                ChangeCitizenInformationElementsEnumUi.EDIT_TEXT_PHONE_NUMBER -> phoneNumber =
                                    BG_COUNTRY_CODE + element.selectedValue
                            }
                        }
                    }
                }
                updateCitizenInformation(
                    data = CitizenUpdateInformationRequestModel(
                        firstName = forname,
                        secondName = fathersName,
                        lastName = surname,
                        firstNameLatin = fornameLatin,
                        secondNameLatin = fathersNameLatin,
                        lastNameLatin = surnameLatin,
                        phoneNumber = phoneNumber,
                        twoFaEnabled = information.twoFaEnabled ?: false
                    )
                )
            }
        }
    }

    private fun setStartScreenElements(information: ApplicationUserDetailsModel) {
        _adapterListLiveData.value = changeCitizenInformationUiMapper.map(from = information)
    }

    private fun changeValidationField(model: CommonValidationFieldUi<*>) {
        _adapterListLiveData.value = _adapterListLiveData.value?.map { item ->
            if (item.elementEnum == model.elementEnum) {
                when (model) {
                    is CommonEditTextUi -> model.copy(validationError = null).apply {
                        triggerValidation()
                    }

                    is CommonPhoneTextUi -> model.copy(validationError = null).apply {
                        triggerValidation()
                    }

                    else -> item
                }
            } else {
                when {
                    ((item is CommonEditTextUi) && (model is CommonEditTextUi)) -> {
                        when {
                            item.elementEnum == ChangeCitizenInformationElementsEnumUi.EDIT_TEXT_MIDDLENAME &&
                                    model.elementEnum == ChangeCitizenInformationElementsEnumUi.EDIT_TEXT_FORNAME -> item.copy(
                                validationError = null,
                                validators = listOf(
                                    MinLengthEditTextValidator(minLength = NAMES_MIN_LENGTH),
                                    FirstUpperCasedValidator(),
                                    SharedNameDependencyValidator(
                                        primaryNameProvider = {
                                            model.selectedValue
                                        },
                                        siblingNameProvider = {
                                            _adapterListLiveData.value?.filterIsInstance<CommonEditTextUi>()
                                                ?.firstOrNull { it.elementEnum == ChangeCitizenInformationElementsEnumUi.EDIT_TEXT_SURNAME }?.selectedValue
                                        }
                                    )
                                )
                            ).apply { triggerValidation() }

                            item.elementEnum == ChangeCitizenInformationElementsEnumUi.EDIT_TEXT_SURNAME &&
                                    model.elementEnum == ChangeCitizenInformationElementsEnumUi.EDIT_TEXT_FORNAME -> item.copy(
                                validationError = null,
                                validators = listOf(
                                    MinLengthEditTextValidator(minLength = NAMES_MIN_LENGTH),
                                    FirstUpperCasedValidator(),
                                    SharedNameDependencyValidator(
                                        primaryNameProvider = {
                                            model.selectedValue
                                        },
                                        siblingNameProvider = {
                                            _adapterListLiveData.value?.filterIsInstance<CommonEditTextUi>()
                                                ?.firstOrNull { it.elementEnum == RegistrationElementsEnumUi.EDIT_TEXT_MIDDLENAME }?.selectedValue
                                        }
                                    )
                                )
                            ).apply { triggerValidation() }


                            (item.elementEnum == ChangeCitizenInformationElementsEnumUi.EDIT_TEXT_MIDDLENAME &&
                                    model.elementEnum == ChangeCitizenInformationElementsEnumUi.EDIT_TEXT_SURNAME) ||
                                    (item.elementEnum == ChangeCitizenInformationElementsEnumUi.EDIT_TEXT_SURNAME &&
                                            model.elementEnum == ChangeCitizenInformationElementsEnumUi.EDIT_TEXT_MIDDLENAME) -> item.copy(
                                validationError = null,
                                validators = listOf(
                                    MinLengthEditTextValidator(minLength = NAMES_MIN_LENGTH),
                                    FirstUpperCasedValidator(),
                                    SharedNameDependencyValidator(
                                        primaryNameProvider = {
                                            _adapterListLiveData.value?.filterIsInstance<CommonEditTextUi>()
                                                ?.firstOrNull { it.elementEnum == ChangeCitizenInformationElementsEnumUi.EDIT_TEXT_FORNAME }?.selectedValue
                                        },
                                        siblingNameProvider = {
                                            model.selectedValue
                                        }
                                    )
                                )
                            ).apply { triggerValidation() }

                            item.elementEnum == ChangeCitizenInformationElementsEnumUi.EDIT_TEXT_MIDDLENAME_LATIN &&
                                    model.elementEnum == ChangeCitizenInformationElementsEnumUi.EDIT_TEXT_FORNAME_LATIN -> item.copy(
                                validationError = null,
                                validators = listOf(
                                    MinLengthEditTextValidator(minLength = NAMES_MIN_LENGTH),
                                    FirstUpperCasedValidator(),
                                    SharedNameDependencyValidator(
                                        primaryNameProvider = {
                                            model.selectedValue
                                        },
                                        siblingNameProvider = {
                                            _adapterListLiveData.value?.filterIsInstance<CommonEditTextUi>()
                                                ?.firstOrNull { it.elementEnum == ChangeCitizenInformationElementsEnumUi.EDIT_TEXT_SURNAME }?.selectedValue
                                        }
                                    )
                                )
                            ).apply { triggerValidation() }

                            item.elementEnum == ChangeCitizenInformationElementsEnumUi.EDIT_TEXT_SURNAME_LATIN &&
                                    model.elementEnum == ChangeCitizenInformationElementsEnumUi.EDIT_TEXT_FORNAME_LATIN -> item.copy(
                                validationError = null,
                                validators = listOf(
                                    MinLengthEditTextValidator(minLength = NAMES_MIN_LENGTH),
                                    FirstUpperCasedValidator(),
                                    SharedNameDependencyValidator(
                                        primaryNameProvider = {
                                            model.selectedValue
                                        },
                                        siblingNameProvider = {
                                            _adapterListLiveData.value?.filterIsInstance<CommonEditTextUi>()
                                                ?.firstOrNull { it.elementEnum == RegistrationElementsEnumUi.EDIT_TEXT_MIDDLENAME }?.selectedValue
                                        }
                                    )
                                )
                            ).apply { triggerValidation() }

                            (item.elementEnum == ChangeCitizenInformationElementsEnumUi.EDIT_TEXT_MIDDLENAME_LATIN &&
                                    model.elementEnum == ChangeCitizenInformationElementsEnumUi.EDIT_TEXT_SURNAME_LATIN) ||
                                    (item.elementEnum == ChangeCitizenInformationElementsEnumUi.EDIT_TEXT_SURNAME_LATIN &&
                                            model.elementEnum == ChangeCitizenInformationElementsEnumUi.EDIT_TEXT_MIDDLENAME_LATIN) -> item.copy(
                                validationError = null,
                                validators = listOf(
                                    MinLengthEditTextValidator(minLength = NAMES_MIN_LENGTH),
                                    FirstUpperCasedValidator(),
                                    SharedNameDependencyValidator(
                                        primaryNameProvider = {
                                            _adapterListLiveData.value?.filterIsInstance<CommonEditTextUi>()
                                                ?.firstOrNull { it.elementEnum == ChangeCitizenInformationElementsEnumUi.EDIT_TEXT_FORNAME_LATIN }?.selectedValue
                                        },
                                        siblingNameProvider = {
                                            model.selectedValue
                                        }
                                    )
                                )
                            ).apply { triggerValidation() }

                            else -> item
                        }

                    }

                    else -> item
                }
            }
        }
        validateInput()
    }

    private fun validateInput() {
        val namesTextFields = _adapterListLiveData.value?.filterIsInstance<CommonEditTextUi>()
        val phoneTextField =
            _adapterListLiveData.value?.filterIsInstance<CommonPhoneTextUi>()?.first()

        val isInformationChanged = (namesTextFields?.all { element ->
            element.selectedValue == when (element.elementEnum) {
                ChangeCitizenInformationElementsEnumUi.EDIT_TEXT_FORNAME -> information.firstName
                ChangeCitizenInformationElementsEnumUi.EDIT_TEXT_MIDDLENAME -> information.secondName
                ChangeCitizenInformationElementsEnumUi.EDIT_TEXT_SURNAME -> information.lastName
                ChangeCitizenInformationElementsEnumUi.EDIT_TEXT_FORNAME_LATIN -> information.firstNameLatin
                ChangeCitizenInformationElementsEnumUi.EDIT_TEXT_MIDDLENAME_LATIN -> information.secondNameLatin
                ChangeCitizenInformationElementsEnumUi.EDIT_TEXT_SURNAME_LATIN -> information.lastNameLatin

                else -> {}
            }
        } == true).not() || (phoneTextField?.selectedValue?.prependIfMissing(BG_COUNTRY_CODE) == information.phoneNumber).not()

        _adapterListLiveData.value = _adapterListLiveData.value?.map { item ->
            if (item.elementEnum == ChangeCitizenInformationElementsEnumUi.BUTTON_CONFIRM) {
                (item as CommonButtonUi).copy(isEnabled = isInformationChanged && canSubmitForm())
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

                is CommonPhoneTextUi -> {
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

    private fun updateCitizenInformation(data: CitizenUpdateInformationRequestModel) {
        isChangeCitizenInformationSuccessful = false
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
                isChangeCitizenInformationSuccessful = true
                _citizenInformationChangeLiveData.setValueOnMainThread(true)
                delay(DELAY_500)
                hideLoader()
                showMessage(
                    DialogMessage(
                        title = StringSource(R.string.information),
                        message = StringSource(R.string.change_citizen_information_success_message),
                        positiveButtonText = StringSource(R.string.ok)
                    )
                )
            }.onFailure { _, _, message, responseCode, errorType ->
                logDebug("onFailure updateCitizenPhone", TAG)
                delay(DELAY_500)
                hideLoader()
                when (errorType) {
                    ErrorType.AUTHORIZATION -> toLoginFragment()

                    else -> showMessage(
                        DialogMessage(
                            title = StringSource(R.string.information),
                            message = message?.let { StringSource(message) } ?: StringSource(
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