/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.empowerment.to.me.details

import androidx.navigation.fragment.navArgs
import com.digitall.eid.R
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.ui.fragments.empowerment.base.details.BaseEmpowermentDetailsFragment
import com.digitall.eid.ui.fragments.empowerment.to.me.details.list.EmpowermentToMeDetailsAdapter
import org.koin.android.ext.android.inject
import org.koin.androidx.viewmodel.ext.android.viewModel

class EmpowermentToMeDetailsFragment :
    BaseEmpowermentDetailsFragment<EmpowermentToMeDetailsViewModel>() {

    companion object {
        private const val TAG = "EmpowermentToMeDetailsFragmentTag"
    }

    override val viewModel: EmpowermentToMeDetailsViewModel by viewModel()
    override val adapter: EmpowermentToMeDetailsAdapter by inject()
    private val args: EmpowermentToMeDetailsFragmentArgs by navArgs()

    override fun setupView() {
        super.setupView()
        binding.toolbar.setTitleText(StringSource(R.string.empowerment_to_me_title))
    }

    override fun parseArguments() {
        try {
            viewModel.setupModel(args.model)
        } catch (e: IllegalStateException) {
            logError("parseArguments Exception: ${e.message}", e, TAG)
        }
    }

}