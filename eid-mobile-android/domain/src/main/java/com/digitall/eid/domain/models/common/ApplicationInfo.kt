/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.models.common

import android.os.Parcelable
import com.digitall.eid.domain.models.user.UserModel
import kotlinx.parcelize.Parcelize

@Parcelize
data class ApplicationInfo(
    val email: String,
    val errorCount: Int,
    val password: String,
    val databaseKey: String,
    val accessToken: String,
    val userModel: UserModel,
    val refreshToken: String,
    val errorTimeCode: Long?,
    val serverPublicKey: Long?,
    val applicationPin: String?,
    val certificatePin: String?,
    val errorStatus: ErrorStatus,
    val biometricStatus: BiometricStatus,
    val mobileApplicationInstanceId: String?,
    val applicationStatus: ApplicationStatus,
    val applicationLanguage: ApplicationLanguage,
    val applicationThemeType: ApplicationThemeType,
) : Parcelable