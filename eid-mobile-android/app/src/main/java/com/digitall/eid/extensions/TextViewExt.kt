/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.extensions

import android.content.res.ColorStateList
import android.graphics.Rect
import android.text.SpannableString
import android.text.SpannableStringBuilder
import android.text.style.UnderlineSpan
import android.widget.TextView
import androidx.annotation.ColorRes
import androidx.annotation.DrawableRes
import androidx.annotation.StringRes
import androidx.core.content.ContextCompat
import androidx.core.graphics.drawable.DrawableCompat
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.ui.view.CenteredImageSpan

fun TextView.setTextSource(text: StringSource) {
    this.text = text.getString(context)
}

/**
 * Method set text from resource in TextView
 */
fun TextView.setTextResource(@StringRes resource: Int) {
    text = context.getString(resource)
}

/**
 * Method set text color from resource in TextView
 */
fun TextView.setTextColorResource(@ColorRes resource: Int) {
    setTextColor(context.color(resource))
}

fun TextView.setTextColorResource(colors: ColorStateList) {
    setTextColor(colors)
}

fun TextView.setCompoundDrawablesExt(
    @DrawableRes start: Int = 0,
    @DrawableRes top: Int = 0,
    @DrawableRes end: Int = 0,
    @DrawableRes bottom: Int = 0,
) {
    setCompoundDrawablesWithIntrinsicBounds(start, top, end, bottom)
}

fun TextView.clearCompoundDrawablesExt() {
    setCompoundDrawablesWithIntrinsicBounds(null, null, null, null)
}

fun TextView.setTextWithIcon(
    textMaxLines: Int = 3,
    isClickable: Boolean = false,
    textStringSource: StringSource,
    @ColorRes textColorRes: Int? = null,
    @DrawableRes iconResLeft: Int? = null,
    @DrawableRes iconResRight: Int? = null,
) {
    try {
        val textString = textStringSource.getString(context)
        text = when {
            iconResLeft != null -> {
                ContextCompat.getDrawable(context, iconResLeft)?.let { drawable ->
                    if (textColorRes != null) {
                        val wrappedDrawable = DrawableCompat.wrap(drawable)
                        val color = ContextCompat.getColor(context, textColorRes)
                        DrawableCompat.setTint(wrappedDrawable, color)
                    }
                    val iconBounds = Rect(-2, -2, 46, 46)
                    drawable.bounds = iconBounds
                    SpannableStringBuilder().apply {
                        append(" ", CenteredImageSpan(drawable), 0)
                        append(" ")
                        append(textString)
                        if (isClickable) {
                            setSpan(UnderlineSpan(), length - textString.length, length, 0)
                        }
                    }
                }
            }

            iconResRight != null -> {
                ContextCompat.getDrawable(context, iconResRight)?.let { drawable ->
                    if (textColorRes != null) {
                        val wrappedDrawable = DrawableCompat.wrap(drawable)
                        val color = ContextCompat.getColor(context, textColorRes)
                        DrawableCompat.setTint(wrappedDrawable, color)
                    }
                    val iconBounds = Rect(-2, -2, 46, 46)
                    drawable.bounds = iconBounds
                    SpannableStringBuilder().apply {
                        append(textString)
                        append(" ")
                        append(" ", CenteredImageSpan(drawable), 0)
                        if (isClickable) {
                            setSpan(UnderlineSpan(), 0, textString.length, 0)
                        }
                    }
                }
            }

            else -> {
                if (isClickable) {
                    SpannableString(textString).apply {
                        setSpan(UnderlineSpan(), 0, textString.length, 0)
                    }
                } else {
                    textString
                }
            }
        }
        textColorRes?.let {
            setTextColorResource(it)
        }
        maxLines = textMaxLines
    } catch (e: Exception) {
        logError("refreshScreen", "TextViewExtTag")
    }
}

fun TextView.setTextWithIcon(
    textMaxLines: Int = 3,
    isClickable: Boolean = false,
    textStringSource: StringSource,
    textColorsRes: ColorStateList? = null,
    @DrawableRes iconResLeft: Int? = null,
    @DrawableRes iconResRight: Int? = null,
) {
    try {
        val textString = textStringSource.getString(context)
        text = when {
            iconResLeft != null -> {
                ContextCompat.getDrawable(context, iconResLeft)?.let { drawable ->
                    val actualDrawable = if (textColorsRes != null) {
                        val wrappedDrawable = DrawableCompat.wrap(drawable)
                        val color = if (isEnabled) textColorsRes.getColorForState(
                            intArrayOf(android.R.attr.state_enabled),
                            0
                        ) else textColorsRes.getColorForState(
                            intArrayOf(-android.R.attr.state_enabled),
                            0
                        )
                        DrawableCompat.setTint(wrappedDrawable, color)
                        wrappedDrawable
                    } else drawable
                    val iconBounds = Rect(-2, -2, 46, 46)
                    actualDrawable.bounds = iconBounds
                    SpannableStringBuilder().apply {
                        append(" ", CenteredImageSpan(drawable), 0)
                        append(" ")
                        append(textString)
                        if (isClickable) {
                            setSpan(UnderlineSpan(), length - textString.length, length, 0)
                        }
                    }
                }
            }

            iconResRight != null -> {
                ContextCompat.getDrawable(context, iconResRight)?.let { drawable ->
                    val actualDrawable = if (textColorsRes != null) {
                        val wrappedDrawable = DrawableCompat.wrap(drawable)
                        val color = if (isEnabled) textColorsRes.getColorForState(
                            intArrayOf(android.R.attr.state_enabled),
                            0
                        ) else textColorsRes.getColorForState(
                            intArrayOf(-android.R.attr.state_enabled),
                            0
                        )
                        DrawableCompat.setTint(wrappedDrawable, color)
                        wrappedDrawable
                    } else drawable
                    val iconBounds = Rect(-2, -2, 46, 46)
                    actualDrawable.bounds = iconBounds
                    SpannableStringBuilder().apply {
                        append(textString)
                        append(" ")
                        append(" ", CenteredImageSpan(drawable), 0)
                        if (isClickable) {
                            setSpan(UnderlineSpan(), 0, textString.length, 0)
                        }
                    }
                }
            }

            else -> {
                if (isClickable) {
                    SpannableString(textString).apply {
                        setSpan(UnderlineSpan(), 0, textString.length, 0)
                    }
                } else {
                    textString
                }
            }
        }
        textColorsRes?.let {
            setTextColorResource(it)
        }
        maxLines = textMaxLines
    } catch (e: Exception) {
        logError("refreshScreen", "TextViewExtTag")
    }
}