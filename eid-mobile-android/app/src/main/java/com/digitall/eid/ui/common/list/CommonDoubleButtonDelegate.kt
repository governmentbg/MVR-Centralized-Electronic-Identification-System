/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.common.list

import android.view.ViewGroup
import androidx.recyclerview.widget.RecyclerView
import com.digitall.eid.R
import com.digitall.eid.databinding.ListItemDoubleButtonBinding
import com.digitall.eid.extensions.inflateBinding
import com.digitall.eid.extensions.onClickThrottle
import com.digitall.eid.extensions.setBackgroundDrawableResource
import com.digitall.eid.models.common.ButtonColorUi
import com.digitall.eid.models.list.CommonDoubleButtonUi
import com.digitall.eid.models.list.CommonListElementAdapterMarker
import com.hannesdorfmann.adapterdelegates4.AdapterDelegate

class CommonDoubleButtonDelegate :
    AdapterDelegate<MutableList<CommonListElementAdapterMarker>>() {

    var clickListener: ((model: CommonDoubleButtonUi) -> Unit)? = null

    override fun isForViewType(
        items: MutableList<CommonListElementAdapterMarker>,
        position: Int
    ): Boolean {
        return items[position] is CommonDoubleButtonUi
    }

    override fun onCreateViewHolder(parent: ViewGroup): RecyclerView.ViewHolder {
        return ViewHolder(parent.inflateBinding(ListItemDoubleButtonBinding::inflate))
    }

    override fun onBindViewHolder(
        items: MutableList<CommonListElementAdapterMarker>,
        position: Int,
        holder: RecyclerView.ViewHolder,
        payloads: MutableList<Any>
    ) {
        (holder as ViewHolder).bind(items[position] as CommonDoubleButtonUi)
    }

    private inner class ViewHolder(
        private val binding: ListItemDoubleButtonBinding
    ) : RecyclerView.ViewHolder(binding.root) {

        fun bind(model: CommonDoubleButtonUi) {
            binding.firstButton.text = model.firstButton.title.getString(binding.root.context)
            binding.secondButton.text = model.secondButton.title.getString(binding.root.context)
            val firstButtonBackgroundDrawableResource = when (model.firstButton.buttonColor) {
                ButtonColorUi.BLUE -> R.drawable.bg_ripple_button_background_295999
                ButtonColorUi.RED -> R.drawable.bg_ripple_button_background_bf1212
                ButtonColorUi.GREEN -> R.drawable.bg_ripple_button_background_018930
                ButtonColorUi.ORANGE -> R.drawable.bg_ripple_button_background_f59e0b
                ButtonColorUi.TRANSPARENT -> R.drawable.bg_ripple_button_background_transparent
            }
            val secondButtonBackgroundDrawableResource = when (model.secondButton.buttonColor) {
                ButtonColorUi.BLUE -> R.drawable.bg_ripple_button_background_295999
                ButtonColorUi.RED -> R.drawable.bg_ripple_button_background_bf1212
                ButtonColorUi.GREEN -> R.drawable.bg_ripple_button_background_018930
                ButtonColorUi.ORANGE -> R.drawable.bg_ripple_button_background_f59e0b
                ButtonColorUi.TRANSPARENT -> R.drawable.bg_ripple_button_background_transparent
            }
            binding.firstButton.setBackgroundDrawableResource(firstButtonBackgroundDrawableResource)
            binding.secondButton.setBackgroundDrawableResource(
                secondButtonBackgroundDrawableResource
            )
            binding.firstButton.onClickThrottle {
                clickListener?.invoke(
                    model.copy(
                        selectedIdentifier = model.firstButton.identifier
                    )
                )
            }
            binding.secondButton.onClickThrottle {
                clickListener?.invoke(
                    model.copy(
                        selectedIdentifier = model.secondButton.identifier
                    )
                )
            }
            binding.rootLayout.requestFocus()
        }
    }
}