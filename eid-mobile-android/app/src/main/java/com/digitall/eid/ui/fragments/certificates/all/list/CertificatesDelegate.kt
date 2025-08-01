/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.certificates.all.list

import android.text.SpannableStringBuilder
import android.text.Spanned
import android.text.style.RelativeSizeSpan
import android.view.View
import android.view.ViewGroup
import androidx.core.content.ContextCompat
import androidx.core.view.isVisible
import androidx.recyclerview.widget.RecyclerView
import com.digitall.eid.R
import com.digitall.eid.databinding.ListItemCertificateBinding
import com.digitall.eid.extensions.inflateBinding
import com.digitall.eid.extensions.onClickThrottle
import com.digitall.eid.extensions.setTextSource
import com.digitall.eid.extensions.setTextWithIcon
import com.digitall.eid.models.certificates.all.CertificateUi
import com.digitall.eid.models.certificates.all.CertificatesStatusEnum
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.utils.RoundedBackgroundSpan
import com.hannesdorfmann.adapterdelegates4.AbsFallbackAdapterDelegate

class CertificatesDelegate :
    AbsFallbackAdapterDelegate<MutableList<CertificateUi>>() {

    companion object {
        private const val TAG = "CertificatesDelegateTag"
    }

    var menuClickListener: ((model: CertificateUi, anchor: View) -> Unit)? = null
    var openClickListener: ((model: CertificateUi) -> Unit)? = null

    override fun onCreateViewHolder(parent: ViewGroup): RecyclerView.ViewHolder {
        return ViewHolder(parent.inflateBinding(ListItemCertificateBinding::inflate))
    }

    override fun onBindViewHolder(
        items: MutableList<CertificateUi>,
        position: Int,
        holder: RecyclerView.ViewHolder,
        payloads: MutableList<Any>
    ) {
        (holder as ViewHolder).bind(items[position])
    }

    private inner class ViewHolder(
        private val binding: ListItemCertificateBinding
    ) : RecyclerView.ViewHolder(binding.root) {

        fun bind(model: CertificateUi) {
            binding.tvSerialNumber.text = model.serialNumber
            binding.tvDateFrom.text = model.validityFrom
            binding.tvAlias.text = model.alias
            binding.tvDateTo.text = if (model.isExpiring) {
                val expiringSoonString =
                    StringSource(R.string.certificate_expiring_soon).getString(binding.root.context)
                        .uppercase()
                val stringBuilder = SpannableStringBuilder()
                stringBuilder.append("${model.validityUntil}\n")
                stringBuilder.append(expiringSoonString)
                stringBuilder.setSpan(
                    RoundedBackgroundSpan(
                        cornerRadius = 4,
                        backgroundColor = ContextCompat.getColor(
                            binding.root.context,
                            R.color.color_F59E0B
                        ),
                        textColor = ContextCompat.getColor(
                            binding.root.context,
                            R.color.color_1C3050
                        )
                    ),
                    stringBuilder.length - expiringSoonString.length,
                    stringBuilder.length,
                    Spanned.SPAN_EXCLUSIVE_EXCLUSIVE
                )
                stringBuilder.setSpan(
                    RelativeSizeSpan(
                        .75f
                    ),
                    stringBuilder.length - expiringSoonString.length,
                    stringBuilder.length,
                    Spanned.SPAN_EXCLUSIVE_EXCLUSIVE
                )

                stringBuilder
            } else model.validityUntil
            binding.tvDeviceType.setTextSource(model.deviceType)
            binding.tvStatus.setTextWithIcon(
                iconResRight = model.status.iconRes,
                textColorRes = model.status.colorRes,
                textStringSource = model.status.title,
            )
            binding.rootLayout.onClickThrottle {
                openClickListener?.invoke(model)
            }
            binding.btnMenu.isVisible =
                model.status == CertificatesStatusEnum.ACTIVE || model.status == CertificatesStatusEnum.STOPPED
            binding.btnMenu.onClickThrottle {
                menuClickListener?.invoke(model, binding.btnMenu)
            }
        }
    }

}