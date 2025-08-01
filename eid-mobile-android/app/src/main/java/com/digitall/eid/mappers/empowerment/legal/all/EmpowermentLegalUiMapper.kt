package com.digitall.eid.mappers.empowerment.legal.all

import com.digitall.eid.R
import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.domain.extensions.getEnumValue
import com.digitall.eid.domain.models.empowerment.common.all.EmpowermentItem
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.empowerment.common.all.EmpowermentElementsEnumUi
import com.digitall.eid.models.empowerment.common.all.EmpowermentSpinnerElementsEnumUi
import com.digitall.eid.models.empowerment.common.filter.EmpowermentFilterStatusEnumUi
import com.digitall.eid.models.empowerment.legal.all.EmpowermentLegalUi
import com.digitall.eid.models.list.CommonSpinnerMenuItemUi
import com.digitall.eid.models.list.CommonSpinnerUi

class EmpowermentLegalUiMapper: BaseMapper<EmpowermentItem, EmpowermentLegalUi>() {

    override fun map(from: EmpowermentItem): EmpowermentLegalUi {
        return with(from) {
            val empowered = empoweredUids?.first()
            val status = getEnumValue<EmpowermentFilterStatusEnumUi>(from.calculatedStatusOn ?: "")
                ?: EmpowermentFilterStatusEnumUi.ALL
            EmpowermentLegalUi(
                id = id,
                status = status,
                originalModel = from,
                name = name ?: "Unknown",
                serviceName = serviceName ?: "Unknown",
                providerName = providerName ?: "Unknown",
                empowered = "${empowered?.uidType}:${empowered?.uid}",
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