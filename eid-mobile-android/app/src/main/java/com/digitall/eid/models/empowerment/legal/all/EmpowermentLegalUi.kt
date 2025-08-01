package com.digitall.eid.models.empowerment.legal.all

import android.os.Parcelable
import com.digitall.eid.domain.models.empowerment.common.all.EmpowermentItem
import com.digitall.eid.extensions.equalTo
import com.digitall.eid.models.empowerment.common.all.EmpowermentAdapterMarker
import com.digitall.eid.models.empowerment.common.filter.EmpowermentFilterStatusEnumUi
import com.digitall.eid.models.list.CommonListElementIdentifier
import com.digitall.eid.models.list.CommonSpinnerUi
import kotlinx.parcelize.Parcelize

@Parcelize
data class EmpowermentLegalUi(
    override val elementId: Int? = null,
    override val elementEnum: CommonListElementIdentifier? = null,
    val id: String,
    val name: String,
    val empowered: String,
    val serviceName: String,
    val providerName: String,
    val spinnerModel: CommonSpinnerUi,
    val originalModel: EmpowermentItem,
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
            { name },
            { status },
            { empowered },
            { elementId },
            { elementEnum },
            { serviceName },
            { spinnerModel },
            { providerName },
        )
    }
}