/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.mappers.applications.show.details

import com.digitall.eid.R
import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.domain.DEVICES
import com.digitall.eid.domain.UiDateFormats
import com.digitall.eid.domain.extensions.fromServerDateToTextDate
import com.digitall.eid.domain.extensions.getEnumValue
import com.digitall.eid.domain.models.applications.all.ApplicationFullDetailsModel
import com.digitall.eid.extensions.notEmpty
import com.digitall.eid.models.applications.all.ApplicationCitizenIdentifierTypeEnum
import com.digitall.eid.models.applications.all.ApplicationDocumentTypeEnum
import com.digitall.eid.models.applications.all.ApplicationStatusEnum
import com.digitall.eid.models.applications.details.ApplicationDetailsAdapterMarker
import com.digitall.eid.models.applications.details.ApplicationDetailsElementsEnumUi
import com.digitall.eid.models.applications.details.ApplicationDetailsTypeEnum
import com.digitall.eid.models.applications.details.ONLINE_OFFICE
import com.digitall.eid.models.common.ButtonColorUi
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonButtonUi
import com.digitall.eid.models.list.CommonEmptySpaceSizeEnum
import com.digitall.eid.models.list.CommonEmptySpaceUi
import com.digitall.eid.models.list.CommonSeparatorUi
import com.digitall.eid.models.list.CommonSimpleTextInFieldUi
import com.digitall.eid.models.list.CommonSimpleTextUi
import com.digitall.eid.models.list.CommonTitleSmallInFieldUi
import com.digitall.eid.models.list.CommonTitleSmallUi
import com.digitall.eid.models.list.CommonTitleUi
import org.koin.core.component.KoinComponent

