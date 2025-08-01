package com.digitall.eid.ui.common.list

import androidx.recyclerview.widget.GridLayoutManager
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import androidx.recyclerview.widget.StaggeredGridLayoutManager
import com.digitall.eid.models.list.CommonListElementAdapterMarker
import com.hannesdorfmann.adapterdelegates4.AdapterDelegate

abstract class CommonInputDelegate :
    AdapterDelegate<MutableList<CommonListElementAdapterMarker>>() {
    var recyclerViewProvider: (() -> RecyclerView?)? = null

    protected fun isCurrentViewHolderVisible(bindingAdapterPosition: Int): Boolean {
        val recyclerView =
            recyclerViewProvider?.invoke() ?: return false
        if (bindingAdapterPosition == RecyclerView.NO_POSITION) {
            return false
        }

        val layoutManager = recyclerView.layoutManager ?: return false

        val firstVisiblePosition: Int
        val lastVisiblePosition: Int

        when (layoutManager) {
            is LinearLayoutManager -> {
                firstVisiblePosition = layoutManager.findFirstVisibleItemPosition()
                lastVisiblePosition = layoutManager.findLastVisibleItemPosition()
            }

            is GridLayoutManager -> { // Ensure you use the correct import
                firstVisiblePosition = layoutManager.findFirstVisibleItemPosition()
                lastVisiblePosition = layoutManager.findLastVisibleItemPosition()
            }

            is StaggeredGridLayoutManager -> {
                val firstVisiblePositions = IntArray(layoutManager.spanCount)
                layoutManager.findFirstVisibleItemPositions(firstVisiblePositions)
                firstVisiblePosition =
                    firstVisiblePositions.minOrNull() ?: RecyclerView.NO_POSITION

                val lastVisiblePositions = IntArray(layoutManager.spanCount)
                layoutManager.findLastVisibleItemPositions(lastVisiblePositions)
                lastVisiblePosition =
                    lastVisiblePositions.maxOrNull() ?: RecyclerView.NO_POSITION
            }

            else -> return false // Or some default assumption
        }

        return if (firstVisiblePosition == RecyclerView.NO_POSITION || lastVisiblePosition == RecyclerView.NO_POSITION) {
            false
        } else {
            bindingAdapterPosition in firstVisiblePosition..lastVisiblePosition
        }
    }
}