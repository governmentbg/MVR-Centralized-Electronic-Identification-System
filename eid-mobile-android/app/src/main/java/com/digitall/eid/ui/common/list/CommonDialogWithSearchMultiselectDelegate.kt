package com.digitall.eid.ui.common.list

import android.view.ViewGroup
import androidx.core.view.isVisible
import androidx.recyclerview.widget.RecyclerView
import com.digitall.eid.R
import com.digitall.eid.databinding.ListItemDatePickerBinding
import com.digitall.eid.extensions.inflateBinding
import com.digitall.eid.extensions.onClickThrottle
import com.digitall.eid.extensions.setBackgroundDrawableResource
import com.digitall.eid.extensions.setCompoundDrawablesExt
import com.digitall.eid.extensions.setTextColorResource
import com.digitall.eid.models.list.CommonDialogWithSearchMultiselectUi
import com.digitall.eid.models.list.CommonListElementAdapterMarker
import com.hannesdorfmann.adapterdelegates4.AdapterDelegate
import org.koin.core.component.KoinComponent

class CommonDialogWithSearchMultiselectDelegate :
    AdapterDelegate<MutableList<CommonListElementAdapterMarker>>(), KoinComponent {

    companion object {
        private const val TAG = "CommonTextFieldDelegateTag"
    }

    var clickListener: ((model: CommonDialogWithSearchMultiselectUi) -> Unit)? = null
    var eraseClickListener: ((model: CommonDialogWithSearchMultiselectUi) -> Unit)? = null

    override fun isForViewType(
        items: MutableList<CommonListElementAdapterMarker>,
        position: Int
    ): Boolean {
        return items[position] is CommonDialogWithSearchMultiselectUi
    }

    override fun onCreateViewHolder(parent: ViewGroup): RecyclerView.ViewHolder {
        return ViewHolder(parent.inflateBinding(ListItemDatePickerBinding::inflate))
    }

    override fun onBindViewHolder(
        items: MutableList<CommonListElementAdapterMarker>,
        position: Int,
        holder: RecyclerView.ViewHolder,
        payloads: MutableList<Any>
    ) {
        (holder as ViewHolder).bind(items[position] as CommonDialogWithSearchMultiselectUi)
    }

    private inner class ViewHolder(
        private val binding: ListItemDatePickerBinding
    ) : RecyclerView.ViewHolder(binding.root) {

        fun bind(model: CommonDialogWithSearchMultiselectUi) {
            binding.tvTitle.text = model.title.getString(binding.root.context)
            binding.tvTitle.maxLines = model.maxLinesTitle
            binding.icQuestion.isVisible = model.question
            binding.icRequired.isVisible = model.required
            if (model.validationError != null) {
                binding.tvError.isVisible = true
                binding.tvError.text = model.validationError?.getString(binding.root.context)
            } else {
                binding.tvError.isVisible = false
            }
            binding.tvError.maxLines = model.maxLinesError
            binding.tvText.text = model.selectedValue?.joinToString { element -> element.text.getString(binding.tvText.context) }
            if (model.selectedValue.isNullOrEmpty().not()) {
                binding.tvText.setBackgroundDrawableResource(R.drawable.bg_text)
                binding.tvText.setTextColorResource(R.color.color_0C53B2)
            } else {
                binding.tvText.setBackgroundDrawableResource(R.drawable.bg_text_not_entered)
                binding.tvText.setTextColorResource(R.color.color_94A3B8)
            }
            binding.tvText.setCompoundDrawablesExt(
                end = R.drawable.ic_arrow_down
            )
            binding.tvText.maxLines = model.maxLinesText
            binding.icErase.isVisible = model.hasEraseButton
            binding.tvText.onClickThrottle {
                clickListener?.invoke(model)
            }
            if (model.hasEraseButton) {
                binding.icErase.onClickThrottle {
                    eraseClickListener?.invoke(model)
                }
            } else {
                binding.icErase.setOnClickListener(null)
            }
            binding.rootLayout.requestFocus()
        }
    }

}