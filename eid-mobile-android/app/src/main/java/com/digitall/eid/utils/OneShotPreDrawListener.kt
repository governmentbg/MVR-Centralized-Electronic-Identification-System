/**
 * An OnPreDrawListener that will remove itself after one OnPreDraw call. Typical
 * usage is:
 * OneShotPreDrawListener.add(view, () -> { view.doSomething(); })
 *
 * The listener will also remove itself from the ViewTreeObserver when the view
 * is detached from the view hierarchy. In that case, the Runnable will never be
 * executed.
 *
 * This is a copy of default OneShotPreDrawListener from androidX core lib,
 * but with the ability to determine the callback result of the [onPreDraw]
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 */
package com.digitall.eid.utils

import android.view.View
import android.view.ViewTreeObserver

class OneShotPreDrawListener private constructor(
    private val view: View,
    private val callbackResult: Boolean,
    private val runnable: Runnable
) : ViewTreeObserver.OnPreDrawListener, View.OnAttachStateChangeListener {

    private var mViewTreeObserver = view.viewTreeObserver

    override fun onPreDraw(): Boolean {
        removeListener()
        runnable.run()
        return callbackResult
    }

    /**
     * Removes the listener from the ViewTreeObserver. This is useful to call if the
     * callback should be removed prior to [onPreDraw].
     */
    fun removeListener() {
        if (mViewTreeObserver!!.isAlive) {
            mViewTreeObserver!!.removeOnPreDrawListener(this)
        } else {
            view.viewTreeObserver.removeOnPreDrawListener(this)
        }
        view.removeOnAttachStateChangeListener(this)
    }

    override fun onViewAttachedToWindow(v: View) {
        mViewTreeObserver = v.viewTreeObserver
    }

    override fun onViewDetachedFromWindow(v: View) {
        removeListener()
    }

    companion object {
        /**
         * Creates a OneShotPreDrawListener and adds it to view's ViewTreeObserver.
         * @param view The view whose ViewTreeObserver the OnPreDrawListener should listen.
         * @param runnable The Runnable to execute in the OnPreDraw (once)
         * @return The added OneShotPreDrawListener. It can be removed prior to
         *         the onPreDraw by calling [removeListener].
         */
        fun add(view: View, callbackResult: Boolean, runnable: Runnable): OneShotPreDrawListener {
            val listener = OneShotPreDrawListener(view, callbackResult, runnable)
            view.viewTreeObserver.addOnPreDrawListener(listener)
            view.addOnAttachStateChangeListener(listener)
            return listener
        }
    }
}