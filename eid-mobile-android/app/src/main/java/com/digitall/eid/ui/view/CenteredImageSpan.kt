package com.digitall.eid.ui.view

import android.graphics.Canvas
import android.graphics.Paint
import android.graphics.drawable.Drawable
import android.text.style.ImageSpan
import androidx.core.graphics.withSave

class CenteredImageSpan(drawable: Drawable): ImageSpan(drawable) {

    override fun getSize(
        paint: Paint,
        text: CharSequence,
        start: Int,
        end: Int,
        fontMetricsInt: Paint.FontMetricsInt?,
    ): Int {
        val drawable = drawable
        val rect = drawable.bounds
        fontMetricsInt?.let { fm ->
            val fmPaint = paint.fontMetricsInt
            val fontHeight = fmPaint.descent - fmPaint.ascent
            val drHeight = rect.bottom - rect.top
            val centerY = fmPaint.ascent + fontHeight / 2

            fm.apply {
                ascent = centerY - drHeight / 2
                top = fontMetricsInt.ascent
                bottom = centerY + drHeight / 2
                descent = fontMetricsInt.bottom
            }
        }
        return rect.right
    }

    /**
     * see detail message in android.text.TextLine
     *
     * @param canvas the canvas, can be null if not rendering
     * @param text the text to be draw
     * @param start the text start position
     * @param end the text end position
     * @param x the edge of the replacement closest to the leading margin
     * @param top the top of the line
     * @param y the baseline
     * @param bottom the bottom of the line
     * @param paint the work paint
     */
    override fun draw(
        canvas: Canvas,
        text: CharSequence,
        start: Int,
        end: Int,
        x: Float,
        top: Int,
        y: Int,
        bottom: Int,
        paint: Paint,
    ) {
        val drawable = drawable
        canvas.withSave {
            val fmPaint = paint.fontMetricsInt
            val fontHeight = fmPaint.descent - fmPaint.ascent
            val centerY = y + fmPaint.descent - fontHeight / 2
            val transY = centerY - (drawable.bounds.bottom - drawable.bounds.top) / 2
            translate(x, transY.toFloat())
            drawable.draw(this)
        }
    }
}