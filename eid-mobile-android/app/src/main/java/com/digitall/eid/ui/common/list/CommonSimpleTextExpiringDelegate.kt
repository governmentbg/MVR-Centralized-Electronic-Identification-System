package com.digitall.eid.ui.common.list

import android.text.SpannableStringBuilder
import android.text.Spanned
import android.text.style.RelativeSizeSpan
import android.view.ViewGroup
import androidx.core.content.ContextCompat
import androidx.recyclerview.widget.RecyclerView
import com.digitall.eid.R
import com.digitall.eid.databinding.ListItemSimpleTextBinding
import com.digitall.eid.extensions.inflateBinding
import com.digitall.eid.extensions.onClickThrottle
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonListElementAdapterMarker
import com.digitall.eid.models.list.CommonSimpleTextExpiringUi
import com.digitall.eid.utils.RoundedBackgroundSpan
import com.hannesdorfmann.adapterdelegates4.AdapterDelegate
import org.koin.core.component.KoinComponent

class CommonSimpleTextExpiringDelegate :
    AdapterDelegate<MutableList<CommonListElementAdapterMarker>>(), KoinComponent {

    companion object {
        private const val TAG = "CommonSimpleTextDelegateTag"
    }

    var clickListener: ((model: CommonSimpleTextExpiringUi) -> Unit)? = null

    override fun isForViewType(
        items: MutableList<CommonListElementAdapterMarker>,
        position: Int
    ): Boolean {
        return items[position] is CommonSimpleTextExpiringUi
    }

    override fun onCreateViewHolder(parent: ViewGroup): RecyclerView.ViewHolder {
        return ViewHolder(parent.inflateBinding(ListItemSimpleTextBinding::inflate))
    }

    override fun onBindViewHolder(
        items: MutableList<CommonListElementAdapterMarker>,
        position: Int,
        holder: RecyclerView.ViewHolder,
        payloads: MutableList<Any>
    ) {
        (holder as ViewHolder).bind(items[position] as CommonSimpleTextExpiringUi)
    }

    private inner class ViewHolder(
        private val binding: ListItemSimpleTextBinding
    ) : RecyclerView.ViewHolder(binding.root) {

        fun bind(model: CommonSimpleTextExpiringUi) {
            binding.tvTitle.text = model.title.getString(binding.root.context)
            binding.tvValue.text = if (model.isExpiring) {
                val expiringSoonString =
                    StringSource(R.string.certificate_expiring_soon).getString(binding.root.context)
                        .uppercase()
                val stringBuilder = SpannableStringBuilder()
                stringBuilder.append("${model.text.getString(binding.root.context)}\n")
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
            } else model.text.getString(binding.root.context)
            binding.rootLayout.requestFocus()
            binding.tvValue.onClickThrottle {
                if (model.isClickable) {
                    clickListener?.invoke(model)
                }
            }
        }
    }
}