/**
 * This class helps to understand when the keyboard is open.
 * This is a crutch that checks the layout height changes and
 * suggested that this is keyboard is open when the height
 * difference is more than default subtitle.
 * But it also maybe not a keyboard at all.
 *
 * @param layout - layout of current screen, used to determine offset.
 *                 Use if you screen is not full screen.
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 */
package com.digitall.eid.utils

import android.content.Context
import android.graphics.Rect
import android.view.View
import android.view.ViewTreeObserver
import android.widget.FrameLayout
import androidx.appcompat.app.AppCompatActivity
import com.digitall.eid.R
import com.digitall.eid.extensions.pxDimen
import java.lang.ref.WeakReference
import java.util.LinkedList

class SoftKeyboardStateWatcher(
    context: Context,
    layout: View? = null,
    private var isSoftKeyboardOpened: Boolean = false
) : ViewTreeObserver.OnGlobalLayoutListener {

    interface SoftKeyboardStateListener {
        fun onSoftKeyboardOpened(keyboardHeight: Int)
        fun onSoftKeyboardClosed()
    }

    // We take straight activity layout height, but not activity layout.root as
    // later will take in cashAccount soft control buttons and screen decoration
    // causing wrong calculations
    private val parentView = WeakReference(
        (context as AppCompatActivity)
            .findViewById<FrameLayout>(R.id.navigationContainer)
    )

    private val listeners = LinkedList<SoftKeyboardStateListener>()
    private val minKeyboardHeight = context.pxDimen(R.dimen.min_keyboard_height)
    private var lastObservedHeight = 0
    private val activityRect = Rect()
    private val height = parentView.get()?.height ?: 0
    private val childLayout = WeakReference(layout)
    private var activityDiff = 0
    private var statusBarOffset = 0

    fun setStatusBarOffset(height: Int) {
        statusBarOffset = height
    }

    override fun onGlobalLayout() {
        parentView.get()?.removeCallbacks(runnable)
        parentView.get()?.getWindowVisibleDisplayFrame(activityRect)
        activityDiff = height - activityRect.height() - statusBarOffset
        when {
            isSoftKeyboardOpened.not() && activityDiff > minKeyboardHeight -> {
                isSoftKeyboardOpened = true
                parentView.get()?.post(runnable)
            }
            else -> {
                isSoftKeyboardOpened = false
                notifyOnSoftKeyboardClosed()
            }
        }
    }

    private val runnable = Runnable {
        notifyOnSoftKeyboardOpened(activityDiff)
    }

    fun addSoftKeyboardStateListener(listener: SoftKeyboardStateListener) {
        listeners.add(listener)
        parentView.get()?.viewTreeObserver?.addOnGlobalLayoutListener(this)
    }

    /**
     * Its important to remove that listener.
     */
    fun removeSoftKeyboardStateListener(listener: SoftKeyboardStateListener) {
        if (parentView.get()?.viewTreeObserver?.isAlive == true) {
            parentView.get()?.viewTreeObserver?.removeOnGlobalLayoutListener(this)
        }
        listeners.remove(listener)
    }

    private fun notifyOnSoftKeyboardOpened(keyboardHeight: Int) {
        lastObservedHeight = keyboardHeight
        //if root view and current view are not same   (eg on screens with tabs) we set offset
        val child = childLayout.get()
        val offset =
            child?.let {
                height - it.height
            } ?: 0
        for (listener in listeners) {
            listener.onSoftKeyboardOpened(keyboardHeight - offset)
        }
    }

    private fun notifyOnSoftKeyboardClosed() {
        for (listener in listeners) {
            listener.onSoftKeyboardClosed()
        }
    }
}