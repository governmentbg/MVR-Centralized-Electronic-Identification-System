/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.common.list

import android.view.ViewGroup
import androidx.recyclerview.widget.RecyclerView
import com.digitall.eid.databinding.ListItemSimpleTextInFieldBinding
import com.digitall.eid.extensions.inflateBinding
import com.digitall.eid.extensions.onClickThrottle
import com.digitall.eid.extensions.setTextWithIcon
import com.digitall.eid.models.list.CommonListElementAdapterMarker
import com.digitall.eid.models.list.CommonSimpleTextInFieldUi
import com.hannesdorfmann.adapterdelegates4.AdapterDelegate
import org.koin.core.component.KoinComponent

class CommonSimpleInFieldTextDelegate :
    AdapterDelegate<MutableList<CommonListElementAdapterMarker>>(), KoinComponent {

    companion object {
        private const val TAG = "CommonSimpleInFieldTextDelegateTag"
    }

    var clickListener: ((model: CommonSimpleTextInFieldUi) -> Unit)? = null

    override fun isForViewType(
        items: MutableList<CommonListElementAdapterMarker>,
        position: Int
    ): Boolean {
        return items[position] is CommonSimpleTextInFieldUi
    }

    override fun onCreateViewHolder(parent: ViewGroup): RecyclerView.ViewHolder {
        return ViewHolder(parent.inflateBinding(ListItemSimpleTextInFieldBinding::inflate))
    }

    override fun onBindViewHolder(
        items: MutableList<CommonListElementAdapterMarker>,
        position: Int,
        holder: RecyclerView.ViewHolder,
        payloads: MutableList<Any>
    ) {
        (holder as ViewHolder).bind(items[position] as CommonSimpleTextInFieldUi)
    }

    private inner class ViewHolder(
        private val binding: ListItemSimpleTextInFieldBinding
    ) : RecyclerView.ViewHolder(binding.root) {

        fun bind(model: CommonSimpleTextInFieldUi) {
            binding.tvTitle.text = model.title.getString(binding.root.context)
            binding.tvValue.text = model.text.getString(binding.root.context)
            binding.tvValue.setTextWithIcon(
                textMaxLines = model.maxLines,
                textStringSource = model.text,
                textColorRes = model.textColorRes,
                isClickable = model.isClickable,
                iconResLeft = model.iconResLeft,
                iconResRight = model.iconResRight
            )
            binding.tvValue.onClickThrottle {
                if (model.isClickable) {
                    clickListener?.invoke(model)
                }
            }
            binding.rootLayout.requestFocus()
        }
    }
}