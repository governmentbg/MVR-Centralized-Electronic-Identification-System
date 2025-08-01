/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.models.applications.all

import com.digitall.eid.R
import com.digitall.eid.domain.models.base.TypeEnum
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonListElementIdentifier
import kotlinx.parcelize.Parcelize

@Parcelize
enum class ApplicationsSortCriteriaEnum(
    override val type: String,
    val title: StringSource,
) : TypeEnum, CommonListElementIdentifier {
    DEFAULT("createDate,desc", StringSource(R.string.applications_by_default_sorting_enum)),
    CREATE_DATE_ASC("createDate,asc", StringSource(R.string.applications_creation_date_asc_sorting_enum)),
    CREATE_DATE_DESC("createDate,desc", StringSource(R.string.applications_creation_date_desc_sorting_enum)),
    APPLICATION_TYPE_ASC("applicationType,asc", StringSource(R.string.applications_application_type_asc_sorting_enum)),
    APPLICATION_TYPE_DESC("applicationType,desc", StringSource(R.string.applications_application_type_desc_sorting_enum)),
    EID_ADMINISTRATOR_NAME_ASC("eidAdministratorName,asc", StringSource(R.string.applications_administrator_asc_sorting_enum)),
    EID_ADMINISTRATOR_NAME_DESC("eidAdministratorName,desc", StringSource(R.string.applications_administrator_desc_sorting_enum)),
    DEVICE_TYPE_ASC("deviceId,asc", StringSource(R.string.applications_device_type_asc_sorting_enum)),
    DEVICE_TYPE_DESC("deviceId,desc", StringSource(R.string.applications_device_type_desc_sorting_enum)),
    STATUS_ASC("status,asc", StringSource(R.string.applications_status_asc_sorting_enum)),
    STATUS_DESC("status,desc", StringSource(R.string.applications_status_desc_sorting_enum)),
}