/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.models.empowerment.create.create

import android.os.Parcelable
import com.digitall.eid.models.empowerment.create.preview.EmpowermentCreatePreviewAdapterMarker
import kotlinx.parcelize.Parcelize

@Parcelize
data class EmpowermentCreatePreviewUiModel(
    val previewList: List<EmpowermentCreatePreviewAdapterMarker>,
    val fromNameOf: EmpowermentCreateFromNameOfEnumUi,
) : Parcelable