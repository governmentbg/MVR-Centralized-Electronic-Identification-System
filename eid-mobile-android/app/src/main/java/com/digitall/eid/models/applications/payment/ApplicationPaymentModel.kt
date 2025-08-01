package com.digitall.eid.models.applications.payment

import android.os.Parcelable
import kotlinx.parcelize.Parcelize

@Parcelize
data class ApplicationPaymentModel(
    val fee: List<Double?>,
    val carrierPrice: Int,
    val currency: List<String?>,
    val paymentAccessCode: String?,
) : Parcelable