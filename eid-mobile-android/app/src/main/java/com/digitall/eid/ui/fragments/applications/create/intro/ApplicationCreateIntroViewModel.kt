/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.applications.create.intro

import androidx.lifecycle.MutableLiveData
import androidx.lifecycle.viewModelScope
import com.digitall.eid.R
import com.digitall.eid.domain.APPLICATION_LANGUAGE
import com.digitall.eid.domain.DELAY_500
import com.digitall.eid.domain.DEVICES
import com.digitall.eid.domain.NAMES_MIN_LENGTH
import com.digitall.eid.domain.TimeZones
import com.digitall.eid.domain.ToServerDateFormats
import com.digitall.eid.domain.UiDateFormats
import com.digitall.eid.domain.extensions.atStartOfDay
import com.digitall.eid.domain.extensions.toServerDate
import com.digitall.eid.domain.extensions.toTextDate
import com.digitall.eid.domain.models.administrators.AdministratorFrontOfficeModel
import com.digitall.eid.domain.models.administrators.AdministratorModel
import com.digitall.eid.domain.models.applications.create.ApplicationCreateInitialInformationModel
import com.digitall.eid.domain.models.base.ErrorType
import com.digitall.eid.domain.models.base.onFailure
import com.digitall.eid.domain.models.base.onLoading
import com.digitall.eid.domain.models.base.onSuccess
import com.digitall.eid.domain.models.common.ApplicationLanguage
import com.digitall.eid.domain.models.common.DeviceType
import com.digitall.eid.domain.models.user.UserAcrEnum
import com.digitall.eid.domain.usecase.administrators.GetAdministratorFrontOfficesUseCase
import com.digitall.eid.domain.usecase.applications.create.ApplicationCreateGetInitialInformationUseCase
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.extensions.isLatin
import com.digitall.eid.extensions.launchInScope
import com.digitall.eid.extensions.readOnly
import com.digitall.eid.extensions.setValueOnMainThread
import com.digitall.eid.mappers.applications.create.intro.CreateApplicationIntroUiMapper
import com.digitall.eid.models.applications.create.ApplicationCreateIntroAdapterMarker
import com.digitall.eid.models.applications.create.ApplicationCreateIntroElementsEnumUi
import com.digitall.eid.models.applications.create.ApplicationCreateIntroSigningMethodsEnumUi
import com.digitall.eid.models.applications.create.ApplicationCreatePreviewUi
import com.digitall.eid.models.applications.filter.ApplicationDeviceType
import com.digitall.eid.models.common.ButtonColorUi
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.common.validator.BulgarianCardDocumentWithChipValidator
import com.digitall.eid.models.common.validator.FirstUpperCasedValidator
import com.digitall.eid.models.common.validator.MinLengthEditTextValidator
import com.digitall.eid.models.common.validator.NonEmptyDialogWithSearchItemValidator
import com.digitall.eid.models.common.validator.NonEmptyEditTextValidator
import com.digitall.eid.models.common.validator.NonEmptySpinnerValidator
import com.digitall.eid.models.common.validator.SharedNameDependencyValidator
import com.digitall.eid.models.list.CommonButtonUi
import com.digitall.eid.models.list.CommonDatePickerUi
import com.digitall.eid.models.list.CommonDialogWithSearchItemUi
import com.digitall.eid.models.list.CommonDialogWithSearchUi
import com.digitall.eid.models.list.CommonEditTextUi
import com.digitall.eid.models.list.CommonSeparatorUi
import com.digitall.eid.models.list.CommonSimpleTextUi
import com.digitall.eid.models.list.CommonSpinnerMenuItemUi
import com.digitall.eid.models.list.CommonSpinnerUi
import com.digitall.eid.models.list.CommonTextFieldUi
import com.digitall.eid.models.list.CommonTitleBigUi
import com.digitall.eid.models.list.CommonTitleSmallUi
import com.digitall.eid.models.list.CommonTitleUi
import com.digitall.eid.models.list.CommonValidationFieldUi
import com.digitall.eid.models.main.MainTabsEnum
import com.digitall.eid.ui.BaseViewModel
import com.digitall.eid.utils.SingleLiveEvent
import kotlinx.coroutines.delay
import kotlinx.coroutines.flow.onEach
import org.koin.core.component.inject

