/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.mappers.certificates.details

import com.digitall.eid.R
import com.digitall.eid.data.mappers.base.BaseMapperWithData
import com.digitall.eid.domain.DEVICES
import com.digitall.eid.domain.UiDateFormats
import com.digitall.eid.domain.extensions.fromServerDateToTextDate
import com.digitall.eid.domain.extensions.getEnumValue
import com.digitall.eid.domain.models.certificates.CertificateFullDetailsModel
import com.digitall.eid.models.certificates.all.CertificatesStatusEnum
import com.digitall.eid.models.certificates.details.CertificateDetailsAdapterMarker
import com.digitall.eid.models.certificates.details.CertificateDetailsElementsEnumUi
import com.digitall.eid.models.certificates.details.CertificateDetailsType
import com.digitall.eid.models.common.ButtonColorUi
import com.digitall.eid.models.common.LevelOfAssuranceEnumUi
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonActionUi
import com.digitall.eid.models.list.CommonButtonUi
import com.digitall.eid.models.list.CommonEmptySpaceSizeEnum
import com.digitall.eid.models.list.CommonEmptySpaceUi
import com.digitall.eid.models.list.CommonSeparatorInFieldUi
import com.digitall.eid.models.list.CommonSeparatorUi
import com.digitall.eid.models.list.CommonSimpleTextExpiringUi
import com.digitall.eid.models.list.CommonSimpleTextInFieldUi
import com.digitall.eid.models.list.CommonSimpleTextUi
import com.digitall.eid.models.list.CommonTitleSmallInFieldUi
import com.digitall.eid.models.list.CommonTitleUi

