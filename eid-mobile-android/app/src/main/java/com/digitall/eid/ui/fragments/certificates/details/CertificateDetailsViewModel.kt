/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.certificates.details

import androidx.lifecycle.viewModelScope
import com.digitall.eid.R
import com.digitall.eid.domain.APPLICATION_LANGUAGE
import com.digitall.eid.domain.extensions.getEnumValue
import com.digitall.eid.domain.extensions.readOnly
import com.digitall.eid.domain.models.base.ErrorType
import com.digitall.eid.domain.models.base.onFailure
import com.digitall.eid.domain.models.base.onLoading
import com.digitall.eid.domain.models.base.onSuccess
import com.digitall.eid.domain.models.certificates.CertificateHistoryElementModel
import com.digitall.eid.domain.usecase.certificates.GetCertificateDetailsUseCase
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.extensions.isFragmentInBackStack
import com.digitall.eid.extensions.launchInScope
import com.digitall.eid.mappers.certificates.details.CertificateDetailsUiMapper
import com.digitall.eid.models.certificates.all.CertificatesStatusEnum
import com.digitall.eid.models.certificates.details.CertificateDetailsAdapterMarker
import com.digitall.eid.models.certificates.details.CertificateDetailsElementsEnumUi
import com.digitall.eid.models.certificates.details.CertificateDetailsType
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonButtonUi
import com.digitall.eid.models.list.CommonSimpleTextInFieldUi
import com.digitall.eid.models.list.CommonSimpleTextUi
import com.digitall.eid.models.main.MainTabsEnum
import com.digitall.eid.ui.BaseViewModel
import com.digitall.eid.ui.fragments.certificates.all.CertificatesFragmentDirections
import com.digitall.eid.ui.fragments.main.tabs.eim.MainTabEIMFragmentDirections
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.onEach
import org.koin.core.component.inject

class CertificateDetailsViewModel : BaseViewModel() {

    companion object {
        private const val TAG = "CertificateDetailsViewModelTag"
        private const val STOPPED_REASON = "STOP_REASON_TYPE"
        private const val RESUME_REASON = "RESUME_REASON_TYPE"
        private const val REVOKE_REASON = "REVOKE_REASON_TYPE"
    }

    private val getCertificateDetailsUseCase: GetCertificateDetailsUseCase by inject()
    private val certificateDetailsUiMapper: CertificateDetailsUiMapper by inject()

    override var mainTabsEnum: MainTabsEnum? = MainTabsEnum.TAB_EIM

    private val _adapterListLiveData =
        MutableStateFlow<List<CertificateDetailsAdapterMarker>>(emptyList())
    val adapterListLiveData = _adapterListLiveData.readOnly()

    private var certificateId: String? = null
    private var applicationId: String? = null
    private var alias: String? = null
    private var detailsType = CertificateDetailsType.DETAILS

    fun setupModel(
        certificateId: String,
        applicationId: String?,
        detailsType: CertificateDetailsType,
    ) {
        logDebug("setupModel certificateId: $certificateId applicationId: $applicationId", TAG)
        this.certificateId = certificateId
        this.applicationId = applicationId
        this.detailsType = detailsType
        refreshScreen()
    }

    fun refreshScreen() {
        logDebug("refreshScreen", TAG)
        getCertificateDetails()
    }

    fun onInFieldTextClicked(model: CommonSimpleTextInFieldUi) {
        logDebug("onTextClicked", TAG)
        when (model.originalModel) {
            is CertificateHistoryElementModel -> navigateInTab(
                MainTabEIMFragmentDirections.toApplicationsFlowFragment(
                    applicationId = model.originalModel.applicationId,
                    certificateId = certificateId,
                )
            )
        }
    }

    fun onFieldTextAction(model: CommonSimpleTextUi) {
        logDebug("onTextClicked", TAG)
        toCertificateEditAlias()
    }

    fun onButtonClicked(model: CommonButtonUi) {
        logDebug("onButtonClicked", TAG)
        when (model.elementEnum) {
            CertificateDetailsElementsEnumUi.BUTTON_RESUME -> toCertificateResume()
            CertificateDetailsElementsEnumUi.BUTTON_STOP -> toCertificateStop()
            CertificateDetailsElementsEnumUi.BUTTON_REVOKE -> toCertificateRevoke()
            CertificateDetailsElementsEnumUi.BUTTON_BACK -> onBackPressed()
        }
    }

