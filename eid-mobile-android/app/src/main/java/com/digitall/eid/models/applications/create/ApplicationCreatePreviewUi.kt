/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.models.applications.create

import android.os.Parcelable
import com.digitall.eid.domain.models.applications.create.ApplicationUserDetailsModel
import kotlinx.parcelize.Parcelize

@Parcelize
data class ApplicationCreatePreviewUi(
    val userModel: ApplicationUserDetailsModel,
    val signMethod: ApplicationCreateIntroSigningMethodsEnumUi?,
    val previewList: List<ApplicationCreatePreviewAdapterMarker>,
) : Parcelable