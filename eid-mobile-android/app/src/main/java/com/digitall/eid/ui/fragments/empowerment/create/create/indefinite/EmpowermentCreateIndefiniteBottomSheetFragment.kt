package com.digitall.eid.ui.fragments.empowerment.create.create.indefinite

import android.content.res.Resources
import android.text.method.ScrollingMovementMethod
import androidx.core.text.bold
import androidx.core.text.buildSpannedString
import com.digitall.eid.R
import com.digitall.eid.databinding.BottomSheetInformationWithButtonBinding
import com.digitall.eid.extensions.onClickThrottle
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.ui.fragments.base.BaseBottomSheetFragment
import com.google.android.material.bottomsheet.BottomSheetBehavior
import org.koin.androidx.viewmodel.ext.android.viewModel

class EmpowermentCreateIndefiniteBottomSheetFragment(private val listener: Listener) :
    BaseBottomSheetFragment<BottomSheetInformationWithButtonBinding, EmpowermentCreateIndefiniteBottomSheetViewModel>() {

    companion object {
        private const val TAG = "EmpowermentCreateIndefiniteBottomSheetFragmentTag"

        // Factory method to create a new instance with arguments
        fun newInstance(listener: Listener) =
            EmpowermentCreateIndefiniteBottomSheetFragment(listener)
    }

    override fun getViewBinding() = BottomSheetInformationWithButtonBinding.inflate(layoutInflater)

    override val viewModel: EmpowermentCreateIndefiniteBottomSheetViewModel by viewModel()

    override val maxHeight = Resources.getSystem().displayMetrics.heightPixels * 80 / 100

    private var currentScrollPositionY = 0

    override fun setupView() {
        val text = buildSpannedString {
            bold{ append(StringSource(R.string.create_empowerment_indefinite_section_1_title).getString(binding.rootLayout.context)) }
            append("\n\n")
            append(StringSource(R.string.create_empowerment_indefinite_section_1_description).getString(binding.rootLayout.context))
            append("\n\n")
            bold { append(StringSource(R.string.create_empowerment_indefinite_section_2_title).getString(binding.rootLayout.context)) }
            append(StringSource(R.string.create_empowerment_indefinite_section_2_description).getString(binding.rootLayout.context))
            append("\n\n")
            bold { append(StringSource(R.string.create_empowerment_indefinite_section_3_title).getString(binding.rootLayout.context)) }
            append(StringSource(R.string.create_empowerment_indefinite_section_3_description).getString(binding.rootLayout.context))
            append("\n\n")
            bold { append(StringSource(R.string.create_empowerment_indefinite_section_4_title).getString(binding.rootLayout.context)) }
            append(StringSource(R.string.create_empowerment_indefinite_section_4_description).getString(binding.rootLayout.context))
            append("\n\n")
            append(StringSource(R.string.create_empowerment_indefinite_section_5_description).getString(binding.rootLayout.context))
        }
        binding.tvMessage.text = text
        binding.btnOk.onClickThrottle {
            listener.operationCompleted()
        }
    }

    override fun setupControls() {
        binding.tvMessage.movementMethod = ScrollingMovementMethod()
        binding.tvMessage.setOnScrollChangeListener { _, _, scrollY, _, _ ->
            currentScrollPositionY = scrollY
        }
    }

    override fun onStateChange(state: Int) {
        if (state == BottomSheetBehavior.STATE_DRAGGING && currentScrollPositionY > 0) {
            setExpandedState()
        }
    }

    interface Listener {
        fun operationCompleted()
    }
}