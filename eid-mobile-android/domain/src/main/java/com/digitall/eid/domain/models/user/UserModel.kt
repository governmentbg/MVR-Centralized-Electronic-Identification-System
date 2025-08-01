/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.models.user

import android.os.Parcelable
import kotlinx.parcelize.Parcelize

@Parcelize
data class UserModel(
    val exp: Int?,
    val iat: Int?,
    val jti: String?,
    val iss: String?,
    val sub: String?,
    val typ: String?,
    val azp: String?,
    val acr: UserAcrEnum?,
    val sid: String?,
    // LOW
    val givenName: String?,
    val givenNameCyrillic: String?,
    val middleName: String?,
    val middleNameCyrillic: String?,
    val familyName: String?,
    val familyNameCyrillic: String?,
    val email: String?,
    val preferredLanguage: String?,
    val citizenProfileId: String?,
    val preferredUsername: String?,

    // SUBSTANTIAL
    val eidEntityId: String?,
    val dateOfBirth: String?,

    // HIGH
    val citizenIdentifier: String?,
    val citizenIdentifierType: String?,

    val scope: String?,
    val userId: String?,
    val locale: String?,
    val sessionState: String?,
    val emailVerified: Boolean?,
    val allowedOrigins: List<String>?,
) : Parcelable {

    val nameCyrillic: String
        get() = (givenNameCyrillic ?: "") + middleNameCyrillic?.let { " $it" } + familyNameCyrillic?.let { " $it" }

    val name: String
        get() = (givenName ?: "") + middleName?.let { " $it" } + familyName?.let { " $it" }
}