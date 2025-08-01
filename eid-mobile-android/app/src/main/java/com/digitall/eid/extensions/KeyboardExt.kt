/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.extensions

import android.app.Activity
import android.content.Context
import android.view.View
import android.view.Window
import android.view.inputmethod.InputMethodManager
import androidx.appcompat.widget.AppCompatEditText
import androidx.core.view.WindowCompat
import androidx.core.view.WindowInsetsCompat
import androidx.fragment.app.Fragment

// Show/Hide keyboard methods for different sources.

fun AppCompatEditText.showKeyboard() {
    if (requestFocus()) {
        val inputMethodManager = context.getSystemService(Context.INPUT_METHOD_SERVICE) as InputMethodManager
        post {
            inputMethodManager.showSoftInput(this, InputMethodManager.SHOW_IMPLICIT)
        }
    }
    isCursorVisible = true
}

fun AppCompatEditText.hideKeyboard() {
    val inputMethodManager = context.getSystemService(Context.INPUT_METHOD_SERVICE) as InputMethodManager
    if (windowToken != null) {
        inputMethodManager.hideSoftInputFromWindow(windowToken, 0)
    }
    clearFocus()
    isCursorVisible = false
}

fun AppCompatEditText.showKeyboard(window: Window) {
    requestFocus()
    WindowCompat.getInsetsController(window, window.decorView).show(WindowInsetsCompat.Type.ime())
}

fun Activity.hideKeyboard() {
    WindowCompat.getInsetsController(window, window.decorView).hide(WindowInsetsCompat.Type.ime())
}

fun Fragment.hideKeyboard() {
    activity?.hideKeyboard()
}

fun View.hideKeyboard() {
    val inputMethodManager = context.getSystemService(Context.INPUT_METHOD_SERVICE) as InputMethodManager
    if (windowToken != null) {
        inputMethodManager.hideSoftInputFromWindow(windowToken, 0)
    }
    clearFocus()
}

fun View.hideKeyboard(window: Window) {
    WindowCompat.getInsetsController(window, this).hide(WindowInsetsCompat.Type.ime())
}