package com.digitall.eid.models.common

import android.os.Parcelable
import kotlinx.parcelize.Parcelize

@Parcelize
sealed class CardScanContentType : Parcelable {
    data class ChangePin(
        val cardCurrentPin: String?,
        val cardNewPin: String?,
        val cardCan: String? = null
    ) : CardScanContentType()

    data class SignChallenge(
        val cardCurrentPin: String?,
        val challenge: String?,
        val cardCan: String? = null
    ) : CardScanContentType()
}