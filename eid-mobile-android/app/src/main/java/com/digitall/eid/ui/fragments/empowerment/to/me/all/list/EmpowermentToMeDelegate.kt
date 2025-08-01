/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.empowerment.to.me.all.list

import android.view.View
import android.view.ViewGroup
import androidx.core.view.isVisible
import androidx.recyclerview.widget.RecyclerView
import com.digitall.eid.R
import com.digitall.eid.databinding.ListItemEmpowermentToMeBinding
import com.digitall.eid.extensions.inflateBinding
import com.digitall.eid.extensions.onClickThrottle
import com.digitall.eid.extensions.setTextWithIcon
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.empowerment.to.me.all.EmpowermentToMeUi
import com.hannesdorfmann.adapterdelegates4.AbsFallbackAdapterDelegate

class EmpowermentToMeDelegate :
    AbsFallbackAdapterDelegate<MutableList<EmpowermentToMeUi>>() {

    var copyClickListener: ((model: EmpowermentToMeUi, anchor: View) -> Unit)? = null
    var openClickListener: ((model: EmpowermentToMeUi) -> Unit)? = null

    override fun onCreateViewHolder(parent: ViewGroup): RecyclerView.ViewHolder {
        return ViewHolder(parent.inflateBinding(ListItemEmpowermentToMeBinding::inflate))
    }

    override fun onBindViewHolder(
        items: MutableList<EmpowermentToMeUi>,
        position: Int,
        holder: RecyclerView.ViewHolder,
        payloads: MutableList<Any>
    ) {
        (holder as ViewHolder).bind(items[position])
    }

    private inner class ViewHolder(
        private val binding: ListItemEmpowermentToMeBinding
    ) : RecyclerView.ViewHolder(binding.root) {

        fun bind(model: EmpowermentToMeUi) {
            binding.tvNumber.text = StringSource(
                R.string.empowerment_to_me_number_title,
                formatArgs = listOf(model.number)
            ).getString(binding.root.context)
            binding.tvProviderName.text = model.providerName
            binding.tvServiceName.text = model.serviceName
            binding.tvEmpower.text = model.empower
            binding.tvStatus.setTextWithIcon(
                iconResLeft = model.status.iconRes,
                textColorRes = model.status.colorRes,
                textStringSource = model.status.title,
            )
            binding.tvDateCreated.text = model.createdOn
            binding.rootLayout.onClickThrottle {
                openClickListener?.invoke(model)
            }
            binding.btnMenu.isVisible = model.spinnerModel != null
            binding.btnMenu.onClickThrottle {
                copyClickListener?.invoke(model, binding.btnMenu)
            }
        }
    }
}