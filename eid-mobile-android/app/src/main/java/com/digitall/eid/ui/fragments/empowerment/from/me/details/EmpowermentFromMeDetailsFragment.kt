/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.empowerment.from.me.details

import androidx.navigation.fragment.navArgs
import com.digitall.eid.R
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.ui.fragments.empowerment.base.details.BaseEmpowermentDetailsFragment
import com.digitall.eid.ui.fragments.empowerment.from.me.details.list.EmpowermentFromMeDetailsAdapter
import org.koin.android.ext.android.inject
import org.koin.androidx.viewmodel.ext.android.viewModel

class EmpowermentFromMeDetailsFragment :
    BaseEmpowermentDetailsFragment<EmpowermentFromMeDetailsViewModel>() {

    companion object {
        private const val TAG = "EmpowermentFromMeDetailsFragmentTag"
    }

    override val viewModel: EmpowermentFromMeDetailsViewModel by viewModel()
    override val adapter: EmpowermentFromMeDetailsAdapter by inject()
    private val args: EmpowermentFromMeDetailsFragmentArgs by navArgs()

    override fun setupView(){
        super.setupView()
        binding.toolbar.setTitleText(StringSource(R.string.empowerment_from_me_title))
    }

    override fun parseArguments() {
        try {
            viewModel.setupModel(args.model)
        } catch (e: IllegalStateException) {
            logError("parseArguments Exception: ${e.message}", e, TAG)
        }
    }

}