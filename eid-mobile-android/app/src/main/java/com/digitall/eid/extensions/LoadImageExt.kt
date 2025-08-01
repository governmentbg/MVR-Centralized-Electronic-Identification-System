/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.extensions

import androidx.annotation.DrawableRes
import androidx.appcompat.widget.AppCompatImageView
import com.bumptech.glide.Glide
import com.bumptech.glide.load.engine.DiskCacheStrategy
import com.bumptech.glide.load.resource.bitmap.CenterCrop
import com.bumptech.glide.load.resource.bitmap.GranularRoundedCorners
import com.digitall.eid.R
import java.io.File

fun AppCompatImageView.loadImage(
    imageUrl: String?,
    @DrawableRes placeholder: Int = R.drawable.bg_placeholder_rounded_icon,
    @DrawableRes error: Int = R.drawable.bg_placeholder_rounded_icon,
) {
    if (!isInEditMode) {
        Glide.with(this)
            .load(imageUrl)
            .placeholder(context.drawable(placeholder))
            .error(context.drawable(error))
            .into(this)
    }
}

fun AppCompatImageView.loadImageWithoutPlaceholder(imageUrl: String?) {
    if (!isInEditMode) {
        Glide.with(this)
            .load(imageUrl)
            .into(this)
    }
}


// Circle image

fun AppCompatImageView.loadCircleImage(imageRes: Int) {
    if (!isInEditMode) {
        Glide.with(this)
            .load(imageRes)
            .circleCrop()
            .into(this)
    }
}

fun AppCompatImageView.loadCircleImage(
    imageUrl: String?,
    @DrawableRes placeholder: Int = R.drawable.bg_placeholder_oval_icon,
    @DrawableRes error: Int = R.drawable.bg_placeholder_oval_icon,
) {
    if (!isInEditMode) {
        Glide.with(this)
            .load(imageUrl)
            .placeholder(context.drawable(placeholder))
            .error(context.drawable(error))
            .circleCrop()
            .into(this)
    }
}

fun AppCompatImageView.loadCircleImageWithoutPlaceHolder(imageUrl: String?) {
    if (!isInEditMode) {
        Glide.with(this)
            .load(imageUrl)
            .circleCrop()
            .into(this)
    }
}

// Top round corners image

fun AppCompatImageView.loadImageWithTopRoundCorners(
    imageUrl: String,
    roundSizeResource: Int = R.dimen.default_semi_corners_radius,
    @DrawableRes placeholder: Int = R.drawable.bg_placeholder_oval_icon,
    @DrawableRes error: Int = R.drawable.bg_placeholder_oval_icon,
) {
    val cornerRoundSize = this.context.pxDimen(roundSizeResource).toFloat()
    loadImageWithRoundCorners(
        imageUrl = imageUrl,
        granularRoundedCorners = GranularRoundedCorners(
            cornerRoundSize,
            cornerRoundSize,
            0f,
            0f,
        ),
        placeholder = placeholder,
        error = error
    )
}

fun AppCompatImageView.loadImageWithTopRoundCorners(
    imageRes: Int,
    roundSizeResource: Int = R.dimen.default_semi_corners_radius,
) {
    val cornerRoundSize = this.context.pxDimen(roundSizeResource).toFloat()
    loadImageWithRoundCorners(
        imageRes = imageRes,
        granularRoundedCorners = GranularRoundedCorners(
            cornerRoundSize,
            cornerRoundSize,
            0f,
            0f,
        ),
    )
}

// Round corners image

fun AppCompatImageView.loadImageWithRoundCorners(
    imageUrl: String,
    roundSizeResource: Int = R.dimen.default_semi_corners_radius,
    @DrawableRes placeholder: Int = R.drawable.bg_placeholder_rounded_icon,
    @DrawableRes error: Int = R.drawable.bg_placeholder_rounded_icon,
) {
    val cornerRoundSize = this.context.pxDimen(roundSizeResource).toFloat()
    loadImageWithRoundCorners(
        imageUrl = imageUrl,
        granularRoundedCorners = GranularRoundedCorners(
            cornerRoundSize,
            cornerRoundSize,
            cornerRoundSize,
            cornerRoundSize
        ),
        placeholder = placeholder,
        error = error
    )
}

private fun AppCompatImageView.loadImageWithRoundCorners(
    imageUrl: String,
    granularRoundedCorners: GranularRoundedCorners,
    @DrawableRes placeholder: Int,
    @DrawableRes error: Int,
) {
    if (!isInEditMode) {
        Glide.with(this)
            .load(imageUrl)
            .placeholder(context.drawable(placeholder))
            .error(context.drawable(error))
            .transform(CenterCrop(), granularRoundedCorners)
            .into(this)
    }
}

fun AppCompatImageView.loadImageWithRoundCorners(
    imageRes: Int,
    roundSizeResource: Int = R.dimen.default_semi_corners_radius,
) {
    val cornerRoundSize = this.context.pxDimen(roundSizeResource).toFloat()
    loadImageWithRoundCorners(
        imageRes = imageRes,
        granularRoundedCorners = GranularRoundedCorners(
            cornerRoundSize,
            cornerRoundSize,
            cornerRoundSize,
            cornerRoundSize
        ),
    )
}

private fun AppCompatImageView.loadImageWithRoundCorners(
    imageRes: Int,
    granularRoundedCorners: GranularRoundedCorners,
) {
    if (!isInEditMode) {
        Glide.with(this)
            .load(imageRes)
            .transform(CenterCrop(), granularRoundedCorners)
            .into(this)
    }
}

private fun AppCompatImageView.loadImageWithRoundCornersWithoutCache(
    imageFile: File,
    granularRoundedCorners: GranularRoundedCorners,
    @DrawableRes placeholder: Int,
    @DrawableRes error: Int
) {
    if (!isInEditMode) {
        Glide.with(this)
            .load(imageFile)
            .placeholder(context.drawable(placeholder))
            .error(context.drawable(error))
            .transform(granularRoundedCorners)
            .diskCacheStrategy(DiskCacheStrategy.NONE)
            .skipMemoryCache(true)
            .into(this)
    }
}

