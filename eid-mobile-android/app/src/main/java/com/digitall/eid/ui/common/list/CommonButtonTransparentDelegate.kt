/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.common.list

import android.view.ViewGroup
import androidx.recyclerview.widget.RecyclerView
import com.digitall.eid.databinding.ListItemTransparentButtonBinding
import com.digitall.eid.extensions.inflateBinding
import com.digitall.eid.extensions.onClickThrottle
import com.digitall.eid.models.list.CommonButtonTransparentUi
import com.digitall.eid.models.list.CommonListElementAdapterMarker
import com.hannesdorfmann.adapterdelegates4.AdapterDelegate

class CommonButtonTransparentDelegate :
    AdapterDelegate<MutableList<CommonListElementAdapterMarker>>() {

    var clickListener: ((model: CommonButtonTransparentUi) -> Unit)? = null

    override fun isForViewType(
        items: MutableList<CommonListElementAdapterMarker>,
        position: Int
    ): Boolean {
        return items[position] is CommonButtonTransparentUi
    }

    override fun onCreateViewHolder(parent: ViewGroup): RecyclerView.ViewHolder {
        return ViewHolder(parent.inflateBinding(ListItemTransparentButtonBinding::inflate))
    }

    override fun onBindViewHolder(
        items: MutableList<CommonListElementAdapterMarker>,
        position: Int,
        holder: RecyclerView.ViewHolder,
        payloads: MutableList<Any>
    ) {
        (holder as ViewHolder).bind(items[position] as CommonButtonTransparentUi)
    }

    private inner class ViewHolder(
        private val binding: ListItemTransparentButtonBinding
    ) : RecyclerView.ViewHolder(binding.root) {

        fun bind(model: CommonButtonTransparentUi) {
            binding.tvTitle.text = model.title.getString(binding.root.context)
            binding.tvTitle.onClickThrottle {
                clickListener?.invoke(model)
            }
            binding.rootLayout.requestFocus()
        }
    }
}