/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.empowerment.create.preview

import androidx.lifecycle.viewModelScope
import com.digitall.eid.R
import com.digitall.eid.domain.extensions.readOnly
import com.digitall.eid.domain.models.base.ErrorType
import com.digitall.eid.domain.models.base.onFailure
import com.digitall.eid.domain.models.base.onLoading
import com.digitall.eid.domain.models.base.onSuccess
import com.digitall.eid.domain.models.empowerment.create.EmpowermentCreateAuthorizerUIDModel
import com.digitall.eid.domain.models.empowerment.create.EmpowermentCreateEmpoweredUIDModel
import com.digitall.eid.domain.models.empowerment.create.EmpowermentCreateModel
import com.digitall.eid.domain.models.empowerment.create.EmpowermentCreateVolumeOfRepresentationModel
import com.digitall.eid.domain.models.empowerment.create.EmpowermentProviderModel
import com.digitall.eid.domain.models.empowerment.create.EmpowermentServiceModel
import com.digitall.eid.domain.models.empowerment.create.EmpowermentServiceScopeModel
import com.digitall.eid.domain.usecase.empowerment.create.CreateEmpowermentUseCase
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.extensions.launchInScope
import com.digitall.eid.extensions.launchWithDispatcher
import com.digitall.eid.extensions.readOnly
import com.digitall.eid.models.common.DialogMessage
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.empowerment.create.create.EmpowermentCreateElementsEnumUi
import com.digitall.eid.models.empowerment.create.create.EmpowermentCreateFromNameOfEnumUi
import com.digitall.eid.models.empowerment.create.create.EmpowermentCreatePreviewUiModel
import com.digitall.eid.models.empowerment.create.preview.EmpowermentCreatePreviewAdapterMarker
import com.digitall.eid.models.list.CommonButtonUi
import com.digitall.eid.models.list.CommonTextFieldMultipleUi
import com.digitall.eid.models.list.CommonTextFieldUi
import com.digitall.eid.models.main.MainTabsEnum
import com.digitall.eid.ui.BaseViewModel
import com.digitall.eid.ui.fragments.main.tabs.more.MainTabMoreFragmentDirections
import com.digitall.eid.utils.SingleLiveEvent
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.onEach
import org.koin.core.component.inject

class EmpowermentCreatePreviewViewModel : BaseViewModel() {

    companion object {
        private const val TAG = "EmpowermentCreatePreviewViewModelTag"
    }

    private val createEmpowermentUseCase: CreateEmpowermentUseCase by inject()

    override var mainTabsEnum: MainTabsEnum? = MainTabsEnum.TAB_MORE

    private val _adapterListLiveData =
        MutableStateFlow<List<EmpowermentCreatePreviewAdapterMarker>>(emptyList())
    val adapterListLiveData = _adapterListLiveData.readOnly()

    private val _scrollToPositionLiveData = SingleLiveEvent<Int>()
    val scrollToPositionLiveData = _scrollToPositionLiveData.readOnly()

    private var previewList: List<EmpowermentCreatePreviewAdapterMarker>? = null

    private var fromMameOf: EmpowermentCreateFromNameOfEnumUi? = null

    fun refreshScreen() {
        logDebug("refreshScreen", TAG)
        createEmpowerment()
    }

    fun setupModel(model: EmpowermentCreatePreviewUiModel) {
        logDebug("setupModel", TAG)
        viewModelScope.launchWithDispatcher {
            fromMameOf = model.fromNameOf
            previewList = model.previewList
            _adapterListLiveData.emit(model.previewList)
        }
    }

    fun onButtonClicked(model: CommonButtonUi) {
        logDebug("onButtonClicked", TAG)
        when (model.elementEnum) {
            EmpowermentCreateElementsEnumUi.BUTTON_SUBMISSION -> createEmpowerment()
            EmpowermentCreateElementsEnumUi.BUTTON_EDIT -> onBackPressed()
        }
    }

    private fun createEmpowerment() {
        logDebug("createEmpowerment", TAG)
        when (fromMameOf) {
            EmpowermentCreateFromNameOfEnumUi.PERSON -> fromPerson()
            EmpowermentCreateFromNameOfEnumUi.COMPANY -> fromCompany()
            else -> logError("Error type", TAG)
        }
    }

