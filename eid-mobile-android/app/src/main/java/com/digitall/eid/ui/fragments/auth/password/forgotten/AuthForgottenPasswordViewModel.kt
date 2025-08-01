package com.digitall.eid.ui.fragments.auth.password.forgotten

import androidx.lifecycle.MutableLiveData
import androidx.lifecycle.viewModelScope
import com.digitall.eid.R
import com.digitall.eid.domain.DELAY_500
import com.digitall.eid.domain.NAMES_MIN_LENGTH
import com.digitall.eid.domain.models.base.onFailure
import com.digitall.eid.domain.models.base.onLoading
import com.digitall.eid.domain.models.base.onSuccess
import com.digitall.eid.domain.usecase.citizen.forgotten.password.CitizenForgottenPasswordUseCase
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.extensions.isCyrillic
import com.digitall.eid.extensions.launchInScope
import com.digitall.eid.extensions.readOnly
import com.digitall.eid.extensions.setValueOnMainThread
import com.digitall.eid.mappers.auth.password.forgotten.AuthForgottenPasswordUiMapper
import com.digitall.eid.models.auth.password.forgotten.AuthForgottenPasswordAdapterMarker
import com.digitall.eid.models.auth.password.forgotten.AuthForgottenPasswordElementsEnumUi
import com.digitall.eid.models.common.AlertDialogResult
import com.digitall.eid.models.common.DialogMessage
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.common.validator.FirstUpperCasedValidator
import com.digitall.eid.models.common.validator.MinLengthEditTextValidator
import com.digitall.eid.models.common.validator.SharedNameDependencyValidator
import com.digitall.eid.models.list.CommonButtonUi
import com.digitall.eid.models.list.CommonEditTextUi
import com.digitall.eid.ui.BaseViewModel
import com.digitall.eid.utils.SingleLiveEvent
import kotlinx.coroutines.delay
import kotlinx.coroutines.flow.onEach
import org.koin.core.component.inject

class AuthForgottenPasswordViewModel : BaseViewModel() {

    companion object {
        private const val TAG = "AuthForgottenPasswordViewModelTag"
    }

    override val isAuthorizationActive = false

    private val authForgottenPasswordUiMapper: AuthForgottenPasswordUiMapper by inject()
    private val citizenForgottenPasswordUseCase: CitizenForgottenPasswordUseCase by inject()

    private val _adapterListLiveData =
        MutableLiveData<List<AuthForgottenPasswordAdapterMarker>>(emptyList())
    val adapterListLiveData = _adapterListLiveData.readOnly()

