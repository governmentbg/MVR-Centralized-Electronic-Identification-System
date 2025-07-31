/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.mappers.applications.show.all

import com.digitall.eid.R
import com.digitall.eid.data.mappers.base.BaseMapperWithData
import com.digitall.eid.domain.DEVICES
import com.digitall.eid.domain.UiDateFormats
import com.digitall.eid.domain.extensions.fromServerDateToTextDate
import com.digitall.eid.domain.extensions.getEnumValue
import com.digitall.eid.domain.models.applications.all.ApplicationItem
import com.digitall.eid.domain.models.common.ApplicationLanguage
import com.digitall.eid.models.applications.all.ApplicationStatusEnum
import com.digitall.eid.models.applications.all.ApplicationTypeEnum
import com.digitall.eid.models.applications.all.ApplicationUi
import com.digitall.eid.models.applications.all.ApplicationsElementsEnumUi
import com.digitall.eid.models.applications.all.ApplicationsSpinnerElementsEnumUi
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonSpinnerMenuItemUi
import com.digitall.eid.models.list.CommonSpinnerUi

class ApplicationsUiMapper :
    BaseMapperWithData<ApplicationItem, ApplicationLanguage, ApplicationUi>() {

    override fun map(from: ApplicationItem, data: ApplicationLanguage?): ApplicationUi {
        return with(from) {
            val status = getEnumValue<ApplicationStatusEnum>(from.status ?: "")
                ?: ApplicationStatusEnum.UNKNOWN

            val spinnerModel = when (status) {
                ApplicationStatusEnum.PENDING_PAYMENT -> CommonSpinnerUi(
                    required = false,
                    question = false,
                    selectedValue = null,
                    title = StringSource(""),
                    elementEnum = ApplicationsElementsEnumUi.SPINNER_MENU,
                    list = buildList {
                        add(
                            CommonSpinnerMenuItemUi(
                                isSelected = false,
                                originalModel = from,
                                text = ApplicationsSpinnerElementsEnumUi.SPINNER_PAYMENT.title,
                                elementEnum = ApplicationsSpinnerElementsEnumUi.SPINNER_PAYMENT,
                                iconRes = R.drawable.ic_credit_card,
                                iconColorRes = R.color.color_1C3050,
                                textColorRes = ApplicationsSpinnerElementsEnumUi.SPINNER_PAYMENT.colorRes,
                            )
                        )
                    }
                )

                else -> null
            }


            ApplicationUi(
                id = id ?: "Unknown",
                applicationNumber = applicationNumber ?: "Unknown",
                status = getEnumValue<ApplicationStatusEnum>(from.status ?: "")
                    ?: ApplicationStatusEnum.UNKNOWN,
                deviceType = when (data) {
                    ApplicationLanguage.BG -> StringSource(DEVICES.firstOrNull { device -> device.id == deviceId }?.name)
                    ApplicationLanguage.EN -> StringSource(DEVICES.firstOrNull { device -> device.id == deviceId }?.description)
                    else -> StringSource(R.string.unknown)

                },
                administrator = eidAdministratorName ?: "Unknown",
                type = getEnumValue<ApplicationTypeEnum>(from.applicationType ?: "")
                    ?: ApplicationTypeEnum.UNKNOWN,
                date = createDate?.fromServerDateToTextDate(
                    dateFormat = UiDateFormats.WITH_TIME,
                ) ?: "Unknown",
                originalModel = from,
                spinnerModel = spinnerModel
            )
        }
    }

}