class CertificateDetailsUiMapper :
    BaseMapperWithData<CertificateFullDetailsModel, CertificateDetailsType, List<CertificateDetailsAdapterMarker>>() {

    override fun map(
        from: CertificateFullDetailsModel,
        data: CertificateDetailsType?
    ): List<CertificateDetailsAdapterMarker> {
        return with(from) {
            buildList {
                add(
                    CommonTitleUi(
                        title = StringSource(R.string.certificate_details_title),
                    )
                )
                add(CommonSeparatorUi())
                val status = getEnumValue<CertificatesStatusEnum>(information?.status ?: "")
                    ?: CertificatesStatusEnum.UNKNOWN
                add(
                    CommonSimpleTextUi(
                        title = StringSource(R.string.certificate_details_serial_number),
                        text = StringSource(information?.serialNumber),
                    )
                )
                add(
                    CommonSimpleTextUi(
                        title = StringSource(R.string.certificate_details_alias),
                        text = StringSource(information?.alias),
                        action = CommonActionUi(
                            icon = R.drawable.ic_edit,
                            color = R.color.color_1C3050
                        )
                    )
                )
                add(
                    CommonSimpleTextUi(
                        title = StringSource(R.string.certificate_details_eid_idenity_id),
                        text = StringSource(information?.eidentityId),
                    )
                )
                add(
                    CommonSimpleTextUi(
                        title = StringSource(R.string.certificate_details_certificate_common_name),
                        text = StringSource(information?.commonName),
                    )
                )
                add(
                    CommonSimpleTextUi(
                        title = StringSource(R.string.certificate_details_certificate_status),
                        text = status.title,
                        colorRes = status.colorRes,
                        iconResLeft = status.iconRes,
                    )
                )
                when {
                    status == CertificatesStatusEnum.ACTIVE && information?.reasonText.isNullOrEmpty()
                        .not() -> {
                        add(
                            CommonSimpleTextUi(
                                title = StringSource(R.string.certificate_details_certificate_resume_reason),
                                text = StringSource(information?.reasonText),
                            )
                        )
                    }

                    status == CertificatesStatusEnum.REVOKED && information?.reasonText.isNullOrEmpty()
                        .not() -> {
                        add(
                            CommonSimpleTextUi(
                                title = StringSource(R.string.certificate_details_certificate_revocation_reason),
                                text = StringSource(information?.reasonText),
                            )
                        )
                    }

                    status == CertificatesStatusEnum.STOPPED && information?.reasonText.isNullOrEmpty()
                        .not() -> {
                        add(
                            CommonSimpleTextUi(
                                title = StringSource(R.string.certificate_details_certificate_stop_reason),
                                text = StringSource(information?.reasonText),
                            )
                        )
                    }
                }
                add(
                    CommonSimpleTextUi(
                        title = StringSource(R.string.certificate_details_certificate_issued_from),
                        text = StringSource(information?.eidAdministratorName),
                    )
                )
                val deviceType =
                    StringSource(DEVICES.firstOrNull { device -> device.id == information?.deviceId }?.name)
                add(
                    CommonSimpleTextUi(
                        title = StringSource(R.string.certificate_details_certificate_carrier),
                        text = deviceType,
                    )
                )
                val levelOfAssurance =
                    getEnumValue<LevelOfAssuranceEnumUi>(information?.levelOfAssurance ?: "")
                        ?: LevelOfAssuranceEnumUi.LOW
                add(
                    CommonSimpleTextUi(
                        title = StringSource(R.string.certificate_details_certificate_carrier_level_of_assurance),
                        text = levelOfAssurance.title,
                    )
                )
                val validityFrom = information?.validityFrom?.fromServerDateToTextDate(
                    dateFormat = UiDateFormats.WITH_TIME,
                )
                add(
                    CommonSimpleTextUi(
                        title = StringSource(R.string.certificate_details_certificate_valid_from),
                        text = StringSource(validityFrom),
                    )
                )
                val validityUntil = information?.validityUntil?.fromServerDateToTextDate(
                    dateFormat = UiDateFormats.WITH_TIME,
                )
                add(
                    CommonSimpleTextExpiringUi(
                        title = StringSource(R.string.certificate_details_certificate_valid_until),
                        text = StringSource(validityUntil),
                        isExpiring = information?.expiring ?: false
                    )
                )
                add(CommonSeparatorUi())
                add(
                    CommonEmptySpaceUi(
                        size = CommonEmptySpaceSizeEnum.SIZE_4,
                    )
                )

                if (history.isNullOrEmpty().not() && data == CertificateDetailsType.DETAILS) {
                    add(
                        CommonTitleSmallInFieldUi(
                            title = StringSource(R.string.certificate_details_full_history_title)
                        )
                    )
                    history?.forEach { element ->
                        add(CommonSeparatorInFieldUi(marginLeft = 32, marginRight = 32))
                        val elementStatus =
                            getEnumValue<CertificatesStatusEnum>(element.status ?: "")
                                ?: CertificatesStatusEnum.UNKNOWN
                        add(
                            CommonSimpleTextInFieldUi(
                                title = StringSource(R.string.certificate_details_full_history_element_status_title),
                                text = elementStatus.title,
                                textColorRes = elementStatus.colorRes,
                                iconResLeft = elementStatus.iconRes,
                            )
                        )

                        val (applicationCreateDateTitle, applicationNumberTitle, applicationModifiedDateTitle) = when (elementStatus) {
                            CertificatesStatusEnum.CREATED -> Triple(
                                StringSource(R.string.certificate_details_full_history_element_created_application_create_date_title),
                                StringSource(R.string.certificate_details_full_history_element_created_application_number_title),
                                StringSource(
                                    R.string.certificate_details_full_history_element_created_application_modified_date_title
                                )
                            )

                            CertificatesStatusEnum.REVOKED -> Triple(
                                StringSource(R.string.certificate_details_full_history_element_revoked_application_create_date_title),
                                StringSource(R.string.certificate_details_full_history_element_revoked_application_number_title),
                                StringSource(
                                    R.string.certificate_details_full_history_element_revoked_application_modified_date_title
                                )
                            )

                            CertificatesStatusEnum.STOPPED -> Triple(
                                StringSource(R.string.certificate_details_full_history_element_stopped_application_create_date_title),
                                StringSource(R.string.certificate_details_full_history_element_stopped_application_number_title),
                                StringSource(
                                    R.string.certificate_details_full_history_element_stopped_application_modified_date_title
                                )
                            )

                            CertificatesStatusEnum.EXPIRED -> Triple(
                                StringSource(R.string.certificate_details_full_history_element_expired_application_create_date_title),
                                StringSource(R.string.certificate_details_full_history_element_expired_application_number_title),
                                StringSource(
                                    R.string.certificate_details_full_history_element_expired_application_modified_date_title
                                )
                            )

                            CertificatesStatusEnum.ACTIVE -> Triple(
                                StringSource(R.string.certificate_details_full_history_element_active_application_create_date_title),
                                StringSource(R.string.certificate_details_full_history_element_active_application_number_title),
                                StringSource(
                                    R.string.certificate_details_full_history_element_active_application_modified_date_title
                                )
                            )

                            else -> Triple(StringSource(R.string.unknown), StringSource(R.string.unknown), StringSource(R.string.unknown))
                        }
                        if (element.hasApplicationNumber) {
                            add(
                                CommonSimpleTextInFieldUi(
                                    title = applicationCreateDateTitle,
                                    text = StringSource(
                                        element.createdDateTime?.fromServerDateToTextDate(
                                            dateFormat = UiDateFormats.WITH_TIME
                                        )
                                    ),
                                )
                            )
                            add(
                                CommonSimpleTextInFieldUi(
                                    title = applicationNumberTitle,
                                    text = StringSource(element.applicationNumber),
                                    iconResRight = if (!element.applicationId.isNullOrEmpty()) R.drawable.ic_link_to_details
                                    else null,
                                    isClickable = !element.applicationId.isNullOrEmpty(),
                                    originalModel = element
                                )
                            )
                        }
                        add(
                            CommonSimpleTextInFieldUi(
                                title = applicationModifiedDateTitle,
                                text = StringSource(
                                    element.modifiedDateTime?.fromServerDateToTextDate(
                                        dateFormat = UiDateFormats.WITH_TIME
                                    )
                                ),
                            )
                        )
                        if (status == CertificatesStatusEnum.CREATED) {
                            add(
                                CommonSimpleTextInFieldUi(
                                    title = StringSource(R.string.certificate_details_full_history_element_valid_from_title),
                                    text = StringSource(
                                        element.validityFrom?.fromServerDateToTextDate(
                                            dateFormat = UiDateFormats.WITH_TIME
                                        )
                                    ),
                                )
                            )
                            add(
                                CommonSimpleTextInFieldUi(
                                    title = StringSource(R.string.certificate_details_full_history_element_valid_until_title),
                                    text = StringSource(
                                        element.validityUntil?.fromServerDateToTextDate(
                                            dateFormat = UiDateFormats.WITH_TIME
                                        )
                                    ),
                                )
                            )
                        }
                        element.reasonText?.let { reason ->
                            add(
                                CommonSimpleTextInFieldUi(
                                    title = StringSource(R.string.certificate_details_full_history_element_reason_title),
                                    text = StringSource(reason),
                                )
                            )
                        }
                    }
                }

                when (data) {
                    CertificateDetailsType.DETAILS_STOP -> add(
                        CommonButtonUi(
                            title = StringSource(R.string.certificate_details_certificate_stop_button),
                            elementEnum = CertificateDetailsElementsEnumUi.BUTTON_STOP,
                            buttonColor = ButtonColorUi.ORANGE,
                            iconResLeft = R.drawable.ic_pause
                        )
                    )

                    CertificateDetailsType.DETAILS_REVOKE -> add(
                        CommonButtonUi(
                            title = StringSource(R.string.certificate_details_certificate_revoke_button),
                            elementEnum = CertificateDetailsElementsEnumUi.BUTTON_REVOKE,
                            buttonColor = ButtonColorUi.RED,
                            iconResLeft = R.drawable.ic_cancel
                        )
                    )

                    CertificateDetailsType.DETAIL_RESUME -> add(
                        CommonButtonUi(
                            title = StringSource(R.string.certificate_details_certificate_resume_button),
                            elementEnum = CertificateDetailsElementsEnumUi.BUTTON_RESUME,
                            buttonColor = ButtonColorUi.GREEN,
                            iconResLeft = R.drawable.ic_play
                        )
                    )

                    else -> {}
                }
                add(
                    CommonButtonUi(
                        title = StringSource(R.string.back),
                        elementEnum = CertificateDetailsElementsEnumUi.BUTTON_BACK,
                        buttonColor = ButtonColorUi.TRANSPARENT,
                    )
                )
            }
        }
    }
}