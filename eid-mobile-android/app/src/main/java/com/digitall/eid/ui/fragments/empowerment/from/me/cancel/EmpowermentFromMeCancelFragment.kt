/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.empowerment.from.me.cancel

import androidx.navigation.fragment.navArgs
import com.digitall.eid.R
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.ui.fragments.empowerment.base.cancel.BaseEmpowermentCancelFragment
import com.digitall.eid.ui.fragments.empowerment.from.me.cancel.list.EmpowermentFromMeCancelAdapter
import com.digitall.eid.ui.fragments.empowerment.from.me.cancel.withdrawal.EmpowermentFromMeCancelWithdrawalBottomSheetFragment
import org.koin.android.ext.android.inject
import org.koin.androidx.viewmodel.ext.android.viewModel

class EmpowermentFromMeCancelFragment :
    BaseEmpowermentCancelFragment<EmpowermentFromMeCancelViewModel>(),
    EmpowermentFromMeCancelWithdrawalBottomSheetFragment.Listener {

    companion object {
        private const val TAG = "EmpowermentFromMeCancelFragmentFragmentTag"
    }

    override val toolbarTitleText = StringSource(R.string.empowerment_from_me_title)

    override val viewModel: EmpowermentFromMeCancelViewModel by viewModel()
    override val adapter: EmpowermentFromMeCancelAdapter by inject()
    private val args: EmpowermentFromMeCancelFragmentArgs by navArgs()

    private var empowermentFromMeCancelWithdrawalBottomSheetFragment: EmpowermentFromMeCancelWithdrawalBottomSheetFragment? =
        null

    override fun setupView() {
        super.setupView()
        binding.toolbar.setTitleText(toolbarTitleText)
    }

    override fun parseArguments() {
        try {
            val reasons = listOf(
                StringSource(R.string.empowerment_cancel_reasons_wont_be_used_type).getString(
                    binding.root.context
                )
            )
            viewModel.setupModel(args.model, reasons)
        } catch (e: IllegalStateException) {
            logError("parseArguments Exception: ${e.message}", e, TAG)
        }
    }

    override fun subscribeToLiveData() {
        super.subscribeToLiveData()
        viewModel.showWithdrawalInformationEvent.observe(viewLifecycleOwner) {
            showWithdrawalInformation()
        }
    }

    private fun showWithdrawalInformation() {
        empowermentFromMeCancelWithdrawalBottomSheetFragment =
            EmpowermentFromMeCancelWithdrawalBottomSheetFragment.newInstance(listener = this)
                .also { bottomSheet ->
                    bottomSheet.show(
                        parentFragmentManager,
                        EmpowermentFromMeCancelWithdrawalBottomSheetFragment.TAG
                    )
                }
    }


    override fun operationCancelled() {
        empowermentFromMeCancelWithdrawalBottomSheetFragment?.dismiss()
    }

    override fun operationCompleted() {
        operationCancelled()
        viewModel.cancelEmpowerment()
    }
}