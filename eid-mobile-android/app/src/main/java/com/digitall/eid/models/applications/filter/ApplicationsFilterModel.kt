/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.models.applications.filter

import android.os.Parcelable
import com.digitall.eid.domain.models.administrators.AdministratorModel
import com.digitall.eid.models.applications.all.ApplicationStatusEnum
import com.digitall.eid.models.applications.all.ApplicationTypeEnum
import kotlinx.parcelize.Parcelize

@Parcelize
data class ApplicationsFilterModel(
    val id: String?,
    val applicationNumber: String?,
    val createDate: Long?,
    val status: ApplicationStatusEnum?,
    val deviceType: ApplicationDeviceType?,
    val administrator: AdministratorModel?,
    val applicationType: ApplicationTypeEnum?,
) : Parcelable {

    val allPropertiesAreNull: Boolean
        get() {
            val primitiveMemberProps = listOf(
                id,
                applicationNumber,
                createDate,
                status,
                deviceType,
                administrator,
                applicationType,
            )
            val arePrimitivesNotInit = primitiveMemberProps.all { member -> member == null }
            return arePrimitivesNotInit
        }
}