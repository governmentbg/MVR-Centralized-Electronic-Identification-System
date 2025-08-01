/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.models.certificates.all

import android.os.Parcelable
import com.digitall.eid.extensions.equalTo
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonSpinnerUi
import kotlinx.parcelize.Parcelize

@Parcelize
data class CertificateUi(
    val id: String,
    val serialNumber: String,
    val alias: String,
    val validityFrom: String,
    val validityUntil: String,
    val status: CertificatesStatusEnum,
    val deviceType: StringSource,
    val isExpiring: Boolean,
    val spinnerModel: CommonSpinnerUi?
) : CertificateAdapterMarker, Parcelable {

    override fun isItemSame(other: Any?): Boolean {
        return equalTo(other)
    }

    override fun isContentSame(other: Any?): Boolean {
        return equalTo(
            other,
            { id },
            { status },
            { alias },
            { deviceType },
            { serialNumber },
            { validityFrom },
            { validityUntil },
            { isExpiring },
            { spinnerModel }
        )
    }
}