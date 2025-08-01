/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.mappers.empowerment.to.me.all

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
import com.digitall.eid.models.empowerment.to.me.all.EmpowermentToMeUi
import com.digitall.eid.models.list.CommonSpinnerMenuItemUi
import com.digitall.eid.models.list.CommonSpinnerUi

class EmpowermentToMeUiMapper :
    BaseMapper<EmpowermentItem, EmpowermentToMeUi>() {

    override fun map(from: EmpowermentItem): EmpowermentToMeUi {
        return with(from) {
            val status = getEnumValue<EmpowermentFilterStatusEnumUi>(from.calculatedStatusOn ?: "")
                ?: EmpowermentFilterStatusEnumUi.UNKNOWN
            val spinnerModel = if (status == EmpowermentFilterStatusEnumUi.ACTIVE) {
                CommonSpinnerUi(
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
                                text = EmpowermentSpinnerElementsEnumUi.SPINNER_TO_ME_CANCEL.title,
                                elementEnum = EmpowermentSpinnerElementsEnumUi.SPINNER_TO_ME_CANCEL,
                                iconRes = R.drawable.ic_cancel,
                                textColorRes = EmpowermentSpinnerElementsEnumUi.SPINNER_TO_ME_CANCEL.colorRes,
                            )
                        )
                    }
                )
            } else null
            EmpowermentToMeUi(
                id = id,
                number = number ?: "Unknown",
                status = status,
                originalModel = from,
                serviceName = serviceName ?: "Unknown",
                providerName = providerName ?: "Unknown",
                empower = name ?: "Unknown",
                spinnerModel = spinnerModel,
                createdOn = createdOn?.fromServerDateToTextDate(dateFormat = UiDateFormats.WITH_TIME) ?: "Unknown"
            )
        }
    }

}