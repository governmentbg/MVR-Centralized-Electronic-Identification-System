/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.main.tabs.eim

import com.digitall.eid.R
import com.digitall.eid.databinding.FragmentMainTabEimBinding
import com.digitall.eid.extensions.onClickThrottle
import com.digitall.eid.models.common.AlertDialogResult
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.ui.fragments.information.InformationBottomSheetFragment
import com.digitall.eid.ui.fragments.main.base.BaseMainTabFragment
import org.koin.androidx.viewmodel.ext.android.viewModel
import kotlin.system.exitProcess

class MainTabEIMFragment :
    BaseMainTabFragment<FragmentMainTabEimBinding, MainTabEIMViewModel>() {

    companion object {
        private const val TAG = "MainTabTwoFragmentTag"
    }

    override fun getViewBinding() = FragmentMainTabEimBinding.inflate(layoutInflater)
    override val viewModel: MainTabEIMViewModel by viewModel()

    override fun setupControls() {
        binding.toolbar.navigationClickListener = {
            viewModel.onBackPressed()
        }
        binding.btnApplications.onClickThrottle {
            viewModel.toApplications()
        }
        binding.btnCertificates.onClickThrottle {
            viewModel.toCertificates()
        }
//        binding.btnScanQR.onClickThrottle {
//            viewModel.toScanCode()
//        }
        binding.btnCreateNew.onClickThrottle {
            viewModel.onCreateClicked()
        }
        binding.toolbar.setSettingsIcon(
            settingsIconRes = R.drawable.ic_information,
            settingsIconColorRes = R.color.color_white,
            settingsClickListener = { showInformationBottomSheet() }
        )
    }

    override fun onAlertDialogResult(result: AlertDialogResult) {
        if (result.messageId == DIALOG_EXIT && result.isPositive) {
            exitProcess(0)
        }
    }

    private fun showInformationBottomSheet() {
        InformationBottomSheetFragment.newInstance(content = StringSource(R.string.bottom_sheet_information_applications_certificates_information))
            .also { bottomSheet ->
                bottomSheet.show(parentFragmentManager, null)
            }
    }

}