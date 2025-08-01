package com.digitall.eid.ui.fragments.certificates.edit.alias

import androidx.lifecycle.MutableLiveData
import androidx.lifecycle.viewModelScope
import com.digitall.eid.R
import com.digitall.eid.domain.DELAY_500
import com.digitall.eid.domain.models.base.ErrorType
import com.digitall.eid.domain.models.base.onFailure
import com.digitall.eid.domain.models.base.onLoading
import com.digitall.eid.domain.models.base.onSuccess
import com.digitall.eid.domain.usecase.certificates.SetCertificateAliasUseCase
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.extensions.launchInScope
import com.digitall.eid.extensions.readOnly
import com.digitall.eid.extensions.setValueOnMainThread
import com.digitall.eid.mappers.certificates.edit.alias.CertificateEditAliasUiMapper
import com.digitall.eid.models.certificates.edit.alias.CertificateEditAliasAdapterMarker
import com.digitall.eid.models.certificates.edit.alias.CertificateEditAliasElementsEnumUi
import com.digitall.eid.models.common.AlertDialogResult
import com.digitall.eid.models.common.DialogMessage
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonButtonUi
import com.digitall.eid.models.list.CommonEditTextUi
import com.digitall.eid.models.main.MainTabsEnum
import com.digitall.eid.ui.BaseViewModel
import kotlinx.coroutines.delay
import kotlinx.coroutines.flow.onEach
import org.koin.core.component.inject

class CertificateEditAliasViewModel : BaseViewModel() {

    companion object {
        private const val TAG = "CertificateEditNameViewModelTag"
        private const val CERTIFICATE_ALIAS_MAX_LENGTH = 30
    }

    override var mainTabsEnum: MainTabsEnum? = MainTabsEnum.TAB_EIM

    private val setCertificateAliasUseCase: SetCertificateAliasUseCase by inject()
    private val certificateEditAliasUiMapper: CertificateEditAliasUiMapper by inject()

    private val _adapterListLiveData =
        MutableLiveData<List<CertificateEditAliasAdapterMarker>>(emptyList())
    val adapterListLiveData = _adapterListLiveData.readOnly()

    private val _certificateAliasChangeLiveData =
        MutableLiveData(false)
    val certificateAliasChangeLiveData = _certificateAliasChangeLiveData.readOnly()

    private lateinit var certificateId: String
    private var currentCertificateAlias: String? = null

    private var isChangeAliasSuccessful = false

    override fun onBackPressed() {
        logDebug("onBackPressed", TAG)
        popBackStack()
    }

    override fun onAlertDialogResult(result: AlertDialogResult) {
        if (result.isPositive && isChangeAliasSuccessful) {
            onBackPressed()
        }
    }

    fun setupModel(certificateId: String, certificateAlias: String?) {
        logDebug("setupModel certificate id: $certificateId name: $certificateAlias", TAG)
        this.certificateId = certificateId
        currentCertificateAlias = certificateAlias?.trim()
        setStartElements(certificateAlias = certificateAlias)
    }

    fun refreshScreen() = setNewCertificateAlias()

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
            CertificateEditAliasElementsEnumUi.BUTTON_BACK -> onBackPressed()
            CertificateEditAliasElementsEnumUi.BUTTON_CONFIRM -> setNewCertificateAlias()
        }
    }

    private fun setStartElements(certificateAlias: String?) {
        _adapterListLiveData.value = certificateEditAliasUiMapper.map(certificateAlias)
    }

    private fun changeValidationField(model: CommonEditTextUi) {
        _adapterListLiveData.value = _adapterListLiveData.value?.map { item ->
            if (item.elementEnum == model.elementEnum) {
                model.copy(validationError = null).apply {
                    triggerValidation()
                }
            } else {
                item
            }
        }
        validateInput()
    }

    private fun validateInput() {
        val inputTextField = _adapterListLiveData.value?.filterIsInstance<CommonEditTextUi>()?.first()
        val isAliasChanged = inputTextField?.selectedValue?.trim() != currentCertificateAlias

        _adapterListLiveData.value = _adapterListLiveData.value?.map { item ->
            if (item.elementEnum == CertificateEditAliasElementsEnumUi.BUTTON_CONFIRM) {
                (item as CommonButtonUi).copy(isEnabled = isAliasChanged && canSubmitForm())
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

    private fun setNewCertificateAlias() {
        val certificateAlias =
            _adapterListLiveData.value?.filterIsInstance<CommonEditTextUi>()
                ?.first()?.selectedValue?.trim()
        isChangeAliasSuccessful = false
        setCertificateAliasUseCase.invoke(
            id = certificateId,
            alias = certificateAlias ?: return,
        ).onEach { result ->
            result.onLoading {
                logDebug("changeStatus onLoading", TAG)
                showLoader()
            }.onSuccess { _, _, _ ->
                logDebug("changeStatus onSuccess", TAG)
                isChangeAliasSuccessful = true
                _certificateAliasChangeLiveData.setValueOnMainThread(true)
                delay(DELAY_500)
                hideLoader()
                hideErrorState()
                showMessage(
                    DialogMessage(
                        message = StringSource(R.string.certificate_edit_alias_success_message),
                        title = StringSource(R.string.information),
                        positiveButtonText = StringSource(R.string.ok),
                    )
                )
            }.onFailure { _, _, message, responseCode, errorType ->
                logError(
                    "changeStatus onFailure",
                    message, TAG
                )
                hideLoader()
                when (errorType) {
                    ErrorType.AUTHORIZATION -> toLoginFragment()

                    else -> showErrorState(
                        title = StringSource(R.string.information),
                        description = message?.let {
                            StringSource(
                                "$it (%s)",
                                formatArgs = listOf(responseCode.toString())
                            )
                        } ?: StringSource(
                            R.string.error_api_general,
                            formatArgs = listOf(responseCode.toString())
                        ),
                    )
                }
            }
        }.launchInScope(viewModelScope)
    }
}