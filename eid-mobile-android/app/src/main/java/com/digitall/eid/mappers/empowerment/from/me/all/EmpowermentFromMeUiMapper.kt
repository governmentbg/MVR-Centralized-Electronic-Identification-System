/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.mappers.empowerment.from.me.all

import com.digitall.eid.R
import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.domain.UiDateFormats
import com.digitall.eid.domain.extensions.fromServerDateToTextDate
import com.digitall.eid.domain.extensions.getEnumValue
import com.digitall.eid.domain.models.empowerment.common.all.EmpowermentItem
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.empowerment.common.all.EmpowermentElementsEnumUi
import com.digitall.eid.models.empowerment.common.all.EmpowermentSpinnerElementsEnumUi
import com.digitall.eid.models.empowerment.common.filter.EmpowermentFilterStatusEnumUi
import com.digitall.eid.models.empowerment.from.me.all.EmpowermentFromMeUi
import com.digitall.eid.models.list.CommonSpinnerMenuItemUi
import com.digitall.eid.models.list.CommonSpinnerUi
import com.digitall.eid.utils.translateUidType

class EmpowermentFromMeUiMapper :
    BaseMapper<EmpowermentItem, EmpowermentFromMeUi>() {

    override fun map(from: EmpowermentItem): EmpowermentFromMeUi {
        return with(from) {
            val empowered = empoweredUids?.first()
            val status = getEnumValue<EmpowermentFilterStatusEnumUi>(from.calculatedStatusOn ?: "")
                ?: EmpowermentFilterStatusEnumUi.UNKNOWN
            val uidType = translateUidType(
                uidType = empowered?.uidType,
                uidEmpowerer = empowered?.uid,
                nameEmpowerer = empowered?.name,
            )
            EmpowermentFromMeUi(
                id = id,
                status = status,
                originalModel = from,
                number = number ?: "Unknown",
                name = name ?: "Unknown",
                serviceName = serviceName ?: "Unknown",
                providerName = providerName ?: "Unknown",
                empowered = uidType,
                additionalEmpoweredPeople = empoweredUids?.size?.minus(1) ?: 0,
                createdOn = createdOn?.fromServerDateToTextDate(dateFormat = UiDateFormats.WITH_TIME)
                    ?: "Unknown",
                spinnerModel = CommonSpinnerUi(
                    required = false,
                    question = false,
                    selectedValue = null,
                    title = StringSource(""),
                    elementEnum = EmpowermentElementsEnumUi.SPINNER_MENU,
                    list = buildList {
                        add(
                            CommonSpinnerMenuItemUi(
                                isSelected = false,
                                originalModel = from,
                                text = EmpowermentSpinnerElementsEnumUi.SPINNER_COPY.title,
                                elementEnum = EmpowermentSpinnerElementsEnumUi.SPINNER_COPY,
                                iconRes = R.drawable.ic_copy,
                                textColorRes = EmpowermentSpinnerElementsEnumUi.SPINNER_COPY.colorRes,
                            )
                        )
                        if (status == EmpowermentFilterStatusEnumUi.ACTIVE) {
                            add(
                                CommonSpinnerMenuItemUi(
                                    isSelected = false,
                                    originalModel = from,
                                    text = EmpowermentSpinnerElementsEnumUi.SPINNER_FROM_ME_CANCEL.title,
                                    elementEnum = EmpowermentSpinnerElementsEnumUi.SPINNER_FROM_ME_CANCEL,
                                    iconRes = R.drawable.ic_cancel,
                                    textColorRes = EmpowermentSpinnerElementsEnumUi.SPINNER_FROM_ME_CANCEL.colorRes,
                                )
                            )
                        }
                    }
                ),
            )
        }
    }

}