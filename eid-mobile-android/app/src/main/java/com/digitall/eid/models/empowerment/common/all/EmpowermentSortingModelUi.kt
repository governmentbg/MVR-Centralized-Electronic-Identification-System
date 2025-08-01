/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.models.empowerment.common.all

import android.os.Parcelable
import com.digitall.eid.domain.models.empowerment.common.all.EmpowermentSortingByEnum
import com.digitall.eid.domain.models.empowerment.common.all.EmpowermentSortingDirectionEnum
import com.digitall.eid.models.list.CommonListElementIdentifier
import kotlinx.parcelize.Parcelize

@Parcelize
data class EmpowermentSortingModelUi(
    var sortBy: EmpowermentSortingByEnum,
    var sortDirection: EmpowermentSortingDirectionEnum,
) : CommonListElementIdentifier, Parcelable