    private fun toCertificateStop() {
        logDebug("toCertificateStop", TAG)
        navigateInFlow(
            CertificatesFragmentDirections.toCertificateStopFragment(
                id = certificateId ?: return,
            )
        )
    }

    private fun toCertificateResume() {
        logDebug("toCertificateResume", TAG)
        navigateInFlow(
            CertificatesFragmentDirections.toCertificateResumeFragment(
                id = certificateId ?: return,
            )
        )
    }

    private fun toCertificateRevoke() {
        logDebug("toCertificateResume", TAG)
        navigateInFlow(
            CertificatesFragmentDirections.toCertificateRevokeFragment(
                id = certificateId ?: return,
            )
        )
    }

    private fun toCertificateEditAlias() {
        logDebug("toCertificateNameEdit", TAG)
        navigateInFlow(
            CertificateDetailsFragmentDirections.toCertificateEditAliasFragment(
                id = certificateId ?: return,
                alias = alias,
            )
        )
    }

    private fun getCertificateDetails() {
        getCertificateDetailsUseCase.invoke(
            id = certificateId ?: return
        ).onEach { result ->
            result.onLoading {
                logDebug("getCertificateDetailsUseCase onLoading", TAG)
                showLoader()
            }.onSuccess { model, _, _ ->
                logDebug("getCertificateDetailsUseCase onSuccess", TAG)
                val language = APPLICATION_LANGUAGE.type
                val reason = model.information?.reasonText ?: getEnumValue<CertificatesStatusEnum>(
                    model.information?.status ?: ""
                )?.let { status ->
                    when (status) {
                        CertificatesStatusEnum.ACTIVE ->
                            model.nomenclatures?.firstOrNull { reason -> reason.name == RESUME_REASON }?.nomenclatures?.firstOrNull { nomenclature ->
                                nomenclature.id == model.information?.reasonId && nomenclature.language == language
                            }?.description

                        CertificatesStatusEnum.STOPPED ->
                            model.nomenclatures?.firstOrNull { reason -> reason.name == STOPPED_REASON }?.nomenclatures?.firstOrNull { nomenclature ->
                                nomenclature.id == model.information?.reasonId && nomenclature.language == language
                            }?.description

                        CertificatesStatusEnum.REVOKED ->
                            model.nomenclatures?.firstOrNull { reason -> reason.name == REVOKE_REASON }?.nomenclatures?.firstOrNull { nomenclature ->
                                nomenclature.id == model.information?.reasonId && nomenclature.language == language
                            }?.description

                        else -> null
                    }
                }
                alias = model.information?.alias
                _adapterListLiveData.emit(
                    certificateDetailsUiMapper.map(
                        model.copy(
                            information = model.information?.copy(
                                reasonText = reason
                            ),
                            history = model.history?.map { element ->
                                element.copy(
                                    reasonText = element.getReason(
                                        reasons = (model.nomenclatures
                                            ?: emptyList()).mapNotNull { it.nomenclatures }
                                            .flatten()
                                            .filter { nomenclature ->
                                                nomenclature.language == language
                                            }
                                    )
                                )
                            }
                        ), detailsType
                    )
                )
                hideLoader()
            }.onFailure { _, _, message, responseCode, errorType ->
                logError("getCertificateDetailsUseCase onFailure", message, TAG)
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

    override fun onBackPressed() {
        logDebug("onBackPressed", TAG)
        when {
            findTabNavController().isFragmentInBackStack(R.id.applicationsFlowFragment) -> popBackStackToFragmentInTab(
                R.id.applicationsFlowFragment
            )

            findTabNavController().isFragmentInBackStack(R.id.applicationsFragment) -> popBackStackToFragmentInTab(
                R.id.applicationsFragment
            )

            applicationId.isNullOrEmpty() -> popBackStackToFragment(R.id.certificatesFragment)

            else -> navigateInTab(
                MainTabEIMFragmentDirections.toApplicationsFlowFragment(
                    applicationId = applicationId,
                    certificateId = null,
                )
            )
        }
    }

}