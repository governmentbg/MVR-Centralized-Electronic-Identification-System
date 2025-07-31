/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.common.list

import android.content.res.ColorStateList
import android.view.ViewGroup
import androidx.recyclerview.widget.RecyclerView
import com.digitall.eid.R
import com.digitall.eid.databinding.ListItemButtonBinding
import com.digitall.eid.extensions.inflateBinding
import com.digitall.eid.extensions.onClickThrottle
import com.digitall.eid.extensions.setBackgroundDrawableResource
import com.digitall.eid.extensions.setTextWithIcon
import com.digitall.eid.models.common.ButtonColorUi
import com.digitall.eid.models.list.CommonButtonUi
import com.digitall.eid.models.list.CommonListElementAdapterMarker
import com.hannesdorfmann.adapterdelegates4.AdapterDelegate

class CommonButtonDelegate :
    AdapterDelegate<MutableList<CommonListElementAdapterMarker>>() {

    var clickListener: ((model: CommonButtonUi) -> Unit)? = null

    override fun isForViewType(
        items: MutableList<CommonListElementAdapterMarker>,
        position: Int
    ): Boolean {
        return items[position] is CommonButtonUi
    }

    override fun onCreateViewHolder(parent: ViewGroup): RecyclerView.ViewHolder {
        return ViewHolder(parent.inflateBinding(ListItemButtonBinding::inflate))
    }

    override fun onBindViewHolder(
        items: MutableList<CommonListElementAdapterMarker>,
        position: Int,
        holder: RecyclerView.ViewHolder,
        payloads: MutableList<Any>
    ) {
        (holder as ViewHolder).bind(items[position] as CommonButtonUi)
    }

    private inner class ViewHolder(
        private val binding: ListItemButtonBinding
    ) : RecyclerView.ViewHolder(binding.root) {

        fun bind(model: CommonButtonUi) {
            val backgroundDrawableResource = when (model.buttonColor) {
                ButtonColorUi.BLUE -> R.drawable.bg_ripple_button_background_295999
                ButtonColorUi.RED -> R.drawable.bg_ripple_button_background_bf1212
                ButtonColorUi.GREEN -> R.drawable.bg_ripple_button_background_018930
                ButtonColorUi.ORANGE -> R.drawable.bg_ripple_button_background_f59e0b
                ButtonColorUi.TRANSPARENT -> R.drawable.bg_ripple_button_background_transparent
            }
            binding.button.setBackgroundDrawableResource(backgroundDrawableResource)
            val textColor = if (model.buttonColor != ButtonColorUi.TRANSPARENT) {
                binding.root.context.getColorStateList(R.color.selector_button_white_text_state_colors)
            } else {
                ColorStateList.valueOf(binding.root.context.getColor(R.color.color_0C53B2))
            }
            binding.button.isEnabled = model.isEnabled
            binding.button.setTextWithIcon(
                textStringSource = model.title,
                textColorsRes = textColor,
                iconResLeft = model.iconResLeft,
                iconResRight = model.iconResRight
            )
            binding.button.onClickThrottle {
                clickListener?.invoke(model)
            }
            binding.rootLayout.requestFocus()
        }
    }
}