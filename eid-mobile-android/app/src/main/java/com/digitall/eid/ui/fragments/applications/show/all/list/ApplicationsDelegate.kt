/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.applications.show.all.list

import android.view.View
import android.view.ViewGroup
import androidx.core.view.isVisible
import androidx.recyclerview.widget.RecyclerView
import com.digitall.eid.databinding.ListItemApplicationBinding
import com.digitall.eid.extensions.inflateBinding
import com.digitall.eid.extensions.onClickThrottle
import com.digitall.eid.extensions.setTextSource
import com.digitall.eid.extensions.setTextWithIcon
import com.digitall.eid.models.applications.all.ApplicationStatusEnum
import com.digitall.eid.models.applications.all.ApplicationUi
import com.hannesdorfmann.adapterdelegates4.AbsFallbackAdapterDelegate

class ApplicationsDelegate :
    AbsFallbackAdapterDelegate<MutableList<ApplicationUi>>() {

    companion object {
        private const val TAG = "ApplicationsDelegateTag"
    }

    var openClickListener: ((model: ApplicationUi) -> Unit)? = null
    var menuClickListener: ((model: ApplicationUi, anchor: View) -> Unit)? = null

    public override fun onCreateViewHolder(parent: ViewGroup): RecyclerView.ViewHolder {
        return ViewHolder(parent.inflateBinding(ListItemApplicationBinding::inflate))
    }

    public override fun onBindViewHolder(
        items: MutableList<ApplicationUi>,
        position: Int,
        holder: RecyclerView.ViewHolder,
        payloads: MutableList<Any>
    ) {
        (holder as ViewHolder).bind(items[position])
    }

    private inner class ViewHolder(
        private val binding: ListItemApplicationBinding
    ) : RecyclerView.ViewHolder(binding.root) {

        fun bind(model: ApplicationUi) {
            binding.tvApplicationNumber.text = model.applicationNumber
            binding.tvType.setTextSource(model.type.title)
            binding.tvDate.text = model.date
            binding.tvAdministrator.text = model.administrator
            binding.tvDeviceType.setTextSource(model.deviceType)
            when (model.status) {
                ApplicationStatusEnum.PENDING_SIGNATURE -> {
                    binding.tvStatus.isVisible = false
                    binding.btnSign.isVisible = true
                }

                else -> {
                    binding.tvStatus.isVisible = true
                    binding.btnSign.isVisible = false
                }
            }
            binding.tvStatus.setTextWithIcon(
                iconResLeft = model.status.iconRes,
                textColorRes = model.status.colorRes,
                textStringSource = model.status.title,
            )
            binding.rootLayout.onClickThrottle {
                openClickListener?.invoke(model)
            }
            binding.btnMenu.isVisible =
                model.status == ApplicationStatusEnum.PENDING_PAYMENT
            binding.btnMenu.onClickThrottle {
                menuClickListener?.invoke(model, binding.btnMenu)
            }
        }
    }

}