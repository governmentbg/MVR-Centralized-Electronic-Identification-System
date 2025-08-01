package com.digitall.eid.ui.common.list

import android.view.ViewGroup
import androidx.core.content.ContextCompat
import androidx.core.graphics.drawable.DrawableCompat
import androidx.core.view.isVisible
import androidx.recyclerview.widget.RecyclerView
import com.digitall.eid.databinding.ListItemTitleDescriptionBinding
import com.digitall.eid.extensions.inflateBinding
import com.digitall.eid.extensions.onClickThrottle
import com.digitall.eid.models.list.CommonTitleDescriptionUi
import com.digitall.eid.models.list.CommonListElementAdapterMarker
import com.hannesdorfmann.adapterdelegates4.AdapterDelegate

class CommonTitleDescriptionDelegate:
    AdapterDelegate<MutableList<CommonListElementAdapterMarker>>() {

    var actionListener: ((model: CommonTitleDescriptionUi) -> Unit)? = null

    override fun isForViewType(
        items: MutableList<CommonListElementAdapterMarker>,
        position: Int
    ): Boolean {
        return items[position] is CommonTitleDescriptionUi
    }

    override fun onCreateViewHolder(parent: ViewGroup): RecyclerView.ViewHolder {
        return ViewHolder(parent.inflateBinding(ListItemTitleDescriptionBinding::inflate))
    }

    override fun onBindViewHolder(
        items: MutableList<CommonListElementAdapterMarker>,
        position: Int,
        holder: RecyclerView.ViewHolder,
        payloads: MutableList<Any>
    ) {
        (holder as ViewHolder).bind(items[position] as CommonTitleDescriptionUi)
    }

    private inner class ViewHolder(
        private val binding: ListItemTitleDescriptionBinding
    ) : RecyclerView.ViewHolder(binding.root) {

        fun bind(model: CommonTitleDescriptionUi) {
            binding.tvTitle.text = model.title.getString(binding.root.context)
            binding.tvDescription.text = model.description.getString(binding.root.context)
            binding.btnEdit.isVisible = model.action != null
            model.action?.let { action ->
                binding.btnEdit.setImageResource(action.icon)
                DrawableCompat.setTint(
                    DrawableCompat.wrap(binding.btnEdit.drawable),
                    ContextCompat.getColor(binding.root.context, action.color)
                )
                binding.btnEdit.onClickThrottle {
                    actionListener?.invoke(model)
                }
            }
        }
    }
}