package com.digitall.eid.models.certificates.all

import android.os.Parcelable
import androidx.annotation.ColorRes
import com.digitall.eid.R
import com.digitall.eid.domain.models.base.TypeEnum
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonListElementIdentifier
import kotlinx.parcelize.Parcelize

@Parcelize
enum class CertificatesElementsEnumUi(
    override val type: String,
) : CommonListElementIdentifier, TypeEnum, Parcelable {
    SPINNER_MENU("SPINNER_MENU"),
    SPINNER_SORTING_CRITERIA("SPINNER_SORTING_CRITERIA"),
}

@Parcelize
enum class CertificatesSpinnerElementsEnumUi(
    override val type: String,
    val title: StringSource,
    @param:ColorRes val colorRes: Int,
) : CommonListElementIdentifier, TypeEnum, Parcelable {
    SPINNER_PAUSE(
        "SPINNER_PAUSE",
        StringSource(R.string.certificate_stop),
        R.color.color_F59E0B,
    ),
    SPINNER_RESUME(
        "SPINNER_RESUME",
        StringSource(R.string.certificate_resume),
        R.color.color_018930,
    ),
    SPINNER_CHANGE_PIN(
        "SPINNER_CHANGE_PIN",
        StringSource(R.string.certificate_change_pin),
        R.color.color_1C3050
    ),
    SPINNER_REVOKE(
        "SPINNER_REVOKE",
        StringSource(R.string.certificate_revoke),
        R.color.color_BF1212
    )
}