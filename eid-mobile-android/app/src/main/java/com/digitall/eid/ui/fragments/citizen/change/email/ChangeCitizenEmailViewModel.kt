package com.digitall.eid.ui.fragments.citizen.change.email

import androidx.lifecycle.MutableLiveData
import androidx.lifecycle.viewModelScope
import com.digitall.eid.R
import com.digitall.eid.domain.DELAY_500
import com.digitall.eid.domain.models.base.ErrorType
import com.digitall.eid.domain.models.base.onFailure
import com.digitall.eid.domain.models.base.onLoading
import com.digitall.eid.domain.models.base.onSuccess
import com.digitall.eid.domain.usecase.citizen.update.email.UpdateCitizenEmailUseCase
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.extensions.launchInScope
import com.digitall.eid.extensions.readOnly
import com.digitall.eid.mappers.citizen.change.email.ChangeCitizenEmailUiMapper
import com.digitall.eid.models.citizen.change.email.ChangeCitizenEmailAdapterMarker
import com.digitall.eid.models.citizen.change.email.ChangeCitizenEmailElementsEnumUi
import com.digitall.eid.models.common.AlertDialogResult
import com.digitall.eid.models.common.DialogMessage
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonButtonUi
import com.digitall.eid.models.list.CommonEditTextUi
import com.digitall.eid.models.list.CommonValidationFieldUi
import com.digitall.eid.models.main.MainTabsEnum
import com.digitall.eid.ui.BaseViewModel
import kotlinx.coroutines.delay
import kotlinx.coroutines.flow.onEach
import org.koin.core.component.inject
import kotlin.concurrent.Volatile


class ChangeCitizenEmailViewModel : BaseViewModel() {

    companion object {
        const val TAG = "ChangeCitizenEmailViewModelTag"
    }

    override var mainTabsEnum: MainTabsEnum? = MainTabsEnum.TAB_MORE

    private val updateCitizenEmailUseCase: UpdateCitizenEmailUseCase by inject()
    private val changeCitizenEmailUiMapper: ChangeCitizenEmailUiMapper by inject()

    private val _adapterListLiveData =
        MutableLiveData<List<ChangeCitizenEmailAdapterMarker>>()
    val adapterListLiveData = _adapterListLiveData.readOnly()

    @Volatile
    private var isChangeUserMailSuccessful = false

    override fun onBackPressed() {
        logDebug("onBackPressed", TAG)
        popBackStackToFragment(R.id.citizenInformationFragment)
    }

    override fun onAlertDialogResult(result: AlertDialogResult) {
        if (result.isPositive && isChangeUserMailSuccessful) {
            onBackPressed()
        }
    }

    override fun onFirstAttach() {
        super.onFirstAttach()
        setStartScreenElements()
    }

    private fun setStartScreenElements() {
        _adapterListLiveData.value = changeCitizenEmailUiMapper.map(Unit)
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
            ChangeCitizenEmailElementsEnumUi.BUTTON_CONFIRM -> {
                if (canSubmitForm()) {
                    changeCitizenEmail()
                }
            }
        }
    }

    private fun changeValidationField(model: CommonValidationFieldUi<*>) {
        _adapterListLiveData.value = _adapterListLiveData.value?.map { item ->
            if (item.elementEnum == model.elementEnum) {
                when (model) {
                    is CommonEditTextUi -> model.copy(validationError = null).apply {
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

    private fun changeCitizenEmail() {
        var email = ""
        _adapterListLiveData.value?.forEach { element ->
            when (element) {
                is CommonEditTextUi -> {
                    when (element.elementEnum) {
                        ChangeCitizenEmailElementsEnumUi.EDIT_TEXT_EMAIL -> email =
                            element.selectedValue ?: ""
                    }
                }
            }
        }

        updateCitizenEmail(email = email)
    }

    private fun updateCitizenEmail(email: String) {
        isChangeUserMailSuccessful = false
        updateCitizenEmailUseCase.invoke(email = email).onEach { result ->
            result.onLoading {
                logDebug(
                    "onLoading updateCitizenEmail",
                    TAG
                )
                showLoader()
            }.onSuccess { _, _, _ ->
                logDebug("onSuccess updateCitizenEmail", TAG)
                isChangeUserMailSuccessful = true
                delay(DELAY_500)
                hideLoader()
                showMessage(
                    DialogMessage(
                        title = StringSource(R.string.information),
                        message = StringSource(R.string.change_user_email_success_message),
                        positiveButtonText = StringSource(R.string.ok)
                    )
                )
            }.onFailure { _, _, message, responseCode, errorType ->
                logDebug("onFailure updateCitizenEmail", TAG)
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