    @Volatile
    private var isChangePasswordSuccessful = false

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
        popBackStackToFragment(R.id.authEnterEmailPasswordFragment)
    }

    override fun onFirstAttach() {
        super.onFirstAttach()
        setStartScreenElements()
    }

    override fun onAlertDialogResult(result: AlertDialogResult) {
        when {
            result.isPositive && isChangePasswordSuccessful -> onBackPressed()
        }
    }

    fun onEditTextFocusChanged(model: CommonEditTextUi) {
        logDebug("onEditTextFocusChanged hasFocus: ${model.hasFocus}", TAG)
        if (model.hasFocus.not()) {
            changeValidationField(model = model)
        }
    }

    fun onEditTextDone(model: CommonEditTextUi) {
        logDebug("onEditTextDone text: ${model.selectedValue}", TAG)
        changeValidationField(model)
    }

    fun onEditTextChanged(model: CommonEditTextUi) {
        logDebug("onEditTextChanged text: ${model.selectedValue}", TAG)
        changeValidationField(model)
    }

    fun onCharacterFilter(model: CommonEditTextUi, char: Char): Boolean {
        return when (model.elementEnum) {
            AuthForgottenPasswordElementsEnumUi.EDIT_TEXT_FORNAME,
            AuthForgottenPasswordElementsEnumUi.EDIT_TEXT_MIDDLENAME,
            AuthForgottenPasswordElementsEnumUi.EDIT_TEXT_SURNAME -> (char.isLetter() && char.isCyrillic()) ||
                    char == '\'' ||
                    char == '-' ||
                    char.isWhitespace()

            else -> true
        }
    }

    fun onButtonClicked(model: CommonButtonUi) {
        logDebug("onButtonClicked", TAG)
        if (canSubmitForm()) {
            changePassword()
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
                            item.elementEnum == AuthForgottenPasswordElementsEnumUi.EDIT_TEXT_MIDDLENAME &&
                                    model.elementEnum == AuthForgottenPasswordElementsEnumUi.EDIT_TEXT_FORNAME -> item.copy(
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
                                                ?.firstOrNull { it.elementEnum == AuthForgottenPasswordElementsEnumUi.EDIT_TEXT_SURNAME }?.selectedValue
                                        }
                                    )
                                )
                            ).apply { triggerValidation() }

                            item.elementEnum == AuthForgottenPasswordElementsEnumUi.EDIT_TEXT_SURNAME &&
                                    model.elementEnum == AuthForgottenPasswordElementsEnumUi.EDIT_TEXT_FORNAME -> item.copy(
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
                                                ?.firstOrNull { it.elementEnum == AuthForgottenPasswordElementsEnumUi.EDIT_TEXT_MIDDLENAME }?.selectedValue
                                        }
                                    )
                                )
                            ).apply { triggerValidation() }

                            (item.elementEnum == AuthForgottenPasswordElementsEnumUi.EDIT_TEXT_MIDDLENAME &&
                                    model.elementEnum == AuthForgottenPasswordElementsEnumUi.EDIT_TEXT_SURNAME) ||
                                    (item.elementEnum == AuthForgottenPasswordElementsEnumUi.EDIT_TEXT_SURNAME &&
                                            model.elementEnum == AuthForgottenPasswordElementsEnumUi.EDIT_TEXT_MIDDLENAME) -> item.copy(
                                validationError = null,
                                validators = listOf(
                                    MinLengthEditTextValidator(minLength = NAMES_MIN_LENGTH),
                                    FirstUpperCasedValidator(),
                                    SharedNameDependencyValidator(
                                        primaryNameProvider = {
                                            _adapterListLiveData.value?.filterIsInstance<CommonEditTextUi>()
                                                ?.firstOrNull { it.elementEnum == AuthForgottenPasswordElementsEnumUi.EDIT_TEXT_FORNAME }?.selectedValue
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
    }


    private fun canSubmitForm(): Boolean {
        var allFieldsValid = true
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

                else -> item
            }
        }

        return allFieldsValid
    }


    private fun setStartScreenElements() {
        _adapterListLiveData.value = authForgottenPasswordUiMapper.map(Unit)
    }

    private fun changePassword() {
        var forname: String? = null
        var middlename: String? = null
        var surname: String? = null
        var email: String? = null
        isChangePasswordSuccessful = false
        _adapterListLiveData.value?.filterIsInstance<CommonEditTextUi>()?.forEach { element ->
            when (element.elementEnum) {
                AuthForgottenPasswordElementsEnumUi.EDIT_TEXT_FORNAME -> forname =
                    element.selectedValue

                AuthForgottenPasswordElementsEnumUi.EDIT_TEXT_MIDDLENAME -> middlename =
                    element.selectedValue

                AuthForgottenPasswordElementsEnumUi.EDIT_TEXT_SURNAME -> surname =
                    element.selectedValue

                AuthForgottenPasswordElementsEnumUi.EDIT_TEXT_EMAIL -> email = element.selectedValue
            }
        }

        citizenForgottenPasswordUseCase.invoke(
            forname = forname,
            middlename = middlename,
            surname = surname,
            email = email
        ).onEach { result ->
            result.onLoading {
                logDebug("changePassword onLoading", TAG)
                showLoader()
            }.onSuccess { _, _, _ ->
                logDebug("changePassword onSuccess", TAG)
                delay(DELAY_500)
                hideLoader()
                isChangePasswordSuccessful = true
                showMessage(
                    DialogMessage(
                        title = StringSource(R.string.information),
                        message = StringSource(R.string.forgotten_password_success_message),
                        positiveButtonText = StringSource(R.string.ok)
                    )
                )
            }.onFailure { _, _, message, responseCode, _ ->
                logDebug("changePassword onFailure", TAG)
                delay(DELAY_500)
                hideLoader()
                showMessage(
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
        }.launchInScope(viewModelScope)
    }
}