/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.empowerment.create.create

import androidx.lifecycle.MutableLiveData
import androidx.lifecycle.viewModelScope
import com.digitall.eid.R
import com.digitall.eid.domain.DELAY_500
import com.digitall.eid.domain.NAMES_MIN_LENGTH
import com.digitall.eid.domain.TimeZones
import com.digitall.eid.domain.ToServerDateFormats
import com.digitall.eid.domain.UiDateFormats
import com.digitall.eid.domain.extensions.atStartOfDay
import com.digitall.eid.domain.extensions.getCalendar
import com.digitall.eid.domain.extensions.getEnumValue
import com.digitall.eid.domain.extensions.minusDays
import com.digitall.eid.domain.extensions.minusYears
import com.digitall.eid.domain.extensions.plusDays
import com.digitall.eid.domain.extensions.plusYears
import com.digitall.eid.domain.extensions.toServerDate
import com.digitall.eid.domain.extensions.toTextDate
import com.digitall.eid.domain.models.base.ErrorType
import com.digitall.eid.domain.models.base.onFailure
import com.digitall.eid.domain.models.base.onLoading
import com.digitall.eid.domain.models.base.onSuccess
import com.digitall.eid.domain.models.empowerment.common.all.EmpowermentItem
import com.digitall.eid.domain.models.empowerment.create.EmpowermentProviderModel
import com.digitall.eid.domain.models.empowerment.create.EmpowermentServiceModel
import com.digitall.eid.domain.models.empowerment.create.EmpowermentServiceScopeModel
import com.digitall.eid.domain.usecase.empowerment.create.GetEmpowermentProvidersUseCase
import com.digitall.eid.domain.usecase.empowerment.create.GetEmpowermentServiceScopeUseCase
import com.digitall.eid.domain.usecase.empowerment.create.GetEmpowermentServicesUseCase
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.extensions.isCyrillic
import com.digitall.eid.extensions.isFragmentInBackStack
import com.digitall.eid.extensions.launchInScope
import com.digitall.eid.extensions.readOnly
import com.digitall.eid.extensions.setValueOnMainThread
import com.digitall.eid.mappers.empowerment.create.CreateEmpowermentFromCompanyUiMapper
import com.digitall.eid.mappers.empowerment.create.CreateEmpowermentFromPersonUiMapper
import com.digitall.eid.models.common.ButtonColorUi
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.common.validator.ExistingValueEditTextValidator
import com.digitall.eid.models.common.validator.FirstUpperCasedValidator
import com.digitall.eid.models.common.validator.MinLengthEditTextValidator
import com.digitall.eid.models.common.validator.NonEmptyDialogWithSearchItemValidator
import com.digitall.eid.models.common.validator.NonEmptyDialogWithSearchMultipleItemsValidator
import com.digitall.eid.models.common.validator.NonEmptyEditTextValidator
import com.digitall.eid.models.common.validator.NonEmptySpinnerValidator
import com.digitall.eid.models.common.validator.PersonalIdentifierValidator
import com.digitall.eid.models.empowerment.common.filter.EmpowermentOnBehalfOf
import com.digitall.eid.models.empowerment.create.create.EmpowermentCreateAdapterMarker
import com.digitall.eid.models.empowerment.create.create.EmpowermentCreateElementsEnumUi
import com.digitall.eid.models.empowerment.create.create.EmpowermentCreateFromNameOfEnumUi
import com.digitall.eid.models.empowerment.create.create.EmpowermentCreateIdTypeEnumUi
import com.digitall.eid.models.empowerment.create.create.EmpowermentCreatePreviewUiModel
import com.digitall.eid.models.empowerment.create.create.EmpowermentCreateUiModel
import com.digitall.eid.models.list.CommonButtonTransparentUi
import com.digitall.eid.models.list.CommonButtonUi
import com.digitall.eid.models.list.CommonDatePickerUi
import com.digitall.eid.models.list.CommonDialogWithSearchItemUi
import com.digitall.eid.models.list.CommonDialogWithSearchMultiselectItemUi
import com.digitall.eid.models.list.CommonDialogWithSearchMultiselectUi
import com.digitall.eid.models.list.CommonDialogWithSearchUi
import com.digitall.eid.models.list.CommonEditTextUi
import com.digitall.eid.models.list.CommonEditTextUiType
import com.digitall.eid.models.list.CommonSeparatorUi
import com.digitall.eid.models.list.CommonSpinnerMenuItemUi
import com.digitall.eid.models.list.CommonSpinnerUi
import com.digitall.eid.models.list.CommonTextFieldMultipleUi
import com.digitall.eid.models.list.CommonTextFieldUi
import com.digitall.eid.models.list.CommonTitleSmallUi
import com.digitall.eid.models.list.CommonTitleUi
import com.digitall.eid.models.list.CommonValidationFieldUi
import com.digitall.eid.models.main.MainTabsEnum
import com.digitall.eid.ui.BaseViewModel
import com.digitall.eid.utils.SingleLiveEvent
import kotlinx.coroutines.delay
import kotlinx.coroutines.flow.onEach
import org.koin.core.component.inject

class EmpowermentCreateViewModel : BaseViewModel() {

    companion object {
        private const val TAG = "EmpowermentCreateViewModelTag"
        private const val ADDITIONAL_AUTHORIZED_PERSONS_LIMIT = 9
        private val startElementsList = listOf(
            CommonTitleUi(
                title = StringSource(R.string.empowerment_create_submit_application_title),
            ),
            CommonSeparatorUi(),
            CommonTitleSmallUi(
                title = StringSource(R.string.empowerment_create_applicant_title)
            ),
            CommonSpinnerUi(
                required = true,
                question = false,
                title = EmpowermentCreateElementsEnumUi.SPINNER_ON_BEHALF_OF.title,
                elementEnum = EmpowermentCreateElementsEnumUi.SPINNER_ON_BEHALF_OF,
                selectedValue = null,
                list = EmpowermentCreateFromNameOfEnumUi.entries.map {
                    CommonSpinnerMenuItemUi(
                        isSelected = false,
                        text = it.title,
                        serverValue = it.type,
                        elementEnum = it,
                    )
                },
            )
        )
    }

