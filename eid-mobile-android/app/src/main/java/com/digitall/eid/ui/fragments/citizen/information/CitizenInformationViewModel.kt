package com.digitall.eid.ui.fragments.citizen.information

import androidx.lifecycle.viewModelScope
import com.digitall.eid.R
import com.digitall.eid.domain.DELAY_500
import com.digitall.eid.domain.extensions.readOnly
import com.digitall.eid.domain.models.applications.create.ApplicationUserDetailsModel
import com.digitall.eid.domain.models.base.ErrorType
import com.digitall.eid.domain.models.base.onFailure
import com.digitall.eid.domain.models.base.onLoading
import com.digitall.eid.domain.models.base.onSuccess
import com.digitall.eid.domain.models.citizen.update.information.CitizenUpdateInformationRequestModel
import com.digitall.eid.domain.usecase.applications.create.GetApplicationUserDetailsUseCase
import com.digitall.eid.domain.usecase.citizen.update.information.UpdateCitizenInformationUseCase
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.extensions.launchInScope
import com.digitall.eid.models.citizen.information.CitizenInformationAdapterMarker
import com.digitall.eid.models.citizen.information.CitizenInformationElementsEnumUi
import com.digitall.eid.models.common.AlertDialogResult
import com.digitall.eid.models.common.ButtonColorUi
import com.digitall.eid.models.common.DialogMessage
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonButtonUi
import com.digitall.eid.models.list.CommonEmptySpaceSizeEnum
import com.digitall.eid.models.list.CommonEmptySpaceUi
import com.digitall.eid.models.list.CommonSeparatorUi
import com.digitall.eid.models.list.CommonTitleCheckboxUi
import com.digitall.eid.models.list.CommonTitleDescriptionUi
import com.digitall.eid.models.list.CommonTitleSmallUi
import com.digitall.eid.models.main.MainTabsEnum
import com.digitall.eid.ui.BaseViewModel
import com.digitall.eid.ui.common.list.CommonActionUi
import kotlinx.coroutines.delay
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.onEach
import org.koin.core.component.inject

class CitizenInformationViewModel : BaseViewModel() {

    companion object {
        const val TAG = "ChangeCitizenPhoneViewModelTag"
    }

    override var mainTabsEnum: MainTabsEnum? = MainTabsEnum.TAB_MORE

    private val currentList = mutableListOf<CitizenInformationAdapterMarker>()

    private val _adapterListLiveData =
        MutableStateFlow<List<CitizenInformationAdapterMarker>>(emptyList())
    val adapterListLiveData = _adapterListLiveData.readOnly()

    private val getApplicationUserDetailsUseCase: GetApplicationUserDetailsUseCase by inject()
    private val updateCitizenInformationUseCase: UpdateCitizenInformationUseCase by inject()

    private lateinit var information: ApplicationUserDetailsModel

    @Volatile
    private var isCitizenEidAssociatedSuccessful = false

    override fun onBackPressed() {
        logDebug("onBackPressed", TAG)
        backToTab()
    }

    override fun onFirstAttach() {
        refreshScreen()
    }

    override fun onAlertDialogResult(result: AlertDialogResult) {
        if (result.isPositive && isCitizenEidAssociatedSuccessful) {
            refreshScreen()
        }
    }

    fun onFieldTextAction(model: CommonTitleDescriptionUi) {
        logDebug("onFieldTextAction", TAG)

        when (model.elementEnum) {
            CitizenInformationElementsEnumUi.FORNAME_TEXT,
            CitizenInformationElementsEnumUi.MIDDLENAME_TEXT,
            CitizenInformationElementsEnumUi.SURNAME_TEXT,
            CitizenInformationElementsEnumUi.MOBILE_PHONE_TEXT,
            CitizenInformationElementsEnumUi.FORNAME_LATIN_TEXT,
            CitizenInformationElementsEnumUi.MIDDLENAME_LATIN_TEXT,
            CitizenInformationElementsEnumUi.SURNAME_LATIN_TEXT -> toCitizenChangeInformation(
                information = information
            )

            CitizenInformationElementsEnumUi.EMAIL_TEXT -> toCitizenChangeEmail()
        }
    }

    fun onButtonClicked(model: CommonButtonUi) {
        when (model.elementEnum) {
            CitizenInformationElementsEnumUi.BUTTON_ASSOCIATE_EID -> citizenAssociateEID()
            CitizenInformationElementsEnumUi.BUTTON_CHANGE_PASSWORD -> toCitizenChangePassword()
        }
    }

