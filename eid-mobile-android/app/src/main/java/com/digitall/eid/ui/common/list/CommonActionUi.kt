package com.digitall.eid.ui.common.list

import android.os.Parcelable
import androidx.annotation.ColorRes
import androidx.annotation.DrawableRes
import kotlinx.parcelize.Parcelize

@Parcelize
data class CommonActionUi(
    @param:DrawableRes val icon: Int,
    @param:ColorRes val color: Int,
): Parcelable
