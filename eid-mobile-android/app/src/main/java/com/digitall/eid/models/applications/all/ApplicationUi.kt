/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.models.applications.all

import android.os.Parcelable
import com.digitall.eid.domain.models.applications.all.ApplicationItem
import com.digitall.eid.extensions.equalTo
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonSpinnerUi
import kotlinx.parcelize.Parcelize

@Parcelize
data class ApplicationUi(
    val id: String,
    val date: String,
    val applicationNumber: String,
    val administrator: String,
    val type: ApplicationTypeEnum,
    val status: ApplicationStatusEnum,
    val originalModel: ApplicationItem,
    val deviceType: StringSource,
    val spinnerModel: CommonSpinnerUi?,
) : ApplicationAdapterMarker, Parcelable {

    override fun isItemSame(other: Any?): Boolean {
        return equalTo(other)
    }

    override fun isContentSame(other: Any?): Boolean {
        return equalTo(
            other,
            { id },
            { type },
            { date },
            { status },
            { applicationNumber },
            { deviceType },
            { spinnerModel },
            { administrator },
        )
    }

}