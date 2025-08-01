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
import com.digitall.eid.extensions.clearCompoundDrawablesExt
import com.digitall.eid.extensions.inflateBinding
import com.digitall.eid.extensions.setTextColorResource
import com.digitall.eid.models.list.CommonListElementAdapterMarker
import com.digitall.eid.models.list.CommonTextFieldUi
import com.hannesdorfmann.adapterdelegates4.AdapterDelegate
import org.koin.core.component.KoinComponent

class CommonTextFieldDelegate :
    AdapterDelegate<MutableList<CommonListElementAdapterMarker>>(), KoinComponent {

    companion object {
        private const val TAG = "CommonTextFieldDelegateTag"
    }

    override fun isForViewType(
        items: MutableList<CommonListElementAdapterMarker>,
        position: Int
    ): Boolean {
        return items[position] is CommonTextFieldUi
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
        (holder as ViewHolder).bind(items[position] as CommonTextFieldUi)
    }

    private inner class ViewHolder(
        private val binding: ListItemDatePickerBinding
    ) : RecyclerView.ViewHolder(binding.root) {

        fun bind(model: CommonTextFieldUi) {
            binding.tvTitle.text = model.title.getString(binding.root.context)
            binding.tvTitle.maxLines = model.maxLinesTitle
            binding.icQuestion.isVisible = model.question
            binding.icRequired.isVisible = model.required
            if (model.error != null) {
                binding.tvError.isVisible = true
                binding.tvError.text = model.error.getString(binding.root.context)
            } else {
                binding.tvError.isVisible = false
            }
            binding.tvError.maxLines = model.maxLinesError
            binding.icErase.isVisible = false
            binding.tvText.setOnClickListener(null)
            binding.tvText.clearCompoundDrawablesExt()
            binding.tvText.setTextColorResource(R.color.color_0C53B2)
            binding.tvText.text = model.text.getString(binding.root.context)
            binding.tvText.setBackgroundResource(R.drawable.bg_text_not_editable)
            binding.tvText.maxLines = model.maxLinesText
            binding.rootLayout.requestFocus()
        }
    }
}