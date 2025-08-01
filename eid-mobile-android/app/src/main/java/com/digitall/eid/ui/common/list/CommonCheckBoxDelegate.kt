/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.common.list

import android.view.ViewGroup
import android.widget.CompoundButton
import androidx.recyclerview.widget.RecyclerView
import com.digitall.eid.databinding.ListItemCheckBoxBinding
import com.digitall.eid.extensions.inflateBinding
import com.digitall.eid.extensions.onClickThrottle
import com.digitall.eid.models.list.CommonCheckBoxUi
import com.digitall.eid.models.list.CommonListElementAdapterMarker
import com.hannesdorfmann.adapterdelegates4.AdapterDelegate

class CommonCheckBoxDelegate :
    AdapterDelegate<MutableList<CommonListElementAdapterMarker>>() {

    var changeListener: ((model: CommonCheckBoxUi) -> Unit)? = null

    override fun isForViewType(
        items: MutableList<CommonListElementAdapterMarker>,
        position: Int
    ): Boolean {
        return items[position] is CommonCheckBoxUi
    }

    override fun onCreateViewHolder(parent: ViewGroup): RecyclerView.ViewHolder {
        return ViewHolder(parent.inflateBinding(ListItemCheckBoxBinding::inflate))
    }

    override fun onBindViewHolder(
        items: MutableList<CommonListElementAdapterMarker>,
        position: Int,
        holder: RecyclerView.ViewHolder,
        payloads: MutableList<Any>
    ) {
        (holder as ViewHolder).bind(items[position] as CommonCheckBoxUi)
    }

    private inner class ViewHolder(
        private val binding: ListItemCheckBoxBinding
    ) : RecyclerView.ViewHolder(binding.root) {

        private var model: CommonCheckBoxUi? = null

        private val listener = CompoundButton.OnCheckedChangeListener { _, isChecked ->
            model?.let {
                changeListener?.invoke(
                    it.copy(
                        isChecked = isChecked
                    )
                )
            }
        }

        fun bind(model: CommonCheckBoxUi) {
            this.model = model
            binding.tvTitle.text = model.title.getString(binding.root.context)
            binding.tvTitle.maxLines = model.maxLines
            binding.checkBox.setOnCheckedChangeListener(null)
            binding.checkBox.isChecked = model.isChecked
            binding.checkBox.isEnabled = model.isEnabled
            binding.checkBox.setOnCheckedChangeListener(listener)
            binding.tvTitle.onClickThrottle {
                changeListener?.invoke(
                    model.copy(
                        isChecked = !model.isChecked
                    )
                )
                binding.checkBox.setOnCheckedChangeListener(null)
                binding.checkBox.isChecked = !model.isChecked
                binding.checkBox.setOnCheckedChangeListener(listener)
            }
            binding.rootLayout.requestFocus()
        }
    }
}