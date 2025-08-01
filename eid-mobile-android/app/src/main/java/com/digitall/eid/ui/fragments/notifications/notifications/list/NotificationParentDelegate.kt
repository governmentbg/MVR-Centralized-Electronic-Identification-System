/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.notifications.notifications.list

import android.view.ViewGroup
import androidx.recyclerview.widget.RecyclerView
import com.digitall.eid.R
import com.digitall.eid.databinding.ListItemNotificationParentBinding
import com.digitall.eid.domain.models.common.SelectionState
import com.digitall.eid.extensions.inflateBinding
import com.digitall.eid.extensions.onClickThrottle
import com.digitall.eid.extensions.setBackgroundColorResource
import com.digitall.eid.extensions.setTextColorResource
import com.digitall.eid.models.notifications.notifications.NotificationAdapterMarker
import com.digitall.eid.models.notifications.notifications.NotificationParentUi
import com.hannesdorfmann.adapterdelegates4.AdapterDelegate

class NotificationParentDelegate :
    AdapterDelegate<MutableList<NotificationAdapterMarker>>() {

    var clickListener: ((id: String) -> Unit)? = null
    var checkBoxClickListener: ((id: String) -> Unit)? = null

    override fun isForViewType(
        items: MutableList<NotificationAdapterMarker>,
        position: Int
    ): Boolean {
        return items[position] is NotificationParentUi
    }

    override fun onCreateViewHolder(parent: ViewGroup): RecyclerView.ViewHolder {
        return ViewHolder(parent.inflateBinding(ListItemNotificationParentBinding::inflate))
    }

    override fun onBindViewHolder(
        items: MutableList<NotificationAdapterMarker>,
        position: Int,
        holder: RecyclerView.ViewHolder,
        payloads: MutableList<Any>
    ) {
        (holder as ViewHolder).bind(items[position] as NotificationParentUi)
    }

    private inner class ViewHolder(
        private val binding: ListItemNotificationParentBinding
    ) : RecyclerView.ViewHolder(binding.root) {

        fun bind(model: NotificationParentUi) {
            val backgroundColorRes = when (model.selectionState) {
                SelectionState.SELECTED,
                SelectionState.NOT_SELECTED,
                SelectionState.INDETERMINATE -> R.color.color_white

                else -> R.color.color_F5F5F5
            }
            val textColorRes = when (model.selectionState) {
                SelectionState.SELECTED,
                SelectionState.NOT_SELECTED,
                SelectionState.INDETERMINATE -> R.color.color_1C3050

                else -> R.color.color_4A6484
            }
            binding.checkBox.setState(model.selectionState)
            binding.tvTitle.text = model.name
            if (model.isOpened) {
                binding.icArrow.setImageResource(R.drawable.ic_arrow_down)
            } else {
                binding.icArrow.setImageResource(R.drawable.ic_arrow_right)
            }
            binding.tvTitle.setTextColorResource(textColorRes)
            binding.rootLayout.setBackgroundColorResource(backgroundColorRes)
            binding.rootLayout.onClickThrottle {
                clickListener?.invoke(model.id)
            }
            binding.checkBox.onClickThrottle {
                when (model.selectionState) {
                    SelectionState.SELECTED,
                    SelectionState.NOT_SELECTED,
                    SelectionState.INDETERMINATE -> {
                        checkBoxClickListener?.invoke(model.id)
                    }

                    else -> {
                        // can not be changed
                    }
                }
            }
        }
    }
}