/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.extensions

import android.content.Context
import android.graphics.Rect
import android.view.View
import androidx.annotation.ColorInt
import androidx.annotation.ColorRes
import androidx.annotation.DrawableRes
import androidx.core.graphics.ColorUtils
import com.digitall.eid.domain.DELAY_250
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.Job
import kotlinx.coroutines.delay
import kotlinx.coroutines.launch

fun isDark(@ColorInt color: Int): Boolean {
    return ColorUtils.calculateLuminance(color) < 0.5
}

fun Context.dpToPx(dp: Int): Int {
    return (dp * resources.displayMetrics.density).toInt()
}

fun View.backgroundColor(@ColorRes colorRes: Int) {
    setBackgroundColor(context.color(colorRes))
}

/**
 * Ignore the fast series of clicks to prevent multiple action calls.
 *
 * @param call - a click listener
 * @return disposable of rx click method. Should be disposed with view destructor
 */
inline fun View.onClickThrottle(delay: Long = 500L, crossinline call: () -> Unit) {
    setOnClickListener(object : View.OnClickListener {
        var time = 0L
        override fun onClick(v: View?) {
            val newTime = System.currentTimeMillis()
            if (newTime - time > delay) {
                call.invoke()
                time = newTime
            }
        }
    })
}

fun Rect.copy(left: Int? = null, top: Int? = null, right: Int? = null, bottom: Int? = null): Rect {
    return Rect(
        left ?: this.left,
        top ?: this.top,
        right ?: this.right,
        bottom ?: this.bottom
    )
}

/**
 * Method set view background color from resource
 */
fun View.setBackgroundColorResource(@ColorRes resource: Int) {
    setBackgroundColor(context.color(resource))
}

fun View.setBackgroundDrawableResource(@DrawableRes resource: Int) {
    background = context.drawable(resource)
}

fun View.performClickAfterDelay(
    delayMillis: Long = DELAY_250,
    scope: CoroutineScope = CoroutineScope(Dispatchers.Main)
): Job {
    return scope.launch {
        delay(delayMillis)
        // Ensure the view is still attached and clickable before performing the click
        if (this@performClickAfterDelay.isAttachedToWindow && this@performClickAfterDelay.isClickable) {
            this@performClickAfterDelay.performClick()
        }
    }
}