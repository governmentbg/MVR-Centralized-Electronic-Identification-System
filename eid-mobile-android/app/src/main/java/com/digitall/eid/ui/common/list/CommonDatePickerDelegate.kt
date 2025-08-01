/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.common.list

import android.view.ViewGroup
import androidx.core.view.isVisible
import androidx.recyclerview.widget.RecyclerView
import com.digitall.eid.R
import com.digitall.eid.databinding.ListItemDatePickerBinding
import com.digitall.eid.domain.extensions.toTextDate
import com.digitall.eid.extensions.inflateBinding
import com.digitall.eid.extensions.onClickThrottle
import com.digitall.eid.extensions.setBackgroundDrawableResource
import com.digitall.eid.extensions.setCompoundDrawablesExt
import com.digitall.eid.extensions.setTextColorResource
import com.digitall.eid.models.list.CommonDatePickerUi
import com.digitall.eid.models.list.CommonListElementAdapterMarker
import com.hannesdorfmann.adapterdelegates4.AdapterDelegate
import org.koin.core.component.KoinComponent

class CommonDatePickerDelegate :
    AdapterDelegate<MutableList<CommonListElementAdapterMarker>>(), KoinComponent {

    companion object {
        private const val TAG = "CommonTextFieldDelegateTag"
    }

    var clickListener: ((model: CommonDatePickerUi) -> Unit)? = null
    var eraseClickListener: ((model: CommonDatePickerUi) -> Unit)? = null

    override fun isForViewType(
        items: MutableList<CommonListElementAdapterMarker>,
        position: Int
    ): Boolean {
        return items[position] is CommonDatePickerUi
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
        (holder as ViewHolder).bind(items[position] as CommonDatePickerUi)
    }

    private inner class ViewHolder(
        private val binding: ListItemDatePickerBinding
    ) : RecyclerView.ViewHolder(binding.root) {

        fun bind(model: CommonDatePickerUi) {
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
            binding.tvText.text = model.selectedValue?.toTextDate(
                dateFormat = model.dateFormat
            )
            if (model.selectedValue != null) {
                binding.tvText.setBackgroundDrawableResource(R.drawable.bg_text)
                binding.tvText.setTextColorResource(R.color.color_0C53B2)
            } else {
                binding.tvText.setBackgroundDrawableResource(R.drawable.bg_text_not_entered)
                binding.tvText.setTextColorResource(R.color.color_94A3B8)
            }
            binding.tvText.setCompoundDrawablesExt(
                end = R.drawable.ic_calendar
            )
            binding.icErase.isVisible = model.hasEraseButton
            binding.tvText.maxLines = 1
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