    fun onCheckBoxChangeState(model: CommonTitleCheckboxUi) {
        updateCitizenInformation(
            data = CitizenUpdateInformationRequestModel(
                firstName = information.firstName,
                secondName = information.secondName,
                lastName = information.lastName,
                firstNameLatin = information.firstNameLatin,
                secondNameLatin = information.secondNameLatin,
                lastNameLatin = information.lastNameLatin,
                phoneNumber = information.phoneNumber,
                twoFaEnabled = model.isChecked
            )
        )
    }

    fun refreshScreen() {
        logDebug("refreshScreen", TAG)
        getApplicationUserDetailsUseCase.invoke().onEach { result ->
            result.onLoading {
                logDebug(
                    "getApplicationUserDetailsUseCase onLoading",
                    TAG
                )
                showLoader()
            }.onSuccess { model, _, _ ->
                logDebug(
                    "getApplicationUserDetailsUseCase onSuccess",
                    TAG
                )
                if (model.firstName.isNullOrEmpty() ||
                    model.lastName.isNullOrEmpty() ||
                    model.secondName.isNullOrEmpty()
                ) {
                    showErrorState(
                        title = StringSource(R.string.error_server_error),
                        description = StringSource("Not received usernames from the server")
                    )
                    return@onSuccess
                }
                information = model
                buildElements(information = information)
                _adapterListLiveData.emit(currentList.toList())
                delay(DELAY_500)
                hideLoader()
                hideErrorState()
            }.onFailure { _, _, message, responseCode, errorType ->
                logError("getApplicationUserDetailsUseCase onFailure", message, TAG)
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

    private fun buildElements(information: ApplicationUserDetailsModel) {
        currentList.apply {
            clear()
            add(
                CommonTitleSmallUi(
                    title = CitizenInformationElementsEnumUi.PERSONAL_DATA_TEXT.title
                )
            )
            add(CommonSeparatorUi())
            add(
                CommonTitleDescriptionUi(
                    title = CitizenInformationElementsEnumUi.FORNAME_TEXT.title,
                    elementEnum = CitizenInformationElementsEnumUi.FORNAME_TEXT,
                    description = StringSource(information.firstName ?: ""),
                    action = CommonActionUi(
                        icon = R.drawable.ic_edit,
                        color = R.color.color_1C3050
                    )
                )
            )
            add(
                CommonTitleDescriptionUi(
                    title = CitizenInformationElementsEnumUi.MIDDLENAME_TEXT.title,
                    elementEnum = CitizenInformationElementsEnumUi.MIDDLENAME_TEXT,
                    description = StringSource(information.secondName ?: ""),
                    action = CommonActionUi(
                        icon = R.drawable.ic_edit,
                        color = R.color.color_1C3050
                    )
                )
            )
            add(
                CommonTitleDescriptionUi(
                    title = CitizenInformationElementsEnumUi.SURNAME_TEXT.title,
                    elementEnum = CitizenInformationElementsEnumUi.SURNAME_TEXT,
                    description = StringSource(information.lastName ?: ""),
                    action = CommonActionUi(
                        icon = R.drawable.ic_edit,
                        color = R.color.color_1C3050
                    )
                )
            )
            add(
                CommonTitleDescriptionUi(
                    title = CitizenInformationElementsEnumUi.FORNAME_LATIN_TEXT.title,
                    elementEnum = CitizenInformationElementsEnumUi.FORNAME_LATIN_TEXT,
                    description = StringSource(information.firstNameLatin ?: ""),
                    action = CommonActionUi(
                        icon = R.drawable.ic_edit,
                        color = R.color.color_1C3050
                    )
                )
            )
            add(
                CommonTitleDescriptionUi(
                    title = CitizenInformationElementsEnumUi.MIDDLENAME_LATIN_TEXT.title,
                    elementEnum = CitizenInformationElementsEnumUi.MIDDLENAME_LATIN_TEXT,
                    description = StringSource(information.secondNameLatin ?: ""),
                    action = CommonActionUi(
                        icon = R.drawable.ic_edit,
                        color = R.color.color_1C3050
                    )
                ),
            )
            add(
                CommonTitleDescriptionUi(
                    title = CitizenInformationElementsEnumUi.SURNAME_LATIN_TEXT.title,
                    elementEnum = CitizenInformationElementsEnumUi.SURNAME_LATIN_TEXT,
                    description = StringSource(information.lastNameLatin ?: ""),
                    action = CommonActionUi(
                        icon = R.drawable.ic_edit,
                        color = R.color.color_1C3050
                    )
                )
            )
            add(
                CommonEmptySpaceUi(size = CommonEmptySpaceSizeEnum.SIZE_16)
            )
            add(
                CommonTitleSmallUi(
                    title = CitizenInformationElementsEnumUi.CONTACTS_TEXT.title
                )
            )
            add(CommonSeparatorUi())
            add(
                CommonTitleDescriptionUi(
                    title = CitizenInformationElementsEnumUi.MOBILE_PHONE_TEXT.title,
                    elementEnum = CitizenInformationElementsEnumUi.MOBILE_PHONE_TEXT,
                    description = information.phoneNumber?.let { StringSource(it) }
                        ?: run { StringSource(R.string.unspecified) },
                    action = CommonActionUi(
                        icon = R.drawable.ic_edit,
                        color = R.color.color_1C3050
                    )
                )
            )
            add(
                CommonTitleDescriptionUi(
                    title = CitizenInformationElementsEnumUi.EMAIL_TEXT.title,
                    elementEnum = CitizenInformationElementsEnumUi.EMAIL_TEXT,
                    description = StringSource(information.email ?: ""),
                    action = CommonActionUi(
                        icon = R.drawable.ic_edit,
                        color = R.color.color_1C3050
                    )
                )
            )
            add(
                CommonEmptySpaceUi(size = CommonEmptySpaceSizeEnum.SIZE_16)
            )
//            add(
//                CommonTitleSmallUi(
//                    title = CitizenInformationElementsEnumUi.PROFILE_SECURITY_DATA_TEXT.title
//                )
//            )
//            add(CommonSeparatorUi())
//            add(
//                CommonTitleCheckboxUi(
//                    title = CitizenInformationElementsEnumUi.MULTI_FACTOR_AUTHENTICATION_CHECKBOX.title,
//                    isChecked = information.twoFaEnabled ?: false
//                )
//            )
            information.eidentityId?.let { eidentityId ->
                add(
                    CommonEmptySpaceUi(size = CommonEmptySpaceSizeEnum.SIZE_16)
                )
                add(
                    CommonTitleSmallUi(
                        title = CitizenInformationElementsEnumUi.ELECTRONIC_IDENTITY_DATA_TEXT.title
                    )
                )
                add(CommonSeparatorUi())
                add(
                    CommonTitleDescriptionUi(
                        title = CitizenInformationElementsEnumUi.ELECTRONIC_IDENTITY_NUMBER_TEXT.title,
                        description = StringSource(eidentityId)
                    )
                )
            }
            add(
                CommonButtonUi(
                    elementEnum = CitizenInformationElementsEnumUi.BUTTON_CHANGE_PASSWORD,
                    title = CitizenInformationElementsEnumUi.BUTTON_CHANGE_PASSWORD.title,
                    buttonColor = ButtonColorUi.BLUE,
                )
            )
        }
    }

    private fun toCitizenChangeInformation(information: ApplicationUserDetailsModel) {
        logDebug("toCitizenChangeInformation", TAG)
        navigateInFlow(
            CitizenInformationFragmentDirections.toChangeCitizenInformation(information = information)
        )
    }

    private fun toCitizenChangeEmail() {
        logDebug("toCitizenChangeEmail", TAG)
        navigateInFlow(
            CitizenInformationFragmentDirections.toChangeCitizenEmail()
        )
    }

    private fun toCitizenChangePassword() {
        logDebug("toCitizenChangePassword", TAG)
        navigateInFlow(
            CitizenInformationFragmentDirections.toChangeCitizenPassword()
        )
    }

    private fun updateCitizenInformation(data: CitizenUpdateInformationRequestModel) {
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
                information = information.copy(twoFaEnabled = data.twoFaEnabled)
                buildElements(information = information)
                _adapterListLiveData.emit(currentList.toList())
                delay(DELAY_500)
                hideLoader()
            }.onFailure { _, _, message, responceCode, errorType ->
                logDebug("onFailure updateCitizenPhone", TAG)
                buildElements(information = information)
                _adapterListLiveData.emit(currentList.toList())
                delay(DELAY_500)
                hideLoader()
                when (errorType) {
                    ErrorType.AUTHORIZATION -> toLoginFragment()

                    else -> showMessage(
                        DialogMessage(
                            title = StringSource(R.string.information),
                            message = message?.let { StringSource(message) }
                                ?: StringSource(R.string.error_api_general, formatArgs = listOf(responceCode.toString())),
                            positiveButtonText = StringSource(R.string.ok)
                        )
                    )
                }
            }
        }.launchInScope(viewModelScope)
    }
}