class ApplicationDetailsUiMapper :
    BaseMapper<ApplicationFullDetailsModel, List<ApplicationDetailsAdapterMarker>>(),
    KoinComponent {

    override fun map(from: ApplicationFullDetailsModel): List<ApplicationDetailsAdapterMarker> {
        return with(from) {
            buildList {
                val applicationType =
                    getEnumValue<ApplicationDetailsTypeEnum>(
                        information?.fromJSON?.applicationType ?: ""
                    )
                        ?: ApplicationDetailsTypeEnum.UNKNOWN
                add(
                    CommonTitleUi(
                        title = applicationType.title,
                    )
                )
                add(CommonSeparatorUi())
                add(
                    CommonTitleSmallUi(
                        title = StringSource(R.string.application_details_title)
                    )
                )
                add(
                    CommonSimpleTextUi(
                        title = StringSource(R.string.application_details_application_number_title),
                        text = StringSource(information?.fromJSON?.applicationNumber),
                    )
                )
                information?.fromJSON?.serialNumber?.let { serialNumber ->
                    add(
                        CommonSimpleTextUi(
                            title = StringSource(R.string.application_details_certificate_number_title),
                            text = StringSource(serialNumber),
                            iconResRight = if (!information?.fromJSON?.certificateId.isNullOrEmpty()) R.drawable.ic_link_to_details
                            else null,
                            isClickable = !information?.fromJSON?.certificateId.isNullOrEmpty()
                        )
                    )
                }
                val status =
                    getEnumValue<ApplicationStatusEnum>(information?.fromJSON?.status ?: "")
                        ?: ApplicationStatusEnum.UNKNOWN
                add(
                    CommonSimpleTextUi(
                        title = StringSource(R.string.application_details_status_title),
                        text = status.title,
                        colorRes = status.colorRes,
                        iconResLeft = status.iconRes,
                    )
                )
                when (status) {
                    ApplicationStatusEnum.PENDING_PAYMENT -> add(
                        CommonSimpleTextUi(
                            title = StringSource(R.string.application_details_payment_code_title),
                            text = information?.fromJSON?.paymentAccessCode?.let { StringSource(it) }
                                ?: run { StringSource(R.string.unknown) }
                        )
                    )

                    else -> {}
                }
                information?.fromJSON?.reasonText?.notEmpty {
                    add(
                        CommonSimpleTextUi(
                            title = StringSource(R.string.application_details_reason_title),
                            text = StringSource(it),
                        )
                    )
                }
                val createDate = information?.fromJSON?.createDate?.fromServerDateToTextDate(
                    dateFormat = UiDateFormats.WITH_TIME,
                )
                add(
                    CommonSimpleTextUi(
                        title = StringSource(R.string.application_details_created_on_title),
                        text = StringSource(createDate),
                    )
                )
                add(
                    CommonSimpleTextUi(
                        title = StringSource(R.string.application_details_submitted_to_title),
                        text = StringSource(information?.fromJSON?.eidAdministratorName),
                    )
                )
                add(
                    CommonSimpleTextUi(
                        title = StringSource(R.string.application_details_administrator_office_title),
                        text = StringSource(information?.fromJSON?.eidAdministratorOfficeName),
                    )
                )
                val deviceType =
                    StringSource(DEVICES.firstOrNull { device -> device.id == information?.fromJSON?.deviceId }?.name)
                add(
                    CommonSimpleTextUi(
                        title = StringSource(R.string.application_details_carrier_title),
                        text = deviceType,
                    )
                )
                add(
                    CommonEmptySpaceUi(
                        size = CommonEmptySpaceSizeEnum.SIZE_4,
                    )
                )
                add(
                    CommonTitleSmallInFieldUi(
                        title = StringSource(R.string.application_details_applicant_personal_details_title),
                        maxLines = 2,
                    )
                )
                add(
                    CommonSimpleTextInFieldUi(
                        title = StringSource(R.string.application_details_names_title),
                        text = StringSource("${information?.fromJSON?.firstName} ${information?.fromJSON?.secondName} ${information?.fromJSON?.lastName}\n(${information?.fromXML?.firstNameLatin} ${information?.fromXML?.secondNameLatin} ${information?.fromXML?.lastNameLatin})"),
                    )
                )
                add(
                    CommonSimpleTextInFieldUi(
                        title = StringSource(R.string.application_details_email_title),
                        text = StringSource(information?.fromJSON?.email),
                    )
                )
                add(
                    CommonSimpleTextInFieldUi(
                        title = StringSource(R.string.application_details_phone_number_title),
                        text = information?.fromJSON?.phoneNumber?.let { StringSource(it) } ?: run {
                            StringSource(
                                R.string.unspecified
                            )
                        },
                    )
                )
                val citizenIdentifierType = getEnumValue<ApplicationCitizenIdentifierTypeEnum>(
                    information?.fromXML?.citizenIdentifierType ?: ""
                ) ?: ApplicationCitizenIdentifierTypeEnum.EGN
                add(
                    CommonSimpleTextInFieldUi(
                        title = StringSource(R.string.application_details_identification_number_type_title),
                        text = citizenIdentifierType.title,
                    )
                )
                add(
                    CommonSimpleTextInFieldUi(
                        title = StringSource(R.string.application_details_identification_number_title),
                        text = StringSource(information?.fromXML?.citizenIdentifierNumber),
                    )
                )
                val documentType = getEnumValue<ApplicationDocumentTypeEnum>(
                    information?.fromJSON?.identityType ?: ""
                ) ?: ApplicationDocumentTypeEnum.IDENTITY_CARD
                add(
                    CommonSimpleTextInFieldUi(
                        title = StringSource(R.string.application_details_document_type_title),
                        text = documentType.title,
                    )
                )
                add(
                    CommonSimpleTextInFieldUi(
                        title = StringSource(R.string.application_details_document_number_title),
                        text = StringSource(information?.fromJSON?.identityNumber),
                    )
                )
                val identityIssueDate = information?.fromJSON?.identityIssueDate?.fromServerDateToTextDate(
                    dateFormat = UiDateFormats.WITHOUT_TIME,
                )
                add(
                    CommonSimpleTextInFieldUi(
                        title = StringSource(R.string.application_details_issued_on_title),
                        text = StringSource(identityIssueDate),
                    )
                )
                val identityValidityToDate =
                    information?.fromJSON?.identityValidityToDate?.fromServerDateToTextDate(
                        dateFormat = UiDateFormats.WITHOUT_TIME,
                    )
                add(
                    CommonSimpleTextInFieldUi(
                        title = StringSource(R.string.application_details_valid_until_title),
                        text = StringSource(identityValidityToDate),
                    )
                )

                when (status) {
                    ApplicationStatusEnum.APPROVED, ApplicationStatusEnum.PAID -> {
                        when {
                            information?.fromJSON?.eidAdministratorOfficeName == ONLINE_OFFICE && applicationType == ApplicationDetailsTypeEnum.ISSUE_EID -> {
//                                val userModel = preferences.readApplicationInfo()?.userModel
//                    if (!userModel?.eidEntityId.isNullOrEmpty() && userModel?.acr == UserAcrEnum.HIGH) {
                                add(
                                    CommonButtonUi(
                                        title = StringSource(R.string.application_details_continue_creation_process_button_title),
                                        elementEnum = ApplicationDetailsElementsEnumUi.BUTTON_CONTINUE,
                                        buttonColor = ButtonColorUi.BLUE,
                                    )
                                )
//                    }
                            }

                            applicationType == ApplicationDetailsTypeEnum.RESUME_EID -> add(
                                CommonButtonUi(
                                    title = StringSource(R.string.application_details_confirm_certificate_resume_button_title),
                                    elementEnum = ApplicationDetailsElementsEnumUi.BUTTON_RESUME,
                                    buttonColor = ButtonColorUi.GREEN,
                                )
                            )

                            applicationType == ApplicationDetailsTypeEnum.REVOKE_EID -> add(
                                CommonButtonUi(
                                    title = StringSource(R.string.application_details_confirm_certificate_revoke_button_title),
                                    elementEnum = ApplicationDetailsElementsEnumUi.BUTTON_REVOKE,
                                    buttonColor = ButtonColorUi.RED,
                                )
                            )

                            applicationType == ApplicationDetailsTypeEnum.STOP_EID -> add(
                                CommonButtonUi(
                                    title = StringSource(R.string.application_details_confirm_certificate_stop_button_title),
                                    elementEnum = ApplicationDetailsElementsEnumUi.BUTTON_STOP,
                                    buttonColor = ButtonColorUi.ORANGE,
                                )
                            )
                        }
                    }

                    ApplicationStatusEnum.PENDING_PAYMENT -> add(
                        CommonButtonUi(
                            title = StringSource(R.string.application_details_payment_button_title),
                            elementEnum = ApplicationDetailsElementsEnumUi.BUTTON_PAYMENT,
                            buttonColor = ButtonColorUi.BLUE,
                        )
                    )

                    else -> {}
                }
                add(
                    CommonButtonUi(
                        title = StringSource(R.string.application_details_back_button_title),
                        elementEnum = ApplicationDetailsElementsEnumUi.BUTTON_BACK,
                        buttonColor = ButtonColorUi.TRANSPARENT,
                    )
                )
            }
        }
    }

}