    override var mainTabsEnum: MainTabsEnum? = MainTabsEnum.TAB_MORE

    private val getEmpowermentServicesUseCase: GetEmpowermentServicesUseCase by inject()
    private val getEmpowermentProvidersUseCase: GetEmpowermentProvidersUseCase by inject()
    private val getEmpowermentServiceScopeUseCase: GetEmpowermentServiceScopeUseCase by inject()
    private val createEmpowermentFromCompanyUiMapper: CreateEmpowermentFromCompanyUiMapper by inject()
    private val createEmpowermentFromPersonUiMapper: CreateEmpowermentFromPersonUiMapper by inject()

    private val _adapterListLiveData =
        MutableLiveData<List<EmpowermentCreateAdapterMarker>>(emptyList())
    val adapterListLiveData = _adapterListLiveData.readOnly()

    private val _showIndefiniteEmpowermentInformationLiveData = SingleLiveEvent<Unit>()
    val showIndefiniteEmpowermentInformationLiveData =
        _showIndefiniteEmpowermentInformationLiveData.readOnly()

    private val _scrollToErrorPositionLiveData = SingleLiveEvent<Int>()
    val scrollToErrorPositionLiveData = _scrollToErrorPositionLiveData.readOnly()

    private var errorPosition: Int? = null
        set(value) {
            field = value
            value?.let { position ->
                _scrollToErrorPositionLiveData.setValueOnMainThread(position)
            }
        }

    private var fromNameOf: EmpowermentCreateFromNameOfEnumUi? = null

    private var empowermentCreateUiModel = EmpowermentCreateUiModel()

    fun setupModel(model: EmpowermentItem) {
        logDebug("setupModel model: $model", TAG)
        if (empowermentCreateUiModel.empowermentItem != null) return
        empowermentCreateUiModel = empowermentCreateUiModel.copy(
            user = preferences.readApplicationInfo()?.userModel,
            empowermentItem = model
        )
        fromNameOf = when (getEnumValue<EmpowermentOnBehalfOf>(model.onBehalfOf ?: "")) {
            EmpowermentOnBehalfOf.INDIVIDUAL -> EmpowermentCreateFromNameOfEnumUi.PERSON
            EmpowermentOnBehalfOf.LEGAL_ENTITY -> EmpowermentCreateFromNameOfEnumUi.COMPANY
            else -> null
        }
        refreshScreen()
    }

    override fun onFirstAttach() {
        logDebug("onFirstAttach", TAG)
        refreshScreen()
    }

    fun refreshScreen() {
        logDebug("refreshScreen", TAG)
        empowermentCreateUiModel = empowermentCreateUiModel.copy(
            user = preferences.readApplicationInfo()?.userModel,
        )
        when (fromNameOf) {
            EmpowermentCreateFromNameOfEnumUi.PERSON -> {
                fromNameOf = EmpowermentCreateFromNameOfEnumUi.PERSON
                _adapterListLiveData.value =
                    createEmpowermentFromPersonUiMapper.map(empowermentCreateUiModel)
                getProviders()
            }

            EmpowermentCreateFromNameOfEnumUi.COMPANY -> {
                fromNameOf = EmpowermentCreateFromNameOfEnumUi.COMPANY
                _adapterListLiveData.value =
                    createEmpowermentFromCompanyUiMapper.map(empowermentCreateUiModel)
                getProviders()
            }

            else -> {
                fromNameOf = null
                _adapterListLiveData.value = startElementsList
            }
        }
    }

    fun getInformationContent(): StringSource {
        return _adapterListLiveData.value?.filterIsInstance<CommonSpinnerUi>()
            ?.firstOrNull { element -> element.elementEnum == EmpowermentCreateElementsEnumUi.SPINNER_ON_BEHALF_OF }
            ?.let { element ->
                when (element.selectedValue?.elementEnum) {
                    EmpowermentCreateFromNameOfEnumUi.PERSON -> StringSource(R.string.bottom_sheet_information_empowerment_creation_individual)
                    EmpowermentCreateFromNameOfEnumUi.COMPANY -> StringSource(R.string.bottom_sheet_information_empowerment_creation_legal_entity)
                    else -> StringSource(R.string.bottom_sheet_information_empowerment_creation)
                }
            } ?: StringSource(R.string.bottom_sheet_information_empowerment_creation)
    }

