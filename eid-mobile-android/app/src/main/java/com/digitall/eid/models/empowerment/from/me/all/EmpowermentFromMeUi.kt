/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.models.empowerment.from.me.all

import android.os.Parcelable
import com.digitall.eid.domain.models.empowerment.common.all.EmpowermentItem
import com.digitall.eid.extensions.equalTo
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.empowerment.common.all.EmpowermentAdapterMarker
import com.digitall.eid.models.empowerment.common.filter.EmpowermentFilterStatusEnumUi
import com.digitall.eid.models.list.CommonListElementIdentifier
import com.digitall.eid.models.list.CommonSpinnerUi
import kotlinx.parcelize.Parcelize

@Parcelize
data class EmpowermentFromMeUi(
    override val elementId: Int? = null,
    override val elementEnum: CommonListElementIdentifier? = null,
    val id: String,
    val name: String,
    val number: String,
    val empowered: StringSource,
    val additionalEmpoweredPeople: Int,
    val serviceName: String,
    val providerName: String,
    val createdOn: String,
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
            { createdOn },
            { additionalEmpoweredPeople }
        )
    }
}