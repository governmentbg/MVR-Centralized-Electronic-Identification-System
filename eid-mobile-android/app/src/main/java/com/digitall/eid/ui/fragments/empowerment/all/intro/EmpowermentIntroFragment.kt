/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.empowerment.all.intro

import com.digitall.eid.R
import com.digitall.eid.databinding.FragmentEmpowermentIntroBinding
import com.digitall.eid.extensions.onClickThrottle
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.ui.fragments.base.BaseFragment
import com.digitall.eid.ui.fragments.information.InformationBottomSheetFragment
import org.koin.androidx.viewmodel.ext.android.viewModel

class EmpowermentIntroFragment :
    BaseFragment<FragmentEmpowermentIntroBinding, EmpowermentIntroViewModel>() {

    companion object {
        private const val TAG = "EmpowermentIntroFragmentTag"
    }

    override fun getViewBinding() = FragmentEmpowermentIntroBinding.inflate(layoutInflater)

    override val viewModel: EmpowermentIntroViewModel by viewModel()

    override fun setupView() {
        binding.toolbar.setSettingsIcon(
            settingsIconRes = R.drawable.ic_information,
            settingsIconColorRes = R.color.color_white,
            settingsClickListener = { showInformationBottomSheet() }
        )
    }

    override fun setupControls() {
        binding.toolbar.navigationClickListener = {
            viewModel.onBackPressed()
        }
        binding.btnFromMe.onClickThrottle {
            viewModel.toEmpowermentFromMe()
        }
        binding.btnToMe.onClickThrottle {
            viewModel.toEmpowermentToMe()
        }
        binding.btnCreateNew.onClickThrottle {
            viewModel.toEmpowermentCreate()
        }
    }

    private fun showInformationBottomSheet() {
        InformationBottomSheetFragment.newInstance(content = StringSource(R.string.bottom_sheet_information_empowerement_registry_information))
            .also { bottomSheet ->
                bottomSheet.show(parentFragmentManager, null)
            }
    }

}