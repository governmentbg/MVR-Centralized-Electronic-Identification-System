/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.view

import android.content.Context
import android.util.AttributeSet
import androidx.appcompat.widget.AppCompatImageView
import com.digitall.eid.R
import com.digitall.eid.domain.models.common.SelectionState
import com.digitall.eid.extensions.onClickThrottle

class ThreeStateCheckBoxView @JvmOverloads constructor(
    context: Context,
    attrs: AttributeSet? = null,
) : AppCompatImageView(context, attrs) {

    var clickListener: ((currentState: SelectionState) -> Unit)? = null

    private lateinit var state: SelectionState

    init {
        setState(SelectionState.NOT_SELECTED)
        setupControls()
    }

    fun setState(state: SelectionState) {
        this.state = state
        when (state) {
            SelectionState.SELECTED -> setImageResource(R.drawable.checkbox_selected_active)
            SelectionState.SELECTED_NOT_ACTIVE -> setImageResource(R.drawable.checkbox_selected_not_active)
            SelectionState.NOT_SELECTED -> setImageResource(R.drawable.checkbox_not_selected_active)
            SelectionState.NOT_SELECTED_NOT_ACTIVE -> setImageResource(R.drawable.checkbox_not_selected_not_active)
            SelectionState.INDETERMINATE -> setImageResource(R.drawable.checkbox_intermediate_active)
            SelectionState.INDETERMINATE_NOT_ACTIVE -> setImageResource(R.drawable.checkbox_intermediate_not_active)
        }
    }

    private fun setupControls() {
        onClickThrottle { clickListener?.invoke(state) }
    }

}