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
import android.view.ViewGroup
import android.webkit.WebView
import androidx.swiperefreshlayout.widget.SwipeRefreshLayout
import com.digitall.eid.R
import com.digitall.eid.domain.utils.LogUtil.logDebug

class ComplexGestureRefreshView @JvmOverloads constructor(
    context: Context,
    attrs: AttributeSet? = null
) : SwipeRefreshLayout(context, attrs) {

    companion object {
        private const val TAG = "ComplexGestureRefreshViewTag"
    }

    private var webView: WebView? = null

    private fun findWebViewChildInHierarchy(viewGroup: ViewGroup) {
        for (i in 0 until viewGroup.childCount) {
            val child = viewGroup.getChildAt(i)
            if (child is WebView) {
                webView = child
                return
            } else if (child is ViewGroup) {
                findWebViewChildInHierarchy(child) // Recursive search
            }
        }
    }

    var isGestureAllowed = true
        set(value) {
            field = value
            isEnabled = value
            if (!value) {
                setOnRefreshListener(null)
            }
        }

    private val helper = ComplexGestureTouchHelper()

    init {
        setProgressBackgroundColorSchemeResource(R.color.color_0C53B2)
        setColorSchemeResources(R.color.color_0C53B2)
    }

    override fun onAttachedToWindow() {
        super.onAttachedToWindow()
        if (webView == null) {
            findWebViewChildInHierarchy(this)
        }
    }

    override fun onInterceptTouchEvent(event: MotionEvent): Boolean {
        logDebug("onInterceptTouchEvent isGestureAllowed: $isGestureAllowed", TAG)

        if (webView == null) {
            findWebViewChildInHierarchy(this) // Attempt to find it
        }

        val canWebViewScrollUp = webView?.canScrollVertically(-1) ?: false

        return if ((isGestureAllowed && helper.onInterceptTouchEvent(event)) && canWebViewScrollUp.not()) {
            super.onInterceptTouchEvent(event)
        } else false
    }

    override fun onTouchEvent(event: MotionEvent?): Boolean {
        logDebug("onTouchEvent isGestureAllowed: $isGestureAllowed", TAG)
        return if (isGestureAllowed) {
            super.onTouchEvent(event)
        } else false
    }

    override fun onNestedScroll(
        target: View,
        dxConsumed: Int,
        dyConsumed: Int,
        dxUnconsumed: Int,
        dyUnconsumed: Int
    ) {
        logDebug("onNestedScroll isGestureAllowed: $isGestureAllowed", TAG)
        if (isGestureAllowed) {
            super.onNestedScroll(target, dxConsumed, dyConsumed, dxUnconsumed, dyUnconsumed)
        }
    }

    override fun onNestedFling(
        target: View,
        velocityX: Float,
        velocityY: Float,
        consumed: Boolean
    ): Boolean {
        logDebug("onNestedFling isGestureAllowed: $isGestureAllowed", TAG)
        return if (isGestureAllowed) {
            super.onNestedFling(target, velocityX, velocityY, consumed)
        } else false
    }

}