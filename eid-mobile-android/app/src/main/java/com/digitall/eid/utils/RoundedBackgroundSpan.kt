package com.digitall.eid.utils

import android.graphics.Bitmap
import android.graphics.Canvas
import android.graphics.Paint
import android.graphics.RectF
import android.text.style.ReplacementSpan
import androidx.annotation.ColorInt
import kotlin.math.roundToInt

class RoundedBackgroundSpan(
    private val cornerRadius: Int,
    @param:ColorInt private val backgroundColor: Int,
    @param:ColorInt private val textColor: Int,
    private val leftBitmap: Bitmap? = null,
    private val rightBitmap: Bitmap? = null
) : ReplacementSpan() {

    override fun draw(
        canvas: Canvas,
        text: CharSequence,
        start: Int,
        end: Int,
        x: Float,
        top: Int,
        y: Int,
        bottom: Int,
        paint: Paint
    ) {
        val additionalWidth = (leftBitmap?.width ?: 0) + (rightBitmap?.width ?: 0)
        val rect = RectF(
            x,
            top.toFloat(),
            x + measureText(paint, text, start, end) + additionalWidth,
            bottom.toFloat()
        )
        paint.color = backgroundColor
        canvas.drawRoundRect(rect, cornerRadius.toFloat(), cornerRadius.toFloat(), paint)
        paint.color = textColor
        leftBitmap?.let {
            canvas.drawBitmap(it, 8f, ((rect.bottom - rect.top) - it.height.toFloat()) / 2, paint)
        }
        canvas.drawText(text, start, end, x + (leftBitmap?.width ?: 0), y.toFloat(), paint)
        rightBitmap?.let {
            canvas.drawBitmap(it, rect.right - it.width + 8f, ((rect.bottom - rect.top) - it.height.toFloat()) / 2, paint)
        }
    }

    override fun getSize(
        paint: Paint,
        text: CharSequence,
        start: Int,
        end: Int,
        fm: Paint.FontMetricsInt?
    ) = paint.measureText(text, start, end).roundToInt()

    private fun measureText(paint: Paint, text: CharSequence, start: Int, end: Int) =
        paint.measureText(text, start, end)
}