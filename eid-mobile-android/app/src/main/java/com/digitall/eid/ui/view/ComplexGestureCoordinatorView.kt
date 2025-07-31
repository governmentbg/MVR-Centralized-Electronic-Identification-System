/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.view

import android.content.Context
import android.util.AttributeSet
import android.view.MotionEvent
import android.view.View
import androidx.coordinatorlayout.widget.CoordinatorLayout

class ComplexGestureCoordinatorView @JvmOverloads constructor(
    context: Context, attrs: AttributeSet? = null
) : CoordinatorLayout(context, attrs) {

    var stopBottomScroll = false

    private val helper = ComplexGestureTouchHelper()

    override fun onNestedPreScroll(target: View, dx: Int, dy: Int, consumed: IntArray, type: Int) {
        if (dy <= 0 || !stopBottomScroll) {
            super.onNestedPreScroll(target, dx, dy, consumed, type)
        }
    }

    override fun onInterceptTouchEvent(event: MotionEvent): Boolean {
        return if (helper.onInterceptTouchEvent(event)) {
            super.onInterceptTouchEvent(event)
        } else {
            false
        }
    }

}