/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.common.list

import android.view.ViewGroup
import androidx.core.view.isVisible
import androidx.recyclerview.widget.RecyclerView
import com.digitall.eid.databinding.ListItemEmptySpaceBinding
import com.digitall.eid.extensions.inflateBinding
import com.digitall.eid.models.list.CommonEmptySpaceSizeEnum
import com.digitall.eid.models.list.CommonEmptySpaceUi
import com.digitall.eid.models.list.CommonListElementAdapterMarker
import com.hannesdorfmann.adapterdelegates4.AdapterDelegate
import org.koin.core.component.KoinComponent

class CommonEmptySpaceDelegate :
    AdapterDelegate<MutableList<CommonListElementAdapterMarker>>(), KoinComponent {

    companion object {
        private const val TAG = "CommonSimpleInFieldTextDelegateTag"
    }

    override fun isForViewType(
        items: MutableList<CommonListElementAdapterMarker>,
        position: Int
    ): Boolean {
        return items[position] is CommonEmptySpaceUi
    }

    override fun onCreateViewHolder(parent: ViewGroup): RecyclerView.ViewHolder {
        return ViewHolder(parent.inflateBinding(ListItemEmptySpaceBinding::inflate))
    }

    override fun onBindViewHolder(
        items: MutableList<CommonListElementAdapterMarker>,
        position: Int,
        holder: RecyclerView.ViewHolder,
        payloads: MutableList<Any>
    ) {
        (holder as ViewHolder).bind(items[position] as CommonEmptySpaceUi)
    }

    private inner class ViewHolder(
        private val binding: ListItemEmptySpaceBinding
    ) : RecyclerView.ViewHolder(binding.root) {

        fun bind(model: CommonEmptySpaceUi) {
            binding.size4.isVisible = model.size == CommonEmptySpaceSizeEnum.SIZE_4
            binding.size8.isVisible = model.size == CommonEmptySpaceSizeEnum.SIZE_8
            binding.size12.isVisible = model.size == CommonEmptySpaceSizeEnum.SIZE_12
            binding.size16.isVisible = model.size == CommonEmptySpaceSizeEnum.SIZE_16
        }
    }
}