/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.data.models.network.user

import com.google.gson.annotations.SerializedName

data class UserJsonData(
    @SerializedName("exp") val exp: Int?,
    @SerializedName("iat") val iat: Int?,
    @SerializedName("jti") val jti: String?,
    @SerializedName("iss") val iss: String?,
    @SerializedName("sub") val sub: String?,
    @SerializedName("typ") val typ: String?,
    @SerializedName("azp") val azp: String?,
    @SerializedName("acr") val acr: String?,
    @SerializedName("sid") val sid: String?,
    // LOW
    @SerializedName("given_name") val givenName: String?,
    @SerializedName("given_name_cyrillic") val givenNameCyrillic: String?,
    @SerializedName("middle_name") val middleName: String?,
    @SerializedName("middle_name_cyrillic") val middleNameCyrillic: String?,
    @SerializedName("family_name") val familyName: String?,
    @SerializedName("family_name_cyrillic") val familyNameCyrillic: String?,
    @SerializedName("email") val email: String?,
    @SerializedName("preferred_language") val preferredLanguage: String?,
    @SerializedName("citizen_profile_id") val citizenProfileId: String?,
    @SerializedName("preferred_username") val preferredUsername: String?,

    // SUBSTANTIAL
    @SerializedName("eidenity_id") val eidEntityId: String?,
    @SerializedName("date_of_birth") val dateOfBirth: String?,

    // HIGH
    @SerializedName("citizen_identifier") val citizenIdentifier: String?,
    @SerializedName("citizen_identifier_type") val citizenIdentifierType: String?,

    @SerializedName("scope") val scope: String?,
    @SerializedName("userid") val userId: String?,
    @SerializedName("locale") val locale: String?,
    @SerializedName("session_state") val sessionState: String?,
    @SerializedName("email_verified") val emailVerified: Boolean?,
    @SerializedName("allowed-origins") val allowedOrigins: List<String>?,
)