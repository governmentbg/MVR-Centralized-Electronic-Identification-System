/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.models.certificates.all

import com.digitall.eid.R
import com.digitall.eid.domain.models.base.TypeEnum
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonListElementIdentifier
import kotlinx.parcelize.Parcelize

@Parcelize
enum class CertificateSortCriteriaEnum(
    override val type: String,
    val title: StringSource,
) : TypeEnum, CommonListElementIdentifier {
    DEFAULT("status,asc", StringSource(R.string.certificates_by_default_sorting_enum)),
    SERIAL_NUMBER_ASC("serialNumber,asc", StringSource(R.string.certificates_serial_number_asc_sorting_enum)),
    SERIAL_NUMBER_DESC("serialNumber,desc", StringSource(R.string.certificates_serial_number_desc_sorting_enum)),
    ALIAS_ASC("serialNumber,asc", StringSource(R.string.certificates_alias_asc_sorting_enum)),
    ALIAS_DESC("serialNumber,desc", StringSource(R.string.certificates_alias_desc_sorting_enum)),
    VALID_FROM_ASC("validFrom,asc", StringSource(R.string.certificates_valid_from_asc_sorting_enum)),
    VALID_FROM_DESC("validFrom,desc", StringSource(R.string.certificates_valid_from_desc_sorting_enum)),
    VALID_UNTIL_ASC("validUntil,asc", StringSource(R.string.certificates_valid_until_asc_sorting_enum)),
    VALID_UNTIL_DESC("validUntil,desc", StringSource(R.string.certificates_valid_until_desc_sorting_enum)),
    EID_ADMINISTRATOR_NAME_ASC("eidAdministratorName,asc", StringSource(R.string.certificates_administrator_asc_sorting_enum)),
    EID_ADMINISTRATOR_NAME_DESC("eidAdministratorName,desc", StringSource(R.string.certificates_administrator_desc_sorting_enum)),
    DEVICE_TYPE_ASC("deviceId,asc", StringSource(R.string.certificates_device_type_asc_sorting_enum)),
    DEVICE_TYPE_DESC("deviceId,desc", StringSource(R.string.certificates_device_type_desc_sorting_enum)),
    STATUS_ASC("status,asc", StringSource(R.string.certificates_status_asc_sorting_enum)),
    STATUS_DESC("status,desc", StringSource(R.string.certificates_status_desc_sorting_enum)),
}