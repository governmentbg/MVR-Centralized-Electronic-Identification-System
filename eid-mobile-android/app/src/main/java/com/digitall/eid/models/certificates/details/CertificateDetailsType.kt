package com.digitall.eid.models.certificates.details

import android.os.Parcelable
import kotlinx.parcelize.Parcelize

@Parcelize
enum class CertificateDetailsType: Parcelable {
    DETAILS,
    DETAILS_REVOKE,
    DETAIL_RESUME,
    DETAILS_STOP
}