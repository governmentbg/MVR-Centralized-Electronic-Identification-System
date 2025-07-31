/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.models.applications.create

import android.os.Parcelable
import kotlinx.parcelize.Parcelize

@Parcelize
data class ApplicationCreatePinModel(
    val certificate: String,
    val applicationId: String,
    val certificateChain: List<String>,
) : Parcelable