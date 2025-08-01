package com.digitall.eid.ui.fragments.registration

import androidx.lifecycle.MutableLiveData
import androidx.lifecycle.viewModelScope
import com.digitall.eid.R
import com.digitall.eid.domain.BG_COUNTRY_CODE
import com.digitall.eid.domain.DELAY_500
import com.digitall.eid.domain.NAMES_MIN_LENGTH
import com.digitall.eid.domain.models.base.ErrorType
import com.digitall.eid.domain.models.base.onFailure
import com.digitall.eid.domain.models.base.onLoading
import com.digitall.eid.domain.models.base.onSuccess
import com.digitall.eid.domain.models.citizen.register.CitizenRegisterNewUserRequestModel
import com.digitall.eid.domain.usecase.citizen.registration.RegistrationRegisterNewCitizenUseCase
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.extensions.isCyrillic
import com.digitall.eid.extensions.isLatin
import com.digitall.eid.extensions.launchInScope
import com.digitall.eid.extensions.readOnly
import com.digitall.eid.extensions.setValueOnMainThread
import com.digitall.eid.mappers.registration.RegistrationUiMapper
import com.digitall.eid.models.registration.RegistrationAdapterMarker
import com.digitall.eid.models.registration.RegistrationElementsEnumUi
import com.digitall.eid.models.common.AlertDialogResult
import com.digitall.eid.models.common.DialogMessage
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.common.validator.FieldsMatchValidator
import com.digitall.eid.models.common.validator.FirstUpperCasedValidator
import com.digitall.eid.models.common.validator.MinLengthEditTextValidator
import com.digitall.eid.models.common.validator.NonEmptyEditTextValidator
import com.digitall.eid.models.common.validator.PasswordValidator
import com.digitall.eid.models.common.validator.SharedNameDependencyValidator
import com.digitall.eid.models.list.CommonButtonUi
import com.digitall.eid.models.list.CommonEditTextUi
import com.digitall.eid.models.list.CommonPhoneTextUi
import com.digitall.eid.ui.BaseViewModel
import com.digitall.eid.utils.SingleLiveEvent
import kotlinx.coroutines.delay
import kotlinx.coroutines.flow.onEach
import org.koin.core.component.inject

class RegistrationViewModel : BaseViewModel() {

    companion object {
        const val TAG = "RegistrationViewModelTag"
    }

    override val isAuthorizationActive = false

    private val registerNewCitizenUseCase: RegistrationRegisterNewCitizenUseCase by inject()
    private val registrationUiMapper: RegistrationUiMapper by inject()

    private val _adapterListLiveData =
        MutableLiveData<List<RegistrationAdapterMarker>>(emptyList())
    val adapterListLiveData = _adapterListLiveData.readOnly()