class ApplicationCreateIntroViewModel : BaseViewModel() {

    companion object {
        private const val TAG = "ApplicationCreateIntroViewModelTag"
    }

    override var mainTabsEnum: MainTabsEnum? = MainTabsEnum.TAB_EIM

    private val getAdministratorFrontOfficesUseCase: GetAdministratorFrontOfficesUseCase by inject()
    private val getCreateApplicationInitialInformationUseCase: ApplicationCreateGetInitialInformationUseCase by inject()
    private val createApplicationIntroUiMapper: CreateApplicationIntroUiMapper by inject()

    private val _adapterListLiveData =
        MutableLiveData<List<ApplicationCreateIntroAdapterMarker>>(emptyList())
    val adapterListLiveData = _adapterListLiveData.readOnly()

    @Volatile
    private var createInitialInformationModel: ApplicationCreateInitialInformationModel? = null

    private val _scrollToErrorPositionLiveData = SingleLiveEvent<Int>()
    val scrollToErrorPositionLiveData = _scrollToErrorPositionLiveData.readOnly()

    private var errorPosition: Int? = null
        set(value) {
            field = value
            value?.let { position ->
                _scrollToErrorPositionLiveData.setValueOnMainThread(position)
            }
        }

    override fun onFirstAttach() {
        logDebug("onFirstAttach", TAG)
        getCreateApplicationInitialInformation()
    }

    fun refreshScreen() {
        logDebug("refreshScreen", TAG)
        getCreateApplicationInitialInformation()
    }

