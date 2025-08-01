/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.extensions

import android.view.inputmethod.EditorInfo
import androidx.appcompat.widget.AppCompatEditText
import androidx.core.widget.addTextChangedListener

inline fun AppCompatEditText.setTextChangeListener(
    crossinline text: (String) -> Unit
) {
    addTextChangedListener { editable ->
        text(editable?.toString() ?: "")
    }
}

inline fun AppCompatEditText.setFocusChangeListener(
    crossinline hasFocus: (Boolean) -> Unit
) {
    setOnFocusChangeListener { _, focus ->
        hasFocus(focus)
    }
}

fun AppCompatEditText.moveCursorToEnd() {
    text?.let {
        setSelection(it.length)
    }
}

inline fun AppCompatEditText.onDonePressed(
    hideKeyboard: Boolean = true,
    crossinline onDone: () -> Unit,
) {
    setOnEditorActionListener { view, actionId, _ ->
        when (actionId) {
            EditorInfo.IME_ACTION_NEXT,
            EditorInfo.IME_ACTION_DONE -> {
                if (hideKeyboard) {
                    view.hideKeyboard()
                }
                onDone()
                true
            }

            else -> false
        }
    }
}