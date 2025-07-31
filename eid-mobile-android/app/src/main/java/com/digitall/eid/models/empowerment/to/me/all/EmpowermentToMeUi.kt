/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.models.empowerment.to.me.all

import android.os.Parcelable
import com.digitall.eid.domain.models.empowerment.common.all.EmpowermentItem
import com.digitall.eid.extensions.equalTo
import com.digitall.eid.models.empowerment.common.all.EmpowermentAdapterMarker
import com.digitall.eid.models.empowerment.common.filter.EmpowermentFilterStatusEnumUi
import com.digitall.eid.models.list.CommonListElementIdentifier
import com.digitall.eid.models.list.CommonSpinnerUi
import kotlinx.parcelize.Parcelize

@Parcelize
data class EmpowermentToMeUi(
    override val elementId: Int? = null,
    override val elementEnum: CommonListElementIdentifier? = null,
    val id: String,
    val number: String,
    val empower: String,
    val serviceName: String,
    val providerName: String,
    val spinnerModel: CommonSpinnerUi?,
    val originalModel: EmpowermentItem,
    val createdOn: String,
    val status: EmpowermentFilterStatusEnumUi,
) : EmpowermentAdapterMarker, Parcelable {

    override fun isItemSame(other: Any?): Boolean {
        return equalTo(
            other,
            { id },
        )
    }

    override fun isContentSame(other: Any?): Boolean {
        return equalTo(
            other,
            { id },
            { number },
            { status },
            { empower },
            { elementId },
            { createdOn },
            { elementEnum },
            { serviceName },
            { spinnerModel },
            { providerName },
        )
    }

}