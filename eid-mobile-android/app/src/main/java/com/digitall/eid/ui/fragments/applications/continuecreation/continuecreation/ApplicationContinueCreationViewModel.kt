/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.applications.continuecreation.continuecreation

import androidx.lifecycle.viewModelScope
import com.digitall.eid.R
import com.digitall.eid.domain.EID_MOBILE_CERTIFICATE_KEYS
import com.digitall.eid.domain.models.base.onFailure
import com.digitall.eid.domain.models.base.onLoading
import com.digitall.eid.domain.models.base.onSuccess
import com.digitall.eid.domain.usecase.applications.create.ApplicationCreateEnrollWithEIDUseCase
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.extensions.launchInScope
import com.digitall.eid.models.applications.create.ApplicationCreatePinModel
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.main.MainTabsEnum
import com.digitall.eid.ui.BaseViewModel
import kotlinx.coroutines.flow.onEach
import org.koin.core.component.inject

class ApplicationContinueCreationViewModel : BaseViewModel() {

    companion object {
        private const val TAG = "ApplicationContinueCreationViewModelTag"
    }

    private val applicationCreateEnrollWithEIDUseCase: ApplicationCreateEnrollWithEIDUseCase by inject()

    private var applicationId: String? = null

    override var mainTabsEnum: MainTabsEnum? = MainTabsEnum.TAB_EIM

    fun setupModel(applicationId: String?) {
        logDebug("setupModel applicationId: $applicationId", TAG)
        if (applicationId.isNullOrEmpty()) {
            showErrorState(
                title = StringSource(R.string.error_internal_error_short),
                description = StringSource("Required element is empty")
            )
            return
        }
        this.applicationId = applicationId
        refreshScreen()
    }

    fun refreshScreen() {
        logDebug("createWithEID", TAG)
        applicationCreateEnrollWithEIDUseCase.invoke(
            applicationId = applicationId ?: return,
            keyAlias = EID_MOBILE_CERTIFICATE_KEYS
        ).onEach { result ->
            result.onLoading {
                showLoader()
            }.onSuccess { model, _, _ ->
                logDebug("createWithEID onSuccess", TAG)
                if (model.certificate.isNullOrEmpty() ||
                    model.certificateChain.isNullOrEmpty()
                ) {
                    showErrorState(
                        title = StringSource(R.string.error_internal_error_short),
                        description = StringSource("Required element is empty")
                    )
                    return@onEach
                }
                hideLoader()
                toCreatePin(
                    model = ApplicationCreatePinModel(
                        applicationId = applicationId ?: return@onEach,
                        certificate = model.certificate ?: return@onEach,
                        certificateChain = model.certificateChain ?: return@onEach,
                    )
                )
            }.onFailure { _, title, message, responseCode, _ ->
                logError("createWithEID onFailure", message, TAG)
                hideLoader()
                showErrorState(
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
        }.launchInScope(viewModelScope)
    }

    private fun toCreatePin(model: ApplicationCreatePinModel) {
        logDebug("toCreatePin", TAG)
        navigateInFlow(
            ApplicationContinueCreationFragmentDirections.toApplicationContinueCreationCreatePinFragment(
                model = model,
            )
        )
    }

    override fun onBackPressed() {
        logDebug("onBackPressed", TAG)
        popBackStackToFragmentInTab(R.id.applicationsFlowFragment)
    }

}