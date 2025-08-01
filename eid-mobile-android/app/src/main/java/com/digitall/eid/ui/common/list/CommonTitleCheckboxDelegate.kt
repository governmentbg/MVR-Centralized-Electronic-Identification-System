package com.digitall.eid.ui.common.list

import android.view.ViewGroup
import android.widget.CompoundButton
import androidx.recyclerview.widget.RecyclerView
import com.digitall.eid.databinding.ListItemTitleCheckboxBinding
import com.digitall.eid.extensions.inflateBinding
import com.digitall.eid.models.list.CommonListElementAdapterMarker
import com.digitall.eid.models.list.CommonTitleCheckboxUi
import com.hannesdorfmann.adapterdelegates4.AdapterDelegate

class CommonTitleCheckboxDelegate :
    AdapterDelegate<MutableList<CommonListElementAdapterMarker>>() {

    var changeListener: ((model: CommonTitleCheckboxUi) -> Unit)? = null

    override fun isForViewType(
        items: MutableList<CommonListElementAdapterMarker>,
        position: Int
    ): Boolean {
        return items[position] is CommonTitleCheckboxUi
    }

    override fun onCreateViewHolder(parent: ViewGroup): RecyclerView.ViewHolder {
        return ViewHolder(parent.inflateBinding(ListItemTitleCheckboxBinding::inflate))
    }

    override fun onBindViewHolder(
        items: MutableList<CommonListElementAdapterMarker>,
        position: Int,
        holder: RecyclerView.ViewHolder,
        payloads: MutableList<Any>
    ) {
        (holder as ViewHolder).bind(items[position] as CommonTitleCheckboxUi)
    }

    private inner class ViewHolder(
        private val binding: ListItemTitleCheckboxBinding
    ) : RecyclerView.ViewHolder(binding.root) {

        private var model: CommonTitleCheckboxUi? = null

        private val listener = CompoundButton.OnCheckedChangeListener { _, isChecked ->
            model?.let {
                changeListener?.invoke(
                    it.copy(
                        isChecked = isChecked
                    )
                )
            }
        }

        fun bind(model: CommonTitleCheckboxUi) {
            this.model = model
            binding.tvTitle.text = model.title.getString(binding.root.context)
            binding.checkBox.setOnCheckedChangeListener(null)
            binding.checkBox.isChecked = model.isChecked
            binding.checkBox.isEnabled = model.isEnabled
            binding.checkBox.setOnCheckedChangeListener(listener)
        }
    }
}