    @Volatile
    private var isRegistrationSuccessful = false

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
        popBackStackToFragment(R.id.authStartFragment)
    }

    override fun onFirstAttach() {
        super.onFirstAttach()
        setStartScreenElements()
    }

    override fun onAlertDialogResult(result: AlertDialogResult) {
        if (result.isPositive && isRegistrationSuccessful) {
            onBackPressed()
        }
    }

    private fun setStartScreenElements() {
        _adapterListLiveData.value = registrationUiMapper.map(Unit)
    }

    fun onEditTextFocusChanged(model: CommonEditTextUi) {
        logDebug("onEditTextFocusChanged hasFocus: ${model.hasFocus}", TAG)
        if (model.hasFocus.not()) {
            changeEditText(model)
        }
    }

    fun onPhoneTextFocusChanged(model: CommonPhoneTextUi) {
        logDebug("onPhoneTextFocusChanged hasFocus: ${model.hasFocus}", TAG)
        if (model.hasFocus.not()) {
            changePhoneText(model)
        }
    }

    fun onEditTextDone(model: CommonEditTextUi) {
        logDebug("onEditTextDone text: ${model.selectedValue}", TAG)
        changeEditText(model)
    }

    fun onPhoneTextDone(model: CommonPhoneTextUi) {
        logDebug("onPhoneTextDone text: ${model.selectedValue}", TAG)
        changePhoneText(model)
    }

    fun onEditTextChanged(model: CommonEditTextUi) {
        logDebug("onEditTextChanged text: ${model.selectedValue}", TAG)
        changeEditText(model)
    }

    fun onPhoneTextChanged(model: CommonPhoneTextUi) {
        logDebug("onPhoneTextChanged text: ${model.selectedValue}", TAG)
        changePhoneText(model)
    }

    fun onCharacterFilter(model: CommonEditTextUi, char: Char): Boolean {
        return when (model.elementEnum) {
            RegistrationElementsEnumUi.EDIT_TEXT_FORNAME,
            RegistrationElementsEnumUi.EDIT_TEXT_MIDDLENAME,
            RegistrationElementsEnumUi.EDIT_TEXT_SURNAME -> (char.isLetter() && char.isCyrillic()) ||
                    char == '\'' ||
                    char == '-' ||
                    char.isWhitespace()

            RegistrationElementsEnumUi.EDIT_TEXT_FORNAME_LATIN,
            RegistrationElementsEnumUi.EDIT_TEXT_MIDDLENAME_LATIN,
            RegistrationElementsEnumUi.EDIT_TEXT_SURNAME_LATIN -> (char.isLetter() && char.isLatin()) ||
                    char == '\'' ||
                    char == '-' ||
                    char.isWhitespace()

            else -> true

        }
    }

    fun onPhoneCharacterFilter(model: CommonPhoneTextUi, char: Char): Boolean {
        return when (model.elementEnum) {
            RegistrationElementsEnumUi.EDIT_TEXT_PHONE -> when {
                char == '0' -> model.selectedValue?.firstOrNull() != '0'
                else -> char.isDigit() || char == '+'
            }

            else -> true
        }
    }

    fun onButtonClicked(model: CommonButtonUi) {
        if (canSubmitForm()) {
            registerUser()
        }
    }

    private fun changeEditText(model: CommonEditTextUi) {
        _adapterListLiveData.value = _adapterListLiveData.value?.map { item ->
            if (item.elementEnum == model.elementEnum) {
                model.copy(validationError = null).apply {
                    triggerValidation()
                }
            } else {
                when (item) {
                    is CommonEditTextUi -> {
                        when {
                            item.elementEnum == RegistrationElementsEnumUi.EDIT_TEXT_MIDDLENAME &&
                                    model.elementEnum == RegistrationElementsEnumUi.EDIT_TEXT_FORNAME -> item.copy(
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
                                                ?.firstOrNull { it.elementEnum == RegistrationElementsEnumUi.EDIT_TEXT_SURNAME }?.selectedValue
                                        }
                                    )
                                )
                            ).apply { triggerValidation() }

                            item.elementEnum == RegistrationElementsEnumUi.EDIT_TEXT_SURNAME &&
                                    model.elementEnum == RegistrationElementsEnumUi.EDIT_TEXT_FORNAME -> item.copy(
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


                            (item.elementEnum == RegistrationElementsEnumUi.EDIT_TEXT_MIDDLENAME &&
                                    model.elementEnum == RegistrationElementsEnumUi.EDIT_TEXT_SURNAME) ||
                                    (item.elementEnum == RegistrationElementsEnumUi.EDIT_TEXT_SURNAME &&
                                            model.elementEnum == RegistrationElementsEnumUi.EDIT_TEXT_MIDDLENAME) -> item.copy(
                                validationError = null,
                                validators = listOf(
                                    MinLengthEditTextValidator(minLength = NAMES_MIN_LENGTH),
                                    FirstUpperCasedValidator(),
                                    SharedNameDependencyValidator(
                                        primaryNameProvider = {
                                            _adapterListLiveData.value?.filterIsInstance<CommonEditTextUi>()
                                                ?.firstOrNull { it.elementEnum == RegistrationElementsEnumUi.EDIT_TEXT_FORNAME }?.selectedValue
                                        },
                                        siblingNameProvider = {
                                            model.selectedValue
                                        }
                                    )
                                )
                            ).apply { triggerValidation() }

                            item.elementEnum == RegistrationElementsEnumUi.EDIT_TEXT_MIDDLENAME_LATIN &&
                                    model.elementEnum == RegistrationElementsEnumUi.EDIT_TEXT_FORNAME_LATIN -> item.copy(
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
                                                ?.firstOrNull { it.elementEnum == RegistrationElementsEnumUi.EDIT_TEXT_SURNAME }?.selectedValue
                                        }
                                    )
                                )
                            ).apply { triggerValidation() }

                            item.elementEnum == RegistrationElementsEnumUi.EDIT_TEXT_SURNAME_LATIN &&
                                    model.elementEnum == RegistrationElementsEnumUi.EDIT_TEXT_FORNAME_LATIN -> item.copy(
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

                            (item.elementEnum == RegistrationElementsEnumUi.EDIT_TEXT_MIDDLENAME_LATIN &&
                                    model.elementEnum == RegistrationElementsEnumUi.EDIT_TEXT_SURNAME_LATIN) ||
                                    (item.elementEnum == RegistrationElementsEnumUi.EDIT_TEXT_SURNAME_LATIN &&
                                            model.elementEnum == RegistrationElementsEnumUi.EDIT_TEXT_MIDDLENAME_LATIN) -> item.copy(
                                validationError = null,
                                validators = listOf(
                                    MinLengthEditTextValidator(minLength = NAMES_MIN_LENGTH),
                                    FirstUpperCasedValidator(),
                                    SharedNameDependencyValidator(
                                        primaryNameProvider = {
                                            _adapterListLiveData.value?.filterIsInstance<CommonEditTextUi>()
                                                ?.firstOrNull { it.elementEnum == RegistrationElementsEnumUi.EDIT_TEXT_FORNAME_LATIN }?.selectedValue
                                        },
                                        siblingNameProvider = {
                                            model.selectedValue
                                        }
                                    )
                                )
                            ).apply { triggerValidation() }

                            item.elementEnum == RegistrationElementsEnumUi.EDIT_TEXT_CONFIRM_PASSWORD &&
                                    model.elementEnum == RegistrationElementsEnumUi.EDIT_TEXT_PASSWORD -> item.copy(
                                validationError = null,
                                validators = listOf(
                                    NonEmptyEditTextValidator(),
                                    PasswordValidator(),
                                    FieldsMatchValidator(originalFieldTextProvider = { model.selectedValue })
                                )
                            ).apply { triggerValidation() }

                            else -> item
                        }

                    }

                    else -> item
                }
            }
        }
    }

    private fun changePhoneText(model: CommonPhoneTextUi) {
        _adapterListLiveData.value = _adapterListLiveData.value?.map { item ->
            if (item.elementEnum == model.elementEnum) {
                model.copy(validationError = null).apply {
                    triggerValidation()
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

    private fun registerUser() {
        var forname: String? = null
        var middlename: String? = null
        var surname: String? = null
        var fornameLatin: String? = null
        var middlenameLatin: String? = null
        var surnameLatin: String? = null
        var email: String? = null
        var password: String? = null
        var phoneNumber: String? = null
        var matchingPassword: String? = null

        _adapterListLiveData.value?.forEach { element ->
            when (element) {
                is CommonEditTextUi -> {
                    when (element.elementEnum) {
                        RegistrationElementsEnumUi.EDIT_TEXT_FORNAME -> forname =
                            element.selectedValue

                        RegistrationElementsEnumUi.EDIT_TEXT_MIDDLENAME -> middlename =
                            element.selectedValue

                        RegistrationElementsEnumUi.EDIT_TEXT_SURNAME -> surname =
                            element.selectedValue

                        RegistrationElementsEnumUi.EDIT_TEXT_FORNAME_LATIN -> fornameLatin =
                            element.selectedValue

                        RegistrationElementsEnumUi.EDIT_TEXT_MIDDLENAME_LATIN -> middlenameLatin =
                            element.selectedValue

                        RegistrationElementsEnumUi.EDIT_TEXT_SURNAME_LATIN -> surnameLatin =
                            element.selectedValue

                        RegistrationElementsEnumUi.EDIT_TEXT_EMAIL -> email =
                            element.selectedValue

                        RegistrationElementsEnumUi.EDIT_TEXT_PASSWORD -> password =
                            element.selectedValue

                        RegistrationElementsEnumUi.EDIT_TEXT_CONFIRM_PASSWORD -> matchingPassword =
                            element.selectedValue
                    }
                }

                is CommonPhoneTextUi -> {
                    when (element.elementEnum) {
                        RegistrationElementsEnumUi.EDIT_TEXT_PHONE -> phoneNumber =
                            if (element.selectedValue.isNullOrEmpty()) null else BG_COUNTRY_CODE + element.selectedValue
                    }
                }
            }
        }

        registerNewCitizenUseCase.invoke(
            data = CitizenRegisterNewUserRequestModel(
                forname = forname?.trim(),
                middlename = middlename?.trim(),
                surname = surname?.trim(),
                fornameLatin = fornameLatin?.trim(),
                middlenameLatin = middlenameLatin?.trim(),
                surnameLatin = surnameLatin?.trim(),
                email = email?.trim(),
                phoneNumber = phoneNumber?.trim(),
                password = password?.trim(),
                matchingPassword = matchingPassword?.trim(),
            ),
        ).onEach { result ->
            result.onLoading {
                logDebug("onLoading registerUser", TAG)
                showLoader()
            }.onSuccess { _, _, _ ->
                logDebug("onSuccess registerUser", TAG)
                delay(DELAY_500)
                hideLoader()
                hideErrorState()
                isRegistrationSuccessful = true
                showMessage(
                    DialogMessage(
                        title = StringSource(R.string.information),
                        message = StringSource(R.string.registration_success_message),
                        positiveButtonText = StringSource(R.string.ok)
                    )
                )
            }.onFailure { _, _, message, responseCode, errorType ->
                logDebug("onFailure registerUser", TAG)
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