    private fun fromPerson() {
        logDebug("fromPerson", TAG)
        var uid: String? = null
        var uidType: String? = null
        var serviceId: String? = null
        var startDate: String? = null
        var expiryDate: String? = null
        var onBehalfOf: String? = null
        var providerId: String? = null
        var serviceName: String? = null
        var providerName: String? = null
        var typeOfEmpowerment: String? = null
        val empoweredUids = mutableListOf<EmpowermentCreateEmpoweredUIDModel>()
        val volumeOfRepresentation = mutableListOf<EmpowermentCreateVolumeOfRepresentationModel>()
        previewList?.filterIsInstance<CommonTextFieldUi>()?.forEach {
            when (it.elementEnum) {
                EmpowermentCreateElementsEnumUi.SPINNER_ON_BEHALF_OF -> {
                    onBehalfOf = it.serverValue
                }

                EmpowermentCreateElementsEnumUi.SPINNER_TYPE_OF_EMPOWERMENT -> {
                    typeOfEmpowerment = it.serverValue
                }

                EmpowermentCreateElementsEnumUi.SPINNER_EMPOWERED_UID_TYPE -> {
                    val textViews = previewList?.filterIsInstance<CommonTextFieldUi>()
                    textViews?.firstOrNull { number ->
                        listOf(
                            EmpowermentCreateElementsEnumUi.SPINNER_EMPOWERED_UID_TYPE
                        ).contains(number.elementEnum) &&
                                number.elementId == it.elementId
                    }?.let { number ->
                        val index = textViews.indexOf(number)
                        val (uidTypeEmpowered, uidEmpowered, namesEmpowered) = textViews.slice(
                            index..index + 2
                        )
                        empoweredUids.add(
                            EmpowermentCreateEmpoweredUIDModel(
                                uid = uidEmpowered.serverValue,
                                uidType = uidTypeEmpowered.serverValue,
                                name = namesEmpowered.serverValue
                            )
                        )
                    }
                }

                EmpowermentCreateElementsEnumUi.TEXT_VIEW_UID_TYPE -> {
                    uidType = it.serverValue
                }

                EmpowermentCreateElementsEnumUi.TEXT_VIEW_LEGAL_REPRESANTATIVE_UID_NUMBER -> {
                    uid = it.serverValue
                }

                EmpowermentCreateElementsEnumUi.DIALOG_SUPPLIER_NAME -> {
                    providerId = (it.originalModel as? EmpowermentProviderModel)?.id
                    providerName =
                        (it.originalModel as? EmpowermentProviderModel)?.name
                }

                EmpowermentCreateElementsEnumUi.DIALOG_SERVICE_NAME -> {
                    serviceName = (it.originalModel as? EmpowermentServiceModel)?.name
                    serviceId = (it.originalModel as? EmpowermentServiceModel)?.serviceNumber
                }

                EmpowermentCreateElementsEnumUi.DIALOG_VOLUME_OF_REPRESENTATION -> {
                    volumeOfRepresentation.add(
                        EmpowermentCreateVolumeOfRepresentationModel(
                            code = (it.originalModel as? EmpowermentServiceScopeModel)?.code,
                            name = (it.originalModel as? EmpowermentServiceScopeModel)?.name,
                        )
                    )
                }

                EmpowermentCreateElementsEnumUi.DATE_PICKER_START_DATE -> {
                    startDate = it.serverValue
                }

                EmpowermentCreateElementsEnumUi.DATE_PICKER_END_DATE -> {
                    expiryDate = it.serverValue
                }
            }
        }
        previewList?.filterIsInstance<CommonTextFieldMultipleUi>()?.forEach {
            when (it.elementEnum) {
                EmpowermentCreateElementsEnumUi.DIALOG_VOLUME_OF_REPRESENTATION -> {
                    it.originalModels?.forEach { model ->
                        volumeOfRepresentation.add(
                            EmpowermentCreateVolumeOfRepresentationModel(
                                code = (model as? EmpowermentServiceScopeModel)?.code,
                                name = (model as? EmpowermentServiceScopeModel)?.name,
                            )
                        )
                    }
                }
            }
        }
        val requestModel = EmpowermentCreateModel(
            uid = uid?.trim(),
            name = null,
            uidType = uidType?.trim(),
            issuerPosition = null,
            serviceId = serviceId?.trim(),
            startDate = startDate?.trim(),
            expiryDate = expiryDate?.trim(),
            onBehalfOf = onBehalfOf?.trim(),
            providerId = providerId?.trim(),
            serviceName = serviceName?.trim(),
            providerName = providerName?.trim(),
            typeOfEmpowerment = typeOfEmpowerment?.trim(),
            empoweredUids = if (empoweredUids.isNotEmpty()) empoweredUids.toList() else null,
            authorizerUids = null,
            volumeOfRepresentation = if (volumeOfRepresentation.isNotEmpty()) volumeOfRepresentation.toList() else null,
        )
        sendEmpowerment(requestModel)
    }

