package com.digitall.eid.models.common

import android.os.Parcelable
import kotlinx.parcelize.Parcelize

@Parcelize
data class CardScanBottomSheetContent(
    val type: CardScanContentType,
) : Parcelable
