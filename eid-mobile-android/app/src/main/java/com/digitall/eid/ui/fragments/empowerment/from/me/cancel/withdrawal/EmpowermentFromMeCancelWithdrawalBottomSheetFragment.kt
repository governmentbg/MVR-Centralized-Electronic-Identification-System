package com.digitall.eid.ui.fragments.empowerment.from.me.cancel.withdrawal

import android.content.res.Resources
import com.digitall.eid.databinding.BottomSheetEmpowermentWithdrawalBinding
import com.digitall.eid.extensions.onClickThrottle
import com.digitall.eid.ui.fragments.base.BaseBottomSheetFragment
import com.digitall.eid.ui.fragments.empowerment.create.create.indefinite.EmpowermentCreateIndefiniteBottomSheetViewModel
import org.koin.androidx.viewmodel.ext.android.viewModel

class EmpowermentFromMeCancelWithdrawalBottomSheetFragment(private val listener: Listener) :
    BaseBottomSheetFragment<BottomSheetEmpowermentWithdrawalBinding, EmpowermentCreateIndefiniteBottomSheetViewModel>() {

    companion object {
        const val TAG = "EmpowermentFromMeCancelWithdrawalBottomSheetFragmentTag"

        // Factory method to create a new instance with arguments
        fun newInstance(listener: Listener) =
            EmpowermentFromMeCancelWithdrawalBottomSheetFragment(listener)
    }

    override fun getViewBinding() = BottomSheetEmpowermentWithdrawalBinding.inflate(layoutInflater)

    override val viewModel: EmpowermentCreateIndefiniteBottomSheetViewModel by viewModel()

    override val maxHeight = Resources.getSystem().displayMetrics.heightPixels * 75 / 100

    override fun setupView() {
        binding.btnYes.onClickThrottle {
            listener.operationCompleted()
        }
        binding.btnNo.onClickThrottle {
            listener.operationCancelled()
        }
    }

    interface Listener {
        fun operationCompleted()
        fun operationCancelled()
    }
}