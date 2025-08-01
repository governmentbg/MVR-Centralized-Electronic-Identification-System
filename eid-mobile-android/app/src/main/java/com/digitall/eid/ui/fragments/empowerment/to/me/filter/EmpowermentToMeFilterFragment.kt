/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.empowerment.to.me.filter

import androidx.navigation.fragment.navArgs
import com.digitall.eid.R
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.ui.fragments.empowerment.base.filter.BaseEmpowermentFilterFragment
import com.digitall.eid.ui.fragments.empowerment.to.me.filter.list.EmpowermentToMeFilterAdapter
import org.koin.android.ext.android.inject
import org.koin.androidx.viewmodel.ext.android.viewModel

class EmpowermentToMeFilterFragment :
    BaseEmpowermentFilterFragment<EmpowermentToMeFilterViewModel>() {

    companion object {
        private const val TAG = "EmpowermentToMeFilterFragmentTag"
    }

    override val viewModel: EmpowermentToMeFilterViewModel by viewModel()
    override val adapter: EmpowermentToMeFilterAdapter by inject()
    private val args: EmpowermentToMeFilterFragmentArgs by navArgs()

    override fun parseArguments() {
        super.parseArguments()
        try {
            viewModel.setFilteringModel(args.model)
        } catch (exception: Exception) {
            logError(
                "parseArguments Exception: ${exception.message}",
                exception,
                TAG
            )
        }
    }

    override fun setupView() {
        super.setupView()
        binding.toolbar.setTitleText(StringSource(R.string.empowerment_to_me_title))
    }

}