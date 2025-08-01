/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.empowerment.from.me.all.list

import android.text.SpannableStringBuilder
import android.text.Spanned
import android.text.style.RelativeSizeSpan
import android.view.View
import android.view.ViewGroup
import androidx.core.text.color
import androidx.core.view.isVisible
import androidx.recyclerview.widget.RecyclerView
import com.digitall.eid.R
import com.digitall.eid.databinding.ListItemEmpowermentFromMeBinding
import com.digitall.eid.domain.repository.common.PreferencesRepository
import com.digitall.eid.extensions.inflateBinding
import com.digitall.eid.extensions.onClickThrottle
import com.digitall.eid.extensions.setTextWithIcon
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.empowerment.common.filter.EmpowermentFilterStatusEnumUi
import com.digitall.eid.models.empowerment.from.me.all.EmpowermentFromMeUi
import com.hannesdorfmann.adapterdelegates4.AbsFallbackAdapterDelegate
import org.koin.core.component.KoinComponent
import org.koin.core.component.inject

class EmpowermentFromMeDelegate : AbsFallbackAdapterDelegate<MutableList<EmpowermentFromMeUi>>(),
    KoinComponent {

    private val preferences: PreferencesRepository by inject()

    var copyClickListener: ((model: EmpowermentFromMeUi, anchor: View) -> Unit)? = null
    var signClickListener: ((model: EmpowermentFromMeUi) -> Unit)? = null
    var openClickListener: ((model: EmpowermentFromMeUi) -> Unit)? = null

    override fun onCreateViewHolder(parent: ViewGroup): RecyclerView.ViewHolder {
        return ViewHolder(parent.inflateBinding(ListItemEmpowermentFromMeBinding::inflate))
    }

    override fun onBindViewHolder(
        items: MutableList<EmpowermentFromMeUi>,
        position: Int,
        holder: RecyclerView.ViewHolder,
        payloads: MutableList<Any>
    ) {
        (holder as ViewHolder).bind(items[position])
    }

    private inner class ViewHolder(
        private val binding: ListItemEmpowermentFromMeBinding
    ) : RecyclerView.ViewHolder(binding.root) {

        fun bind(model: EmpowermentFromMeUi) {
            binding.tvNumber.text = StringSource(
                R.string.empowerment_from_me_number_title,
                formatArgs = listOf(model.number)
            ).getString(binding.root.context)
            binding.tvProviderName.text = model.providerName
            binding.tvName.text = model.name
            binding.tvServiceName.text = model.serviceName
            binding.tvDateCreated.text = model.createdOn
            val firstEmpowerer = model.empowered.getString(binding.root.context)
            binding.tvEmpowered.text = if (model.additionalEmpoweredPeople > 0) {
                val additionalEmpowerersText = StringSource(
                    R.string.empowerment_from_me_additional_empowerers,
                    formatArgs = listOf(model.additionalEmpoweredPeople.toString())
                ).getString(binding.root.context)
                val stringBuilder = SpannableStringBuilder()
                stringBuilder.append(firstEmpowerer)
                stringBuilder.append("\n")
                stringBuilder.color(binding.root.context.getColor(R.color.color_0C53B2)) {
                    stringBuilder.append(additionalEmpowerersText)
                }
                stringBuilder.setSpan(
                    RelativeSizeSpan(
                        .75f
                    ),
                    stringBuilder.length - additionalEmpowerersText.length,
                    stringBuilder.length,
                    Spanned.SPAN_EXCLUSIVE_EXCLUSIVE
                )

                stringBuilder
            } else {
                firstEmpowerer
            }
            when (model.status) {
                EmpowermentFilterStatusEnumUi.COLLECTING_AUTHORIZER_SIGNATURES -> {
                    val isSignedByMe =
                        model.originalModel.empowermentSignatures?.any {
                            element -> element.signerUid == preferences.readApplicationInfo()?.userModel?.citizenIdentifier
                        } == true
                    binding.tvStatus.isVisible = isSignedByMe
                    binding.btnSign.isVisible = isSignedByMe.not()
                }

                EmpowermentFilterStatusEnumUi.AWAITING_SIGNATURE -> {
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