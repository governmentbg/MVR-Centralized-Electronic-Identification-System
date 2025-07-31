/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.common.search.normal.list

import android.view.ViewGroup
import androidx.recyclerview.widget.RecyclerView
import com.digitall.eid.databinding.ListItemBottomSheetWithSearchBinding
import com.digitall.eid.extensions.inflateBinding
import com.digitall.eid.extensions.onClickThrottle
import com.digitall.eid.models.list.CommonDialogWithSearchAdapterMarker
import com.digitall.eid.models.list.CommonDialogWithSearchItemUi
import com.hannesdorfmann.adapterdelegates4.AdapterDelegate

class CommonBottomSheetWithSearchDelegate :
    AdapterDelegate<MutableList<CommonDialogWithSearchAdapterMarker>>() {

    var clickListener: ((model: CommonDialogWithSearchItemUi) -> Unit)? = null

    override fun isForViewType(
        items: MutableList<CommonDialogWithSearchAdapterMarker>,
        position: Int
    ): Boolean {
        return items[position] is CommonDialogWithSearchItemUi
    }

    override fun onCreateViewHolder(parent: ViewGroup): RecyclerView.ViewHolder {
        return ViewHolder(parent.inflateBinding(ListItemBottomSheetWithSearchBinding::inflate))
    }

    override fun onBindViewHolder(
        items: MutableList<CommonDialogWithSearchAdapterMarker>,
        position: Int,
        holder: RecyclerView.ViewHolder,
        payloads: MutableList<Any>
    ) {
        (holder as ViewHolder).bind(items[position] as CommonDialogWithSearchItemUi)
    }

    private inner class ViewHolder(
        private val binding: ListItemBottomSheetWithSearchBinding
    ) : RecyclerView.ViewHolder(binding.root) {

        fun bind(model: CommonDialogWithSearchItemUi) {
            binding.tvTitle.text = model.text.getString(binding.root.context)
            binding.tvTitle.maxLines = model.maxLines
            binding.root.isEnabled = model.selectable
            binding.tvTitle.onClickThrottle {
                if (model.selectable.not()) return@onClickThrottle
                clickListener?.invoke(model)
            }
        }
    }
}