/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.extensions

import androidx.recyclerview.widget.RecyclerView
import androidx.recyclerview.widget.SimpleItemAnimator
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.utils.RecyclerViewAdapterDataObserver

/**
 * This method helps to remove the top overscroll effect from the recycler view.
 */

private const val TAG = "RecyclerViewExtTag"

fun RecyclerView.enableChangeAnimations(isEnable: Boolean) {
    (itemAnimator as SimpleItemAnimator).supportsChangeAnimations = isEnable
}

fun RecyclerView.Adapter<RecyclerView.ViewHolder>.registerChangeStateObserver(
    observer: RecyclerViewAdapterDataObserver,
    changeStateListener: (() -> Unit),
) {
    try {
        registerAdapterDataObserver(observer)
        observer.changeStateListener = changeStateListener
    } catch (e: Exception) {
        logError("registerChangeStateObserver Exception: ${e.message}", e, TAG)
    }
}

fun RecyclerView.Adapter<RecyclerView.ViewHolder>.unregisterChangeStateObserver(
    observer: RecyclerViewAdapterDataObserver,
) {
    try {
        if (hasObservers()) {
            unregisterAdapterDataObserver(observer)
        }
        observer.changeStateListener = null
    } catch (e: Exception) {
        logError("unregisterChangeStateObserver Exception: ${e.message}", e, TAG)
    }
}

