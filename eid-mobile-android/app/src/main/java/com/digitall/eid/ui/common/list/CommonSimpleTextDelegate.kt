/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.common.list

import android.view.ViewGroup
import androidx.core.content.ContextCompat
import androidx.core.graphics.drawable.DrawableCompat
import androidx.core.view.isVisible
import androidx.recyclerview.widget.RecyclerView
import com.digitall.eid.databinding.ListItemSimpleTextBinding
import com.digitall.eid.extensions.inflateBinding
import com.digitall.eid.extensions.onClickThrottle
import com.digitall.eid.extensions.setTextWithIcon
import com.digitall.eid.models.list.CommonListElementAdapterMarker
import com.digitall.eid.models.list.CommonSimpleTextUi
import com.hannesdorfmann.adapterdelegates4.AdapterDelegate
import org.koin.core.component.KoinComponent

class CommonSimpleTextDelegate :
    AdapterDelegate<MutableList<CommonListElementAdapterMarker>>(), KoinComponent {

    companion object {
        private const val TAG = "CommonSimpleTextDelegateTag"
    }

    var clickListener: ((model: CommonSimpleTextUi) -> Unit)? = null
    var actionListener: ((model: CommonSimpleTextUi) -> Unit)? = null

    override fun isForViewType(
        items: MutableList<CommonListElementAdapterMarker>,
        position: Int
    ): Boolean {
        return items[position] is CommonSimpleTextUi
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
        (holder as ViewHolder).bind(items[position] as CommonSimpleTextUi)
    }

    private inner class ViewHolder(
        private val binding: ListItemSimpleTextBinding
    ) : RecyclerView.ViewHolder(binding.root) {

        fun bind(model: CommonSimpleTextUi) {
            binding.tvTitle.text = model.title.getString(binding.root.context)
            binding.tvValue.setTextWithIcon(
                textStringSource = model.text,
                textColorRes = model.colorRes,
                isClickable = model.isClickable,
                iconResLeft = model.iconResLeft,
                iconResRight = model.iconResRight,
                textMaxLines = model.maxLines,
            )
            binding.btnEdit.isVisible = model.action != null
            model.action?.let { action ->
                binding.btnEdit.setImageResource(action.icon)
                DrawableCompat.setTint(
                    DrawableCompat.wrap(binding.btnEdit.drawable),
                    ContextCompat.getColor(binding.root.context, action.color)
                )
                binding.btnEdit.onClickThrottle {
                    actionListener?.invoke(model)
                }
            }
            binding.rootLayout.requestFocus()
            binding.tvValue.onClickThrottle {
                if (model.isClickable) {
                    clickListener?.invoke(model)
                }
            }
        }
    }
}