/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.extensions

import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.annotation.LayoutRes
import androidx.core.view.ViewCompat
import androidx.core.view.forEach
import androidx.viewbinding.ViewBinding

/**
 * Because we have a lot of fragments here, the first fragment would catch insets and
 * consume them. So we need manually to throw insets to all fragments (not only the first)
 * one by one. And then they will decide to use insets or not.
 *
 * This is because of [android:fitsSystemWindows] flag behavior. It consumes all
 * insets and do not spread them further or not even for his neighbours by hierarchy.
 * If two fragment in view pager for example, only the first one will get insets.
 *
 * param this - a container of fragments or insets catcher
 */
@Suppress("DEPRECATION")
fun ViewGroup.throwContainerInsetsFurther() {
    setOnApplyWindowInsetsListener { view, insets ->
        var consumed = false
        (view as ViewGroup).forEach { child ->
            val childResult = child.dispatchApplyWindowInsets(insets)
            // If the child consumed the insets, record it
            if (childResult.isConsumed) {
                consumed = true
            }
        }

        // If any of the children consumed the insets, return
        // an appropriate value
        if (consumed) insets.consumeSystemWindowInsets() else insets
    }
    ViewCompat.requestApplyInsets(this)
}

fun ViewGroup.inflate(@LayoutRes layoutRes: Int, attachToRoot: Boolean = false): View =
    LayoutInflater.from(context).inflate(layoutRes, this, attachToRoot)

fun <T : ViewBinding> ViewGroup.inflateBinding(
    inflateFunc: (LayoutInflater, ViewGroup, Boolean) -> T
): T = inflateFunc(LayoutInflater.from(context), this, false)