/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.models.applications.create

import android.os.Parcelable
import kotlinx.parcelize.Parcelize

@Parcelize
data class ApplicationUserDetailsModel(
    val email: String?,
    val active: Boolean?,
    val lastName: String?,
    val firstName: String?,
    val secondName: String?,
    val lastNameLatin: String?,
    val firstNameLatin: String?,
    val secondNameLatin: String?,
    val eidentityId: String?,
    val phoneNumber: String?,
    val citizenProfileId: String?,
    val citizenIdentifierType: String?,
    val citizenIdentifierNumber: String?,
    val twoFaEnabled: Boolean?,
) : Parcelable