    private fun fromCompany() {
        logDebug("fromCompany", TAG)
        var uid: String? = null
        var name: String? = null
        var serviceId: String? = null
        var startDate: String? = null
        var expiryDate: String? = null
        var onBehalfOf: String? = null
        var providerId: String? = null
        var serviceName: String? = null
        var providerName: String? = null
        var issuerPosition: String? = null
        var typeOfEmpowerment: String? = null
        val empoweredUids = mutableListOf<EmpowermentCreateEmpoweredUIDModel>()
        val authorizerUids = mutableListOf<EmpowermentCreateAuthorizerUIDModel>()
        val volumeOfRepresentation = mutableListOf<EmpowermentCreateVolumeOfRepresentationModel>()
        previewList?.filterIsInstance<CommonTextFieldUi>()?.forEach {
            when (it.elementEnum) {
                EmpowermentCreateElementsEnumUi.SPINNER_ON_BEHALF_OF -> {
                    onBehalfOf = it.serverValue
                }

                EmpowermentCreateElementsEnumUi.SPINNER_TYPE_OF_EMPOWERMENT -> {
                    typeOfEmpowerment = it.serverValue
                }

                EmpowermentCreateElementsEnumUi.SPINNER_EMPOWERED_UID_TYPE -> {
                    val textViews = previewList?.filterIsInstance<CommonTextFieldUi>()
                    textViews?.firstOrNull { number ->
                        listOf(
                            EmpowermentCreateElementsEnumUi.SPINNER_EMPOWERED_UID_TYPE,
                            EmpowermentCreateElementsEnumUi.EDIT_TEXT_EMPOWERED_UID_NUMBER
                        ).contains(number.elementEnum) &&
                                number.elementId == it.elementId
                    }?.let { number ->
                        val index = textViews.indexOf(number)
                        val (uidTypeEmpowered, uidEmpowered, namesEmpowered) = textViews.slice(
                            index..index + 2
                        )
                        empoweredUids.add(
                            EmpowermentCreateEmpoweredUIDModel(
                                uid = uidEmpowered.serverValue,
                                uidType = uidTypeEmpowered.serverValue,
                                name = namesEmpowered.serverValue
                            )
                        )
                    }
                }

                EmpowermentCreateElementsEnumUi.TEXT_VIEW_UID_TYPE,
                EmpowermentCreateElementsEnumUi.SPINNER_LEGAL_REPRESENTATIVE_UID_TYPE -> {
                    val textViews = previewList?.filterIsInstance<CommonTextFieldUi>()
                    textViews?.firstOrNull { number ->
                        listOf(
                            EmpowermentCreateElementsEnumUi.TEXT_VIEW_UID_TYPE,
                            EmpowermentCreateElementsEnumUi.SPINNER_LEGAL_REPRESENTATIVE_UID_TYPE
                        ).contains(number.elementEnum) &&
                                number.elementId == it.elementId
                    }?.let { number ->
                        val index = textViews.indexOf(number)
                        val (uidTypeAuthotizer, uidAuthorizer, namesAuthorizer) = textViews.slice(
                            index..index + 2
                        )
                        authorizerUids.add(
                            EmpowermentCreateAuthorizerUIDModel(
                                uid = uidAuthorizer.serverValue,
                                uidType = uidTypeAuthotizer.serverValue,
                                name = namesAuthorizer.serverValue,
                                issuer = false
                            )
                        )
                    }
                }

                EmpowermentCreateElementsEnumUi.EDIT_TEXT_COMPANY_NUMBER -> {
                    uid = it.serverValue
                }

                EmpowermentCreateElementsEnumUi.EDIT_TEXT_COMPANY_NAME -> {
                    name = it.serverValue
                }

                EmpowermentCreateElementsEnumUi.EDIT_TEXT_ISSUER_POSITION -> {
                    issuerPosition = it.serverValue
                }

                EmpowermentCreateElementsEnumUi.DIALOG_SUPPLIER_NAME -> {
                    providerId = (it.originalModel as? EmpowermentProviderModel)?.id
                    providerName =
                        (it.originalModel as? EmpowermentProviderModel)?.name
                }

                EmpowermentCreateElementsEnumUi.DIALOG_SERVICE_NAME -> {
                    serviceName = (it.originalModel as? EmpowermentServiceModel)?.name
                    serviceId = (it.originalModel as? EmpowermentServiceModel)?.serviceNumber
                }

                EmpowermentCreateElementsEnumUi.DATE_PICKER_START_DATE -> {
                    startDate = it.serverValue
                }

                EmpowermentCreateElementsEnumUi.DATE_PICKER_END_DATE -> {
                    expiryDate = it.serverValue
                }
            }
        }
        previewList?.filterIsInstance<CommonTextFieldMultipleUi>()?.forEach { element ->
            when (element.elementEnum) {
                EmpowermentCreateElementsEnumUi.DIALOG_VOLUME_OF_REPRESENTATION -> {
                    element.originalModels?.filterIsInstance<EmpowermentServiceScopeModel>()?.forEach { model ->
                        volumeOfRepresentation.add(
                            EmpowermentCreateVolumeOfRepresentationModel(
                                code = model.code,
                                name = model.name,
                            )
                        )
                    }
                }
            }
        }

        if (authorizerUids.isNotEmpty()) {
            authorizerUids.removeAt(0)
        }
        val requestModel = EmpowermentCreateModel(
            uid = uid?.trim(),
            name = name?.trim(),
            uidType = null,
            serviceId = serviceId?.trim(),
            startDate = startDate?.trim(),
            expiryDate = expiryDate?.trim(),
            onBehalfOf = onBehalfOf?.trim(),
            providerId = providerId?.trim(),
            serviceName = serviceName?.trim(),
            providerName = providerName?.trim(),
            issuerPosition = issuerPosition?.trim(),
            typeOfEmpowerment = typeOfEmpowerment?.trim(),
            empoweredUids = if (empoweredUids.isNotEmpty()) empoweredUids.toList() else null,
            authorizerUids = if (authorizerUids.isNotEmpty()) authorizerUids.toList() else null,
            volumeOfRepresentation = if (volumeOfRepresentation.isNotEmpty()) volumeOfRepresentation.toList() else null,
        )
        sendEmpowerment(requestModel)
    }

    private fun sendEmpowerment(requestModel: EmpowermentCreateModel) {
        createEmpowermentUseCase.invoke(requestModel).onEach { result ->
            result.onLoading {
                logDebug("sendEmpowerment onLoading", TAG)
                showLoader()
            }.onSuccess { _, _, _ ->
                logDebug("sendEmpowerment onSuccess", TAG)
                showMessage(
                    DialogMessage(
                        message = StringSource(R.string.empowerment_create_success_message),
                        title = StringSource(R.string.information),
                        positiveButtonText = StringSource(R.string.ok),
                    )
                )
                hideLoader()
            }.onFailure { _, _, message, responseCode, errorType ->
                logError("sendEmpowerment onFailure", message, TAG)
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

    override fun onAlertDialogResult() {
        logDebug("onAlertDialogResult open fragment", TAG)
        navigateInTab(MainTabMoreFragmentDirections.toEmpowermentFromMeFlowFragment())
    }

    override fun onBackPressed() {
        logDebug("onBackPressed", TAG)
        popBackStackToFragment(R.id.empowermentCreateFragment)
    }

}