    private fun getProviders() {
        logDebug("getProviders", TAG)
        getEmpowermentProvidersUseCase.invoke().onEach { result ->
            result.onLoading {
                logDebug("getEmpowermentProvidersUseCase onLoading", TAG)
                showLoader()
            }.onSuccess { model, _, _ ->
                logDebug("getEmpowermentProvidersUseCase onSuccess size: ${model.size}", TAG)
                setupProviders(model)
                delay(DELAY_500)
                hideLoader()
                hideErrorState()
            }.onFailure { _, _, message, responseCode, errorType ->
                logError("getEmpowermentProvidersUseCase onFailure", message, TAG)
                hideLoader()
                when (errorType) {
                    ErrorType.NO_INTERNET_CONNECTION -> showErrorState(
                        title = StringSource(R.string.error_network_not_available),
                        description = StringSource(R.string.error_network_not_available_description),
                    )

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

    private fun setupProviders(providers: List<EmpowermentProviderModel>) {
        logDebug("setupProviders size: ${providers.size}", TAG)
        val selectedProvider =
            if (empowermentCreateUiModel.empowermentItem?.providerId.isNullOrEmpty().not()) {
                providers.firstOrNull {
                    it.id == empowermentCreateUiModel.empowermentItem?.providerId
                }
            } else null
        val providersDialog = CommonDialogWithSearchUi(
            required = true,
            question = false,
            title = EmpowermentCreateElementsEnumUi.DIALOG_SUPPLIER_NAME.title,
            elementEnum = EmpowermentCreateElementsEnumUi.DIALOG_SUPPLIER_NAME,
            selectedValue = selectedProvider?.let {
                CommonDialogWithSearchItemUi(
                    serverValue = it.id,
                    originalModel = it,
                    text = StringSource(it.name ?: ""),
                )
            },
            list = providers.map { provider ->
                CommonDialogWithSearchItemUi(
                    serverValue = provider.id,
                    originalModel = provider,
                    text = StringSource(provider.name ?: ""),
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
        _adapterListLiveData.postValue(
            _adapterListLiveData.value?.map { item ->
                if (item.elementEnum == EmpowermentCreateElementsEnumUi.DIALOG_SUPPLIER_NAME) {
                    providersDialog
                } else {
                    item
                }
            }
        )
    }

    private fun getServices(provideId: String) {
        logDebug("getServices", TAG)
        getEmpowermentServicesUseCase.invoke(
            provideId = provideId,
        ).onEach { result ->
            result.onLoading {
                logDebug("getEmpowermentServicesUseCase onLoading", TAG)
                showLoader()
            }.onSuccess { model, _, _ ->
                logDebug("getEmpowermentServicesUseCase onSuccess size: ${model.size}", TAG)
                setupServices(model)
                delay(DELAY_500)
                hideLoader()
                hideErrorState()
            }.onFailure { _, _, message, responseCode, errorType ->
                logError("getEmpowermentServicesUseCase onFailure", message, TAG)
                hideLoader()
                when (errorType) {
                    ErrorType.NO_INTERNET_CONNECTION -> showErrorState(
                        title = StringSource(R.string.error_network_not_available),
                        description = StringSource(R.string.error_network_not_available_description),
                    )

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

    private fun setupServices(services: List<EmpowermentServiceModel>) {
        logDebug("setupServices size: ${services.size}", TAG)
        val selectedService =
            if (empowermentCreateUiModel.empowermentItem?.serviceId.isNullOrEmpty().not()) {
                services.firstOrNull {
                    it.serviceNumber == empowermentCreateUiModel.empowermentItem?.serviceId
                }
            } else null

        val servicesDialog = CommonDialogWithSearchUi(
            required = true,
            question = false,
            selectedValue = selectedService?.let { service ->
                CommonDialogWithSearchItemUi(
                    serverValue = service.id,
                    originalModel = service,
                    text = StringSource(service.name ?: ""),
                    elementEnum = EmpowermentCreateElementsEnumUi.DIALOG_SERVICE_NAME,
                )
            },
            title = EmpowermentCreateElementsEnumUi.DIALOG_SERVICE_NAME.title,
            elementEnum = EmpowermentCreateElementsEnumUi.DIALOG_SERVICE_NAME,
            list = services.map { service ->
                CommonDialogWithSearchItemUi(
                    serverValue = service.id,
                    originalModel = service,
                    text = StringSource(service.name ?: ""),
                    elementEnum = EmpowermentCreateElementsEnumUi.DIALOG_SERVICE_NAME,
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

        val volumeOfRepresentationField = CommonTextFieldUi(
            required = true,
            question = false,
            text = StringSource(R.string.please_select),
            title = EmpowermentCreateElementsEnumUi.DIALOG_VOLUME_OF_REPRESENTATION.title,
            elementEnum = EmpowermentCreateElementsEnumUi.DIALOG_VOLUME_OF_REPRESENTATION,
        )

        _adapterListLiveData.postValue(
            _adapterListLiveData.value?.map { item ->
                when (item.elementEnum) {
                    EmpowermentCreateElementsEnumUi.DIALOG_SERVICE_NAME -> servicesDialog
                    EmpowermentCreateElementsEnumUi.DIALOG_VOLUME_OF_REPRESENTATION -> volumeOfRepresentationField
                    else -> item
                }
            }
        )
        if (selectedService != null && selectedService.id.isNotEmpty()) {
            getServiceScopes(selectedService.id)
        }
    }

    private fun getServiceScopes(serviceId: String) {
        logDebug("getServiceScopes", TAG)
        getEmpowermentServiceScopeUseCase.invoke(
            serviceId = serviceId
        ).onEach { result ->
            result.onLoading {
                logDebug("getEmpowermentServiceScope onLoading", TAG)
                showLoader()
            }.onSuccess { model, _, _ ->
                logDebug("getEmpowermentServiceScope onSuccess size: ${model.size}", TAG)
                setupServiceScopes(model)
                delay(DELAY_500)
                hideLoader()
                hideErrorState()
            }.onFailure { _, _, message, responseCode, errorType ->
                logError("getEmpowermentServiceScope onFailure", message, TAG)
                hideLoader()
                when (errorType) {
                    ErrorType.NO_INTERNET_CONNECTION -> showErrorState(
                        title = StringSource(R.string.error_network_not_available),
                        description = StringSource(R.string.error_network_not_available_description),
                    )

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

    private fun setupServiceScopes(scopes: List<EmpowermentServiceScopeModel>) {
        logDebug("setupServiceScopes size: ${scopes.size}", TAG)
        val volumeOfRepresentation =
            empowermentCreateUiModel.empowermentItem?.volumeOfRepresentation?.map { element -> element.code }
                ?: emptyList()
        val selectedServiceScopes =
            if (empowermentCreateUiModel.empowermentItem?.volumeOfRepresentation.isNullOrEmpty()
                    .not()
            ) {
                scopes.filter { element -> volumeOfRepresentation.contains(element.code) }
            } else null

        val servicesScopesDialog = CommonDialogWithSearchMultiselectUi(
            required = true,
            question = false,
            selectedValue = selectedServiceScopes?.map { scope ->
                CommonDialogWithSearchMultiselectItemUi(
                    serverValue = scope.id,
                    originalModel = scope,
                    text = StringSource(scope.name ?: ""),
                )
            },
            title = EmpowermentCreateElementsEnumUi.DIALOG_VOLUME_OF_REPRESENTATION.title,
            elementEnum = EmpowermentCreateElementsEnumUi.DIALOG_VOLUME_OF_REPRESENTATION,
            list = scopes.mapIndexed { index, element ->
                CommonDialogWithSearchMultiselectItemUi(
                    elementId = index,
                    serverValue = element.id,
                    originalModel = element,
                    text = StringSource(element.name ?: ""),
                    isSelected = volumeOfRepresentation.contains(element.code)
                )
            }.toMutableList().takeIf { list -> list.isNotEmpty() }?.apply {
                add(
                    0, CommonDialogWithSearchMultiselectItemUi(
                        text = StringSource(R.string.select_all),
                        isSelectAllOption = true
                    )
                )
            } ?: listOf(
                CommonDialogWithSearchMultiselectItemUi(
                    text = StringSource(R.string.no_search_results),
                    selectable = false
                )
            ),
            validators = listOf(
                NonEmptyDialogWithSearchMultipleItemsValidator()
            )
        )
        _adapterListLiveData.postValue(
            _adapterListLiveData.value?.map { item ->
                if (item.elementEnum == EmpowermentCreateElementsEnumUi.DIALOG_VOLUME_OF_REPRESENTATION) {
                    servicesScopesDialog
                } else {
                    item
                }
            }
        )
    }

    fun onEraseClicked(model: EmpowermentCreateAdapterMarker) {
        logDebug("onEraseClicked", TAG)
        val currentItems = _adapterListLiveData.value?.toMutableList() ?: mutableListOf()
        when (model.elementEnum) {
            EmpowermentCreateElementsEnumUi.SPINNER_EMPOWERED_UID_TYPE -> {
                currentItems.removeAll { element ->
                    element.elementId == model.elementId &&
                            listOf(
                                EmpowermentCreateElementsEnumUi.SPINNER_EMPOWERED_UID_TYPE,
                                EmpowermentCreateElementsEnumUi.EDIT_TEXT_EMPOWERED_UID_NUMBER,
                                EmpowermentCreateElementsEnumUi.EDIT_TEXT_EMPOWERED_NAMES
                            ).contains(element.elementEnum)
                }
                addOrRemoveTypeOfEmpowerment()
            }

            EmpowermentCreateElementsEnumUi.SPINNER_LEGAL_REPRESENTATIVE_UID_TYPE -> {
                currentItems.removeAll { element ->
                    element.elementId == model.elementId &&
                            listOf(
                                EmpowermentCreateElementsEnumUi.SPINNER_LEGAL_REPRESENTATIVE_UID_TYPE,
                                EmpowermentCreateElementsEnumUi.EDIT_TEXT_LEGAL_REPRESENTATIVE_UID_NUMBER,
                                EmpowermentCreateElementsEnumUi.EDIT_TEXT_LEGAL_REPRESENTATIVE_NAMES,
                                EmpowermentCreateElementsEnumUi.TEXT_VIEW_LEGAL_REPRESANTATIVE_UID_NUMBER,
                                EmpowermentCreateElementsEnumUi.TEXT_VIEW_LEGAL_REPRESENTATIVE_NAMES
                            ).contains(element.elementEnum)
                }
                addOrRemoveAddAuthorizedPersonsButton()
            }
        }
        _adapterListLiveData.value = currentItems
    }

    override fun onSpinnerSelected(model: CommonSpinnerUi) {
        logDebug("onSpinnerSelected", TAG)
        changeValidationField(model)
        when (model.selectedValue?.elementEnum) {
            EmpowermentCreateFromNameOfEnumUi.PERSON -> {
                fromNameOf = EmpowermentCreateFromNameOfEnumUi.PERSON
                _adapterListLiveData.value =
                    createEmpowermentFromPersonUiMapper.map(empowermentCreateUiModel)
                getProviders()
                return
            }

            EmpowermentCreateFromNameOfEnumUi.COMPANY -> {
                fromNameOf = EmpowermentCreateFromNameOfEnumUi.COMPANY
                _adapterListLiveData.value =
                    createEmpowermentFromCompanyUiMapper.map(empowermentCreateUiModel)
                getProviders()
                return
            }
        }

        val currentItems = _adapterListLiveData.value?.toMutableList() ?: mutableListOf()
        val elementIndex = currentItems.indexOfFirst { element ->
            element.elementEnum == model.elementEnum &&
                    element.elementId == model.elementId
        }
        when (model.elementEnum) {
            EmpowermentCreateElementsEnumUi.SPINNER_EMPOWERED_UID_TYPE -> {
                val allCurrentEmpoweredPeopleIds =
                    currentItems.filter { element ->
                        element.elementEnum == EmpowermentCreateElementsEnumUi.EDIT_TEXT_EMPOWERED_UID_NUMBER
                    }
                        .takeWhile { item -> item is CommonEditTextUi || item is CommonTextFieldUi }
                        .map { item ->
                            when (item) {
                                is CommonEditTextUi -> item.selectedValue
                                is CommonTextFieldUi -> item.serverValue
                                else -> null
                            }
                        }.takeWhile { item -> item.isNullOrBlank().not() }
                if (elementIndex != -1) {
                    currentItems[elementIndex + 1] = CommonEditTextUi(
                        elementId = model.elementId,
                        required = true,
                        question = false,
                        selectedValue = null,
                        maxSymbols = 10,
                        type = CommonEditTextUiType.NUMBERS,
                        title = EmpowermentCreateElementsEnumUi.EDIT_TEXT_EMPOWERED_UID_NUMBER.title,
                        elementEnum = EmpowermentCreateElementsEnumUi.EDIT_TEXT_EMPOWERED_UID_NUMBER,
                        validators = listOf(
                            NonEmptyEditTextValidator(),
                            PersonalIdentifierValidator(),
                            ExistingValueEditTextValidator(
                                existingValuesProvider = { listOf(empowermentCreateUiModel.user?.citizenIdentifier) },
                                errorMessage = StringSource(R.string.error_empowerment_self)
                            ),
                            ExistingValueEditTextValidator(
                                existingValuesProvider = { allCurrentEmpoweredPeopleIds },
                                errorMessage = StringSource(R.string.error_personal_identifier_already_used)
                            )
                        )
                    )
                    currentItems[elementIndex + 2] = CommonEditTextUi(
                        elementId = model.elementId,
                        required = true,
                        question = false,
                        selectedValue = null,
                        title = EmpowermentCreateElementsEnumUi.EDIT_TEXT_EMPOWERED_NAMES.title,
                        elementEnum = EmpowermentCreateElementsEnumUi.EDIT_TEXT_EMPOWERED_NAMES,
                        minSymbols = 3,
                        maxSymbols = 200,
                        type = CommonEditTextUiType.TEXT_INPUT_CAP,
                        validators = listOf(
                            NonEmptyEditTextValidator(),
                            MinLengthEditTextValidator(minLength = NAMES_MIN_LENGTH),
                            FirstUpperCasedValidator(),
                        )
                    )
                }
            }

            EmpowermentCreateElementsEnumUi.SPINNER_LEGAL_REPRESENTATIVE_UID_TYPE -> {
                val allCurrentAuthorizedPeopleIds =
                    currentItems.filter { element ->
                        element.elementEnum == EmpowermentCreateElementsEnumUi.EDIT_TEXT_LEGAL_REPRESENTATIVE_UID_NUMBER ||
                                element.elementEnum == EmpowermentCreateElementsEnumUi.TEXT_VIEW_LEGAL_REPRESANTATIVE_UID_NUMBER
                    }
                        .takeWhile { item -> item is CommonEditTextUi || item is CommonTextFieldUi }
                        .map { item ->
                            when (item) {
                                is CommonEditTextUi -> item.selectedValue
                                is CommonTextFieldUi -> item.serverValue
                                else -> null
                            }
                        }.takeWhile { item -> item.isNullOrBlank().not() }
                if (elementIndex != -1) {
                    currentItems[elementIndex + 1] = CommonEditTextUi(
                        elementId = model.elementId,
                        required = true,
                        question = false,
                        selectedValue = null,
                        maxSymbols = 10,
                        type = CommonEditTextUiType.NUMBERS,
                        title = EmpowermentCreateElementsEnumUi.EDIT_TEXT_LEGAL_REPRESENTATIVE_UID_NUMBER.title,
                        elementEnum = EmpowermentCreateElementsEnumUi.EDIT_TEXT_LEGAL_REPRESENTATIVE_UID_NUMBER,
                        validators = listOf(
                            NonEmptyEditTextValidator(),
                            PersonalIdentifierValidator(),
                            ExistingValueEditTextValidator(
                                existingValuesProvider = { listOf(empowermentCreateUiModel.user?.citizenIdentifier) },
                                errorMessage = StringSource(R.string.error_empowerment_self_as_representative)
                            ),
                            ExistingValueEditTextValidator(
                                existingValuesProvider = {
                                    allCurrentAuthorizedPeopleIds
                                },
                                errorMessage = StringSource(R.string.error_personal_identifier_already_used)
                            )
                        )
                    )
                    currentItems[elementIndex + 2] = CommonEditTextUi(
                        elementId = model.elementId,
                        required = true,
                        question = false,
                        selectedValue = null,
                        minSymbols = 3,
                        maxSymbols = 200,
                        type = CommonEditTextUiType.TEXT_INPUT_CAP,
                        title = EmpowermentCreateElementsEnumUi.EDIT_TEXT_LEGAL_REPRESENTATIVE_NAMES.title,
                        elementEnum = EmpowermentCreateElementsEnumUi.EDIT_TEXT_LEGAL_REPRESENTATIVE_NAMES,
                        validators = listOf(
                            NonEmptyEditTextValidator(),
                            MinLengthEditTextValidator(minLength = NAMES_MIN_LENGTH),
                            FirstUpperCasedValidator(),
                        )
                    )
                }
            }
        }
        _adapterListLiveData.value = currentItems
    }

    override fun onDialogElementSelected(model: CommonDialogWithSearchUi) {
        logDebug("onDialogElementSelected", TAG)
        changeValidationField(model)
        when (model.elementEnum) {
            EmpowermentCreateElementsEnumUi.DIALOG_SUPPLIER_NAME -> {
                logDebug("onDialogElementSelected DIALOG_SUPPLIER_NAME", TAG)
                getServices(
                    provideId = model.selectedValue?.serverValue ?: return
                )
            }

            EmpowermentCreateElementsEnumUi.DIALOG_SERVICE_NAME -> {
                logDebug("onDialogElementSelected DIALOG_SERVICE_NAME", TAG)
                getServiceScopes(
                    serviceId = model.selectedValue?.serverValue ?: return
                )
            }
        }
    }

    override fun onDialogMultiselectSelected(model: CommonDialogWithSearchMultiselectUi) {
        logDebug("onDialogElementSelected", TAG)
        changeValidationField(model)
    }

    override fun onDatePickerChanged(model: CommonDatePickerUi) {
        logDebug("onDatePickerChanged", TAG)
        changeValidationField(model)
        when (model.elementEnum) {
            EmpowermentCreateElementsEnumUi.DATE_PICKER_START_DATE ->
                onDatePickerStartDateChanged(model)

            EmpowermentCreateElementsEnumUi.DATE_PICKER_END_DATE -> {
                onDatePickerEndDateChanged(model)
            }
        }
    }

    private fun onDatePickerStartDateChanged(start: CommonDatePickerUi) {
        logDebug("onDatePickerStartDateChanged start: $start", TAG)
        val currentItems = _adapterListLiveData.value?.toMutableList() ?: mutableListOf()

        currentItems.filterIsInstance<CommonDatePickerUi>().firstOrNull {
            it.elementEnum == EmpowermentCreateElementsEnumUi.DATE_PICKER_END_DATE
        }?.let { end ->
            val needChangeEnd = end.selectedValue != null &&
                    start.selectedValue != null &&
                    start.selectedValue.timeInMillis >= end.selectedValue.timeInMillis
            currentItems[currentItems.indexOf(end)] = end.copy(
                elementId = end.elementId,
                elementEnum = end.elementEnum,
                selectedValue = if (needChangeEnd)
                    start.selectedValue?.plusDays(1)
                else end.selectedValue,
                minDate = if (start.selectedValue != null) start.selectedValue.plusDays(1)
                else getCalendar(),
                maxDate = if (start.selectedValue != null) start.selectedValue.plusYears(100)
                else getCalendar(plusYears = 100),
            )
        }
        _adapterListLiveData.value = currentItems
    }

    private fun onDatePickerEndDateChanged(end: CommonDatePickerUi) {
        logDebug("onDatePickerEndDateChanged end: $end", TAG)
        val currentItems = _adapterListLiveData.value?.toMutableList() ?: mutableListOf()
        currentItems.filterIsInstance<CommonDatePickerUi>().firstOrNull {
            it.elementEnum == EmpowermentCreateElementsEnumUi.DATE_PICKER_START_DATE
        }?.let { start ->
            val needChangeStart = start.selectedValue != null &&
                    end.selectedValue != null &&
                    start.selectedValue.timeInMillis >= end.selectedValue.timeInMillis
            currentItems[currentItems.indexOf(start)] = start.copy(
                elementId = start.elementId,
                elementEnum = start.elementEnum,
                selectedValue = if (needChangeStart)
                    end.selectedValue.minusDays(1)
                else start.selectedValue,
                minDate = if (end.selectedValue != null) end.selectedValue.minusYears(100)
                else getCalendar(),
                maxDate = if (end.selectedValue != null) end.selectedValue.minusDays(1)
                else getCalendar(plusYears = 100),
            )
        }

        _adapterListLiveData.value = currentItems
    }

    fun onButtonClicked(model: CommonButtonUi) {
        logDebug("onButtonClicked", TAG)
        when (model.elementEnum) {
            EmpowermentCreateElementsEnumUi.BUTTON_PREVIEW -> {
                logDebug("onButtonClicked BUTTON_PREVIEW", TAG)
                tryToShowPreview()
            }

            EmpowermentCreateElementsEnumUi.BUTTON_CANCEL -> {
                logDebug("onButtonClicked BUTTON_CANCEL", TAG)
                onBackPressed()
            }

            EmpowermentCreateElementsEnumUi.BUTTON_SEND -> {
                logDebug("onButtonClicked BUTTON_SEND", TAG)
                onBackPressed()
            }

            EmpowermentCreateElementsEnumUi.BUTTON_ADD_LEGAL_REPRESENTATIVE -> addLegalRepresentative()
        }
    }

    private fun tryToShowPreview() {
        logDebug("tryToShowPreview", TAG)
        if (canSubmitForm()) {
            when {
                _adapterListLiveData.value?.filterIsInstance<CommonDatePickerUi>()
                    ?.firstOrNull { element ->
                        element.elementEnum == EmpowermentCreateElementsEnumUi.DATE_PICKER_END_DATE
                    }?.selectedValue == null ->
                    _showIndefiniteEmpowermentInformationLiveData.callOnMainThread()

                else -> toPreview()
            }
        }
    }

    fun onButtonTransparentClicked(model: CommonButtonTransparentUi) {
        logDebug("onCenterButtonClicked", TAG)
        when (model.elementEnum) {
            EmpowermentCreateElementsEnumUi.BUTTON_ADD_EMPOWERED -> addEmpoweredPerson()
        }
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
            EmpowermentCreateElementsEnumUi.EDIT_TEXT_EMPOWERED_NAMES,
            EmpowermentCreateElementsEnumUi.EDIT_TEXT_LEGAL_REPRESENTATIVE_NAMES -> (char.isLetter() && char.isCyrillic()) ||
                    char == '\'' ||
                    char == '-' ||
                    char.isWhitespace()

            else -> true
        }
    }

    private fun changeValidationField(model: CommonValidationFieldUi<*>) {
        logDebug("changeValidationField", TAG)
        _adapterListLiveData.value = _adapterListLiveData.value?.map { item ->
            if (item.elementEnum == model.elementEnum && item.elementId == model.elementId) {
                when (model) {
                    is CommonEditTextUi -> model.copy(validationError = null).apply {
                        triggerValidation()
                    }

                    is CommonDatePickerUi -> model.copy(validationError = null).apply {
                        triggerValidation()
                    }

                    is CommonSpinnerUi -> model.copy(validationError = null).apply {
                        triggerValidation()
                    }

                    is CommonDialogWithSearchUi -> model.copy(validationError = null).apply {
                        triggerValidation()
                    }

                    is CommonDialogWithSearchMultiselectUi -> model.copy(validationError = null)
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

                is CommonDialogWithSearchMultiselectUi -> {
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

    private fun addEmpoweredPerson() {
        logDebug("addEmpoweredPerson", TAG)
        val currentItems = _adapterListLiveData.value?.toMutableList() ?: mutableListOf()
        currentItems.lastOrNull {
            it.elementEnum == EmpowermentCreateElementsEnumUi.SPINNER_EMPOWERED_UID_TYPE
        }?.let { element ->
            val index = currentItems.indexOf(element)
            val nextId = (element.elementId?.plus(1))
            currentItems.add(
                index + 3,
                CommonSpinnerUi(
                    elementId = nextId,
                    required = true,
                    question = false,
                    title = EmpowermentCreateElementsEnumUi.SPINNER_EMPOWERED_UID_TYPE.title,
                    elementEnum = EmpowermentCreateElementsEnumUi.SPINNER_EMPOWERED_UID_TYPE,
                    selectedValue = null,
                    hasEraseButton = true,
                    list = EmpowermentCreateIdTypeEnumUi.entries.map { createIdType ->
                        CommonSpinnerMenuItemUi(
                            isSelected = false,
                            text = createIdType.title,
                            elementEnum = createIdType,
                            serverValue = createIdType.type,
                        )
                    },
                    validators = listOf(
                        NonEmptySpinnerValidator()
                    )
                ),
            )
            currentItems.add(
                index + 4,
                CommonTextFieldUi(
                    elementId = nextId,
                    required = true,
                    question = false,
                    title = EmpowermentCreateElementsEnumUi.EDIT_TEXT_EMPOWERED_UID_NUMBER.title,
                    elementEnum = EmpowermentCreateElementsEnumUi.EDIT_TEXT_EMPOWERED_UID_NUMBER,
                    text = StringSource(R.string.empowerment_create_choose_identifier_type_hint),
                )
            )
            currentItems.add(
                index + 5,
                CommonTextFieldUi(
                    elementId = nextId,
                    required = true,
                    question = false,
                    title = EmpowermentCreateElementsEnumUi.EDIT_TEXT_EMPOWERED_NAMES.title,
                    elementEnum = EmpowermentCreateElementsEnumUi.EDIT_TEXT_EMPOWERED_NAMES,
                    text = StringSource(R.string.empowerment_create_choose_identifier_type_hint),
                )
            )
            addOrRemoveTypeOfEmpowerment()
            _adapterListLiveData.value = currentItems
        }
    }

    private fun addOrRemoveTypeOfEmpowerment() {
        logDebug("addOrRemoveTypeOfEmpowerment", TAG)
        val currentItems = _adapterListLiveData.value?.toMutableList() ?: mutableListOf()
        currentItems.removeIf { element ->
            element.elementEnum == EmpowermentCreateElementsEnumUi.SPINNER_TYPE_OF_EMPOWERMENT
        }
        val countOfEmpowered = currentItems.count { element ->
            element.elementEnum == EmpowermentCreateElementsEnumUi.SPINNER_EMPOWERED_UID_TYPE
        }
        if (countOfEmpowered > 1) {
            val indexOfLast = currentItems.indexOfLast { element ->
                element.elementEnum == EmpowermentCreateElementsEnumUi.SPINNER_EMPOWERED_UID_TYPE
            }
            if (indexOfLast != -1) {
                currentItems.add(
                    indexOfLast + 3,
                    CommonSpinnerUi(
                        required = true,
                        question = false,
                        title = EmpowermentCreateElementsEnumUi.SPINNER_TYPE_OF_EMPOWERMENT.title,
                        elementEnum = EmpowermentCreateElementsEnumUi.SPINNER_TYPE_OF_EMPOWERMENT,
                        selectedValue = null,
                        list = buildList {
                            add(
                                CommonSpinnerMenuItemUi(
                                    id = 1,
                                    serverValue = "1",
                                    isSelected = false,
                                    elementEnum = EmpowermentCreateElementsEnumUi.SPINNER_TYPE_OF_EMPOWERMENT,
                                    text = StringSource(R.string.empowerment_create_empowerment_type_together_enum_type),
                                )
                            )
                            add(
                                CommonSpinnerMenuItemUi(
                                    id = 0,
                                    serverValue = "0",
                                    isSelected = false,
                                    elementEnum = EmpowermentCreateElementsEnumUi.SPINNER_TYPE_OF_EMPOWERMENT,
                                    text = StringSource(R.string.empowerment_create_empowerment_type_separate_enum_type),
                                )
                            )
                        },
                        validators = listOf(
                            NonEmptySpinnerValidator()
                        )
                    )
                )

                _adapterListLiveData.value = currentItems
            }
        }
    }

    private fun addLegalRepresentative() {
        logDebug("addLegalRepresentative", TAG)
        val currentItems = _adapterListLiveData.value?.toMutableList() ?: mutableListOf()
        val index = currentItems.indexOfFirst { element ->
            element.elementEnum == EmpowermentCreateElementsEnumUi.BUTTON_ADD_LEGAL_REPRESENTATIVE
        }
        if (index != -1) {
            val elementId =
                (currentItems.lastOrNull { element -> element.elementEnum == EmpowermentCreateElementsEnumUi.SPINNER_LEGAL_REPRESENTATIVE_UID_TYPE }?.elementId
                    ?: 0).plus(1)

            currentItems.addAll(
                index,
                listOf(
                    CommonSpinnerUi(
                        elementId = elementId,
                        required = true,
                        question = false,
                        title = EmpowermentCreateElementsEnumUi.SPINNER_LEGAL_REPRESENTATIVE_UID_TYPE.title,
                        elementEnum = EmpowermentCreateElementsEnumUi.SPINNER_LEGAL_REPRESENTATIVE_UID_TYPE,
                        selectedValue = null,
                        hasEraseButton = true,
                        list = EmpowermentCreateIdTypeEnumUi.entries.map { createIdType ->
                            CommonSpinnerMenuItemUi(
                                isSelected = false,
                                text = createIdType.title,
                                elementEnum = createIdType,
                                serverValue = createIdType.type,
                            )
                        },
                        validators = listOf(
                            NonEmptySpinnerValidator()
                        )
                    ),
                    CommonTextFieldUi(
                        elementId = elementId,
                        required = true,
                        question = false,
                        title = EmpowermentCreateElementsEnumUi.TEXT_VIEW_LEGAL_REPRESANTATIVE_UID_NUMBER.title,
                        elementEnum = EmpowermentCreateElementsEnumUi.TEXT_VIEW_LEGAL_REPRESANTATIVE_UID_NUMBER,
                        text = StringSource(R.string.empowerment_create_choose_identifier_type_hint),
                    ),
                    CommonTextFieldUi(
                        elementId = elementId,
                        required = true,
                        question = false,
                        title = EmpowermentCreateElementsEnumUi.TEXT_VIEW_LEGAL_REPRESENTATIVE_NAMES.title,
                        elementEnum = EmpowermentCreateElementsEnumUi.TEXT_VIEW_LEGAL_REPRESENTATIVE_NAMES,
                        text = StringSource(R.string.empowerment_create_choose_identifier_type_hint),
                    ),
                ),
            )
            _adapterListLiveData.value = currentItems
            addOrRemoveAddAuthorizedPersonsButton()
        }
    }


    private fun addOrRemoveAddAuthorizedPersonsButton() {
        val currentItems = _adapterListLiveData.value?.toMutableList() ?: mutableListOf()
        val authorizedPeopleCount =
            currentItems.count { it.elementEnum == EmpowermentCreateElementsEnumUi.SPINNER_LEGAL_REPRESENTATIVE_UID_TYPE }

        when {
            authorizedPeopleCount < ADDITIONAL_AUTHORIZED_PERSONS_LIMIT -> {
                val addButtonExists =
                    currentItems.firstOrNull { it.elementEnum == EmpowermentCreateElementsEnumUi.BUTTON_ADD_LEGAL_REPRESENTATIVE } != null

                if (addButtonExists.not()) {
                    val lastInputFieldIndex = currentItems.indexOfLast {
                        listOf(
                            EmpowermentCreateElementsEnumUi.TEXT_VIEW_LEGAL_REPRESENTATIVE_NAMES,
                            EmpowermentCreateElementsEnumUi.EDIT_TEXT_LEGAL_REPRESENTATIVE_NAMES
                        ).contains(it.elementEnum)
                    }

                    if (lastInputFieldIndex != -1) {
                        currentItems.add(
                            lastInputFieldIndex + 1,
                            CommonButtonUi(
                                title = EmpowermentCreateElementsEnumUi.BUTTON_ADD_LEGAL_REPRESENTATIVE.title,
                                elementEnum = EmpowermentCreateElementsEnumUi.BUTTON_ADD_LEGAL_REPRESENTATIVE,
                                buttonColor = ButtonColorUi.GREEN,
                            )
                        )
                    }
                }
            }

            else -> {
                currentItems.removeIf { it.elementEnum == EmpowermentCreateElementsEnumUi.BUTTON_ADD_LEGAL_REPRESENTATIVE }
            }
        }
        _adapterListLiveData.value = currentItems
    }


    fun toPreview() {
        logDebug("toPreview", TAG)
        fromNameOf?.let { fromNameOf ->
            val previewList = buildList {
                add(CommonTitleUi(title = StringSource(R.string.empowerment_create_preview_and_save_button_title)))
                add(CommonSeparatorUi())
                _adapterListLiveData.value?.forEach { model ->
                    when (model) {
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
                                        serverValue = model.selectedValue.atStartOfDay()
                                            .toServerDate(
                                                dateFormat = ToServerDateFormats.WITH_MILLIS,
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

                        is CommonDialogWithSearchMultiselectUi -> {
                            logDebug("setupModel add CommonDialogWithSearchMultiselectUi", TAG)
                            add(
                                CommonTextFieldMultipleUi(
                                    title = model.title,
                                    required = model.required,
                                    question = model.question,
                                    elementId = model.elementId,
                                    elementEnum = model.elementEnum,
                                    serverValues = model.selectedValue?.map { element -> element.serverValue },
                                    originalModels = model.selectedValue?.map { element -> element.originalModel },
                                    text = StringSource(sources = model.selectedValue?.map { element -> element.text }
                                        ?: listOf(StringSource(R.string.no_value)),
                                        separator = ", "),
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
                        }

                        is CommonTextFieldUi -> {
                            logDebug("setupModel add CommonTextFieldUi", TAG)
                            add(model.copy())
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
                                    text = if (model.selectedValue.isNullOrEmpty())
                                        StringSource(R.string.no_value)
                                    else StringSource(model.selectedValue),
                                )
                            )
                        }

                        is CommonTitleSmallUi -> {
                            logDebug("setupModel add CommonTitleSmallUi", TAG)
                            add(model)
                        }
                    }
                }
                add(
                    CommonButtonUi(
                        title = EmpowermentCreateElementsEnumUi.BUTTON_SUBMISSION.title,
                        elementEnum = EmpowermentCreateElementsEnumUi.BUTTON_SUBMISSION,
                        buttonColor = ButtonColorUi.BLUE,
                    )
                )
                add(
                    CommonButtonUi(
                        title = EmpowermentCreateElementsEnumUi.BUTTON_EDIT.title,
                        elementEnum = EmpowermentCreateElementsEnumUi.BUTTON_EDIT,
                        buttonColor = ButtonColorUi.TRANSPARENT,
                    )
                )
            }
            navigateInFlow(
                EmpowermentCreateFragmentDirections.toEmpowermentCreatePreviewFragment(
                    model = EmpowermentCreatePreviewUiModel(
                        fromNameOf = fromNameOf,
                        previewList = previewList,
                    )
                )
            )
        }
    }

    override fun onBackPressed() {
        logDebug("onBackPressed", TAG)
        when {
            findTabNavController().isFragmentInBackStack(R.id.empowermentFromMeFlowFragment) -> popBackStackToFragmentInTab(
                R.id.empowermentFromMeFlowFragment
            )

            else -> popBackStackToFragmentInTab(
                R.id.empowermentFlowFragment
            )
        }
    }

}