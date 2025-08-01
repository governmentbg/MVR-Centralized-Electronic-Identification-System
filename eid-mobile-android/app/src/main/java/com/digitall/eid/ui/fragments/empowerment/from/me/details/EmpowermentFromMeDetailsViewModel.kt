/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.empowerment.from.me.details

import androidx.lifecycle.viewModelScope
import com.digitall.eid.R
import com.digitall.eid.domain.models.empowerment.common.all.EmpowermentItem
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.extensions.isFragmentInBackStack
import com.digitall.eid.extensions.launchWithDispatcher
import com.digitall.eid.mappers.empowerment.from.me.details.EmpowermentFromMeDetailsUiMapper
import com.digitall.eid.models.main.MainTabsEnum
import com.digitall.eid.ui.fragments.empowerment.base.details.BaseEmpowermentDetailsViewModel
import org.koin.core.component.inject

class EmpowermentFromMeDetailsViewModel : BaseEmpowermentDetailsViewModel() {

    companion object {
        private const val TAG = "EmpowermentFromMeDetailsViewModelTag"
    }

    private val empowermentFromMeDetailsUiMapper: EmpowermentFromMeDetailsUiMapper by inject()

    private var model: EmpowermentItem? = null

    override var mainTabsEnum: MainTabsEnum? = MainTabsEnum.TAB_MORE

    override fun setupModel(model: EmpowermentItem) {
        logDebug("setupModel", TAG)
        this.model = model
        viewModelScope.launchWithDispatcher {
            _adapterListLiveData.emit(
                empowermentFromMeDetailsUiMapper.map(
                    from = model,
                )
            )
        }
    }

    override fun onBackPressed() {
        logDebug("onBackPressed", TAG)
        when {
            findFlowNavController().isFragmentInBackStack(R.id.empowermentsLegalFragment) -> popBackStackToFragment(
                R.id.empowermentsLegalFragment
            )

            else -> popBackStackToFragment(
                R.id.empowermentFromMeFragment
            )
        }
    }

}