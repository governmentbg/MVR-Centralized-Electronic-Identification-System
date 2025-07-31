/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.notifications.channels.list

import android.view.ViewGroup
import androidx.recyclerview.widget.RecyclerView
import com.digitall.eid.R
import com.digitall.eid.databinding.ListItemNotificationChannelBinding
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.extensions.inflateBinding
import com.digitall.eid.extensions.onClickThrottle
import com.digitall.eid.extensions.setTextColorResource
import com.digitall.eid.models.notifications.channels.NotificationChannelUi
import com.digitall.eid.models.notifications.channels.NotificationChannelsAdapterMarker
import com.hannesdorfmann.adapterdelegates4.AdapterDelegate

class NotificationChannelDelegate :
    AdapterDelegate<MutableList<NotificationChannelsAdapterMarker>>() {

    companion object {
        private const val TAG = "NotificationChannelDelegateTag"
    }

    var clickListener: ((id: String) -> Unit)? = null

    override fun isForViewType(
        items: MutableList<NotificationChannelsAdapterMarker>,
        position: Int
    ): Boolean {
        return items[position] is NotificationChannelUi
    }

    override fun onCreateViewHolder(parent: ViewGroup): RecyclerView.ViewHolder {
        return ViewHolder(parent.inflateBinding(ListItemNotificationChannelBinding::inflate))
    }

    override fun onBindViewHolder(
        items: MutableList<NotificationChannelsAdapterMarker>,
        position: Int,
        holder: RecyclerView.ViewHolder,
        payloads: MutableList<Any>
    ) {
        (holder as ViewHolder).bind(items[position] as NotificationChannelUi)
    }

    private inner class ViewHolder(
        private val binding: ListItemNotificationChannelBinding
    ) : RecyclerView.ViewHolder(binding.root) {

        fun bind(model: NotificationChannelUi) {
            val backgroundColor = if (model.isEnabled) {
                R.color.color_white
            } else {
                R.color.color_F5F5F5
            }
            val textColorRes = if (model.isEnabled) {
                R.color.color_1C3050
            } else {
                R.color.color_4A6484
            }
            binding.rootLayout.setBackgroundResource(backgroundColor)
            binding.tvTitle.text = model.name.getString(binding.root.context)
            binding.tvTitle.setTextColorResource(textColorRes)
            binding.tvDescription.text = model.description.getString(binding.root.context)
            binding.tvDescription.setTextColorResource(textColorRes)
            binding.switchSelected.isChecked = model.isSelected
            binding.switchSelected.isEnabled = model.isEnabled
            binding.switchSelected.onClickThrottle {
                logDebug("onClickThrottle", TAG)
                if (model.isEnabled) {
                    binding.switchSelected.isChecked = !binding.switchSelected.isChecked
                    clickListener?.invoke(model.id)
                }
            }
        }
    }

}