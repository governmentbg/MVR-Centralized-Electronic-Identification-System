package com.digitall.eid.ui.fragments.citizen.change.password

import androidx.lifecycle.MutableLiveData
import androidx.lifecycle.viewModelScope
import com.digitall.eid.R
import com.digitall.eid.domain.DELAY_500
import com.digitall.eid.domain.models.base.ErrorType
import com.digitall.eid.domain.models.base.onFailure
import com.digitall.eid.domain.models.base.onLoading
import com.digitall.eid.domain.models.base.onSuccess
import com.digitall.eid.domain.usecase.citizen.update.password.UpdateCitizenPasswordUseCase
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.extensions.launchInScope
import com.digitall.eid.extensions.readOnly
import com.digitall.eid.mappers.citizen.change.password.ChangeCitizenPasswordUiMapper
import com.digitall.eid.models.citizen.change.password.ChangeCitizenPasswordAdapterMarker
import com.digitall.eid.models.citizen.change.password.ChangeCitizenPasswordElementsEnumUi
import com.digitall.eid.models.common.AlertDialogResult
import com.digitall.eid.models.common.DialogMessage
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.common.validator.FieldsMatchValidator
import com.digitall.eid.models.common.validator.NonEmptyEditTextValidator
import com.digitall.eid.models.common.validator.PasswordValidator
import com.digitall.eid.models.list.CommonButtonUi
import com.digitall.eid.models.list.CommonEditTextUi
import com.digitall.eid.models.main.MainTabsEnum
import com.digitall.eid.ui.BaseViewModel
import kotlinx.coroutines.delay
import kotlinx.coroutines.flow.onEach
import org.koin.core.component.inject

class ChangeCitizenPasswordViewModel : BaseViewModel() {

    companion object {
        const val TAG = "ChangeCitizenPasswordViewModelTag"
    }

    override var mainTabsEnum: MainTabsEnum? = MainTabsEnum.TAB_MORE

    private val updateCitizenPasswordUseCase: UpdateCitizenPasswordUseCase by inject()
    private val changeCitizenPasswordUiMapper: ChangeCitizenPasswordUiMapper by inject()

    private val _adapterListLiveData =
        MutableLiveData<List<ChangeCitizenPasswordAdapterMarker>>(emptyList())
    val adapterListLiveData = _adapterListLiveData.readOnly()

    @Volatile
    private var isChangeUserPasswordSuccessful = false

    override fun onBackPressed() {
        logDebug("onBackPressed", TAG)
        popBackStackToFragment(R.id.citizenInformationFragment)
    }

    override fun onFirstAttach() {
        super.onFirstAttach()
        setStartScreenElements()
    }

    override fun onAlertDialogResult(result: AlertDialogResult) {
        if (result.isPositive && isChangeUserPasswordSuccessful) {
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

    fun onEditTextDone(model: CommonEditTextUi) {
        logDebug(
            "onEditTextDone text: ${model.selectedValue}",
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

    fun onButtonClicked(model: CommonButtonUi) {
        when (model.elementEnum) {
            ChangeCitizenPasswordElementsEnumUi.BUTTON_CONFIRM -> {
                if (canSubmitForm()) {
                    changeCitizenPassword()
                }
            }
        }
    }

    private fun setStartScreenElements() {
        _adapterListLiveData.value = changeCitizenPasswordUiMapper.map(Unit)
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
                            item.elementEnum == ChangeCitizenPasswordElementsEnumUi.EDIT_TEXT_CONFIRM_NEW_PASSWORD &&
                                    model.elementEnum == ChangeCitizenPasswordElementsEnumUi.EDIT_TEXT_NEW_PASSWORD -> item.copy(
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

    private fun changeCitizenPassword() {
        var oldPassword: String? = null
        var newPassword: String? = null
        var confirmedNewPassword: String? = null
        _adapterListLiveData.value?.forEach { element ->
            when (element) {
                is CommonEditTextUi -> {
                    when (element.elementEnum) {
                        ChangeCitizenPasswordElementsEnumUi.EDIT_TEXT_OLD_PASSWORD -> oldPassword =
                            element.selectedValue

                        ChangeCitizenPasswordElementsEnumUi.EDIT_TEXT_NEW_PASSWORD -> newPassword =
                            element.selectedValue

                        ChangeCitizenPasswordElementsEnumUi.EDIT_TEXT_CONFIRM_NEW_PASSWORD -> confirmedNewPassword =
                            element.selectedValue
                    }
                }
            }
        }

        updateCitizenPassword(
            oldPassword = oldPassword,
            newPassword = newPassword,
            confirmedPassword = confirmedNewPassword
        )
    }

    private fun updateCitizenPassword(
        oldPassword: String?,
        newPassword: String?,
        confirmedPassword: String?
    ) {
        isChangeUserPasswordSuccessful = false
        updateCitizenPasswordUseCase.invoke(
            oldPassword = oldPassword,
            newPassword = newPassword,
            confirmedPassword = confirmedPassword
        ).onEach { result ->
            result.onLoading {
                logDebug(
                    "onLoading updateCitizenPassword",
                    TAG
                )
                showLoader()
            }.onSuccess { _, _, _ ->
                logDebug("onSuccess updateCitizenPassword", TAG)
                isChangeUserPasswordSuccessful = true
                delay(DELAY_500)
                hideLoader()
                showMessage(
                    DialogMessage(
                        title = StringSource(R.string.information),
                        message = StringSource(R.string.change_user_password_success_message),
                        positiveButtonText = StringSource(R.string.ok)
                    )
                )
            }.onFailure { _, _, message, responseCode, errorType ->
                logDebug("onFailure updateCitizenPassword", TAG)
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