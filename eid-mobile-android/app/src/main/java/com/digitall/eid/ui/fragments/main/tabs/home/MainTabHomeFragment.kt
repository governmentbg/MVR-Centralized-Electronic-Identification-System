/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.main.tabs.home

import androidx.lifecycle.lifecycleScope
import com.digitall.eid.R
import com.digitall.eid.domain.APPLICATION_LANGUAGE
import com.digitall.eid.domain.DELAY_2500
import com.digitall.eid.domain.DELAY_500
import com.digitall.eid.domain.ENVIRONMENT
import com.digitall.eid.extensions.launchWithDispatcher
import com.digitall.eid.models.common.AlertDialogResult
import com.digitall.eid.models.common.CardScanBottomSheetContent
import com.digitall.eid.models.common.CardScanResult
import com.digitall.eid.models.common.CardScanScreenStates
import com.digitall.eid.ui.fragments.base.BaseWebViewFragment
import com.digitall.eid.ui.fragments.card.enter.pin.auth.AuthCardBottomSheetFragment
import com.digitall.eid.ui.fragments.card.scan.ScanCardBottomSheetFragment
import com.digitall.eid.ui.fragments.pin.citizen.profile.create.CreatePinCitizenProfileBottomSheetFragment
import kotlinx.coroutines.delay
import org.koin.androidx.viewmodel.ext.android.viewModel
import kotlin.system.exitProcess

class MainTabHomeFragment :
    BaseWebViewFragment<MainTabHomeViewModel>(),
    AuthCardBottomSheetFragment.Listener,
    ScanCardBottomSheetFragment.Listener,
    CreatePinCitizenProfileBottomSheetFragment.Listener {

    companion object {
        private const val TAG = "MainTabHomeFragmentTag"
        private val URL_TO_LOAD = "${ENVIRONMENT.urlBase}mobile/home"
    }

    override val viewModel: MainTabHomeViewModel by viewModel()

    override val showToolbar: Boolean = true

    override val showSettingsButton: Boolean = false

    override val toolbarNavigationIconRes: Int? = null

    override val toolbarNavigationTextRes: Int = R.string.begin

    override val fabButtonText = R.string.citizen_information_change_associate_eid_button_title

    private var scanCardBottomSheet: ScanCardBottomSheetFragment? = null
    private var authCardBottomSheet: AuthCardBottomSheetFragment? = null
    private var createPinCitizenProfileBottomSheetFragment: CreatePinCitizenProfileBottomSheetFragment? = null

    private var state: CardScanScreenStates? = null

    override fun onAlertDialogResult(result: AlertDialogResult) {
        if (result.messageId == DIALOG_EXIT && result.isPositive) {
            exitProcess(0)
        }
    }

    override fun onCreated() {
        super.onCreated()
        val language = APPLICATION_LANGUAGE.type
        loadWebPage("${URL_TO_LOAD}?lang=$language")
    }

    override fun onResumed() {
        super.onResumed()
        viewModel.showActionButton()
        viewModel.promptUserToSecureCredentials()
    }

    override fun subscribeToLiveData() {
        viewModel.showCertificateAuthenticationLiveData.observe(viewLifecycleOwner) {
            showCertificateAuthentication()
        }
        viewModel.showAssociateEIDActionLiveData.observe(viewLifecycleOwner) { flag ->
            showActionButton(isVisible = flag)
        }
        viewModel.showCreatePinBottomSheetEvent.observe(viewLifecycleOwner) {
            showCreatePinBottomSheet()
        }
    }

    private fun showCertificateAuthentication() {
        authCardBottomSheet =
            AuthCardBottomSheetFragment.newInstance(
                listener = this,
                shouldShowCan = state == CardScanScreenStates.Suspended
            ).also { bottomSheet ->
                bottomSheet.show(parentFragmentManager, "AuthCardBottomSheetFragmentTag")
            }
    }

    override fun operationCompleted(result: CardScanBottomSheetContent) {
        lifecycleScope.launchWithDispatcher {
            delay(DELAY_500)
            authCardBottomSheet?.dismiss().also {
                authCardBottomSheet = null
                scanCardBottomSheet = ScanCardBottomSheetFragment.newInstance(
                    content = result,
                    listener = this@MainTabHomeFragment
                ).also { bottomSheet ->
                    bottomSheet.show(parentFragmentManager, "ScanCardBottomSheetFragmentTag")
                }
            }
        }
    }

    override fun operationCompleted(result: CardScanResult) {
        when (result) {
            is CardScanResult.ChallengeSigned -> {
                lifecycleScope.launchWithDispatcher {
                    delay(DELAY_500)
                    scanCardBottomSheet?.dismiss().also {
                        scanCardBottomSheet = null
                        viewModel.associateEid(
                            signature = result.signature,
                            challenge = result.challenge,
                            certificate = result.certificate,
                            certificateChain = result.certificateChain,
                        )
                    }
                }
            }

            else -> {}
        }
    }

    override fun operationFailed(state: CardScanScreenStates) {
        lifecycleScope.launchWithDispatcher {
            delay(DELAY_2500)
            this@MainTabHomeFragment.state = state
            scanCardBottomSheet?.dismiss().also {
                scanCardBottomSheet = null
            }
        }
    }

    override fun operationCompleted(pin: String?) {
        createPinCitizenProfileBottomSheetFragment?.dismiss().also {
            createPinCitizenProfileBottomSheetFragment = null
        }
        viewModel.setupApplicationPin(pin = pin)
    }

    private fun showCreatePinBottomSheet() {
        createPinCitizenProfileBottomSheetFragment = CreatePinCitizenProfileBottomSheetFragment.newInstance(listener = this)
            .also { bottomSheet ->
                bottomSheet.show(
                    parentFragmentManager,
                    "ApplicationConfirmPinBottomSheetFragmentTag"
                )
            }

    }

}