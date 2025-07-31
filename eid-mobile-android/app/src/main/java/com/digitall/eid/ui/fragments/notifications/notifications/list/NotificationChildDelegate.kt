/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.notifications.notifications.list

import android.view.ViewGroup
import androidx.recyclerview.widget.RecyclerView
import com.digitall.eid.R
import com.digitall.eid.databinding.ListItemNotificationChildBinding
import com.digitall.eid.domain.models.common.SelectionState
import com.digitall.eid.extensions.inflateBinding
import com.digitall.eid.extensions.onClickThrottle
import com.digitall.eid.extensions.setBackgroundColorResource
import com.digitall.eid.extensions.setTextColorResource
import com.digitall.eid.models.notifications.notifications.NotificationAdapterMarker
import com.digitall.eid.models.notifications.notifications.NotificationChildUi
import com.hannesdorfmann.adapterdelegates4.AdapterDelegate

class NotificationChildDelegate : AdapterDelegate<MutableList<NotificationAdapterMarker>>() {

    var clickListener: ((id: String) -> Unit)? = null

    override fun isForViewType(
        items: MutableList<NotificationAdapterMarker>, position: Int
    ): Boolean {
        return items[position] is NotificationChildUi
    }

    override fun onCreateViewHolder(parent: ViewGroup): RecyclerView.ViewHolder {
        return ViewHolder(parent.inflateBinding(ListItemNotificationChildBinding::inflate))
    }

    override fun onBindViewHolder(
        items: MutableList<NotificationAdapterMarker>,
        position: Int,
        holder: RecyclerView.ViewHolder,
        payloads: MutableList<Any>
    ) {
        (holder as ViewHolder).bind(items[position] as NotificationChildUi)
    }

    private inner class ViewHolder(
        private val binding: ListItemNotificationChildBinding
    ) : RecyclerView.ViewHolder(binding.root) {

        fun bind(model: NotificationChildUi) {
            val backgroundColorRes = if (model.isMandatory == true) {
                R.color.color_F5F5F5
            } else {
                R.color.color_white
            }
            val textColorRes = when {
                model.isSelected && model.isMandatory == true -> R.color.color_4A6484
                model.isSelected && model.isMandatory != true -> R.color.color_1C3050
                !model.isSelected && model.isMandatory == true -> R.color.color_4A6484
                !model.isSelected && model.isMandatory != true -> R.color.color_1C3050
                else -> R.color.color_1C3050
            }
            val checkBoxState = when {
                model.isSelected && model.isMandatory == true -> SelectionState.SELECTED_NOT_ACTIVE
                model.isSelected && model.isMandatory != true -> SelectionState.SELECTED
                !model.isSelected && model.isMandatory == true -> SelectionState.NOT_SELECTED_NOT_ACTIVE
                !model.isSelected && model.isMandatory != true -> SelectionState.NOT_SELECTED
                else -> SelectionState.NOT_SELECTED_NOT_ACTIVE
            }
            binding.tvTitle.text = model.name
            binding.tvTitle.setTextColorResource(textColorRes)
            binding.checkBox.setState(checkBoxState)
            binding.rootLayout.setBackgroundColorResource(backgroundColorRes)
            binding.rootLayout.onClickThrottle {
                if (model.isMandatory != true) {
                    clickListener?.invoke(model.id)
                }
            }
            binding.checkBox.onClickThrottle {
                if (model.isMandatory != true) {
                    clickListener?.invoke(model.id)
                }
            }
        }
    }

}