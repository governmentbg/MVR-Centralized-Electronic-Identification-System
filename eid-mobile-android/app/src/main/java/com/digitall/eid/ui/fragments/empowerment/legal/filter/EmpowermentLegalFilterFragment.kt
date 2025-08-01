package com.digitall.eid.ui.fragments.empowerment.legal.filter

import androidx.navigation.fragment.navArgs
import com.digitall.eid.R
import com.digitall.eid.domain.utils.LogUtil
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.ui.fragments.empowerment.base.filter.BaseEmpowermentFilterFragment
import com.digitall.eid.ui.fragments.empowerment.legal.filter.list.EmpowermentLegalFilterAdapter
import org.koin.android.ext.android.inject
import org.koin.androidx.viewmodel.ext.android.viewModel
import kotlin.properties.Delegates

class EmpowermentLegalFilterFragment :
    BaseEmpowermentFilterFragment<EmpowermentLegalFilterViewModel>() {

    companion object {
        private const val TAG = "EmpowermentLegalFilterFragmentTag"
    }

    override val viewModel: EmpowermentLegalFilterViewModel by viewModel()
    override val adapter: EmpowermentLegalFilterAdapter by inject()
    private val args: EmpowermentLegalFilterFragmentArgs by navArgs()

    private var legalNumber: String? by Delegates.observable(
        null
    ) { _, _, newValue ->
        binding.toolbar.setTitleText(StringSource(R.string.empowerments_legal_entity_title, listOf(newValue ?: "")))
    }

    override fun parseArguments() {
        try {
            legalNumber = args.legalNumber
            viewModel.setFilteringModel(args.model)
        } catch (exception: Exception) {
            LogUtil.logError(
                "parseArguments Exception: ${exception.message}",
                exception,
                TAG
            )
        }
    }

}