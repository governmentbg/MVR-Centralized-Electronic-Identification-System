/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.empowerment.to.me.cancel

import androidx.navigation.fragment.navArgs
import com.digitall.eid.R
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.ui.fragments.empowerment.base.cancel.BaseEmpowermentCancelFragment
import com.digitall.eid.ui.fragments.empowerment.to.me.cancel.list.EmpowermentToMeCancelAdapter
import org.koin.android.ext.android.inject
import org.koin.androidx.viewmodel.ext.android.viewModel

class EmpowermentToMeCancelFragment :
    BaseEmpowermentCancelFragment<EmpowermentToMeCancelViewModel>() {

    companion object {
        private const val TAG = "EmpowermentToMeCancelFragmentTag"
    }

    override val toolbarTitleText = StringSource(R.string.empowerment_to_me_title)

    override val viewModel: EmpowermentToMeCancelViewModel by viewModel()
    override val adapter: EmpowermentToMeCancelAdapter by inject()
    private val args: EmpowermentToMeCancelFragmentArgs by navArgs()

    override fun setupView() {
        super.setupView()
        binding.toolbar.setTitleText(StringSource(R.string.empowerment_to_me_title))
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

}