    private fun getCreateApplicationInitialInformation() {
        getCreateApplicationInitialInformationUseCase.invoke().onEach { result ->
            result.onLoading {
                logDebug("getCreateApplicationInitialInformationUseCase onLoading", TAG)
                showLoader()
            }.onSuccess { model, _, _ ->
                logDebug("getCreateApplicationInitialInformationUseCase onSuccess", TAG)
                createInitialInformationModel = model
                setStartScreenElements(createInitialInformationModel = model)
                delay(DELAY_500)
                hideErrorState()
                hideLoader()
            }.onFailure { _, _, message, responseCode, errorType ->
                logError("getCreateApplicationInitialInformationUseCase onFailure", message, TAG)
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

    private fun setStartScreenElements(createInitialInformationModel: ApplicationCreateInitialInformationModel) {
        logDebug("setStartScreenElements", TAG)
        _adapterListLiveData.postValue(
            createApplicationIntroUiMapper.map(
                from = createInitialInformationModel,
                data = preferences.readApplicationInfo()?.userModel?.acr ?: return
            )
        )
    }

    override fun onDialogElementSelected(model: CommonDialogWithSearchUi) {
        logDebug("onDialogElementSelected", TAG)
        changeValidationField(model)
        when (model.elementEnum) {
            ApplicationCreateIntroElementsEnumUi.DIALOG_ADMINISTRATOR -> {
                val currentItems = _adapterListLiveData.value?.toMutableList()
                val administratorDialogItemIndex =
                    currentItems?.indexOfFirst { it.elementEnum == model.elementEnum }
                administratorDialogItemIndex?.let { index ->
                    val administrator = model.selectedValue?.originalModel as? AdministratorModel
                    val language = APPLICATION_LANGUAGE
                    currentItems[index + 1] = CommonSpinnerUi(
                        required = true,
                        question = false,
                        title = ApplicationCreateIntroElementsEnumUi.SPINNER_DEVICE_TYPE.title,
                        elementEnum = ApplicationCreateIntroElementsEnumUi.SPINNER_DEVICE_TYPE,
                        selectedValue = null,
                        list = DEVICES.filter { deviceModel ->
                            administrator?.deviceIds?.contains(deviceModel.id) == true
                        }.map { deviceModel ->
                            val element = ApplicationDeviceType(
                                type = deviceModel.type ?: "",
                                serverValue = deviceModel.id,
                                title = when (language) {
                                    ApplicationLanguage.BG -> StringSource(
                                        deviceModel.name ?: ""
                                    )

                                    ApplicationLanguage.EN -> StringSource(
                                        deviceModel.description ?: ""
                                    )
                                }
                            )
                            CommonSpinnerMenuItemUi(
                                text = element.title,
                                elementEnum = element,
                                isSelected = false,
                                serverValue = element.serverValue,
                            )
                        },
                        validators = listOf(
                            NonEmptySpinnerValidator()
                        )
                    )

                    _adapterListLiveData.value = currentItems
                }
            }
        }
    }

    override fun onSpinnerSelected(model: CommonSpinnerUi) {
        logDebug("onSpinnerSelected", TAG)
        changeValidationField(model)
        val spinnerItemIndex = _adapterListLiveData.value?.indexOf(model)
        spinnerItemIndex?.let { spinnerIndex ->
            when (model.elementEnum) {
                ApplicationCreateIntroElementsEnumUi.SPINNER_DEVICE_TYPE -> {
                    val administrator =
                        (_adapterListLiveData.value?.get(spinnerIndex - 1) as? CommonDialogWithSearchUi)?.selectedValue?.originalModel as? AdministratorModel
                    getAdministratorFrontOffices(
                        eidAdministratorId = administrator?.id ?: return@let
                    )
                    val user = preferences.readApplicationInfo()?.userModel
                    val currentItems = _adapterListLiveData.value?.toMutableList()
                    val documentNumberItemIndex =
                        currentItems?.indexOfFirst { item -> item.elementEnum == ApplicationCreateIntroElementsEnumUi.EDIT_TEXT_DOCUMENT_NUMBER }
                    documentNumberItemIndex?.let { index ->
                        if (index != -1) {
                            currentItems[index] = when {
                                ((model.selectedValue?.elementEnum as? ApplicationDeviceType)?.type == DeviceType.CHIP_CARD.type ||
                                        user?.acr == UserAcrEnum.HIGH) -> (currentItems[index] as CommonEditTextUi).copy(
                                    validators = listOf(
                                        NonEmptyEditTextValidator(),
                                        BulgarianCardDocumentWithChipValidator()
                                    )
                                ).apply {
                                    triggerValidation()
                                }

                                else -> (currentItems[index] as CommonEditTextUi).copy(
                                    validators = listOf(
                                        NonEmptyEditTextValidator()
                                    )
                                ).apply {
                                    triggerValidation()
                                }
                            }

                            _adapterListLiveData.value = currentItems
                        }
                    }
                }
            }
        }
    }

    override fun onDatePickerChanged(model: CommonDatePickerUi) {
        logDebug("onDatePickerChanged date: ${model.selectedValue}", TAG)
        changeValidationField(model)
    }

    fun onEditTextChanged(model: CommonEditTextUi) {
        logDebug("onEditTextChanged text: ${model.selectedValue}", TAG)
        changeValidationField(model)
    }

    fun onEnterTextDone(model: CommonEditTextUi) {
        logDebug("onEnterTextDone text: ${model.selectedValue}", TAG)
        changeValidationField(model)
    }

    fun onFocusChanged(model: CommonEditTextUi) {
        logDebug("onFocusChanged hasFocus: ${model.hasFocus}", TAG)
        if (model.hasFocus.not()) {
            changeValidationField(model)
        }
    }

    fun onCharacterFilter(model: CommonEditTextUi, char: Char): Boolean {
        return when (model.elementEnum) {
            ApplicationCreateIntroElementsEnumUi.EDIT_TEXT_FIRST_LATIN_NAME,
            ApplicationCreateIntroElementsEnumUi.EDIT_TEXT_SECOND_LATIN_NAME,
            ApplicationCreateIntroElementsEnumUi.EDIT_TEXT_LAST_LATIN_NAME -> (char.isLetter() && char.isLatin()) ||
                    char == '\'' ||
                    char == '-' ||
                    char.isWhitespace()

            ApplicationCreateIntroElementsEnumUi.EDIT_TEXT_DOCUMENT_NUMBER -> (char.isLetter() && char.isLatin()) || char.isDigit()

            else -> true
        }
    }

    private fun changeValidationField(model: CommonValidationFieldUi<*>) {
        _adapterListLiveData.value = _adapterListLiveData.value?.map { item ->
            if (item.elementEnum == model.elementEnum) {
                when (model) {
                    is CommonEditTextUi -> model.copy(validationError = null).apply {
                        triggerValidation()
                    }

                    is CommonDatePickerUi -> model.copy(validationError = null).apply {
                        triggerValidation()
                    }

                    is CommonDialogWithSearchUi -> model.copy(validationError = null).apply {
                        triggerValidation()
                    }

                    is CommonSpinnerUi -> model.copy(validationError = null).apply {
                        triggerValidation()
                    }

                    else -> item
                }
            } else {
                when {
                    (item is CommonEditTextUi) && (model is CommonEditTextUi) -> {
                        when {
                            item.elementEnum == ApplicationCreateIntroElementsEnumUi.EDIT_TEXT_SECOND_LATIN_NAME &&
                                    model.elementEnum == ApplicationCreateIntroElementsEnumUi.EDIT_TEXT_FIRST_LATIN_NAME -> item.copy(
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
                                                ?.firstOrNull { it.elementEnum == ApplicationCreateIntroElementsEnumUi.EDIT_TEXT_LAST_LATIN_NAME }?.selectedValue
                                        }
                                    )
                                )
                            ).apply { triggerValidation() }

                            item.elementEnum == ApplicationCreateIntroElementsEnumUi.EDIT_TEXT_LAST_LATIN_NAME &&
                                    model.elementEnum == ApplicationCreateIntroElementsEnumUi.EDIT_TEXT_FIRST_LATIN_NAME -> item.copy(
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
                                                ?.firstOrNull { it.elementEnum == ApplicationCreateIntroElementsEnumUi.EDIT_TEXT_SECOND_LATIN_NAME }?.selectedValue
                                        }
                                    )
                                )
                            ).apply { triggerValidation() }

                            (item.elementEnum == ApplicationCreateIntroElementsEnumUi.EDIT_TEXT_SECOND_LATIN_NAME &&
                                    model.elementEnum == ApplicationCreateIntroElementsEnumUi.EDIT_TEXT_LAST_LATIN_NAME) ||
                                    (item.elementEnum == ApplicationCreateIntroElementsEnumUi.EDIT_TEXT_LAST_LATIN_NAME &&
                                            model.elementEnum == ApplicationCreateIntroElementsEnumUi.EDIT_TEXT_SECOND_LATIN_NAME) -> item.copy(
                                validationError = null,
                                validators = listOf(
                                    MinLengthEditTextValidator(minLength = NAMES_MIN_LENGTH),
                                    FirstUpperCasedValidator(),
                                    SharedNameDependencyValidator(
                                        primaryNameProvider = {
                                            _adapterListLiveData.value?.filterIsInstance<CommonEditTextUi>()
                                                ?.firstOrNull { it.elementEnum == ApplicationCreateIntroElementsEnumUi.EDIT_TEXT_FIRST_LATIN_NAME }?.selectedValue
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

    fun onButtonClicked(model: CommonButtonUi) {
        logDebug("onButtonClicked", TAG)
        when (model.elementEnum) {
            ApplicationCreateIntroElementsEnumUi.BUTTON_APPLY -> {
                if (canSubmitForm()) {
                    toPreview()
                }
            }

            ApplicationCreateIntroElementsEnumUi.BUTTON_CANCEL -> onBackPressed()

            else -> {
                logError("onButtonClicked unknown button", TAG)
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

    private fun toPreview() {
        logDebug("toPreview", TAG)
        var authMethod: ApplicationCreateIntroSigningMethodsEnumUi? = null
        val user = preferences.readApplicationInfo()?.userModel
        val previewList = buildList {
            _adapterListLiveData.value?.forEach { model ->
                when (model) {
                    is CommonTitleUi -> {
                        logDebug("setupModel add CommonTitleUi", TAG)
                        add(model.copy())
                    }

                    is CommonTextFieldUi -> {
                        logDebug("setupModel add CommonTextFieldUi", TAG)
                        add(model.copy())
                    }

                    is CommonTitleSmallUi -> {
                        logDebug("setupModel add CommonTitleSmallUi", TAG)
                        add(model)
                    }

                    is CommonTitleBigUi -> {
                        logDebug("setupModel add CommonTitleBigUi", TAG)
                        add(model.copy())
                    }

                    is CommonSeparatorUi -> {
                        logDebug("setupModel add CommonSeparatorUi", TAG)
                        add(model.copy())
                    }

                    is CommonSimpleTextUi -> {
                        logDebug("setupModel add CommonSimpleTextUi", TAG)
                        add(model.copy())
                    }

                    is CommonDatePickerUi -> {
                        logDebug("setupModel add CommonDatePickerUi", TAG)
                        if (model.selectedValue == null) {
                            add(
                                CommonTextFieldUi(
                                    serverValue = null,
                                    title = model.title,
                                    required = model.required,
                                    question = model.question,
                                    elementId = model.elementId,
                                    elementEnum = model.elementEnum,
                                    text = StringSource(R.string.no_value),
                                )
                            )
                        } else {
                            add(
                                CommonTextFieldUi(
                                    title = model.title,
                                    required = model.required,
                                    question = model.question,
                                    elementId = model.elementId,
                                    elementEnum = model.elementEnum,
                                    serverValue = model.selectedValue.atStartOfDay().toServerDate(
                                        dateFormat = ToServerDateFormats.ONLY_DATE,
                                        timeZone = TimeZones.DEFAULT,
                                    ),
                                    text = StringSource(
                                        model.selectedValue.toTextDate(
                                            dateFormat = UiDateFormats.WITHOUT_TIME,
                                        )
                                    ),
                                )
                            )
                        }
                    }

                    is CommonDialogWithSearchUi -> {
                        logDebug("setupModel add CommonDialogWithSearchUi", TAG)
                        add(
                            CommonTextFieldUi(
                                title = model.title,
                                required = model.required,
                                question = model.question,
                                elementId = model.elementId,
                                elementEnum = model.elementEnum,
                                serverValue = model.selectedValue?.serverValue,
                                originalModel = model.selectedValue?.originalModel,
                                text = model.selectedValue?.text
                                    ?: StringSource(R.string.no_value),
                            )
                        )
                    }

                    is CommonSpinnerUi -> {
                        logDebug("setupModel add CommonSpinnerUi", TAG)
                        add(
                            CommonTextFieldUi(
                                title = model.title!!,
                                required = model.required!!,
                                question = model.question!!,
                                elementId = model.elementId,
                                elementEnum = model.elementEnum,
                                serverValue = model.selectedValue?.serverValue,
                                originalModel = model.selectedValue?.originalModel,
                                text = model.selectedValue?.text
                                    ?: StringSource(R.string.no_value),
                            )
                        )
                        if (model.elementEnum == ApplicationCreateIntroElementsEnumUi.SPINNER_AUTH_TYPE) {
                            authMethod =
                                model.selectedValue?.elementEnum as? ApplicationCreateIntroSigningMethodsEnumUi
                        }
                    }

                    is CommonEditTextUi -> {
                        logDebug("setupModel add CommonEditTextUi", TAG)
                        add(
                            CommonTextFieldUi(
                                title = model.title,
                                required = model.required,
                                question = model.question,
                                elementId = model.elementId,
                                elementEnum = model.elementEnum,
                                serverValue = model.selectedValue,
                                text = if (model.selectedValue.isNullOrEmpty()) StringSource(R.string.no_value) else StringSource(
                                    model.selectedValue
                                ),
                            )
                        )
                    }
                }
            }
            val buttonType =
                if (user?.acr == UserAcrEnum.LOW) ApplicationCreateIntroElementsEnumUi.BUTTON_SIGN else ApplicationCreateIntroElementsEnumUi.BUTTON_SEND
            add(
                CommonButtonUi(
                    title = buttonType.title,
                    elementEnum = buttonType,
                    buttonColor = ButtonColorUi.BLUE,
                )
            )
            add(
                CommonButtonUi(
                    title = ApplicationCreateIntroElementsEnumUi.BUTTON_EDIT.title,
                    elementEnum = ApplicationCreateIntroElementsEnumUi.BUTTON_EDIT,
                    buttonColor = ButtonColorUi.TRANSPARENT,
                )
            )
        }
        navigateInFlow(
            ApplicationCreateIntroFragmentDirections.toApplicationCreatePreviewFragment(
                model = ApplicationCreatePreviewUi(
                    previewList = previewList,
                    userModel = createInitialInformationModel?.userModel ?: return,
                    signMethod = authMethod,
                )
            )
        )
    }

    private fun getAdministratorFrontOffices(eidAdministratorId: String) {
        getAdministratorFrontOfficesUseCase.invoke(eidAdministratorId = eidAdministratorId)
            .onEach { result ->
                result.onLoading {
                    logDebug("getAdministratorFrontOffices onLoading", TAG)
                }.onSuccess { model, _, _ ->
                    logDebug("getAdministratorFrontOffices onSuccess", TAG)
                    setupAdministratorFrontOffices(frontOffices = model.filter { it.active == true })
                }.onFailure { _, _, message, responseCode, errorType ->
                    logError("getAdministratorFrontOffices onFailure", message, TAG)
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

    private fun setupAdministratorFrontOffices(frontOffices: List<AdministratorFrontOfficeModel>) {
        _adapterListLiveData.postValue(_adapterListLiveData.value?.map { item ->
            if (item.elementEnum == ApplicationCreateIntroElementsEnumUi.DIALOG_ADMINISTRATOR_OFFICE) {
                CommonDialogWithSearchUi(
                    required = true,
                    question = false,
                    title = ApplicationCreateIntroElementsEnumUi.DIALOG_ADMINISTRATOR_OFFICE.title,
                    elementEnum = ApplicationCreateIntroElementsEnumUi.DIALOG_ADMINISTRATOR_OFFICE,
                    selectedValue = null,
                    list = frontOffices
                        .map { frontOffice ->
                            CommonDialogWithSearchItemUi(
                                serverValue = frontOffice.id,
                                originalModel = frontOffice,
                                text = StringSource(frontOffice.name ?: ""),
                            )
                        }.takeIf { list -> list.isNotEmpty() } ?: listOf(
                        CommonDialogWithSearchItemUi(
                            text = StringSource(R.string.no_search_results),
                            selectable = false
                        )
                    ),
                    validators = listOf(
                        NonEmptyDialogWithSearchItemValidator()
                    )
                )
            } else {
                item
            }
        })
    }


    override fun onBackPressed() {
        logDebug("onBackPressed", TAG)
        backToTab()
    }

}