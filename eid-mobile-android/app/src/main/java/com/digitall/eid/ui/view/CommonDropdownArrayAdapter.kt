/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.view

import android.content.Context
import android.view.View
import android.view.ViewGroup
import android.widget.ArrayAdapter
import androidx.core.view.isInvisible
import androidx.core.view.isVisible
import com.digitall.eid.databinding.ListItemDropDownMenuBinding
import com.digitall.eid.extensions.inflateBinding
import com.digitall.eid.extensions.setTextColorResource
import com.digitall.eid.extensions.tintRes
import com.digitall.eid.models.list.CommonSpinnerMenuItemUi

class CommonDropdownArrayAdapter(
    context: Context,
    private val clickListener: ((model: CommonSpinnerMenuItemUi) -> Unit)
) : ArrayAdapter<CommonSpinnerMenuItemUi>(
    context, 0, mutableListOf()
) {

    override fun getView(position: Int, convertView: View?, parent: ViewGroup): View {
        return parent.inflateBinding(ListItemDropDownMenuBinding::inflate).run {
            val item = getItem(position)!!
            tvItemName.text = item.text.getString(context)
            tvItemName.maxLines = item.maxLines
            ivCheck.isInvisible = !item.isSelected
            divider.isVisible = position < count - 1
            if (item.iconRes != null) {
                ivIcon.isVisible = true
                ivIcon.setImageResource(item.iconRes)
            } else {
                ivIcon.isVisible = false
            }
            tvItemName.setTextColorResource(item.textColorRes)
            if (item.iconColorRes != null) {
                ivIcon.tintRes(item.iconColorRes)
            } else {
                // TODO
            }
            root.setOnClickListener {
                clickListener.invoke(item)
            }
            root
        }
    }

}