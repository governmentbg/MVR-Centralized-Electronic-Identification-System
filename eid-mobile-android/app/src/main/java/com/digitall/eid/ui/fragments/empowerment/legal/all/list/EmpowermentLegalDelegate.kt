package com.digitall.eid.ui.fragments.empowerment.legal.all.list

import android.view.View
import android.view.ViewGroup
import androidx.core.view.isVisible
import androidx.recyclerview.widget.RecyclerView
import com.digitall.eid.databinding.ListItemEmpowermentLegalBinding
import com.digitall.eid.extensions.inflateBinding
import com.digitall.eid.extensions.onClickThrottle
import com.digitall.eid.extensions.setTextWithIcon
import com.digitall.eid.models.empowerment.common.filter.EmpowermentFilterStatusEnumUi
import com.digitall.eid.models.empowerment.legal.all.EmpowermentLegalUi
import com.hannesdorfmann.adapterdelegates4.AbsFallbackAdapterDelegate

class EmpowermentLegalDelegate : AbsFallbackAdapterDelegate<MutableList<EmpowermentLegalUi>>() {

    var copyClickListener: ((model: EmpowermentLegalUi, anchor: View) -> Unit)? = null
    var signClickListener: ((model: EmpowermentLegalUi) -> Unit)? = null
    var openClickListener: ((model: EmpowermentLegalUi) -> Unit)? = null

    override fun onCreateViewHolder(parent: ViewGroup): RecyclerView.ViewHolder {
        return ViewHolder(parent.inflateBinding(ListItemEmpowermentLegalBinding::inflate))
    }

    override fun onBindViewHolder(
        items: MutableList<EmpowermentLegalUi>,
        position: Int,
        holder: RecyclerView.ViewHolder,
        payloads: MutableList<Any>
    ) {
        (holder as ViewHolder).bind(items[position])
    }

    private inner class ViewHolder(
        private val binding: ListItemEmpowermentLegalBinding
    ) : RecyclerView.ViewHolder(binding.root) {

        fun bind(model: EmpowermentLegalUi) {
            binding.tvProviderName.text = model.providerName
            binding.tvName.text = model.name
            binding.tvServiceName.text = model.serviceName
            binding.tvEmpowered.text = model.empowered
            when (model.status) {
                EmpowermentFilterStatusEnumUi.COLLECTING_AUTHORIZER_SIGNATURES -> {
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
            binding.btnSign.onClickThrottle {
                signClickListener?.invoke(model)
            }
            binding.btnCopy.onClickThrottle {
                copyClickListener?.invoke(model, binding.btnCopy)
            }
        }
    }
}
