package com.digitall.eid.ui.fragments.common.search.multiselect.list

import android.view.ViewGroup
import android.widget.CompoundButton
import androidx.recyclerview.widget.RecyclerView
import com.digitall.eid.databinding.ListItemBottomSheetWithSearchMultiselectBinding
import com.digitall.eid.extensions.inflateBinding
import com.digitall.eid.extensions.onClickThrottle
import com.digitall.eid.models.list.CommonDialogWithSearchAdapterMarker
import com.digitall.eid.models.list.CommonDialogWithSearchMultiselectItemUi
import com.hannesdorfmann.adapterdelegates4.AdapterDelegate

class CommonBottomSheetWithSearchMultiselectDelegate :
    AdapterDelegate<MutableList<CommonDialogWithSearchAdapterMarker>>() {

    var changeListener: ((model: CommonDialogWithSearchMultiselectItemUi) -> Unit)? = null

    override fun isForViewType(
        items: MutableList<CommonDialogWithSearchAdapterMarker>,
        position: Int
    ): Boolean {
        return items[position] is CommonDialogWithSearchMultiselectItemUi
    }

    override fun onCreateViewHolder(parent: ViewGroup): RecyclerView.ViewHolder {
        return ViewHolder(parent.inflateBinding(ListItemBottomSheetWithSearchMultiselectBinding::inflate))
    }

    override fun onBindViewHolder(
        items: MutableList<CommonDialogWithSearchAdapterMarker>,
        position: Int,
        holder: RecyclerView.ViewHolder,
        payloads: MutableList<Any>
    ) {
        (holder as ViewHolder).bind(items[position] as CommonDialogWithSearchMultiselectItemUi)
    }

    private inner class ViewHolder(
        private val binding: ListItemBottomSheetWithSearchMultiselectBinding
    ) : RecyclerView.ViewHolder(binding.root) {

        private var model: CommonDialogWithSearchMultiselectItemUi? = null

        private val listener = CompoundButton.OnCheckedChangeListener { _, isChecked ->
            model?.let {
                changeListener?.invoke(
                    it.copy(
                        isSelected = isChecked
                    )
                )
            }
        }

        fun bind(model: CommonDialogWithSearchMultiselectItemUi) {
            this.model = model
            binding.tvTitle.text = model.text.getString(binding.root.context)
            binding.tvTitle.maxLines = model.maxLines
            binding.root.isEnabled = model.selectable
            binding.checkBox.setOnCheckedChangeListener(null)
            binding.checkBox.isChecked = model.isSelected
            binding.checkBox.setOnCheckedChangeListener(listener)
            binding.tvTitle.onClickThrottle {
                changeListener?.invoke(
                    model.copy(
                        isSelected = !model.isSelected
                    )
                )
                binding.checkBox.setOnCheckedChangeListener(null)
                binding.checkBox.isChecked = !model.isSelected
                binding.checkBox.setOnCheckedChangeListener(listener)
            }
        }
    }
}