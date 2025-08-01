package com.digitall.eid.ui.fragments.information

import android.content.res.Resources
import android.text.Html
import android.text.method.ScrollingMovementMethod
import androidx.core.os.bundleOf
import com.digitall.eid.data.extensions.getParcelableCompat
import com.digitall.eid.databinding.BottomSheetInformationBinding
import com.digitall.eid.domain.INFORMATION_BOTTOM_SHEET_CONTENT_KEY
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.ui.fragments.base.BaseBottomSheetFragment
import com.google.android.material.bottomsheet.BottomSheetBehavior
import org.koin.androidx.viewmodel.ext.android.viewModel

class InformationBottomSheetFragment :
    BaseBottomSheetFragment<BottomSheetInformationBinding, InformationBottomSheetViewModel>() {

    companion object {
        private const val TAG = "InformationBottomSheetFragmentTag"

        // Factory method to create a new instance with arguments
        fun newInstance(content: StringSource) = InformationBottomSheetFragment().apply {
            arguments = bundleOf(INFORMATION_BOTTOM_SHEET_CONTENT_KEY to content)
        }
    }

    override fun getViewBinding() = BottomSheetInformationBinding.inflate(layoutInflater)

    override val viewModel: InformationBottomSheetViewModel by viewModel()

    override val maxHeight = Resources.getSystem().displayMetrics.heightPixels * 90 / 100

    private var currentScrollPositionY = 0

    override fun setupControls() {
        parseArguments()
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

    private fun parseArguments() {
        try {
            val content =
                arguments?.getParcelableCompat<StringSource>(INFORMATION_BOTTOM_SHEET_CONTENT_KEY)
            content?.let {
                binding.tvMessage.text =
                    Html.fromHtml(it.getString(binding.root.context), Html.FROM_HTML_MODE_LEGACY)
            }
        } catch (exception: IllegalStateException) {
            logError("parseArguments Exception: ${exception.message}", exception, TAG)
        }
    }

}